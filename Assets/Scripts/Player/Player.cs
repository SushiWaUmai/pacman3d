using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using UnityTimer;
using NaughtyAttributes;

public class Player : MonoBehaviour
{
    [SerializeField] private GameEvent OnPowerPalletCollect;
    [SerializeField] private GameEvent OnPowerPalletEnd;
    [SerializeField] private GameEvent OnPlayerDie;
    [SerializeField] private float powerPalletTime;
    [ShowNonSerializedField] private bool canKillGhost;

    [Header("Post Processing")]
    [SerializeField] private PlayerPostProcessing playerPostProcessing;
    [SerializeField] private LayerMask ghostLayer;
    [SerializeField] private float maxDistance = 10;

    private Timer powerPelletTimer;

    private void Start()
    {
        OnPowerPalletCollect.AddListener(PowerPelletCollect);
        OnPowerPalletEnd.AddListener(PowerPelletEnd);
        Ghost.ResetKillCount();
    }

    private void PowerPelletCollect()
    {
        canKillGhost = true;
        Ghost.ResetKillCount();
        playerPostProcessing.SetGlitchIntensity(0);

        if (powerPelletTimer != null)
            powerPelletTimer.Cancel();

        powerPelletTimer = this.AttachTimer(powerPalletTime, OnPowerPalletEnd.Raise);
    }

    private void PowerPelletEnd()
    {
        canKillGhost = false;
    }

    private void FixedUpdate()
    {
        if (!canKillGhost)
        {
            Collider[] ghosts = Physics.OverlapSphere(transform.position, maxDistance, ghostLayer);

            float closestGhostSqrDistance = float.MaxValue;
            foreach (Collider c in ghosts)
            {
                float sqrMag = (c.transform.position - transform.position).sqrMagnitude;
                if (sqrMag < closestGhostSqrDistance)
                    closestGhostSqrDistance = sqrMag;
            }
            float dist = Mathf.Sqrt(closestGhostSqrDistance);
            float val = 1 - Mathf.Clamp01(dist / maxDistance);

            playerPostProcessing.SetGlitchIntensity(val);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Ghost ghost))
        {
            if (canKillGhost)
            {
                ghost.Die();
            }
            else
            {
                Die();
            }
        }
    }
    
    private void Die()
    {
        OnPlayerDie.Raise();
        playerPostProcessing.GlitchOut();
        Time.timeScale = 0;
    }
}