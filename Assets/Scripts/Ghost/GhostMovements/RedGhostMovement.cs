using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RedGhostMovement : GhostMovement
{
    protected override void CalculateChaseMovementDirection(Vector2Int position, List<Vector2Int> validDirections)
    {
        Vector2Int bestDir = ShortestPath(validDirections, position, PlayerMovement.Position);
        ApplyDirection(bestDir);
    }

    protected override void ScatterMovement(Vector2Int position, List<Vector2Int> validDirections)
    {
        CalculateChaseMovementDirection(position, validDirections);
    }
}