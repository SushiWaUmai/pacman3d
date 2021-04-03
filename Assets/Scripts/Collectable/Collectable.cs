using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using UnityTimer;

public class Collectable : MonoBehaviour
{
    [SerializeField] private IntVariable totalScore;
    [SerializeField] private int score;
    [SerializeField] private GameObject collectedParticleEffect;
    [SerializeField] private float effectDuration = 1;

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

        Destroy(Instantiate(collectedParticleEffect, transform.position, Quaternion.identity), effectDuration);
    }
}
