using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkGhostMovement : GhostMovement
{
    [SerializeField] private int predictionTiles = 4;

    protected override void UpateChaseTargetTile(Vector2Int position)
    {
        targetTile = PlayerMovement.Position + PlayerMovement.Direction * predictionTiles;
    }
}
