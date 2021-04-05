using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedGhostTargeting : GhostTargeting
{
    public override Vector2? GetTargetTile(Vector2Int position)
    {
        return PlayerMovement.Position;
    }
}