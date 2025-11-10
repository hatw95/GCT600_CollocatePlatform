// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DecalProjection"
{
    Properties
    {
        _BaseMap ("Base Texture (Original)", 2D) = "white" {}   // ✅ URP 표준명
        _DecalTex ("Decal Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _BaseMap;    // ✅ 수정됨 (URP)
            sampler2D _DecalTex;

            float4x4 _ProjectorMatrix;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float4 world = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = world;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ✅ 1. 원래 백자 텍스처 유지
                fixed4 baseColor = tex2D(_BaseMap, i.uv);

                // ✅ 2. 투영 UV 계산
                float4 projCoord = mul(_ProjectorMatrix, i.worldPos);
                float2 projUV = projCoord.xy;

                // ✅ 3. decal 영역만 적용
                if (projUV.x >= 0 && projUV.x <= 1 &&
                    projUV.y >= 0 && projUV.y <= 1)
                {
                    fixed4 decal = tex2D(_DecalTex, projUV);
                    baseColor = lerp(baseColor, decal, decal.a);
                }

                return baseColor;
            }

            ENDCG
        }
    }
}
