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
    [SerializeField] private GameEvent OnGameClear;
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
    [SerializeField] private BoolVariable canInteract;
    [SerializeField] private IntVariable livesLeft;

    [Header("Ghost Kill")]
    [SerializeField] private float slomoSpeed = 0.2f;
    [SerializeField] private float slomoDuration = 0.5f;

    private Timer powerPelletTimer;

    private void Start()
    {
        OnPowerPalletCollect.AddListener(PowerPelletCollect);
        OnPowerPalletEnd.AddListener(PowerPelletEnd);
        OnPlayerDie.AddListener(OnDie);
        OnGameClear.AddListener(GameClear);

        Ghost.ResetKillCount();

        canInteract.Value = true;
        livesLeft.Value = 3;
    }

    private void OnDestroy()
    {
        OnPowerPalletCollect.RemoveListener(PowerPelletCollect);
        OnPowerPalletEnd.RemoveListener(PowerPelletEnd);
        OnPlayerDie.RemoveListener(OnDie);
        OnGameClear.RemoveListener(GameClear);
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

    private void GameClear()
    {
        canInteract.Value = false;
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
        if (canInteract)
        {
            if (other.gameObject.TryGetComponent(out Ghost ghost))
            {
                if (!ghost.IsEaten)
                {
                    if (canKillGhost)
                    {
                        playerPostProcessing.CreateShockWave();
                        ghost.Die();

                        SlomoManager.SetTimeScale(slomoSpeed);
                        this.AttachTimer(slomoDuration, () => SlomoManager.SetTimeScale(1), null, false, true);
                    }
                    else
                    {
                        OnPlayerDie.Raise(livesLeft.Value--);
                    }
                }
            }
        }
    }

    private void OnDie()
    {
        canInteract.Value = false;
        if (livesLeft != 0)
        {
            this.AttachTimer(glitchoutTime, () =>
            {
                playerPostProcessing.ResetPlayer();
                this.AttachTimer(resetAnimationTime, () => canInteract.Value = true);
            }, null, false, true);
        }
    }
}