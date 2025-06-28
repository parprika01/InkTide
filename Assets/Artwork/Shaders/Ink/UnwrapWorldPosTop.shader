Shader "Unlit/UnwrapWorldPosTopOnly"
{
    Properties
    {
        _LightmapST ("Lightmap ST", Vector) = (1,1,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float3 normal : NORMAL;
                float2 uv1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            float4 _LightmapST;

            v2f vert (appdata v)
            {
                v2f o;
                float2 lightuv = v.uv1 * _LightmapST.xy + _LightmapST.zw;
                float4 uvWorldPos = float4(lightuv * 2 - 1, 0.5, 1);
                o.vertex = mul(UNITY_MATRIX_VP, uvWorldPos);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (dot(i.worldNormal, float3(0,1,0)) < 0.9)
                    return float4(0, 0, 0, 0);
                return float4(i.worldPos, 1.0);
            }
            ENDCG
        }
    }
}
