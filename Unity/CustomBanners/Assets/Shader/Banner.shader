Shader "CustomBanners/Banner"
{
    Properties
    {
        _MainTex ("1st Main Texture", 2D) = "white" {}
        _SecMainTex ("2nd Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Intensity ("Intensity", Float) = 1
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Transition ("Transition", Range(0, 1)) = 0
        [KeywordEnum(Zero, One, DstColor, SrcColor, OneMinusDstColor, SrcAlpha, OneMinusSrcColor, DstAlpha, OneMinusDstAlpha, SrcAlphaSaturate, OneMinusSrcAlpha)] _BlendSrcFactorA ("BlendSrcFactorA", Float) = 5
        [KeywordEnum(Zero, One, DstColor, SrcColor, OneMinusDstColor, SrcAlpha, OneMinusSrcColor, DstAlpha, OneMinusDstAlpha, SrcAlphaSaturate, OneMinusSrcAlpha)] _BlendDstFactorA ("BlendSrcFactorA", Float) = 10
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Cull Off Lighting Off ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha, [_BlendSrcFactorA] [_BlendDstFactorA]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _SecMainTex;
            float4 _SecMainTex_ST;

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            fixed _Transition;
            fixed _Intensity;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed noise = tex2D(_NoiseTex, i.uv).r;
                fixed4 col1 = tex2D(_MainTex, i.uv);
                fixed4 col2 = tex2D(_SecMainTex, i.uv);

                fixed2 chromAberrAmount = float2(0.03, 0.01) * 0.8;

                fixed2 uv = i.uv + float2(0.1 - noise * 0.15, 0);
                fixed4 a = tex2D(_MainTex, uv + chromAberrAmount).r;
                fixed4 b = tex2D(_MainTex, uv).g;
                fixed4 c = tex2D(_MainTex, uv - chromAberrAmount).b;
                fixed4 glitch = fixed4(a.r, b.g, c.b, col1.a);

                fixed fac = 10;
                fixed off = 1 / (2 * fac) + 0.5;
                fixed transition = (1 + noise * 0.05 - _Transition * off * 1.25) * 2 - 1;

                fixed4 col = lerp(col1, glitch, saturate((i.uv.y - transition) * fac));
                       col = lerp(col, col2, saturate((i.uv.y - 0.2 - transition) * fac));

                UNITY_APPLY_FOG(i.fogCoord, col);

                return fixed4(col.r, col.g, col.b, col.a * col.a * 0.5) * (_Intensity * 0.5 + 0.2) * _Color;
            }
            ENDCG
        }
    }
}
