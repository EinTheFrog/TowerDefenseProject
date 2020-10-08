using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Text;
using System.Linq;
using System;

public class Graph
{
    Dictionary<Node, Dictionary<Node, float>> graph = new Dictionary<Node, Dictionary<Node, float>>();
    Dictionary<int, Node> nodes = new Dictionary<int, Node>();

    public void AddNode(Node newNode, Dictionary<Node, float> neighbours)
    {
        if (!graph.ContainsKey(newNode))
        {
            graph[newNode] = neighbours;
            nodes[newNode.Id] = newNode;
        } 
        else
        {
            foreach (KeyValuePair<Node, float> pair in neighbours)
            {
                graph[newNode][pair.Key] = pair.Value;
                if (pair.Value < 0) throw new Exception("Negative cost");
            }
        }  

        foreach (Node neighbour in neighbours.Keys)
        {
            if (!graph.ContainsKey(neighbour))
            {
                graph[neighbour] = new Dictionary<Node, float>();
            }
            graph[neighbour][newNode] = graph[newNode][neighbour];
        }
    }

    public void RemoveNode(int nodeId)
    {
        if (!nodes.ContainsKey(nodeId)) throw new Exception("Trying to remove node which doesn't exist");

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
        Dictionary<Node, float> pathCost = new Dictionary<Node, float>();
        Dictionary<Node, Node> pathParts = new Dictionary<Node, Node>();
        pathCost[start] = 0f;
        priorityQueue.Enqueue(start, pathCost[start]);
        visited.Add(start);

        while (priorityQueue.Count > 0 && !pathParts.Keys.Contains(finish))
        {
            var node = priorityQueue.Dequeue();
            foreach (Node neighbour in graph[node].Keys)
            {
                float roughtRange = Mathf.Abs(neighbour.Position.X - finish.Position.X + (neighbour.Position.Y - finish.Position.Y));
                float cost = pathCost[node] + graph[node][neighbour];
                if (cost < 0)
                {
                    throw new Exception("Negative cost");
                }
                if (!visited.Contains(neighbour))
                {
                    priorityQueue.Enqueue(neighbour, cost + roughtRange);
                    visited.Add(neighbour);
                    pathCost[neighbour] = cost;
                    pathParts[neighbour] = node;
                }
                else if (cost < pathCost[neighbour]) 
                {
                    if (priorityQueue.Contains(neighbour))
                    {
                        priorityQueue.UpdatePriority(neighbour, cost + roughtRange);
                    } else
                    {
                        priorityQueue.Enqueue(neighbour, cost + roughtRange);
                    }
                    pathCost[neighbour] = cost;
                    pathParts[neighbour] = node;
                }
                else continue;

                if (neighbour == finish) break;
            }
        }
        return WritePath(pathParts, start, finish);
    }

    public void UpdateCost(Node start, Node finish, float costChange)
    {
        if (graph.ContainsKey(start) && graph[start].ContainsKey(finish))
        {
            if (graph[start][finish] + costChange < 0) throw new Exception("Negative cost");
            graph[start][finish] += costChange;
            graph[finish][start] += costChange;
        }
    }

    public void UpdateCost(Node node, float costChange)
    {
        if (graph.ContainsKey(node))
        {
            Node[] keys = new Node[graph[node].Keys.Count];
            graph[node].Keys.CopyTo(keys, 0);
            foreach (Node neighbour in keys)
            {
                if (graph[node][neighbour] + costChange < 0)
                {
                    throw new Exception("Negative cost");
                }
                if (graph[neighbour][node] + costChange < 0)
                {
                    throw new Exception("Negative cost");
                }
                graph[node][neighbour] += costChange;
                graph[neighbour][node] += costChange;
            }
        } else
        {
            throw new Exception("Trying to update cost of node which doesn't exits");
        }
    }

    private List<Node> WritePath(Dictionary<Node, Node> pathParts, Node start, Node finish)
    {
        List<Node> path = new List<Node>();
        if (!pathParts.ContainsKey(finish))
        {
            Debug.LogError("Cannot find path to the finish " + finish);
            return null;
        } 
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

    public void RemoveConnection(Node start, Node finish)
    {
        graph[start].Remove(finish);
        graph[finish].Remove(start);
    }

    public void AddConnection(Node start, Node finish, float cost)
    {
        graph[start][finish] = cost;
        graph[finish][start] = cost;
    }
}


