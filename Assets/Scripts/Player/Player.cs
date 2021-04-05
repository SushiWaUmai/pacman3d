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
    [SerializeField] private float minDistance;
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
        Collider[] ghosts = Physics.OverlapSphere(transform.position, minDistance, ghostLayer);

        float closestGhostSqrDistance;
        foreach (Collider c in ghosts)
        {
            
        }

        //playerPostProcessing.SetGlitchIntensity();
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