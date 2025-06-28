Shader "Unlit/UnwrapWorldPos"
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
                float2 uv1 : TEXCOORD1;
            };

            struct v2f
            {
                float3 worldPos : TEXCOORD;
                float4 vertex : SV_POSITION;
            };

            float4 _LightmapST; // 外部传入的缩放偏移参数

            v2f vert (appdata v)
            {
                v2f o;
                float2 lightuv = v.uv1 * _LightmapST.xy + _LightmapST.zw;
                float4 uvWorldPos = float4(lightuv * 2 - 1, 0.5, 1);
                o.vertex = mul(UNITY_MATRIX_VP, uvWorldPos);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //return float4(_LightmapST.xy, 0, 1);
                return float4(i.worldPos, 1.0);
            }
            ENDCG
        }
    }
}
