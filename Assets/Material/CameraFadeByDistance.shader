Shader "Custom/CameraFadeByDistance"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _FadeStart ("Fade Start (distance)", Float) = 0.5
        _FadeEnd   ("Fade End (distance)", Float) = 2.0
        _LifeTime ("LifeTime", Float) = 1.0
        _SpawnTime ("Spawn Time", Float) = 0.0
        _CurrentTime ("Current Time", Float) = 0.0
        _AlphaMultiplier ("Alpha Multiplier", Range(0,1)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 worldPos   : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _BaseColor;
            float4 _MainTex_ST;

            float _FadeStart;
            float _FadeEnd;
            float _LifeTime;
            float _SpawnTime;
            float _CurrentTime;
            float _AlphaMultiplier;

            Varyings vert(Attributes v)
            {
                Varyings o;
                float3 worldPos = TransformObjectToWorld(v.positionOS.xyz);
                o.worldPos = worldPos;
                o.positionCS = TransformWorldToHClip(worldPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                // --- 距離フェード ---
                // _WorldSpaceCameraPos は URP で自動的に定義済み
                float dist = distance(i.worldPos, _WorldSpaceCameraPos.xyz);
                float fadeStart = min(_FadeStart, _FadeEnd);
                float fadeEnd   = max(_FadeStart, _FadeEnd);

                // smoothstep で滑らかにフェード（近いと透明・遠いと不透明）
                float distAlpha = smoothstep(fadeStart, fadeEnd, dist);

                // --- 時間フェード ---
                float age = _CurrentTime - _SpawnTime;
                float timeAlpha = saturate(1.0 - age / max(0.0001, _LifeTime));

                // --- テクスチャ合成 ---
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float3 rgb = tex.rgb * _BaseColor.rgb;
                float a = tex.a * _BaseColor.a * _AlphaMultiplier * distAlpha * timeAlpha;

                return float4(rgb, a);
            }
            ENDHLSL
        }
    }

    FallBack "Unlit/Transparent"
}
