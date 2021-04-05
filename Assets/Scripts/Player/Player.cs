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
    [SerializeField] private float powerPalletTime;
    [ShowNonSerializedField] private bool canKillGhost;

    [Header("Post Processing")]
    [SerializeField] private PlayerPostProcessing playerPostProcessing;
    [SerializeField] private LayerMask ghostLayer;
    [SerializeField] private float maxDistance = 10;

    [Header("Respawning")]
    [SerializeField] private IntGameEvent OnPlayerDie;
    [SerializeField] private float glitchoutTime;
    [SerializeField] private FloatVariable resetAnimationTime;
    [SerializeField] private BoolVariable isRespawning;
    [SerializeField] private IntVariable livesLeft;

    private Timer powerPelletTimer;

    private void Start()
    {
        OnPowerPalletCollect.AddListener(PowerPelletCollect);
        OnPowerPalletEnd.AddListener(PowerPelletEnd);
        OnPlayerDie.AddListener(OnDie);
        
        Ghost.ResetKillCount();

        isRespawning.Value = false;
        livesLeft.Value = 3;
    }

    private void OnDestroy()
    {
        OnPowerPalletCollect.RemoveListener(PowerPelletCollect);
        OnPowerPalletEnd.RemoveListener(PowerPelletEnd);
        OnPlayerDie.RemoveListener(OnDie);
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
        if (!isRespawning)
        {
            if (other.gameObject.TryGetComponent(out Ghost ghost))
            {
                if (canKillGhost)
                {
                    ghost.Die();
                }
                else
                {
                    OnPlayerDie.Raise(livesLeft.Value--);
                }
            }
        }
    }

    private void OnDie()
    {
        isRespawning.Value = true;
        if (livesLeft != 0)
        {
            this.AttachTimer(glitchoutTime, () =>
            {
                playerPostProcessing.ResetPlayer();
                this.AttachTimer(resetAnimationTime, () => isRespawning.Value = false);
            }, null, false, true);
        }
    }
}