Shader "Custom/DecalProjection"
{
    Properties
    {
        _BaseMap    ("Base Texture (Original)", 2D) = "white" {}
        _BaseColor  ("Base Color", Color) = (1,1,1,1)
        _DecalTex   ("Decal Texture", 2D) = "white" {}

        // ---- 추가: 등장/사라짐 페이드 ----
        _DecalAlpha ("Decal Alpha", Range(0,1)) = 1

        // ---- 추가: 가장자리 페더링(옵션) ----
        _Feather    ("Edge Feather (px in UV0..1)", Range(0,0.5)) = 0
        _HardClip   ("Hard Clip Outside", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _BaseMap; float4 _BaseMap_ST;
            float4 _BaseColor;

            sampler2D _DecalTex;
            float4x4 _ProjectorMatrix;

            // 추가 파라미터
            float _DecalAlpha; // 0~1
            float _Feather;    // 0~0.5
            float _HardClip;   // 0 or 1

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f {
                float4 pos      : SV_POSITION;
                float4 worldPos : TEXCOORD0;
                float2 uv0      : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos      = UnityObjectToClipPos(v.vertex);
                o.uv0      = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            // 페더링 마스크: 투사 사각형의 테두리 쪽을 부드럽게 0으로 감쇠
            // _Feather=0 이면 1을 반환(즉, 비활성)
            float edgeFeatherMask(float2 uv, float feather)
            {
                if (feather <= 0) return 1.0;

                // 사각영역(0..1)의 내부에서 가장자리까지의 최소 거리
                float2 d = min(uv, 1.0 - uv); // 좌/우, 상/하 중 가까운 거리
                float minEdge = min(d.x, d.y);

                // feather 거리에서 0, feather*2 안쪽에서 1로 부드럽게
                // (원한다면 곡선 변경 가능)
                float t = saturate( (minEdge - 0.0) / max(feather, 1e-5) );
                // 살짝 더 부드럽게
                t = smoothstep(0.0, 1.0, t);
                return t;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 투사용 UV
                float4 projCoord = mul(_ProjectorMatrix, i.worldPos);
                // (직교 투영이면 w=1이지만 일반성을 위해 나눔)
                projCoord.xyz /= max(projCoord.w, 1e-6);
                float2 projUV = projCoord.xy;

                // 투사 범위 체크(0..1) + 깊이 0..1
                bool inside =
                    projUV.x >= 0 && projUV.x <= 1 &&
                    projUV.y >= 0 && projUV.y <= 1 &&
                    projCoord.z >= 0 && projCoord.z <= 1;

                if (!inside)
                {
                    // 하드 클립이면 완전 투명 반환
                    if (_HardClip >= 0.5) return 0;
                    // 소프트 처리 원하면 여기서도 0 반환
                    return 0;
                }

                fixed4 decal = tex2D(_DecalTex, projUV);

                // 가장자리 페더링 적용(옵션)
                float fea = edgeFeatherMask(projUV, _Feather);

                // 최종 알파 = 텍스처 알파 * 전역 페이드 * 페더링
                decal.a *= _DecalAlpha * fea;

                // 데칼만 그리는 셰이더이므로 decal 그대로 출력
                // (배경은 다른 머티리얼 패스가 이미 렌더함)
                return decal;
            }
            ENDCG
        }
    }
}
