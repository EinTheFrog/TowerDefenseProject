using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.GraphSearch;
using Gameplay.Platforms;
using UnityEngine;

namespace Gameplay.Managers
{
    public class RoadManager : MonoBehaviour
    {
        [SerializeField] private EnemyManager enemyManager = null;
        
        private Dictionary<RoadPlatform, Node> _roadNodes;
        private Dictionary<Node, RoadPlatform> _nodeRoads;
        private Dictionary<RoadPlatform, KeyValuePair<RoadPlatform, RoadPlatform>> _roadsBetweenNodes;
        private Graph _graph;

        public EnemyManager EnemyManager => enemyManager;

        private void OnValidate()
        {
            if (EnemyManager == null || EnemyManager.RoadManager != this)
            {
                Debug.Log("Reference connection between RoadManger and EnemyManager was broken");
            }
        }

        private void Awake()
        {
            _graph = new Graph();
            _roadNodes = new Dictionary<RoadPlatform, Node>();
            _nodeRoads = new Dictionary<Node, RoadPlatform>();
            _roadsBetweenNodes = new Dictionary<RoadPlatform, KeyValuePair<RoadPlatform, RoadPlatform>>();
        }

        private void Start()
        {
            var roads = GetComponentsInChildren<RoadPlatform>();
            foreach (var road in roads)
            {
            }
            StartCoroutine(AddAllRoadParts());

        }

