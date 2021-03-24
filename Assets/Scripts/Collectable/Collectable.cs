using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class Collectable : MonoBehaviour
{
    [SerializeField] private IntVariable totalScore;
    [SerializeField] private int score;

    private void Start() => Init();

    protected virtual void Init()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            GetCollected();
    }

    protected virtual void GetCollected()
    {
        totalScore.Value += score;
    }
}
