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

    protected override void Init()
    {
        base.Init();
        OnPowerPelletCollect.AddListener(() => current = MovementMode.Frightened);
    }

    protected override void Move()
    {
        base.Move();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Intersection"))
        {
            CalculateDirection(other.transform.position);
        }
    }

    protected abstract void CalculateDirection(Vector3 position);
}