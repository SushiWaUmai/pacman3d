using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    protected override void Init()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        base.Init();
    }
}
