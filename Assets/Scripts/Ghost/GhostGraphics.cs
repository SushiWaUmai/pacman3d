using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using ScriptableObjectArchitecture;

[RequireComponent(typeof(Renderer))]
public class GhostGraphics : MonoBehaviour
{
    [SerializeField] private Material FrightenedMaterial;
    [SerializeField] private GameEvent OnPowerPelletCollect;
    [SerializeField] private GameEvent OnPowerPelletEnd;
    [ShowNonSerializedField] private Material ghostOrigMaterial;
    [ShowNonSerializedField] private Renderer ghostRenderer;

    private void Start()
    {
        ghostRenderer = GetComponent<Renderer>();
        ghostOrigMaterial = ghostRenderer.sharedMaterial;

        OnPowerPelletCollect.AddListener(PowerPelletStart);
        OnPowerPelletEnd.AddListener(PowerPelletEnd);
    }

    private void PowerPelletStart()
    {
        ghostRenderer.sharedMaterial = FrightenedMaterial;
    }

    private void PowerPelletEnd()
    {
        ghostRenderer.sharedMaterial = ghostOrigMaterial;
    }
}