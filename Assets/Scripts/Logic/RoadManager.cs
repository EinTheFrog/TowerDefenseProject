using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [SerializeField] EnemyManager enemyManager = null;

    Dictionary<RoadPlatform, List<Vector3>> roadNodeDirs;
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
        roadNodeDirs = new Dictionary<RoadPlatform, List<Vector3>>();
        roadNodes = new Dictionary<RoadPlatform, Node>();
        nodeRoads = new Dictionary<Node, RoadPlatform>();
        roadsBetweenNodes = new Dictionary<RoadPlatform, KeyValuePair<RoadPlatform, RoadPlatform>>();
        paths = new Dictionary<RoadPlatform, List<List<RoadPlatform>>>();
    }

    private void OnEnable()
    {
        foreach (RoadPlatform road in GetComponentsInChildren<RoadPlatform>())
        {
            road.Manager = this;
        }
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
            if (
                roadPart.Neighbours.Count > 2 ||
                Vector3.Dot(roadPart.NeighboursDirs[0], roadPart.NeighboursDirs[1]) == 0
                )
            {
                roadNodeDirs.Add(roadPart, roadPart.NeighboursDirs);
            }
        }

        foreach (RoadPlatform road in roadNodeDirs.Keys)
        {
            AddNodeToGraph(road);
        }
    }

    private void AddNodeToGraph(RoadPlatform roadNode)
    {
        if (!roadNodeDirs.ContainsKey(roadNode))
        {
            throw new ArgumentException("roadNode hasn't been added to the roadNodeDirs");
        }

        Node node = new Node(
                roadNode.Id,
                roadNode.transform.localPosition.x,
                roadNode.transform.localPosition.z
                );
        Dictionary<Node, float> nodeNeighbours = new Dictionary<Node, float>();

        if (roadsBetweenNodes.ContainsKey(roadNode))
        {
            RemoveDeprecatedPaths(roadsBetweenNodes[roadNode]);
            roadsBetweenNodes.Remove(roadNode);
            enemyManager.AwareEnemies(new List<RoadPlatform> { roadNode });
        }

        foreach (Vector3 direction in roadNodeDirs[roadNode])
        {
            HashSet<RoadPlatform> roadsBetween = new HashSet<RoadPlatform>();
            RoadPlatform next = roadNode;
            float pathCost = 0;
            while (next.Neighbours.ContainsKey(direction))
            {
                next = next.Neighbours[direction];
                pathCost += next.Danger;
                if (roadNodeDirs.ContainsKey(next))
                {
                    float x = next.transform.localPosition.x;
                    float z = next.transform.localPosition.z;
                    Node nodeNeighbour = roadNodes.ContainsKey(next) ? roadNodes[next] : new Node(next.Id, x, z);
                    nodeNeighbours[nodeNeighbour] = pathCost;
                    roadNodeDirs[next].Remove(direction * (-1));

                    KeyValuePair<RoadPlatform, RoadPlatform> value = new KeyValuePair<RoadPlatform, RoadPlatform>(roadNode, next);
                    foreach (RoadPlatform road in roadsBetween)
                    {
                        roadsBetweenNodes[road] = value;
                    }

                    break;
                }
                else
                {
                    roadsBetween.Add(next);
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
            roadNodeDirs[roadStart] = roadStart.NeighboursDirs;
            AddNodeToGraph(roadStart);
        }

        if (!roadNodes.ContainsKey(roadFinish) && roadNodeDirs.ContainsKey(roadFinish))
        {
            throw new Exception("road node isn't in roadNodes but in roadNodeDirs");
        }
        if (!roadNodes.ContainsKey(roadFinish))
        {
            roadNodeDirs[roadFinish] = roadFinish.NeighboursDirs;
            AddNodeToGraph(roadFinish);
        }
        return ConvertNodesToRoads(graph.FindPath(roadNodes[roadStart], roadNodes[roadFinish]));
    }

    public void RemoveNode(RoadPlatform road)
    {
        graph.RemoveNode(road.Id);
        roadNodeDirs.Remove(road);
        roadsBetweenNodes[road] = FindNearestRoadNodes(road);
        RemoveDeprecatedPaths(road);
    }

    public bool ShouldNodeBeDeleted(RoadPlatform road)
    {
        if (road.Neighbours.Count == 2 &&
            Vector3.Dot(road.NeighboursDirs[0], road.NeighboursDirs[1]) != 0) return true;
        else return false;
    }

    public KeyValuePair<RoadPlatform, RoadPlatform>? GetNodesForRoadBetween(RoadPlatform road)
    {
        if (roadsBetweenNodes.ContainsKey(road)) return roadsBetweenNodes[road];
        else return null;
    }

    private KeyValuePair<RoadPlatform, RoadPlatform> FindNearestRoadNodes(RoadPlatform road)
    {
        List<RoadPlatform> nearestRoadNodes = new List<RoadPlatform>();
        foreach (RoadPlatform neighbour in road.Neighbours.Values)
        {
            if (roadsBetweenNodes.ContainsKey(neighbour))
            {
                nearestRoadNodes.Add(roadsBetweenNodes[neighbour].Key != road ? roadsBetweenNodes[neighbour].Key : roadsBetweenNodes[neighbour].Value);
            }
            else if (roadNodeDirs.ContainsKey(neighbour))
            {
                nearestRoadNodes.Add(neighbour);
            }
            else
            {
                throw new Exception("neighbour rode is not a roadNode but it is also not in roadsBetween");
            }
        }
        if (nearestRoadNodes.Count != 2)
        {
            throw new ArgumentException("You are trying to remove useful roadnode");
        }
        return new KeyValuePair<RoadPlatform, RoadPlatform>(nearestRoadNodes[0], nearestRoadNodes[1]);
    }

    private List<RoadPlatform> ConvertNodesToRoads(List<Node> nodes)
    {
        List<RoadPlatform> result = new List<RoadPlatform>();
        foreach (Node node in nodes)
        {
            if (nodeRoads[node] == null)
            {
                Debug.LogError("Null in nodeRoads");
            }
            result.Add(nodeRoads[node]);
        }
        return result;
    }

    public void UpdateDanger(Dictionary<RoadPlatform, float> roadChanges)
    {
        foreach (RoadPlatform road in roadChanges.Keys)
        {
            if (roadNodes.ContainsKey(road))
            {
                graph.UpdateCost(roadNodes[road], roadChanges[road]);
            }
            else if (roadsBetweenNodes.ContainsKey(road))
            {
                graph.UpdateCost(roadNodes[roadsBetweenNodes[road].Key], roadNodes[roadsBetweenNodes[road].Value], roadChanges[road]);
            }
            else continue;

            paths.Remove(road);
            foreach (RoadPlatform start in paths.Keys)
            {
                List<RoadPlatform>[] pathsWithSameStart = new List<RoadPlatform>[paths[start].Count];
                paths[start].CopyTo(pathsWithSameStart, 0);
                foreach (List<RoadPlatform> path in pathsWithSameStart)
                {
                    if (path.Contains(road))
                    {
                        paths[start].Remove(path);
                    }
                }
            }
        }

        enemyManager.AwareEnemies(roadChanges.Keys);
    }

    public List<RoadPlatform> GetOrAddPath(RoadPlatform start, RoadPlatform finish)
    {
        //сохраняем найденные оптимальные пути и используем их заново при необходимости
        //(не вызвывая лишний раз поиск нового пути)
        List<RoadPlatform> currentPath = new List<RoadPlatform>();

        /*if (paths.ContainsKey(start))
        {
            foreach (List<RoadPlatform> path in paths[start])
            {
                if (path.Contains(finish))
                {
                    int ind = path.IndexOf(finish);
                    currentPath = path.GetRange(0, ind + 1);
                    return currentPath;
                }
            }
        }
        else
        {
            paths[start] = new List<List<RoadPlatform>>();
        }

        if (currentPath.Count == 0)
        {
            currentPath = FindPath(start, finish);
        }
        for (int i = 0; i < paths[start].Count; i++)
        {
            if (currentPath.Contains(paths[start][i].Last()))
            {
                paths[start][i] = currentPath;
                return currentPath;
            }
        }
        paths[start].Add(currentPath);*/
        currentPath = FindPath(start, finish);
        return currentPath;
    }

    public void UpdateDangerInRadius(Vector3 center, float radius, float dangerChange)
    {
        Collider[] collidersInRadius = new Collider[100];
        Dictionary<RoadPlatform, float> roads = new Dictionary<RoadPlatform, float>();
        int count = Physics.OverlapSphereNonAlloc(center, radius, collidersInRadius);
        for (int i = 0; i < count; i++)
        {
            RoadPlatform road = collidersInRadius[i].GetComponent<RoadPlatform>();
            if (road != null)
            {
                road.Danger += dangerChange;
                roads[road] = dangerChange;
            }
        }
        UpdateDanger(roads);
    }

    private void RemoveDeprecatedPaths(RoadPlatform road)
    {
        RoadPlatform[] starts = new RoadPlatform[paths.Keys.Count];
        paths.Keys.CopyTo(starts, 0);
        foreach (RoadPlatform start in starts)
        {
            if (start == road)
            {
                paths.Remove(start);
            } 
            else
            {
                List<RoadPlatform>[] pathsCollection = new List<RoadPlatform>[paths[start].Count];
                paths[start].CopyTo(pathsCollection);
                foreach (List<RoadPlatform> path in pathsCollection)
                {
                    if (path.Contains(road))
                    {
                        paths[start].Remove(path);
                    }
                }
            }
        }
    }

    private void RemoveDeprecatedPaths(KeyValuePair<RoadPlatform, RoadPlatform> roads)
    {
        RemoveDeprecatedPaths(roads.Key);
        RemoveDeprecatedPaths(roads.Value);
    }
}


