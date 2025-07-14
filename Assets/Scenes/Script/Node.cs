using UnityEngine;
using System;

public class Node : IComparable<Node>
{
    public Vector2Int gridPos;
    public bool walkable = true;
    public Node parent;

    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;

    public Node(Vector2Int pos, bool walkable = true)
    {
        this.gridPos = pos;
        this.walkable = walkable;
    }

    public override bool Equals(object obj)
    {
        return obj is Node node && node.gridPos == this.gridPos;
    }

    public override int GetHashCode()
    {
        return gridPos.GetHashCode();
    }

    public int CompareTo(Node other)
    {
        int result = fCost.CompareTo(other.fCost);
        return result == 0 ? hCost.CompareTo(other.hCost) : result;
    }
}