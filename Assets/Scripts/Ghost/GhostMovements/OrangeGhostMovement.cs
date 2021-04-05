using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeGhostMovement : GhostMovement
{
    [SerializeField] private float distanceToPlayerForScatter = 8;

    protected override void UpateChaseTargetTile(Vector2Int position)
    {
        if(distanceToPlayerForScatter * distanceToPlayerForScatter > (PlayerMovement.Position - position).sqrMagnitude)
        {
            targetTile = PlayerMovement.Position;
        }
    }
}
