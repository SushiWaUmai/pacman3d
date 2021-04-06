using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using ScriptableObjectArchitecture;
using UnityTimer;

public class PlayerPostProcessing : MonoBehaviour
{
    [SerializeField] private float digitalGlitchIntensity = 1;
    [SerializeField] private float scanLineJitterIntensity = 1;
    [SerializeField] private float horizontalShakeIntensity = 1;
    [SerializeField] private float colorDriftIntensity = 1;

    [SerializeField] private FloatVariable respawnAnimationTime;
    [SerializeField] private IntGameEvent OnPlayerDie;
    [SerializeField] private BoolVariable isRespawning;

    private DigitalGlitch digitalGlitch;
    private AnalogGlitch analogGlitch;

    private void Start()
    {
        PostProcessVolume vol = GetComponent<PostProcessVolume>();
        vol.profile.TryGetSettings(out digitalGlitch);
        vol.profile.TryGetSettings(out analogGlitch);

        SetGlitchIntensity(0);

        OnPlayerDie.AddListener(GlitchOut);
    }

    public void SetGlitchIntensity(float glitchValue)
    {
        if (!isRespawning.Value)
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
}
