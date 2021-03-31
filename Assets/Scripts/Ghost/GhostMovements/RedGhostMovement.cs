using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RedGhostMovement : GhostMovement
{
    protected override void CalculateDirection(Vector2Int position)
    {
        GridbasedPathfinding.instance.FindPath(position, PlayerMovement.Position);
    }
}