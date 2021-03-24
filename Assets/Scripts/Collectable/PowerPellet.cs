using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class PowerPellet : Pellet
{
    [SerializeField] private GameEvent OnPowerPelletCollect;

    protected override void GetCollected()
    {
        base.GetCollected();
        OnPowerPelletCollect.Raise();
    }
}