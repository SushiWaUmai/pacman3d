using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyanGhostTargeting : GhostTargeting
{
    private Transform redGhostTransform;
    [SerializeField] private int predictionTiles = 2;

    private void Start()
    {
        RedGhostTargeting[] rgms = FindObjectsOfType<RedGhostTargeting>();
        if(rgms.Length == 0)
        {
            Destroy(gameObject);
            return;
        }
        redGhostTransform = rgms[Random.Range(0, rgms.Length)].transform;
    }

    public override Vector2? GetTargetTile(Vector2Int position)
    {
        Vector2 playerPos = PlayerMovement.Position + PlayerMovement.Direction * predictionTiles;
        Vector2Int redPos = Map.PositionToIndex(redGhostTransform.position);
        return (playerPos - redPos) * 2 + redPos;
    }
}