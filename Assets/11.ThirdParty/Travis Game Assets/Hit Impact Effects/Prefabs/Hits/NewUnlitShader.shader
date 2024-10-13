Shader "Unlit/NewUnlitShader"
{
    Properties
    {
         _MainTex ("Albedo", 2D) = "white" { }
 _Color ("Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _Cutoff ("Alpha Cutoff", Range(0.000000,1.000000)) = 0.500000
 _BumpScale ("Scale", Float) = 1.000000
 _BumpMap ("Normal Map", 2D) = "bump" { }
 _EmissionColor ("Color", Color) = (0.000000,0.000000,0.000000,1.000000)
 _EmissionMap ("Emission", 2D) = "white" { }
 _DistortionStrength ("Strength", Float) = 1.000000
 _DistortionBlend ("Blend", Range(0.000000,1.000000)) = 0.500000
 _SoftParticlesNearFadeDistance ("Soft Particles Near Fade", Float) = 0.000000
 _SoftParticlesFarFadeDistance ("Soft Particles Far Fade", Float) = 1.000000
 _CameraNearFadeDistance ("Camera Near Fade", Float) = 1.000000
 _CameraFarFadeDistance ("Camera Far Fade", Float) = 2.000000
[HideInInspector]  _Mode ("__mode", Float) = 0.000000
[HideInInspector]  _ColorMode ("__colormode", Float) = 0.000000
[HideInInspector]  _FlipbookMode ("__flipbookmode", Float) = 0.000000
[HideInInspector]  _LightingEnabled ("__lightingenabled", Float) = 0.000000
[HideInInspector]  _DistortionEnabled ("__distortionenabled", Float) = 0.000000
[HideInInspector]  _EmissionEnabled ("__emissionenabled", Float) = 0.000000
[HideInInspector]  _BlendOp ("__blendop", Float) = 0.000000
[HideInInspector]  _SrcBlend ("__src", Float) = 1.000000
[HideInInspector]  _DstBlend ("__dst", Float) = 0.000000
[HideInInspector]  _ZWrite ("__zw", Float) = 1.000000
[HideInInspector]  _Cull ("__cull", Float) = 2.000000
[HideInInspector]  _SoftParticlesEnabled ("__softparticlesenabled", Float) = 0.000000
[HideInInspector]  _CameraFadingEnabled ("__camerafadingenabled", Float) = 0.000000
[HideInInspector]  _SoftParticleFadeParams ("__softparticlefadeparams", Vector) = (0.000000,0.000000,0.000000,0.000000)
[HideInInspector]  _CameraFadeParams ("__camerafadeparams", Vector) = (0.000000,0.000000,0.000000,0.000000)
[HideInInspector]  _ColorAddSubDiff ("__coloraddsubdiff", Vector) = (0.000000,0.000000,0.000000,0.000000)
[HideInInspector]  _DistortionStrengthScaled ("__distortionstrengthscaled", Float) = 0.000000
    }
    SubShader
    {
         Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane"}
         ZWrite [_ZWrite]
  Cull [_Cull]
  Blend [_SrcBlend] [_DstBlend]
 BlendOp [_BlendOp]
  ColorMask RGB
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
