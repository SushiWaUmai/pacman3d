using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyanGhostMovement : GhostMovement
{
    private Transform redGhostTransform;
    [SerializeField] private int predictionTiles = 2;

    protected override void Init()
    {
        base.Init();
        RedGhostMovement[] rgms = FindObjectsOfType<RedGhostMovement>();
        redGhostTransform = rgms[Random.Range(0, rgms.Length)].transform;
    }

    protected override void UpateChaseTargetTile(Vector2Int position)
    {
        Vector2 playerPos = PlayerMovement.Position + PlayerMovement.Direction * predictionTiles;
        Vector2Int redPos = Map.PositionToIndex(redGhostTransform.position);
        targetTile = (playerPos - redPos) * 2 + redPos;
    }
}