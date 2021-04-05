using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerPostProcessing : MonoBehaviour
{
    [SerializeField] private float digitalGlitchIntensity = 1;
    [SerializeField] private float scanLineJitterIntensity = 1;
    [SerializeField] private float horizontalShakeIntensity = 1;
    [SerializeField] private float colorDriftIntensity = 1;

    private DigitalGlitch digitalGlitch;
    private AnalogGlitch analogGlitch;

    private void Start()
    {
        PostProcessVolume vol = GetComponent<PostProcessVolume>();
        vol.profile.TryGetSettings(out digitalGlitch);
        vol.profile.TryGetSettings(out analogGlitch);

        SetGlitchIntensity(0);
    }

    public void SetGlitchIntensity(float glitchValue)
    {
        digitalGlitch.intensity.value = Mathf.Lerp(0, digitalGlitchIntensity, glitchValue);
        analogGlitch.scanLineJitter.value = Mathf.Lerp(0, scanLineJitterIntensity, glitchValue);
        analogGlitch.horizontalShake.value = Mathf.Lerp(0, horizontalShakeIntensity, glitchValue);
        analogGlitch.colorDrift.value = Mathf.Lerp(0, colorDriftIntensity, glitchValue);
    }
}
