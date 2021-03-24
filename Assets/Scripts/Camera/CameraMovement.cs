using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CameraMovement : MonoBehaviour
{
    [HideInInspector, ShowNonSerializedField] public float xRotation;
    [SerializeField] private float rotationClamp;
    [ShowNonSerializedField] private Transform camTransform;

    private void Start()
    {
        camTransform = GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        xRotation = Mathf.Clamp(xRotation, -rotationClamp, rotationClamp);

        camTransform.localRotation = Quaternion.AngleAxis(xRotation, Vector3.right);
    }
}