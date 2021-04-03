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
    private void Start()
    {
        OnPowerPalletCollect.AddListener(PowerPalletCollect);
        OnPowerPalletEnd.AddListener(() => canKillGhost = false);
    }

    private void PowerPalletCollect()
    {
        canKillGhost = true;
        this.AttachTimer(powerPalletTime, OnPowerPalletEnd.Raise);
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