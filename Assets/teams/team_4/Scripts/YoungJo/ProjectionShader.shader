Shader "Custom/DecalProjection"
{
    Properties
    {
        _BaseMap ("Base Texture (Original)", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _DecalTex ("Decal Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        // ✅ Blend 설정 추가
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _BaseMap;
            float4 _BaseMap_ST;
            float4 _BaseColor;
            
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
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ✅ 데칼 영역만 반환 (원본은 표시 안 함)
                float4 projCoord = mul(_ProjectorMatrix, i.worldPos);
                projCoord.xyz /= projCoord.w;
                float2 projUV = projCoord.xy;

                // 데칼 영역 체크
                if (projUV.x >= 0 && projUV.x <= 1 &&
                    projUV.y >= 0 && projUV.y <= 1 &&
                    projCoord.z >= 0 && projCoord.z <= 1)
                {
                    fixed4 decal = tex2D(_DecalTex, projUV);
                    return decal;  // ✅ 데칼만 반환
                }

                // 데칼 영역 밖은 투명
                return fixed4(0, 0, 0, 0);
            }

            ENDCG
        }
    }
}