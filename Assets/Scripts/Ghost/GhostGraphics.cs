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
    [SerializeField] private GhostMovement ghostMvment;

    private void Start()
    {
        ghostRenderer = GetComponent<Renderer>();
        ghostOrigMaterial = ghostRenderer.sharedMaterial;

        OnPowerPelletCollect.AddListener(PowerPelletStart);
        OnPowerPelletEnd.AddListener(PowerPelletEnd);


        Transform current = transform;
        while (!ghostMvment)
        {
            ghostMvment = current.GetComponent<GhostMovement>();
            current = current.parent;
        }

        ghostMvment.OnGhostEaten += GhostEaten;
        ghostMvment.OnGhostRecover += GhostRecover;
    }

    private void OnDestroy()
    {
        OnPowerPelletCollect.RemoveListener(PowerPelletStart);
        OnPowerPelletEnd.RemoveListener(PowerPelletEnd);

        ghostMvment.OnGhostEaten -= GhostEaten;
        ghostMvment.OnGhostRecover -= GhostRecover;
    }

    private void PowerPelletStart()
    {
        ghostRenderer.sharedMaterial = FrightenedMaterial;
    }

    private void PowerPelletEnd()
    {
        ghostRenderer.sharedMaterial = ghostOrigMaterial;
    }

    private void GhostEaten()
    {
        PowerPelletEnd();
        ghostRenderer.gameObject.SetActive(false);
    }

    private void GhostRecover()
    {
        ghostRenderer.gameObject.SetActive(true);
    }
}