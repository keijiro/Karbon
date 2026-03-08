Shader "Hidden/Karbon/OverlayFX"
{
HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"

half _Opacity;

half4 Frag(Varyings input) : SV_Target
{
    half2 uv = input.texcoord;
    half4 src = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

    float n = SimplexNoise(float2(uv.x * 16, _Time.y));
    if (abs(n) < 0.1)
    {
        return 1;
    }
    else
    {
        return src;
    }
}

ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Name "OverlayFX"
            ZTest Always ZWrite Off Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
