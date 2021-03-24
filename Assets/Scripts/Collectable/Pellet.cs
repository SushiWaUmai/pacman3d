using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class Pellet : Collectable
{
    private static List<Pellet> allFood = new List<Pellet>();
    [SerializeField] private GameEvent OnGameClear;

    protected override void Init()
    {
        base.Init();
        allFood.Add(this);
    }

    protected override void GetCollected()
    {
        base.GetCollected();
        allFood.Remove(this);

        if(allFood.Count <= 0)
        {
            OnGameClear.Raise();
        }

        Destroy(gameObject);
    }
}