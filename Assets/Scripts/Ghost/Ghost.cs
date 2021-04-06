using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class Ghost : MonoBehaviour
{
    private GhostMovement ghostmvment;
    [SerializeField] private IntVariable totalScore;
    private bool isDead;

    public bool IsEaten => ghostmvment.IsEaten;

    [Header("Scoring")]
    [SerializeField] private int killScore = 200;
    private static int killedGhost;

    public static void ResetKillCount() => killedGhost = 0;

    private void Start()
    {
        ghostmvment = GetComponent<GhostMovement>();
        ghostmvment.OnGhostRecover += () => isDead = false;
    }

    public void Die()
    {
        if (!isDead)
        {
            killedGhost++;
            totalScore.Value += (int)Mathf.Pow(killScore / 100, killedGhost) * 100;
            ghostmvment.GetEaten();

            isDead = true;
        }
    }
}