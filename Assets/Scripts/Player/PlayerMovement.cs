using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    private static PlayerMovement instance;
    public static Vector2Int Direction
    {
        get
        {
            Vector3 absoluteDir = instance.transform.TransformDirection(instance.dir);
            Vector2 dir2D = new Vector2(absoluteDir.x, absoluteDir.z).normalized;
            float absX = Mathf.Abs(dir2D.x);
            float absY = Mathf.Abs(dir2D.y);
            if(absX > absY)
            {
                return new Vector2Int(System.Math.Sign(dir2D.x), 0);
            }
            else if (absY > absX)
            {
                return new Vector2Int(0, System.Math.Sign(dir2D.y));
            }

            return Vector2Int.zero;
        }
    }

    public static Vector2Int Position
    {
        get
        {
            return Map.PositionToIndex(instance.transform.position);
        }
    }

    private void Awake() => instance = this;

    protected override void Init()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        base.Init();
    }
}
