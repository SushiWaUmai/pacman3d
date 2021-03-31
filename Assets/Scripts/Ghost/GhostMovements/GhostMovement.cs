using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public abstract class GhostMovement : Movement
{
    public enum MovementMode
    {
        Chase,
        Scatter,
        Frightened
    }

    [SerializeField] protected MovementMode current;
    [SerializeField] private GameEvent OnPowerPelletCollect;
    [SerializeField] private float minTileDistance = 0.1f;

    protected override void Init()
    {
        base.Init();
        OnPowerPelletCollect.AddListener(() => current = MovementMode.Frightened);
    }

    protected override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Vector3 ghostPos = Vector3.Scale(new Vector3(1, 0, 1), transform.position);
        Vector3 alignedPos = Map.AlignToGrid(transform.position);
        Vector3 checkPos = Vector3.Scale(new Vector3(1, 0, 1), alignedPos);
        if ((ghostPos - checkPos).sqrMagnitude < minTileDistance * minTileDistance)
        {
            transform.position = alignedPos;
            CalculateDirection(Map.PositionToIndex(alignedPos));
        }
    }

    protected abstract void CalculateDirection(Vector2Int position);
}