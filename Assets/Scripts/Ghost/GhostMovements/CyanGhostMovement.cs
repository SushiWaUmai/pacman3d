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

    Vector2 target;


    protected override void CalculateChaseMovementDirection(Vector2Int position, List<Vector2Int> validDirections)
    {
        Vector2 playerPos = PlayerMovement.Position + PlayerMovement.Direction * predictionTiles;
        Vector2Int redPos = Map.PositionToIndex(redGhostTransform.position);
        Vector2 targetPos = (playerPos - redPos) * 2 + redPos;

        target = targetPos;

        Vector2Int bestDir = ShortestPath(validDirections, position, targetPos);
        ApplyDirection(bestDir);
    }

    protected void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(target.x * 2, 0, target.y * 2), 3);
    }
}