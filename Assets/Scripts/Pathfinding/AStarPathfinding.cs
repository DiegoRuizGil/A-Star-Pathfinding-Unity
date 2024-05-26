using System.Collections.Generic;
using System.Linq;
using GridSystem;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Pathfinding
{
    public static class AStarPathfinding
    {
        private static readonly int STRAIGHT_MOVE_COST = 10;
        private static readonly int DIAGONAL_MOVE_COST = 15;

        private static List<Node> _openList;
        private static List<Node> _closedList;

        public static List<Node> GetPath(Node startNode, Node finalNode, Grid grid)
        {
            // reset nodes costs
            ResetNodes(grid);

            _openList = new List<Node>() { startNode };
            _closedList = new List<Node>();

            startNode.GCost = 0;
            startNode.HCost = GetHeuristic(startNode.Position, finalNode.Position);

            Node currentNode = null;
            while (_openList.Count > 0 || currentNode == finalNode) // || currentNode == finalNode
            {
                currentNode = _openList.OrderBy(node => node.FCost).FirstOrDefault();
                _closedList.Add(currentNode);
                _openList.Remove(currentNode);
                
                // stop condition
                if (currentNode == finalNode) break;

                foreach (var nextNode in currentNode.neighbours)
                {
                    int actionCost = GetCost(currentNode, nextNode);
                    
                    if (_openList.Contains(nextNode) && (currentNode.GCost + actionCost) < nextNode.GCost)
                    {
                        nextNode.nodeParent = currentNode;
                        nextNode.GCost = currentNode.GCost + actionCost;
                        nextNode.HCost = GetHeuristic(nextNode.Position, finalNode.Position);
                    }
                    else if (_closedList.Contains(nextNode) && (currentNode.GCost + actionCost) < nextNode.GCost)
                    {
                        nextNode.nodeParent = currentNode;
                        nextNode.GCost = currentNode.GCost + actionCost;
                        nextNode.HCost = GetHeuristic(nextNode.Position, finalNode.Position);

                        _closedList.Remove(nextNode);
                        _openList.Add(nextNode);
                    }
                    else if (!_openList.Contains(nextNode) && !_closedList.Contains(nextNode))
                    {
                        nextNode.nodeParent = currentNode;
                        nextNode.GCost = currentNode.GCost + actionCost;
                        nextNode.HCost = GetHeuristic(nextNode.Position, finalNode.Position);
                        
                        _openList.Add(nextNode);
                    }
                }
            }

            List<Node> path = new List<Node>();
            if (finalNode.nodeParent != null) // path founded
            {
                Node nodeToAdd = finalNode;
                path.Add(nodeToAdd);
                while (nodeToAdd.nodeParent != null)
                {
                    nodeToAdd = nodeToAdd.nodeParent;
                    path.Add(nodeToAdd);
                }
            }

            path.Reverse();
            return path;
        }

        private static void ResetNodes(Grid grid)
        {
            foreach (var node in grid.nodes)
            {
                node.Reset();
            }
        }

        private static int GetHeuristic(Vector3 startPosition, Vector3 finalPosition)
        {
            // Manhattan distance
            float xValue = Mathf.Abs(finalPosition.x - startPosition.x);
            float yValue = Mathf.Abs(finalPosition.y - startPosition.y);

            return Mathf.FloorToInt(xValue + yValue);
        }

        private static int GetCost(Node a, Node b)
        {
            if (a == b)
                return 0;

            if (a.gridX == b.gridX || a.gridY == b.gridY)
                return STRAIGHT_MOVE_COST;
            
            return DIAGONAL_MOVE_COST;
        }
    }
}