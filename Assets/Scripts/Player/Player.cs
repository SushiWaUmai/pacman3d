using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using UnityTimer;
using NaughtyAttributes;

[RequireComponent(typeof(PlayerPostProcessing))]
public class Player : MonoBehaviour
{
    [SerializeField] private GameEvent OnPowerPalletCollect;
    [SerializeField] private GameEvent OnPowerPalletEnd;
    [SerializeField] private float powerPalletTime;
    [ShowNonSerializedField] private bool canKillGhost;

    [Header("Post Processing")]
    [SerializeField] private LayerMask ghostLayer;
    [SerializeField] private float maxDistance;
    private PlayerPostProcessing playerPostProcessing;

    private void Start()
    {
        playerPostProcessing = GetComponent<PlayerPostProcessing>();
        OnPowerPalletCollect.AddListener(PowerPelletCollect);
        OnPowerPalletEnd.AddListener(PowerPelletEnd);
    }

    private void PowerPelletCollect()
    {
        canKillGhost = true;
        this.AttachTimer(powerPalletTime, OnPowerPalletEnd.Raise);
    }

    private void PowerPelletEnd()
    {
        canKillGhost = false;
    }

    private void FixedUpdate()
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
        float val = Mathf.Clamp01(closestGhostSqrDistance / maxDistance);

        playerPostProcessing.SetGlitchIntensity(val);
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
        Debug.Log("Player Died");
    }
}