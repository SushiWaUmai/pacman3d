using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector2Int gridIndex;
    public Node parent;
    public int gCost;
    public int hCost;
    private int heapIndex;

    public int fCost => gCost + hCost;

    public int HeapIndex { get => heapIndex; set => heapIndex = value; }

    public Node(bool walkable, Vector2Int gridIndex)
    {
        this.walkable = walkable;
        this.gridIndex = gridIndex;
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}
