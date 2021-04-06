using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using ScriptableObjectArchitecture;
using UnityTimer;

public class PlayerPostProcessing : MonoBehaviour
{
    [Header("Glitch")]
    [SerializeField] private float digitalGlitchIntensity = 1;
    [SerializeField] private float scanLineJitterIntensity = 1;
    [SerializeField] private float horizontalShakeIntensity = 1;
    [SerializeField] private float colorDriftIntensity = 1;

    [Header("White Balance")]
    [SerializeField] private float normalWhiteBalance;
    [SerializeField] private float powerPelletWhiteBalance;

    [Header("Chromatic Aberration")]
    [SerializeField] private float normalChromaticAberrationIntensity;
    [SerializeField] private float powerPelletChromaticAberrationIntensity;

    [Header("Scriptable Objects")]
    [SerializeField] private GameEvent OnPowerPelletCollect;
    [SerializeField] private GameEvent OnPowerPelletEnd;
    [SerializeField] private FloatVariable respawnAnimationTime;
    [SerializeField] private IntGameEvent OnPlayerDie;
    [SerializeField] private BoolVariable canInteract;

    private DigitalGlitch digitalGlitch;
    private AnalogGlitch analogGlitch;
    private ShockWave shockWave;
    private ColorGrading colorGrading;
    private ChromaticAberration chromaticAberration;

    private void Start()
    {
        PostProcessVolume vol = GetComponent<PostProcessVolume>();
        vol.profile.TryGetSettings(out digitalGlitch);
        vol.profile.TryGetSettings(out analogGlitch);
        vol.profile.TryGetSettings(out shockWave);
        vol.profile.TryGetSettings(out colorGrading);
        vol.profile.TryGetSettings(out chromaticAberration);

        SetGlitchIntensity(0);

        OnPlayerDie.AddListener(GlitchOut);

        OnPowerPelletCollect.AddListener(PowerPelletStart);
        OnPowerPelletEnd.AddListener(PowerPelletEnd);
    }

    private void OnDestroy()
    {
        OnPlayerDie.RemoveListener(GlitchOut);

        OnPowerPelletCollect.RemoveListener(PowerPelletStart);
        OnPowerPelletEnd.RemoveListener(PowerPelletEnd);
    }

    public void CreateShockWave()
    {
        shockWave.CreateShockWave();
    }

    public void SetGlitchIntensity(float glitchValue)
    {
        if (canInteract.Value)
        {
            digitalGlitch.intensity.value = Mathf.Lerp(0, digitalGlitchIntensity, glitchValue);
            analogGlitch.scanLineJitter.value = Mathf.Lerp(0, scanLineJitterIntensity, glitchValue);
            analogGlitch.horizontalShake.value = Mathf.Lerp(0, horizontalShakeIntensity, glitchValue);
            analogGlitch.colorDrift.value = Mathf.Lerp(0, colorDriftIntensity, glitchValue);
        }
    }

    public void ResetPlayer()
    {
        this.AttachTimer(respawnAnimationTime, null, x =>
        {
            digitalGlitch.intensity.value = Mathf.Clamp01(Mathf.Lerp(0.9f, 0, x / respawnAnimationTime * 4));
            analogGlitch.scanLineJitter.value = Mathf.Clamp01(Mathf.Lerp(1, 0, x / respawnAnimationTime * 4));
            analogGlitch.horizontalShake.value = Mathf.Clamp01(Mathf.Lerp(1, 0, x / respawnAnimationTime * 4));
            analogGlitch.colorDrift.value = Mathf.Clamp01(Mathf.Lerp(1, 0, x / respawnAnimationTime * 4));

            analogGlitch.verticalJump.value = Mathf.PingPong(x / respawnAnimationTime * 2, 1);
        }, false, true);
    }

    public void GlitchOut(int livesLeft)
    {
        digitalGlitch.intensity.value = 0.9f;
        analogGlitch.scanLineJitter.value = 1;
        analogGlitch.horizontalShake.value = 1;
        analogGlitch.colorDrift.value = 1;
    }

    private void PowerPelletStart()
    {
        colorGrading.temperature.value = powerPelletWhiteBalance;
        chromaticAberration.intensity.value = powerPelletChromaticAberrationIntensity;
    }

    private void PowerPelletEnd()
    {
        colorGrading.temperature.value = normalWhiteBalance;
        chromaticAberration.intensity.value = normalChromaticAberrationIntensity;
    }
}
