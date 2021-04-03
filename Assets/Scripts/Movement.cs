using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody))]
public class Movement : PortalTraveller
{
    [HideInInspector, ShowNonSerializedField] public Vector3 dir;
    [HideInInspector, ShowNonSerializedField] public float entityRotation;
    [SerializeField] private float speed;
    private Rigidbody rb;

    private void Start()
    {
        Init();   
    }

    protected virtual void Init()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected void MovementUpdate()
    {
        rb.velocity = dir * speed;
        transform.rotation = Quaternion.AngleAxis(entityRotation, Vector2.up);
    }
}