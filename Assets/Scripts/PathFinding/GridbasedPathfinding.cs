using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridbasedPathfinding : MonoBehaviour
{
    public static GridbasedPathfinding instance;

    private NodeGrid grid;

    private void Awake() => instance = this;

    public void CreateGrid(bool[,] walls)
    {
        int mapWidth = walls.GetLength(0);
        int mapHeight = walls.GetLength(1);
        grid = new NodeGrid(mapWidth, mapHeight);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                grid[x, y] = new Node(!walls[x, y], new Vector2Int(x, y));
            }
        }
    }

    private void ResetGrid()
    {
        foreach (Node n in grid)
        {
            n.gCost = 0;
            n.hCost = 0;
            n.parent = null;
        }
    }

    public bool FindPath(Vector2Int startPoint, Vector2Int endPoint, out List<Vector2Int> result)
    {
        ResetGrid();

        Heap<Node> openNodes = new Heap<Node>(grid.height * grid.width);
        HashSet<Node> closedNodes = new HashSet<Node>();
        Node current = null;

        openNodes.Add(grid[startPoint.x, startPoint.y]);

        while (true)
        {
            if (openNodes.Count <= 0)
                break;

            current = openNodes.RemoveFirst();
            closedNodes.Add(current);

            if (current.gridIndex == grid[endPoint.x, endPoint.y].gridIndex)
                break;

            foreach (Node neighbour in grid.GetNeighbour(current.gridIndex.x, current.gridIndex.y))
            {
                if (!neighbour.walkable || closedNodes.Contains(neighbour))
                    continue;

                if (neighbour.gCost > current.gCost + 1 || !openNodes.Contains(neighbour))
                {
                    neighbour.hCost = FindDistance(neighbour.gridIndex, endPoint);
                    neighbour.gCost = current.gCost + 1;
                    
                    neighbour.parent = current;

                    if (!openNodes.Contains(neighbour))
                        openNodes.Add(neighbour);
                }
            }
        }

        result = new List<Vector2Int>();

        result.Add(current.gridIndex);
        while (current.parent != null)
        {
            result.Add(current.parent.gridIndex);
            current = current.parent;
        }

        result.Reverse();

        return openNodes.Count >= 0;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> result;
        FindPath(start, end, out result);
        return result;
    }

    public bool ExistsPath(Vector2Int start, Vector2Int end)
    {
        return FindPath(start, end, out List<Vector2Int> result);
    }

    private int FindDistance(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(from.x + to.x) + Mathf.Abs(from.y + to.y);
    }
}