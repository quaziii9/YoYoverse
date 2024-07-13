Shader "Custom/VisionConeProjector"
{
    Properties
    {
        _MainColor("Main Color", Color) = (1,0,0,0.5)
        _Cookie("Cookie", 2D) = "white" {}
    }
        SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 100

        Stencil
        {
            Ref 1
            Comp Equal
        }

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        sampler2D _Cookie;
        fixed4 _MainColor;

        struct Input
        {
            float4 screenPos;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            float2 uv = IN.screenPos.xy / IN.screenPos.w;
            fixed4 c = tex2D(_Cookie, uv) * _MainColor;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
        FallBack "Diffuse"
}