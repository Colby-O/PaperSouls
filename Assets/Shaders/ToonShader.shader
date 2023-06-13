// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ToonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Shininess ("Sininess Factor", Float) = 10
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _Color("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags {"RenderType" = "Opaque" "LightMode" = "ForwardBase" "PassFlags" = "OnlyDirectional"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 position: POSITION;
                float3 normal : NORMAL;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            uniform float4 _LightColor0;
            uniform float4 _Color;
            uniform float4 _SpecularColor;
            uniform float _Shininess;

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 normalDir = normalize(i.normal);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos.xyz);

                float3 vertexToLightSrc = _WorldSpaceLightPos0.xyz - i.worldPos.xyz;

                float attenuation = lerp(1.0, 1.0 / length(vertexToLightSrc), _WorldSpaceLightPos0.w);

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz * _WorldSpaceLightPos0.w);

                float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rbg * _Color.rbg;

                float diffuseTerm = max(0.0, dot(normalDir, lightDir));

                if (diffuseTerm < 0.25) {
                    diffuseTerm = 0.25;
                }
                else if (diffuseTerm > 0.75) {
                    diffuseTerm = 0.75;
                }
                else {
                    diffuseTerm = 0.5;
                }

                float3 diffuseReflection = attenuation * _LightColor0.rbg * _Color.rbg * diffuseTerm;
                float3 specularReflection;

                if (dot(i.normal, lightDir) < 0.0) {
                    specularReflection = float3(0.0, 0.0, 0.0);
                }
                else {

                    float specularTerm = pow(max(0.0, dot(reflect(-lightDir, normalDir), viewDir)), _Shininess);

                    if (specularTerm < 0.25) {
                        specularTerm = 0;
                    }
                    else {
                        specularTerm = 0.05;
                    }

                    specularReflection = attenuation * _LightColor0.rbg * _SpecularColor * specularTerm;
                }

                float3 color = (ambientLighting + diffuseReflection) * tex2D(_MainTex, i.uv) + specularReflection;

                return float4(color, 1.0);
            }
            ENDCG
        }
    }
}
