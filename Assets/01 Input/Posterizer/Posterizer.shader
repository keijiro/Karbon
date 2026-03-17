Shader "Karbon/Posterizer"
{
Properties
{
    _MainTex("Source", 2D) = "white" {}

    _Threshold("Threshold", Range(0, 1)) = 0.5
    _DitherStrength("Dither Strength", Range(0, 1)) = 0.4
    _YOffset("Y Offset", Float) = 0

    [KeywordEnum(Disabled, Foreground, Background)]
    _FilterMode("Filter Mode", Float) = 0
}

HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/jp.keijiro.bodypix/Shaders/Common.hlsl"
#include "Assets/00 Common/Shaders/CustomRenderTexture.hlsl"

#define _DUOTONE_BAYER4X4
#include "Packages/jp.keijiro.duotone/Shaders/DuotoneDither.hlsl"

TEXTURE2D(_MainTex);
float4 _MainTex_TexelSize;

float _Threshold;
float _DitherStrength;
float _YOffset;

static const half kAlphaThreshold = 0.5h;

half4 FragUpdate(CustomRenderTextureVaryings i) : SV_Target
{
    float2 uv = i.globalTexcoord.xy;
    uv.y += _YOffset;

    float srcAspect = _MainTex_TexelSize.y / _MainTex_TexelSize.x;
    float dstAspect = _CustomRenderTextureWidth / _CustomRenderTextureHeight;

    if (dstAspect > srcAspect)
    {
        float scaleY = srcAspect / dstAspect;
        uv.y = (uv.y - 0.5) * scaleY + 0.5;
    }
    else if (dstAspect < srcAspect)
    {
        float scaleX = dstAspect / srcAspect;
        uv.x = (uv.x - 0.5) * scaleX + 0.5;
    }

    float4 src = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv);

#if defined(_FILTERMODE_FOREGROUND)
    if (src.a < kAlphaThreshold) return 0;
#elif defined(_FILTERMODE_BACKGROUND)
    if (src.a >= kAlphaThreshold) return 0;
#endif

    uint2 psp = uint2(i.globalTexcoord.xy * _CustomRenderTextureInfo.xy);
    float dither = DuotoneDither(psp);

    float lum = Luminance(LinearToSRGB(src.rgb));
    float bw = _Threshold < (lum + (dither - 0.5) * _DitherStrength);

    return half4(bw.xxx, 1);
}

ENDHLSL

    SubShader
    {
        ZTest Always ZWrite Off Cull Off
        Pass
        {
            Name "Update"
            HLSLPROGRAM
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment FragUpdate
            #pragma shader_feature_local _FILTERMODE_DISABLED _FILTERMODE_FOREGROUND _FILTERMODE_BACKGROUND
            ENDHLSL
        }
    }
}
