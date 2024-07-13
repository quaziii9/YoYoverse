Shader "Custom/StencilMask"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry-1" }
        ColorMask 0
        ZWrite Off

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        CGPROGRAM
        #pragma surface surf Lambert alpha

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Alpha = 1;
        }
        ENDCG
    }
}