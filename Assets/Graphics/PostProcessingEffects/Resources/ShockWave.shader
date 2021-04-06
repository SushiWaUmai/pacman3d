Shader "Custom/PostProcessing/ShockWave"
{
	HLSLINCLUDE

	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

	float2 _FocalPoint;
	float _Size;
	float _Magnification;
	float _Speed;
	float _ShaderTime;

	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float2 dir = i.texcoord.xy - _FocalPoint;
		float dist = length(dir);
		float time = frac(_ShaderTime * _Speed);

		float2 uv = smoothstep(time - _Size, time + _Size, dist);
		uv *= -uv;
		uv *= normalize(dir) * _Magnification;
		uv += i.texcoord;

		return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM
			#pragma vertex VertDefault
			#pragma fragment Frag
			ENDHLSL
		}
	}
}