using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    Dictionary<RoadPlatform, List<Vector3>> roadDirs;
    Dictionary<RoadPlatform, Node> roadNodes;
    Dictionary<Node, RoadPlatform> nodeRoads;
    Graph graph;
    
    public static RoadManager Instance { get; private set; }

    public bool IsReady { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        IsReady = false;
        graph = new Graph();
        roadDirs = new Dictionary<RoadPlatform, List<Vector3>>();
        roadNodes = new Dictionary<RoadPlatform, Node>();
        nodeRoads = new Dictionary<Node, RoadPlatform>();

        StartCoroutine(AddAllRoadParts());
        IsReady = true;
    }

    IEnumerator AddAllRoadParts()
    {
        yield return new WaitForEndOfFrame();
        int id = 0;
        foreach (RoadPlatform roadPart in gameObject.GetComponentsInChildren<RoadPlatform>())
        {
            roadPart.Id = id++;
            if (roadPart.Neighbours.Count == 1) continue;
            if (roadPart.Neighbours.Count > 2 ||
                Vector3.Dot(roadPart.NeighboursDirs[0], roadPart.NeighboursDirs[1]) == 0)
                roadDirs.Add(roadPart, roadPart.NeighboursDirs);
        }

        foreach (RoadPlatform road in roadDirs.Keys)
        {
            AddNodeToGraph(road);
        }
    }

    public List<RoadPlatform> FindPath(RoadPlatform roadStart, RoadPlatform roadFinish)
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
        Dictionary<Node, int> nodeNeighbours = new Dictionary<Node, int>();
        foreach (Vector3 direction in roadDirs[roadNode])
        {
            RoadPlatform next = roadNode;
            int pathLength = 0;
            while (next.Neighbours.ContainsKey(direction))
            {
                next = next.Neighbours[direction];
                pathLength++;
                if (roadDirs.ContainsKey(next))
                {
                    var x = next.transform.localPosition.x;
                    var z = next.transform.localPosition.z;
                    nodeNeighbours[new Node(next.Id, x, z)] = pathLength;
                    roadDirs[next].Remove(direction * (-1));
                    break;
                }
            }
        }
        graph.AddNode(node, nodeNeighbours);
        roadNodes[roadNode] = node;
        nodeRoads[node] = roadNode;
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
        foreach (Node node in nodes) result.Add(nodeRoads[node]);
        return result;
    }

}


