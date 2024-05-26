using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Grid = GridSystem.Grid;
using Node = GridSystem.Node;

public class PathfindingBehaviour : MonoBehaviour
{
    [Header("Configuration")]
    public Transform pointA;
    public Transform pointB;
    public int gridWidth = 4;
    public int gridHeight = 2;
    public float cellSize = 1f;
    public Vector3 originPosition = Vector3.zero;
    public LayerMask wallLayer;

    [Header("Path points")]
    public Transform startPoint;
    public Transform finalPoint;

    [Header("Debug")]
    public Transform debugPoint;

    [Header("Events")]
    public UnityEvent<Vector3[]> onPathGenerated;
    public UnityEvent<int> onPathErrors;

    private Grid grid;
    private List<Node> openList;
    private List<Node> closedList;
    private List<Node> solution = new List<Node>();
    private Vector3[] pathToSend = new Vector3[0];
    private const int STRAIGHT_MOVE_COST = 10;
    private const int DIAGONAL_MOVE_COST = 15;

    void Start()
    {
        Debug.Log("buenas");
        grid = new Grid(gridWidth, gridHeight, cellSize, originPosition, wallLayer);
        grid.DrawGrid();
    }

    private void Update()
    {
        if (grid.GetNode(startPoint.position) == null || grid.GetNode(finalPoint.position) == null)
        {
            Debug.LogError("Pathfinding: The start/final point it's out of bounds of the grid");
            if (onPathErrors != null)
                onPathErrors.Invoke(0);
        }
        else
        {
            SetPath(startPoint.position, finalPoint.position);
            //DrawPath();
        }
    }

    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            //for (int i = 0; i < gridWidth; i++)
            //{
            //    for (int j = 0; j < gridHeight; j++)
            //    {
            //        if (solution.Contains(grid.nodes[i, j]))
            //            Gizmos.color = new Color(0, 1, 0, 0.5f);
            //        else if (grid.nodes[i, j].isWall)
            //            Gizmos.color = new Color(1, 0, 1, 0.5f);
            //        else
            //            Gizmos.color = new Color(0, 0, 0, 0.5f);

            //        Vector3 nodePosition = grid.GetNodePosition(i, j);
            //        Vector3 center = new Vector3(nodePosition.x + (cellSize * 1f / 2), nodePosition.y + (cellSize * 1f / 2), nodePosition.z);
            //        Gizmos.DrawCube(center, new Vector3(cellSize, cellSize, 0));
            //    }
            //}

            if (solution.Count < 2)
            {
                Debug.Log("No hay camino para dibujar");
            }
            else
            {
                Gizmos.color = Color.green;

                for (int i = 1; i < solution.Count; i++)
                {
                    Vector3 currentPosition = GetNodeCenter(solution[i - 1]);
                    Vector3 nextPosition = GetNodeCenter(solution[i]);

                    Gizmos.DrawLine(currentPosition, nextPosition);
                }
            }
        }
    }

    private void SetPath(Vector3 startPosition, Vector3 finalPosition)
    {
        Node startNode = grid.GetNode(startPosition);
        Node finalNode = grid.GetNode(finalPosition);

        openList = new List<Node>() { startNode };
        closedList = new List<Node>();

        startNode.GCost = 0;
        startNode.HCost = GetHeuristic(startPosition, finalPosition);

        while (openList.Count > 0)
        {
            List<Node> sortedOpenNodes = openList.OrderBy(node => node.FCost).ToList();
            Node currentNode = sortedOpenNodes[0];
            closedList.Add(currentNode);
            openList.Remove(currentNode);

            // Stop condition
            if (currentNode == finalNode)
            {
                solution = GetNodesParent(finalNode);
                ResetNodes();

                if(onPathGenerated != null && solution.Count > 0)
                {
                    Vector3[] generatedPath = GetArrayPath();
                    if(pathToSend.Intersect(generatedPath).Count() != generatedPath.Length)
                    {
                        pathToSend = generatedPath;
                        onPathGenerated.Invoke(pathToSend);
                    }
                        
                }

                return;
            }

            List<Node> neighbours = currentNode.neighbours;
            foreach (Node nextNode in neighbours)
            {
                int actionCost = GetCost(currentNode, nextNode);

                if (openList.Contains(nextNode) && (currentNode.GCost + actionCost) < nextNode.GCost)
                {
                    nextNode.nodeParent = currentNode;
                    nextNode.GCost = currentNode.GCost + actionCost;
                    nextNode.HCost = GetHeuristic(nextNode, finalNode);
                }
                else if (closedList.Contains(nextNode) && (currentNode.GCost + actionCost) < nextNode.GCost)
                {
                    nextNode.nodeParent = currentNode;
                    nextNode.GCost = currentNode.GCost + actionCost;
                    nextNode.HCost = GetHeuristic(nextNode, finalNode);

                    closedList.Remove(nextNode);
                    openList.Add(nextNode);
                }
                else if (!openList.Contains(nextNode) && !closedList.Contains(nextNode))
                {
                    nextNode.nodeParent = currentNode;
                    nextNode.GCost = currentNode.GCost + actionCost;
                    nextNode.HCost = GetHeuristic(nextNode, finalNode);

                    openList.Add(nextNode);
                }
            }
        }

        Debug.LogError("Pathfinding: There is no pat");
        if (onPathErrors != null)
            onPathErrors.Invoke(0);
    }

    private Vector3[] GetArrayPath()
    {
        Vector3[] path = new Vector3[solution.Count];
        float offset = cellSize / 2;

        for (int i = 0; i < solution.Count; i++)
        {
            Vector3 nodePosition = grid.GetNodePosition(solution[i]);
            path[i] = new Vector3(nodePosition.x + offset, nodePosition.y + offset, 0);
        }

        return path;
    }

    private void ResetNodes()
    {
        if (openList == null || closedList == null)
            return;
        if (openList.Count == 0 && closedList.Count == 0)
            return;

        List<Node> visitedNodes = new List<Node>();
        visitedNodes.AddRange(openList);
        visitedNodes.AddRange(closedList);

        foreach (Node node in visitedNodes)
        {
            node.GCost = int.MaxValue / 2;
            node.HCost = int.MaxValue / 2;
            node.nodeParent = null;
        }
    }

    private Vector3 GetNodeCenter(Node node)
    {
        return grid.GetNodePosition(node) + new Vector3(cellSize / 2, cellSize / 2, 0);
    }

    private List<Node> GetNodesParent(Node node)
    {
        if (node.nodeParent == null)
            return new List<Node>() { node };
        else
        {
            List<Node> list = new List<Node>();
            list.AddRange(GetNodesParent(node.nodeParent));
            list.Add(node);
            return list;
        }
    }

    private int GetCost(Node a, Node b)
    {
        if (a == b)
            return 0;
        else if (a.gridX == b.gridX || a.gridY == b.gridY)
            return STRAIGHT_MOVE_COST;
        else
            return DIAGONAL_MOVE_COST;
    }

    private int GetHeuristic(Node startNode, Node finalNode)
    {
        Vector3 startPosition = grid.GetNodePosition(startNode.gridX, startNode.gridY);
        Vector3 finalPosition = grid.GetNodePosition(finalNode.gridX, finalNode.gridY);

        return GetHeuristic(startPosition, finalPosition);
    }

    private int GetHeuristic(Vector3 startPosition, Vector3 finalPosition)
    {
        float xValue = Mathf.Abs(finalPosition.x - startPosition.x);
        float yValue = Mathf.Abs(finalPosition.y - startPosition.y);

        return Mathf.FloorToInt(xValue + yValue);
    }
}
