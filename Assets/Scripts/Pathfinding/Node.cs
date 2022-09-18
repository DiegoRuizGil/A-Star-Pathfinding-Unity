using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    /*
     * G cost --> known cost from starting node (distance)
     * H cost --> (herusitic) distance from end node (Manhatten distance)
     * F cost --> G cost + H cost
    */
    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    public int gridX;
    public int gridY;

    public bool isWall;
    public Vector3 Position;

    public Node nodeParent;

    public List<Node> neighbours;

    public Node(int gridX, int gridY, bool isWall, Vector3 position)
    {
        this.gridX = gridX;
        this.gridY = gridY;
        this.isWall = isWall;
        this.Position = position;

        // avoid overflow for fCost
        this.gCost = int.MaxValue / 2;
        this.hCost = int.MaxValue / 2;
    }

    public override string ToString()
    {
        return "(" + gridX + ", " + gridY + ")";
    }
}
