using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkGhostMovement : GhostMovement
{
    [SerializeField] private int predictionTiles = 4;

    protected override void CalculateChaseMovementDirection(Vector2Int position, List<Vector2Int> validDirections)
    {
        Vector2Int bestDir = ShortestPath(validDirections, position, PlayerMovement.Position + PlayerMovement.Direction * predictionTiles);
        ApplyDirection(bestDir);
    }
}
