Shader "Hidden/Pixelize"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
        }

        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On

        HLSLINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionHCS : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        TEXTURE2D(_MainTex);
        float4 _MainTex_TexelSize;
        float4 _MainTex_ST;

        SamplerState sampler_point_clamp;
        
        uniform float2 _PixelCount;
        uniform float2 _PixelSize;
        uniform float2 _HalfPixelSize;
        uniform float _Power;
        
        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
            return OUT;
        }

        ENDHLSL

        Pass
        {
            Name "Pixelize"
             
            HLSLPROGRAM
            half4 frag(Varyings IN) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                float2 pixelPos = floor(IN.uv * _PixelCount);
                float2 pixelCenter = pixelPos * _PixelSize + _HalfPixelSize;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, pixelCenter);

                return col;
            }
            ENDHLSL
        }
    }
}