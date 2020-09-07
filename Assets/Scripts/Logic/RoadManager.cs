using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [SerializeField] EnemyManager enemyManager = null;

    Dictionary<RoadPlatform, List<Vector3>> roadDirs;
    Dictionary<RoadPlatform, Node> roadNodes;
    Dictionary<Node, RoadPlatform> nodeRoads;
    Dictionary<RoadPlatform, KeyValuePair<RoadPlatform, RoadPlatform>> roadsBetweenNodes;
    Graph graph;
    Dictionary<RoadPlatform, List<List<RoadPlatform>>> paths;

    public EnemyManager EnemyManager { get => enemyManager; }

    private void OnValidate()
    {
        if (EnemyManager == null ||
            EnemyManager.RoadManager != this)
            Debug.LogError("Reference connection between RoadManger and EnemyManager was broken");
    }

    private void Awake()
    {
        graph = new Graph();
        roadDirs = new Dictionary<RoadPlatform, List<Vector3>>();
        roadNodes = new Dictionary<RoadPlatform, Node>();
        nodeRoads = new Dictionary<Node, RoadPlatform>();
        roadsBetweenNodes = new Dictionary<RoadPlatform, KeyValuePair<RoadPlatform, RoadPlatform>>();
        paths = new Dictionary<RoadPlatform, List<List<RoadPlatform>>>();
    }

    private void OnEnable()
    {
        foreach (RoadPlatform road in GetComponentsInChildren<RoadPlatform>())
            road.Manager = this;
    }

    private void Start()
    {
        AddAllRoadParts();
    }

    private void AddAllRoadParts()
    {
        int id = 0;
        foreach (RoadPlatform roadPart in gameObject.GetComponentsInChildren<RoadPlatform>())
        {
            roadPart.Id = id++;
            if (roadPart.Neighbours.Count == 1) continue;
            if (roadPart.Neighbours.Count > 2 ||
                Vector3.Dot(roadPart.NeighboursDirs[0], roadPart.NeighboursDirs[1]) == 0)
                roadDirs.Add(roadPart, roadPart.NeighboursDirs);
        }

        foreach (RoadPlatform road in roadDirs.Keys) AddNodeToGraph(road);
    }

    private void AddNodeToGraph(RoadPlatform roadNode)
    {
        if (!roadDirs.ContainsKey(roadNode))
        {
            Debug.LogError("roadNode hasn't been added to roadDirs");
            return;
        }
        Node node = new Node(
                roadNode.Id,
                roadNode.transform.localPosition.x,
                roadNode.transform.localPosition.z
                );
        Dictionary<Node, float> nodeNeighbours = new Dictionary<Node, float>();
        foreach (Vector3 direction in roadDirs[roadNode])
        {
            HashSet<RoadPlatform> roadsBetween = new HashSet<RoadPlatform>();
            roadsBetween.Add(roadNode);
            RoadPlatform next = roadNode;
            float pathCost = 0;
            while (next.Neighbours.ContainsKey(direction))
            {
                next = next.Neighbours[direction];
                pathCost += next.Danger;
                roadsBetween.Add(next);
                if (roadDirs.ContainsKey(next))
                {
                    var x = next.transform.localPosition.x;
                    var z = next.transform.localPosition.z;
                    nodeNeighbours[new Node(next.Id, x, z)] = pathCost;
                    roadDirs[next].Remove(direction * (-1));

                    KeyValuePair<RoadPlatform, RoadPlatform> value = new KeyValuePair<RoadPlatform, RoadPlatform>(roadNode, next);
                    foreach (RoadPlatform road in roadsBetween) roadsBetweenNodes[road] = value;

                    break;
                }
            }
        }
        graph.AddNode(node, nodeNeighbours);
        roadNodes[roadNode] = node;
        nodeRoads[node] = roadNode;
    }

    private List<RoadPlatform> FindPath(RoadPlatform roadStart, RoadPlatform roadFinish)
    {
        if (!roadNodes.ContainsKey(roadStart))
        {
            roadDirs.Add(roadStart, roadStart.NeighboursDirs);
            AddNodeToGraph(roadStart);
        }
        if (!roadNodes.ContainsKey(roadFinish))
        {
            roadDirs.Add(roadFinish, roadFinish.NeighboursDirs);
            AddNodeToGraph(roadFinish);
        }
        return ConvertNodesToRoads(graph.FindPath(roadNodes[roadStart], roadNodes[roadFinish]));
    }

    public void RemoveNodeIfUseless(RoadPlatform road)
    {
        //todo: проверить на вшивость (не нашел ключ в графе)
        if (road.Neighbours.Count < 2 ||
             (road.Neighbours.Count == 2 && Vector3.Dot(road.NeighboursDirs[0], road.NeighboursDirs[1]) != 0))
            graph.RemoveNode(road.Id);
    }

    private List<RoadPlatform> ConvertNodesToRoads(List<Node> nodes)
    {
        List<RoadPlatform> result = new List<RoadPlatform>();
        foreach (Node node in nodes)
        {
            if (nodeRoads[node] == null)
                Debug.LogError("Null in nodeRoads");
            result.Add(nodeRoads[node]);
        }
        return result;
    }

    public void UpdateDanger(RoadPlatform road, float newDanger)
    {
        float costChange = newDanger - road.Danger;
        if (!roadsBetweenNodes.ContainsKey(road)) return;
        graph.UpdateCost(roadNodes[roadsBetweenNodes[road].Key], roadNodes[roadsBetweenNodes[road].Value], costChange);
        foreach (RoadPlatform start in paths.Keys)
        {
            foreach (List<RoadPlatform> path in paths[start])
            {
                if (path.Contains(road)) paths[start].Remove(path);
            }
        }
        enemyManager.AwareEnemies(road);
    }

    public List<RoadPlatform> GetOrAddPath(RoadPlatform start, RoadPlatform finish)
    {
        //сохраняем найденные оптимальные пути и используем их заново при необходимости
        //(не вызвывая лишний раз поиск нового пути)
        List<RoadPlatform> currentPath = new List<RoadPlatform>();

        if (paths.ContainsKey(start))
        {
            foreach (List<RoadPlatform> path in paths[start])
            {
                if (path.Contains(finish))
                {
                    int ind = path.IndexOf(finish);
                    currentPath = path.GetRange(0, ind);
                }
            }
        }
        else paths[start] = new List<List<RoadPlatform>>();

        if (currentPath.Count == 0) currentPath = FindPath(start, finish);

        for (int i = 0; i < paths[start].Count; i++)
        {
            if (currentPath.Contains(paths[start][i].Last()))
            {
                paths[start][i] = currentPath;
                break;
            }
        }
        return currentPath;
    }

    public void UpdateDangerInRadius(Vector3 center, float radius)
    {
        Collider[] collidersInRadius = new Collider[100];
        int count = Physics.OverlapSphereNonAlloc(center, radius, collidersInRadius);
        for (int i = 0; i < count; i++)
        {
            RoadPlatform road = collidersInRadius[i].GetComponent<RoadPlatform>();
            if (road != null) road.Danger++;
        }
    }

}


