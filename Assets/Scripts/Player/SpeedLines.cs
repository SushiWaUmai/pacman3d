using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

[RequireComponent(typeof(ParticleSystem))]
public class SpeedLines : MonoBehaviour
{
    [SerializeField] private GameEvent OnPowerPelletCollect;
    [SerializeField] private GameEvent OnPowerPelletEnd;

    [SerializeField] private float emmision;

    private ParticleSystem particles;
    private ParticleSystem.EmissionModule emissionModule;

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
        emissionModule = particles.emission;

        OnPowerPelletCollect.AddListener(EnableParticles);
        OnPowerPelletEnd.AddListener(DisableParticles);
    }

    private void OnDestroy()
    {
        OnPowerPelletCollect.RemoveListener(EnableParticles);
        OnPowerPelletEnd.RemoveListener(DisableParticles);
    }

    private void EnableParticles()
    {
        emissionModule.rateOverTime = emmision;
    }

    private void DisableParticles()
    {
        emissionModule.rateOverTime = 0;
    }
}