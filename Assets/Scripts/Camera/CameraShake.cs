using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraShake : MonoBehaviour
{
    [SerializeField] private GameEvent OnPowerPelletCollect;
    [SerializeField] private GameEvent OnPowerPelletEnd;
    private CinemachineBasicMultiChannelPerlin shaker;

    [SerializeField] private float powerPelletCamShake = 1;

    private void Start()
    {
        shaker = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        OnPowerPelletCollect.AddListener(EnableShake);
        OnPowerPelletEnd.AddListener(DisableShake);
    }

    private void OnDestroy()
    {
        OnPowerPelletCollect.RemoveListener(EnableShake);
        OnPowerPelletEnd.RemoveListener(DisableShake);
    }

    private void EnableShake()
    {
        shaker.m_AmplitudeGain = powerPelletCamShake;
        shaker.m_FrequencyGain = powerPelletCamShake;
    }

    private void DisableShake()
    {
        shaker.m_AmplitudeGain = 0;
        shaker.m_FrequencyGain = 0;
    }
}