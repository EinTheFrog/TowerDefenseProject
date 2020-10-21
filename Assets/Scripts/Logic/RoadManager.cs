using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [SerializeField] EnemyManager enemyManager = null;

    //Dictionary<RoadPlatform, List<Vector3>> roadNodeDirs;
    Dictionary<RoadPlatform, Node> roadNodes;
    Dictionary<Node, RoadPlatform> nodeRoads;
    Dictionary<RoadPlatform, KeyValuePair<RoadPlatform, RoadPlatform>> roadsBetweenNodes;
    Graph graph;
    //Dictionary<RoadPlatform, List<List<RoadPlatform>>> paths;

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
        //roadNodeDirs = new Dictionary<RoadPlatform, List<Vector3>>();
        roadNodes = new Dictionary<RoadPlatform, Node>();
        nodeRoads = new Dictionary<Node, RoadPlatform>();
        roadsBetweenNodes = new Dictionary<RoadPlatform, KeyValuePair<RoadPlatform, RoadPlatform>>();
       // paths = new Dictionary<RoadPlatform, List<List<RoadPlatform>>>();
    }

    private void Start()
    {
        RoadPlatform[] roads = GetComponentsInChildren<RoadPlatform>();
        foreach (RoadPlatform road in roads)
        {
            road.Manager = this;
        }
        StartCoroutine(AddAllRoadParts());

    }

    private IEnumerator AddAllRoadParts()
    {
        yield return new WaitForEndOfFrame();
        int id = 1;
        RoadPlatform[] roads = GetComponentsInChildren<RoadPlatform>();
        foreach (RoadPlatform road in roads)
        {
            road.Id = id++;
        }
        foreach (RoadPlatform road in roads)
        {
            if (CheckIfNode(road))
            {
                AddNodeToGraph(road);
            }
        }
    }

    private bool CheckIfNode(RoadPlatform road) =>
               roadNodes.ContainsKey(road) ||
               road.Neighbours.Count > 2 ||
               (road.Neighbours.Count == 2 &&
               Vector3.Dot(road.NeighboursDirs[0], road.NeighboursDirs[1]) == 0);

    private void AddNodeToGraph(RoadPlatform roadNode)
    {
        Node node = new Node(
                roadNode.Id,
                roadNode.transform.localPosition.x,
                roadNode.transform.localPosition.z
                );
        Dictionary<Node, float> nodeNeighbours = new Dictionary<Node, float>();

        foreach (Vector3 direction in roadNode.NeighboursDirs)
        {
            HashSet<RoadPlatform> roadsBetween = new HashSet<RoadPlatform>();
            RoadPlatform next = roadNode;
            float pathCost = roadNode.Cost;
            while (next.Neighbours.ContainsKey(direction))
            {
                next = next.Neighbours[direction];
                pathCost += next.Cost;
                if (CheckIfNode(next))
                {
                    float x = next.transform.localPosition.x;
                    float z = next.transform.localPosition.z;
                    Node nodeNeighbour = roadNodes.ContainsKey(next) ? roadNodes[next] : new Node(next.Id, x, z);
                    nodeNeighbours[nodeNeighbour] = pathCost;

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

        if (roadsBetweenNodes.ContainsKey(roadNode))
        {
            //RemoveDeprecatedPaths(roadsBetweenNodes[roadNode]);
            graph.RemoveConnection(roadNodes[roadsBetweenNodes[roadNode].Key], roadNodes[roadsBetweenNodes[roadNode].Value]);
            roadsBetweenNodes.Remove(roadNode);
            enemyManager.AwareEnemies(new List<RoadPlatform> { roadNode });
        }

        graph.AddNode(node, nodeNeighbours);
        roadNodes[roadNode] = node;
        nodeRoads[node] = roadNode;
    }

    private List<RoadPlatform> FindPath(RoadPlatform roadStart, RoadPlatform roadFinish)
    {
        if (!roadNodes.ContainsKey(roadStart))
        {
            AddNodeToGraph(roadStart);
        }
        if (!roadNodes.ContainsKey(roadFinish))
        {
            AddNodeToGraph(roadFinish);
        }
        return ConvertNodesToRoads(graph.FindPath(roadNodes[roadStart], roadNodes[roadFinish]));
    }

    public void RemoveNode(RoadPlatform roadNode)
    {
        graph.RemoveNode(roadNode.Id);
        roadsBetweenNodes[roadNode] = FindNearestRoadNodes(roadNode);

        RoadPlatform[] roadsBetweenNodesKeys = new RoadPlatform[roadsBetweenNodes.Count];
        roadsBetweenNodes.Keys.CopyTo(roadsBetweenNodesKeys, 0);
        nodeRoads.Remove(roadNodes[roadNode]);
        roadNodes.Remove(roadNode);
        foreach (RoadPlatform roadBetween in roadsBetweenNodesKeys)
        {
            if (roadsBetweenNodes[roadBetween].Key == roadNode || roadsBetweenNodes[roadBetween].Value == roadNode)
            {
                roadsBetweenNodes[roadBetween] = FindNearestRoadNodes(roadBetween);
            }
        }
        float pathCost = CalculatePathCost(roadsBetweenNodes[roadNode].Key, roadsBetweenNodes[roadNode].Value);
        graph.AddConnection(roadNodes[roadsBetweenNodes[roadNode].Key], roadNodes[roadsBetweenNodes[roadNode].Value], pathCost); 
        //RemoveDeprecatedPaths(road);
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

    private KeyValuePair<RoadPlatform, RoadPlatform> FindNearestRoadNodes(RoadPlatform start)
    {
        List<RoadPlatform> nearestRoadNodes = new List<RoadPlatform>();
        if (start.Neighbours.Count != 2) throw new ArgumentException("It is prohibited to delete useful node");

        foreach (Vector3 dir in start.NeighboursDirs)
        {
            RoadPlatform next = start.Neighbours[dir];
            while (!roadNodes.ContainsKey(next) && next.Neighbours.ContainsKey(dir))
            {
                next = next.Neighbours[dir];
            }
            if (roadNodes.ContainsKey(next)) nearestRoadNodes.Add(next);
        }
        if (nearestRoadNodes.Count != 2)
        {
            throw new ArgumentException("This is a roadNode and it shouldn't be in roadsBetween");
        }
        return new KeyValuePair<RoadPlatform, RoadPlatform>(nearestRoadNodes[0], nearestRoadNodes[1]);
    }

    private float CalculatePathCost (RoadPlatform start, RoadPlatform finish)
    {
        float cost;
        foreach (Vector3 dir in start.NeighboursDirs)
        {
            RoadPlatform next = start.Neighbours[dir];
            cost = start.Cost + next.Cost;
            while (!roadNodes.ContainsKey(next) && next.Neighbours.ContainsKey(dir))
            {
                next = next.Neighbours[dir];
                cost += next.Cost;
            }
            if (next == finish) return cost;
        }
        throw new ArgumentException("There is no path between these roadNodes");
    }

    private List<RoadPlatform> ConvertNodesToRoads(List<Node> nodes)
    {
        List<RoadPlatform> result = new List<RoadPlatform>();
        foreach (Node node in nodes)
        {
            if (!nodeRoads.ContainsKey(node))
            {
                throw new ArgumentException("This node wasn't presented in graph");
            }
            if (nodeRoads[node] == null)
            {
                throw new ArgumentException("Null in nodeRoads");
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
                if (!roadNodes.ContainsKey(roadsBetweenNodes[road].Key) || !roadNodes.ContainsKey(roadsBetweenNodes[road].Value)) 
                {
                    throw new ArgumentException("Node in roadBetweenNodes value isn't in roadNodes");
                }
                graph.UpdateCost(roadNodes[roadsBetweenNodes[road].Key], roadNodes[roadsBetweenNodes[road].Value], roadChanges[road]);
            }
            else continue;

            /*paths.Remove(road);
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
            }*/
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
        Collider[] collidersInRadius = new Collider[200];
        Dictionary<RoadPlatform, float> roads = new Dictionary<RoadPlatform, float>();
        int count = Physics.OverlapSphereNonAlloc(center, radius, collidersInRadius);
        for (int i = 0; i < count; i++)
        {
            RoadPlatform road = collidersInRadius[i].GetComponent<RoadPlatform>();
            if (road != null)
            {
                road.Cost += dangerChange;
                if (road.Cost < 0)
                {
                    throw new Exception("Road cost is less than 0");
                }
                roads[road] = dangerChange;
            }
        }
        UpdateDanger(roads);
    }

    /*  private void RemoveDeprecatedPaths(RoadPlatform road)
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
        }*/

    /*    private void RemoveDeprecatedPaths(KeyValuePair<RoadPlatform, RoadPlatform> roads)
        {
            RemoveDeprecatedPaths(roads.Key);
            RemoveDeprecatedPaths(roads.Value);
        }*/
}


