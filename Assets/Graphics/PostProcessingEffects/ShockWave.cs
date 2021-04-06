using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityTimer;

[Serializable]
[PostProcess(typeof(ShockWaveRenderer), PostProcessEvent.BeforeStack, "Custom/PostProcessing/ShockWave")]
public class ShockWave : PostProcessEffectSettings
{
    [Tooltip("Focal Point")]
    public Vector2Parameter focalPoint = new Vector2Parameter { value = Vector2.one * 0.5f };

    [Tooltip("Size")]
    public FloatParameter size = new FloatParameter { value = 1 };

    [Tooltip("Magnification")]
    public FloatParameter magnification = new FloatParameter { value = 2 };

    [Tooltip("Speed")]
    public FloatParameter speed = new FloatParameter { value = 2 };

    private float shockwaveStart = 0;
    public float ShaderTime => Time.time - shockwaveStart;

    public void CreateShockWave()
    {
        enabled.value = true;
        shockwaveStart = Time.time;
        Timer.Register(1 / speed, ResetTime);
    }

    private void ResetTime() => enabled.value = false;
}

public sealed class ShockWaveRenderer : PostProcessEffectRenderer<ShockWave>
{
    static class ShaderPropertyID
    {
        internal static readonly int FocalPoint = Shader.PropertyToID("_FocalPoint");
        internal static readonly int Size = Shader.PropertyToID("_Size");
        internal static readonly int Magnification = Shader.PropertyToID("_Magnification");
        internal static readonly int Speed = Shader.PropertyToID("_Speed");
        internal static readonly int ShaderTime = Shader.PropertyToID("_ShaderTime");
    }

    //float verticalJumpTime;
    public override void Render(PostProcessRenderContext context)
    {
        PropertySheet sheet = context.propertySheets.Get(Shader.Find("Custom/PostProcessing/ShockWave"));

        sheet.properties.SetVector(ShaderPropertyID.FocalPoint, settings.focalPoint);
        sheet.properties.SetFloat(ShaderPropertyID.Size, settings.size * 0.1f);
        sheet.properties.SetFloat(ShaderPropertyID.Magnification, -settings.magnification * 0.1f);
        sheet.properties.SetFloat(ShaderPropertyID.Speed, settings.speed);
        sheet.properties.SetFloat(ShaderPropertyID.ShaderTime, settings.ShaderTime);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
