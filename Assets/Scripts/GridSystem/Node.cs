using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class Node
    {
        /*
         * G cost --> known cost from starting node (distance)
         * H cost --> (herusitic) distance from end node (Manhatten distance)
         * F cost --> G cost + H cost
        */
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost;

        public int gridX;
        public int gridY;

        public bool isWall;
        public Vector3 Position;
        public float Size { get; private set; }

        public Vector3 CenterPosition => Position + new Vector3(Size / 2, Size / 2, 0f);

        public Node nodeParent;

        public List<Node> neighbours;

        public Node(int gridX, int gridY, bool isWall, Vector3 position, float size)
        {
            this.gridX = gridX;
            this.gridY = gridY;
            this.isWall = isWall;
            this.Position = position;
            Size = size;

            // avoid overflow for fCost
            this.GCost = int.MaxValue / 2;
            this.HCost = int.MaxValue / 2;
        }

        public void Reset()
        {
            GCost = int.MaxValue / 2;
            HCost = int.MaxValue / 2;
            nodeParent = null;
        }

        public override string ToString()
        {
            // return "(" + gridX + ", " + gridY + ") => ";
            return $"({gridX}, {gridY}) => {Position} // {CenterPosition}";
        }
    }
}
