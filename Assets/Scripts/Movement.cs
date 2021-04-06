using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using ScriptableObjectArchitecture;

[RequireComponent(typeof(Rigidbody))]
public class Movement : PortalTraveller
{
    [HideInInspector, ShowNonSerializedField] public Vector3 dir;
    [HideInInspector, ShowNonSerializedField] public float entityRotation;
    [SerializeField] protected float speed;
    private Rigidbody rb;

    [SerializeField] private IntGameEvent OnPlayerDie;
    [SerializeField] protected BoolVariable canInteract;
    [SerializeField] private GameEvent OnGameClear;
    private Vector3 origPosition;
    private float origRotation;

    private void Start() => Init();

    protected virtual void Init()
    {
        rb = GetComponent<Rigidbody>();

        origPosition = transform.position;
        origRotation = entityRotation;
        OnPlayerDie.AddListener(ResetPosition);
        OnGameClear.AddListener(StopMovement);
    }

    private void OnDestroy() => Destruct();

    protected virtual void Destruct()
    {
        OnPlayerDie.RemoveListener(ResetPosition);
        OnGameClear.RemoveListener(StopMovement);
    }

    protected void MovementUpdate()
    {
        if (canInteract.Value)
        {
            rb.velocity = dir * speed;
            transform.rotation = Quaternion.AngleAxis(entityRotation, Vector2.up);
        }
    }

    private void ResetPosition()
    {
        transform.position = origPosition;
        entityRotation = origRotation;
        StopMovement();
    }

    private void StopMovement()
    {
        rb.velocity = Vector3.zero;
    }
}