using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GhostTargeting : MonoBehaviour
{
    public abstract Vector2? GetTargetTile(Vector2Int position);
}