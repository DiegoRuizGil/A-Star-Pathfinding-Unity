using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public int width;
    public int height;
    public float cellSize;
    public Node[,] nodes;
    public Vector3 originPosition;
    public LayerMask wallLayer;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, LayerMask wallLayer)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.wallLayer = wallLayer;

        nodes = new Node[width, height];

        // CheckWall function
        Debug.LogWarning("Pathfinding: If it doesn't detect the not wallkable colliders, make sure the 'Geometry Type' is 'Polygons'.");

        // Create nodes
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                bool isWall = false;
                Vector3 center = GetNodePosition(i, j) + new Vector3(this.cellSize / 2, this.cellSize / 2, 0);
                
                if (CheckWall(center, wallLayer))
                    isWall = true;

                // centro de cada casilla
                Debug.DrawLine(new Vector3(center.x - 0.1f, center.y), new Vector3(center.x + 0.1f, center.y), Color.red, Mathf.Infinity);
                Debug.DrawLine(new Vector3(center.x, center.y - 0.1f), new Vector3(center.x, center.y + 0.1f), Color.red, Mathf.Infinity);

                nodes[i, j] = new Node(i, j, isWall, GetNodePosition(i, j));
            }
        }

        // Set neighbours
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                nodes[i, j].neighbours = GetNeighbours(nodes[i, j]);
            }
        }
    }

    public void DrawGrid()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Debug.DrawLine(GetNodePosition(i, j), GetNodePosition(i, j + 1), Color.white, Mathf.Infinity);
                Debug.DrawLine(GetNodePosition(i, j), GetNodePosition(i + 1, j), Color.white, Mathf.Infinity);
            }
        }

        Debug.DrawLine(originPosition + new Vector3(0, height, 0), originPosition + new Vector3(width, height, 0), Color.white, Mathf.Infinity);
        Debug.DrawLine(originPosition + new Vector3(width, 0, 0), originPosition + new Vector3(width, height, 0), Color.white, Mathf.Infinity);

    }

    public Vector3 GetNodePosition(Node node)
    {
        return GetNodePosition(node.gridX, node.gridY);
    }

    public Vector3 GetNodePosition(int x, int y)
    {
        return new Vector3(x * cellSize, y * cellSize, 0) + originPosition;
    }

    public void SetNode(int x, int y, Node node)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
            this.nodes[x, y] = node;
    }

    public Node GetNode(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
            return nodes[x, y];
        else
            return null;
    }

    public Node GetNode(Vector3 position)
    {
        int x = Mathf.FloorToInt((position.x - originPosition.x) / cellSize);
        int y = Mathf.FloorToInt((position.y - originPosition.y) / cellSize);

        return GetNode(x,y);
    }
    
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        int nodeX = node.gridX;
        int nodeY = node.gridY;

        for (int i = nodeX - 1; i < nodeX + 2; i++)
        {
            for (int j = nodeY - 1; j < nodeY + 2; j++)
            {
                if(!(i == nodeX && j == nodeY))
                {
                    Node n = GetNode(i, j);
                    if (n != null)
                    {
                        if(!n.isWall && CheckDiagonals(node, n))
                            neighbours.Add(n);
                    }
                        
                }
            }
        }

        return neighbours;
    }

    public bool CheckDiagonals(Node origin, Node nextNode)
    {
        bool diagonalNotBlocked = true;

        // first quadrant
        if(nextNode.gridX > origin.gridX && nextNode.gridY > origin.gridY)
        {
            if (GetNode(nextNode.gridX - 1, nextNode.gridY).isWall && GetNode(nextNode.gridX, nextNode.gridY - 1).isWall)
                diagonalNotBlocked = false;
        }
        // second quadrant
        else if (nextNode.gridX < origin.gridX && nextNode.gridY > origin.gridY)
        {
            if (GetNode(nextNode.gridX + 1, nextNode.gridY).isWall && GetNode(nextNode.gridX, nextNode.gridY - 1).isWall)
                diagonalNotBlocked = false;
        }
        // third quadrant
        else if (nextNode.gridX < origin.gridX && nextNode.gridY < origin.gridY)
        {
            if (GetNode(nextNode.gridX + 1, nextNode.gridY).isWall && GetNode(nextNode.gridX, nextNode.gridY + 1).isWall)
                diagonalNotBlocked = false;
        }
        // fourth quadrant
        else if (nextNode.gridX > origin.gridX && nextNode.gridY < origin.gridY)
        {
            if (GetNode(nextNode.gridX - 1, nextNode.gridY).isWall && GetNode(nextNode.gridX, nextNode.gridY + 1).isWall)
                diagonalNotBlocked = false;
        }


        return diagonalNotBlocked;
    }

    public bool CheckWall(Vector3 position, LayerMask wallLayer)
    {
        return Physics2D.OverlapPoint(position, wallLayer);
    }

}
