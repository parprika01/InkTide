// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Unlit/PaintObj"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _InkColor ("Ink Color", Color) = (1,1,1,1)
    	_InkRange ("Ink Range", Float) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 lightmapUV : TEXCOORD1;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 lightmapUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 normalWS : TEXCOORD2;
                float3 tangentWS : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
            	float3 posWS : TEXCOORD5;
            };

            TEXTURE2D(_MainTex);
            TEXTURE2D(_MaskTex);
            TEXTURE2D(_NormalMap);
            TEXTURE2D(_SplatBumpTex);
            SAMPLER(sampler_MaskTex);
            SAMPLER(sampler_MainTex);
            SAMPLER(sampler_NormalMap);
            SAMPLER(sampler_SplatBumpTex);
            
            float4 _MainTex_ST;
            float4 _MaskTex_TexelSize;
            float _InkRange;
            float4 _InkColor;
            
            // 梯度噪声函数
            float2 gradientNoise_dir(float2 p) {
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float gradient_noise(float2 p) {
                float2 ip = floor(p);
                float2 fp = frac(p);
                
                float d00 = dot(gradientNoise_dir(ip), fp);
                float d01 = dot(gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
                
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
            }
            
			float GradientNoise(float2 UV, float Scale) {
				 return gradient_noise(UV * Scale) + 0.5;
			}

            float3 NormalFromHeight(float height, float strength, float3 worldPos, float3x3 tangentMatrix) {
                // 计算世界空间导数
                float3 worldDerivativeX = ddx(worldPos);
                float3 worldDerivativeY = ddy(worldPos);

                // 计算辅助向量
                float3 crossX = cross(tangentMatrix[2].xyz, worldDerivativeX); // 副切线方向
                float3 crossY = cross(worldDerivativeY, tangentMatrix[2].xyz); // 切线方向

                // 计算表面修正系数
                float d = dot(worldDerivativeX, crossY);
                float sgn = d < 0.0 ? -1.0 : 1.0;
                float surface = sgn / max(1.192093e-14, abs(d)); // 防止除以零

                // 获取高度图梯度
                float dHdx = ddx(height);
                float dHdy = ddy(height);
                
                // 计算表面梯度
                float3 surfGrad = surface * (dHdx * crossY + dHdy * crossX);
                
                // 生成世界空间法线并转换到切线空间
                float3 worldNormal = normalize(tangentMatrix[2].xyz - (strength * surfGrad));
                return mul(tangentMatrix, worldNormal);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.lightmapUV = v.lightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
                o.normalWS = TransformObjectToWorldNormal(v.normal);
                o.tangentWS = TransformObjectToWorldDir(v.tangent.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * v.tangent.w;
				o.posWS = TransformObjectToWorld(v.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 mask_col = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex ,i.lightmapUV);
            	
				// 获取墨水的覆盖范围,注意后面可以分别测试a和r的效果
            	float inkEdge = smoothstep(_InkRange, _InkRange, mask_col.a);
                col = lerp(col , _InkColor, inkEdge);
                
            	// 使用噪声生成法线
            	float inkNoise = GradientNoise(i.lightmapUV, 15.0);
            	inkNoise = lerp(inkNoise, inkNoise * mask_col.a, 0.3) * inkEdge;
            	
                float3x3 TBN = float3x3(
                    normalize(i.tangentWS),
                    normalize(i.bitangentWS),
                    normalize(i.normalWS)
                );
            	
            	float3 inkNormalTS = NormalFromHeight(inkNoise, 0.2, i.posWS, TBN);
                float3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap, i.uv), 1.0);	            	
				float3 combinedNormalTS = normalize(inkNormalTS + normalTS);

            	half3 normalWS = mul(combinedNormalTS, TBN);
            	
            	
                // bake光线计算
                half3 bakedGI = SampleLightmap(i.lightmapUV, normalTS);
                // 直接光源计算
                Light mainLight = GetMainLight();
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half3 diffuse = NdotL * mainLight.color;
                
                col.rgb *= bakedGI + 0.5 * diffuse * mainLight.shadowAttenuation;

                // 搭建pbr光照模型
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = col;
                surfaceData.normalTS = combinedNormalTS;
                surfaceData.metallic = 0;
                surfaceData.smoothness = inkEdge;
                surfaceData.occlusion = 1.0;
                surfaceData.emission = 0.0;
                surfaceData.alpha = 1.0;

                
                InputData inputData = (InputData)0;
                inputData.normalWS = normalWS;
                inputData.positionWS = i.posWS;
                inputData.viewDirectionWS = normalize(_WorldSpaceCameraPos - i.posWS);
                inputData.bakedGI = bakedGI;
                
                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                return color;
                
            }
            ENDHLSL
        }
    }
}