        private IEnumerator AddAllRoadParts()
        {
            yield return new WaitForEndOfFrame();
            var id = 1;
            var roads = GetComponentsInChildren<RoadPlatform>();
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
            _roadNodes.ContainsKey(road) ||
            road.Neighbours.Count > 2 ||
            (road.Neighbours.Count == 2 &&
             Vector3.Dot(road.NeighboursDirs[0], road.NeighboursDirs[1]) == 0);

        private void AddNodeToGraph(RoadPlatform roadNode)
        {
            var localPosition = roadNode.transform.localPosition;
            var node = new Node(
                roadNode.Id,
                localPosition.x,
                localPosition.z
            );
            var nodeNeighbours = new Dictionary<Node, float>();

            foreach (var direction in roadNode.NeighboursDirs)
            {
                var roadsBetween = new HashSet<RoadPlatform>();
                var next = roadNode;
                var pathCost = roadNode.cost;
                while (next.Neighbours.ContainsKey(direction))
                {
                    next = next.Neighbours[direction];
                    pathCost += next.cost;
                    if (CheckIfNode(next))
                    {
                        var transform1 = next.transform;
                        var position = transform1.localPosition;
                        var x = position.x;
                        var z = position.z;
                        var nodeNeighbour = _roadNodes.ContainsKey(next) ? _roadNodes[next] : new Node(next.Id, x, z);
                        nodeNeighbours[nodeNeighbour] = pathCost;

                        var value = new KeyValuePair<RoadPlatform, RoadPlatform>(roadNode, next);
                        foreach (var road in roadsBetween)
                        {
                            _roadsBetweenNodes[road] = value;
                        }
                        break;
                    }

                    roadsBetween.Add(next);
                }
            }

            if (_roadsBetweenNodes.ContainsKey(roadNode))
            {
                _graph.RemoveConnection(_roadNodes[_roadsBetweenNodes[roadNode].Key], _roadNodes[_roadsBetweenNodes[roadNode].Value]);
                _roadsBetweenNodes.Remove(roadNode);
                enemyManager.AwareEnemies(new List<RoadPlatform> { roadNode });
            }

            _graph.AddNode(node, nodeNeighbours);
            _roadNodes[roadNode] = node;
            _nodeRoads[node] = roadNode;
        }

        public List<RoadPlatform> FindPath(RoadPlatform roadStart, RoadPlatform roadFinish)
        {
            if (!_roadNodes.ContainsKey(roadStart))
            {
                AddNodeToGraph(roadStart);
            }
            if (!_roadNodes.ContainsKey(roadFinish))
            {
                AddNodeToGraph(roadFinish);
            }
            return ConvertNodesToRoads(_graph.FindPath(_roadNodes[roadStart], _roadNodes[roadFinish]));
        }

        public void RemoveNode(RoadPlatform roadNode)
        {
            _graph.RemoveNode(roadNode.Id);
            _roadsBetweenNodes[roadNode] = FindNearestRoadNodes(roadNode);

            var roadsBetweenNodesKeys = new RoadPlatform[_roadsBetweenNodes.Count];
            _roadsBetweenNodes.Keys.CopyTo(roadsBetweenNodesKeys, 0);
            _nodeRoads.Remove(_roadNodes[roadNode]);
            _roadNodes.Remove(roadNode);
            foreach (var roadBetween in roadsBetweenNodesKeys)
            {
                if (_roadsBetweenNodes[roadBetween].Key == roadNode || _roadsBetweenNodes[roadBetween].Value == roadNode)
                {
                    _roadsBetweenNodes[roadBetween] = FindNearestRoadNodes(roadBetween);
                }
            }
            var pathCost = CalculatePathCost(_roadsBetweenNodes[roadNode].Key, _roadsBetweenNodes[roadNode].Value);
            _graph.AddConnection(_roadNodes[_roadsBetweenNodes[roadNode].Key], _roadNodes[_roadsBetweenNodes[roadNode].Value], pathCost);
        }

        public static bool ShouldNodeBeDeleted(RoadPlatform road)
        {
            return road.Neighbours.Count == 2 &&
                   Vector3.Dot(road.NeighboursDirs[0], road.NeighboursDirs[1]) != 0;
        }

        public KeyValuePair<RoadPlatform, RoadPlatform>? GetNodesForRoadBetween(RoadPlatform road)
        {
            if (_roadsBetweenNodes.ContainsKey(road)) return _roadsBetweenNodes[road];
            return null;
        }

        private KeyValuePair<RoadPlatform, RoadPlatform> FindNearestRoadNodes(RoadPlatform start)
        {
            var nearestRoadNodes = new List<RoadPlatform>();
            if (start.Neighbours.Count != 2) throw new ArgumentException("It is prohibited to delete useful node");

            foreach (var dir in start.NeighboursDirs)
            {
                var next = start.Neighbours[dir];
                while (!_roadNodes.ContainsKey(next) && next.Neighbours.ContainsKey(dir))
                {
                    next = next.Neighbours[dir];
                }
                if (_roadNodes.ContainsKey(next)) nearestRoadNodes.Add(next);
            }
            if (nearestRoadNodes.Count != 2)
            {
                throw new ArgumentException("This is a roadNode and it shouldn't be in roadsBetween");
            }
            return new KeyValuePair<RoadPlatform, RoadPlatform>(nearestRoadNodes[0], nearestRoadNodes[1]);
        }

        private float CalculatePathCost (RoadPlatform start, RoadPlatform finish)
        {
            foreach (var dir in start.NeighboursDirs)
            {
                var next = start.Neighbours[dir];
                var cost = start.cost + next.cost;
                while (!_roadNodes.ContainsKey(next) && next.Neighbours.ContainsKey(dir))
                {
                    next = next.Neighbours[dir];
                    cost += next.cost;
                }
                if (next == finish) return cost;
            }
            throw new ArgumentException("There is no path between these roadNodes");
        }

        private List<RoadPlatform> ConvertNodesToRoads(List<Node> nodes)
        {
            var result = new List<RoadPlatform>();
            foreach (var node in nodes)
            {
                if (!_nodeRoads.ContainsKey(node))
                {
                    throw new ArgumentException("This node wasn't presented in graph");
                }
                if (_nodeRoads[node] == null)
                {
                    throw new ArgumentException("Null in nodeRoads");
                }
                result.Add(_nodeRoads[node]);
            }
            return result;
        }

        private void UpdateDanger(Dictionary<RoadPlatform, float> roadChanges)
        {
            foreach (var road in roadChanges.Keys)
            {
                if (_roadNodes.ContainsKey(road))
                {
                    _graph.UpdateCost(_roadNodes[road], roadChanges[road]);
                }
                else if (_roadsBetweenNodes.ContainsKey(road))
                {
                    if (!_roadNodes.ContainsKey(_roadsBetweenNodes[road].Key) || !_roadNodes.ContainsKey(_roadsBetweenNodes[road].Value)) 
                    {
                        throw new ArgumentException("Node in roadBetweenNodes value isn't in roadNodes");
                    }
                    _graph.UpdateCost(_roadNodes[_roadsBetweenNodes[road].Key], _roadNodes[_roadsBetweenNodes[road].Value], roadChanges[road]);
                }
            }

            enemyManager.AwareEnemies(roadChanges.Keys);
        }

        public void UpdateDangerInRadius(Vector3 center, float radius, float dangerChange)
        {
            var collidersInRadius = new Collider[200];
            var roads = new Dictionary<RoadPlatform, float>();
            int count = Physics.OverlapSphereNonAlloc(center, radius, collidersInRadius);
            for (int i = 0; i < count; i++)
            {
                var road = collidersInRadius[i].GetComponent<RoadPlatform>();
                if (road == null) continue;
                road.cost += dangerChange;
                if (road.cost < 0)
                {
                    throw new Exception("Road cost is less than 0");
                }
                roads[road] = dangerChange;
            }
            UpdateDanger(roads);
        }
    }
}


