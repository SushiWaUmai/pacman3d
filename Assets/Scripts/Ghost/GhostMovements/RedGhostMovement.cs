using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RedGhostMovement : GhostMovement
{
    protected override void UpateChaseTargetTile(Vector2Int position)
    {
        targetTile = PlayerMovement.Position;
    }
}