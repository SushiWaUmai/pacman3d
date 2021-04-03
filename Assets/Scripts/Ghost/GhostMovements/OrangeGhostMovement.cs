using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeGhostMovement : GhostMovement
{
    [SerializeField] private float distanceToPlayerForScatter = 8;

    protected override void CalculateChaseMovementDirection(Vector2Int position, List<Vector2Int> validDirections)
    {
        if(distanceToPlayerForScatter * distanceToPlayerForScatter > (PlayerMovement.Position - position).sqrMagnitude)
        {
            Vector2Int bestDir = ShortestPath(validDirections, position, PlayerMovement.Position);
            ApplyDirection(bestDir);
        }
        else
        {
            ScatterMovement(position, validDirections);
        }
    }
}
