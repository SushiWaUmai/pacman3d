using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using DUCK.Utils;
using NaughtyAttributes;

public class Player : MonoBehaviour
{
    [SerializeField] private GameEvent OnPowerPalletCollect;
    [SerializeField] private GameEvent OnPowerPalletEnd;
    [SerializeField] private float powerPalletTime;
    [ShowNonSerializedField] private bool canKillGhost;
    private Timer powerPelletTimer;

    private void Start()
    {
        OnPowerPalletCollect.AddListener(PowerPalletCollect);
        OnPowerPalletEnd.AddListener(() => canKillGhost = false);
    }

    private void StopPowerPallet()
    {
        if(powerPelletTimer != null)
            powerPelletTimer.Stop();
    }

    private void PowerPalletCollect()
    {
        canKillGhost = true;
        powerPelletTimer = Timer.SetTimeout(powerPalletTime, OnPowerPalletEnd.Raise);
    }

    private void OnDestroy()
    {
        StopPowerPallet();
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