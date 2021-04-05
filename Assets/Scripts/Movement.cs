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
    [SerializeField] private float speed;
    private Rigidbody rb;

    [SerializeField] private IntGameEvent OnPlayerDie;
    [SerializeField] protected BoolVariable isRespawning;
    private Vector3 origPosition;
    private float origRotation;

    private void Start() => Init();

    protected virtual void Init()
    {
        rb = GetComponent<Rigidbody>();

        origPosition = transform.position;
        origRotation = entityRotation;
        OnPlayerDie.AddListener(ResetPosition);
    }

    private void OnDestroy() => Destruct();

    protected virtual void Destruct()
    {
        OnPlayerDie.RemoveListener(ResetPosition);
    }

    protected void MovementUpdate()
    {
        if (!isRespawning.Value)
        {
            rb.velocity = dir * speed;
            transform.rotation = Quaternion.AngleAxis(entityRotation, Vector2.up);
        }
    }

    private void ResetPosition()
    {
        transform.position = origPosition;
        entityRotation = origRotation;
        rb.velocity = Vector3.zero;
    }
}