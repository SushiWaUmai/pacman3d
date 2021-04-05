using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class Ghost : MonoBehaviour
{
    private GhostMovement ghostmvment;
    [SerializeField] private IntVariable totalScore;
    private bool isDead;

    private void Start()
    {
        ghostmvment = GetComponent<GhostMovement>();
        ghostmvment.OnGhostRecover += () => isDead = false;
    }

    public void Die()
    {
        if (!isDead)
        {
            totalScore.Value += 2000;
            Debug.Log($"Ghost {gameObject.name} Died");
            ghostmvment.GetEaten();
            isDead = true;
        }
    }
}