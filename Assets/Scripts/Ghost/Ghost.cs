using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public void Die()
    {
        Debug.Log($"Ghost {gameObject.name} Died");
    }
}