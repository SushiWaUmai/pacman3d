using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(CameraMovement))]
public class PlayerInput : MonoBehaviour
{
    private PlayerMovement plMovement;
    private CameraMovement camMovement;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private BoolVariable canInteract;

    private void Start()
    {
        plMovement = GetComponent<PlayerMovement>();
        camMovement = GetComponent<CameraMovement>();
    }

    private void Update()
    {
        if (canInteract.Value)
        {
            MoveInput();
            LookInput();
        }
    }

    private void MoveInput()
    {
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        moveInput = moveInput.sqrMagnitude < 1 ? moveInput : moveInput.normalized;

        plMovement.dir = transform.TransformDirection(moveInput);
    }

    private void LookInput()
    {
        Vector2 lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        lookInput *= mouseSensitivity;

        plMovement.entityRotation += lookInput.x;
        camMovement.xRotation -= lookInput.y;
    }
}