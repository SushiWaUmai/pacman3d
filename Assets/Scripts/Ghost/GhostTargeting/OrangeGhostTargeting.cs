using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeGhostTargeting : GhostTargeting
{
    [SerializeField] private float distanceToPlayerForScatter = 8;

    public override Vector2? GetTargetTile(Vector2Int position)
    {
        if (distanceToPlayerForScatter * distanceToPlayerForScatter > (PlayerMovement.Position - position).sqrMagnitude)
        {
            return PlayerMovement.Position;
        }

        return null;
    }
}