using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkGhostTargeting : GhostTargeting
{
    [SerializeField] private int predictionTiles = 4;

    public override Vector2? GetTargetTile(Vector2Int position)
    {
        return PlayerMovement.Position + PlayerMovement.Direction * predictionTiles;
    }
}