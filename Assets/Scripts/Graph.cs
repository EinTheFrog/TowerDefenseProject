using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Text;
using System.Linq;

public class Graph
{
    Dictionary<Node, Dictionary<Node, int>> graph = new Dictionary<Node, Dictionary<Node, int>>();
    Dictionary<int, Node> nodes = new Dictionary<int, Node>();

    public void AddNode(Node newNode, Dictionary<Node, int> neighbours)
    {
        if (!graph.ContainsKey(newNode)) graph[newNode] = neighbours;
        else
        {
            foreach (KeyValuePair<Node, int> pair in neighbours)
                graph[newNode].Add(pair.Key, pair.Value);
        }
        nodes[newNode.Id] = newNode;
        foreach (Node neighbour in neighbours.Keys)
        {
            if (!graph.ContainsKey(neighbour)) graph[neighbour] = new Dictionary<Node, int>();
            graph[neighbour][newNode] = graph[newNode][neighbour];
        }
    }

    public void RemoveNode(int nodeId)
    {
        if (!nodes.ContainsKey(nodeId)) return;
        Node node = nodes[nodeId];
        graph.Remove(node);
        foreach (Node keyNode in graph.Keys)
        {
            graph[keyNode].Remove(node);
        }
        nodes.Remove(nodeId);
    }

    public List<Node> FindPath (Node start, Node finish)
    {
        if (start == finish) return new List<Node> { start };

        SimplePriorityQueue<Node> priorityQueue = new SimplePriorityQueue<Node>();
        HashSet<Node> visited = new HashSet<Node>();
        Dictionary<Node, int> pathCost = new Dictionary<Node, int>();
        Dictionary<Node, Node> pathParts = new Dictionary<Node, Node>();
        pathCost[start] = 0;
        priorityQueue.Enqueue(start, pathCost[start]);
        visited.Add(start);

        while (priorityQueue.Count > 0 && !pathParts.Keys.Contains(finish))
        {
            var node = priorityQueue.Dequeue();
            foreach (Node neighbour in graph[node].Keys)
            {
                int cost = pathCost[node] + graph[node][neighbour];
                if (!visited.Contains(neighbour)) priorityQueue.Enqueue(neighbour, cost);
                else if (cost < pathCost[neighbour]) priorityQueue.UpdatePriority(neighbour, cost);
                else continue;
                pathCost[neighbour] = cost;
                pathParts[neighbour] = node;
                visited.Add(neighbour);
                if (neighbour == finish) break;
            }
        }
        return WritePath(pathParts, start, finish);
    }

    private List<Node> WritePath(Dictionary<Node, Node> pathParts, Node start, Node finish)
    {
        List<Node> path = new List<Node>();
        if (!pathParts.ContainsKey(finish)) return null;
        var node = finish;
        path.Add(finish);
        while (!path.Contains(start))
        {
            node = pathParts[node];
            path.Add(node);
        }
        path.Reverse();
        return path;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<Node, Dictionary<Node, int>> pair1 in graph)
        {
            sb.Append("\n");
            sb.Append(pair1.Key.Id);
            sb.Append(":");
            foreach (KeyValuePair<Node,  int> pair2 in pair1.Value)
            {
                sb.Append($"{pair2.Key.Id}-{pair2.Value}");
            }
            sb.Append("\n");
        }
        return sb.ToString();
    }
}


