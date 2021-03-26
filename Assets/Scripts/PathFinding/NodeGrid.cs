using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : IEnumerable
{
    private Node[,] grid;

    public int width => grid.GetLength(0);
    public int height => grid.GetLength(1);

    public NodeGrid(int x, int y)
    {
        grid = new Node[x, y];
    }

    public Node this[int x, int y]
    {
        get
        {
            return grid[x, y];
        }
        set
        {
            grid[x, y] = value;
        }
    }

    public List<Node> GetNeighbour(int x, int y)
    {
        List<Node> result = new List<Node>();

        if (x < width - 1)
            result.Add(grid[x + 1, y]);
        if (x > 0)
            result.Add(grid[x - 1, y]);
        if (y < height - 1)
            result.Add(grid[x, y + 1]);
        if (y > 0)
            result.Add(grid[x, y - 1]);

        return result;
    }

    public IEnumerator GetEnumerator()
    {
        return grid.GetEnumerator();
    }
}