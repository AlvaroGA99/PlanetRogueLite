Shader "Hidden/Atmosphere"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WorldSpherePos ("Sphere Position", Vector) = (1000,0,0,1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define maxFloat 3.402823466e+38
            struct appdata {
					float4 vertex : POSITION;
					float4 uv : TEXCOORD0;
			};

			struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 viewVector : TEXCOORD1;
			};

			v2f vert (appdata v) {
					v2f output;
					output.pos = UnityObjectToClipPos(v.vertex);
					output.uv = v.uv;
					// Camera space matches OpenGL convention where cam forward is -z. In unity forward is positive z.
					// (https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html)
					float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv.xy * 2 - 1, 0, -1));
					output.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
					return output;
			}

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float3 _WorldSpherePos;

            float2 raySphere(float3 sphereCentre, float sphereRadius, float3 rayOrigin, float3 rayDir) {
                float3 offset = rayOrigin - sphereCentre;
                float a = 1; // Set to dot(rayDir, rayDir) if rayDir might not be normalized
                float b = 2 * dot(offset, rayDir);
                float c = dot (offset, offset) - sphereRadius * sphereRadius;
                float d = b * b - 4 * a * c; // Discriminant from quadratic formula

                // Number of intersections: 0 when d < 0; 1 when d = 0; 2 when d > 0
                if (d > 0) {
                    float s = sqrt(d);
                    float dstToSphereNear = max(0, (-b - s) / (2 * a));
                    float dstToSphereFar = (-b + s) / (2 * a);

                    // Ignore intersections that occur behind the ray
                    if (dstToSphereFar >= 0) {
                        return float2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
                    }
                }
                // Ray did not intersect sphere
                return float2(maxFloat, 0);
            }

            float densityAtPoint(float3 densitySamplePoint) {
				float heightAboveSurface = length(densitySamplePoint - _WorldSpherePos) - 0.0;
				float height01 = heightAboveSurface / (60.0 - 0.0);
				float localDensity = exp(-height01 * 0.5f) * (1 - height01);
				return localDensity;
			}
			
			float opticalDepth(float3 rayOrigin, float3 rayDir, float rayLength) {
				float3 densitySamplePoint = rayOrigin;
				float stepSize = rayLength / (10 - 1);
				float opticalDepth = 0;

				for (int i = 0; i < 10; i ++) {
					float localDensity = densityAtPoint(densitySamplePoint);
					opticalDepth += localDensity * stepSize;
					densitySamplePoint += rayDir * stepSize;
				}
				return opticalDepth;
			}
			// float3 calculateLight(float3 rayOrigin, float3 rayDir, float rayLength, float3 originalCol, float2 uv) {
			// 	float blueNoise = tex2Dlod(_BlueNoise, float4(squareUV(uv) * ditherScale,0,0));
			// 	blueNoise = (blueNoise - 0.5) * ditherStrength;
				
			// 	float3 inScatterPoint = rayOrigin;
			// 	float stepSize = rayLength / (numInScatteringPoints - 1);
			// 	float3 inScatteredLight = 0;
			// 	float viewRayOpticalDepth = 0;

			// 	for (int i = 0; i < numInScatteringPoints; i ++) {
			// 		float sunRayLength = raySphere(planetCentre, atmosphereRadius, inScatterPoint, dirToSun).y;
			// 		float sunRayOpticalDepth = opticalDepth(inScatterPoint + dirToSun * ditherStrength, dirToSun,stepSize*i);
			// 		float localDensity = densityAtPoint(inScatterPoint);
			// 		viewRayOpticalDepth = opticalDepth(rayOrigin, rayDir, stepSize * i);
			// 		float3 transmittance = exp(-(sunRayOpticalDepth + viewRayOpticalDepth) * scatteringCoefficients);
					
			// 		inScatteredLight += localDensity * transmittance;
			// 		inScatterPoint += rayDir * stepSize;
			// 	}
			// 	inScatteredLight *= scatteringCoefficients * intensity * stepSize / planetRadius;
			// 	//inScatteredLight += blueNoise * 0.01;

			// 	// Attenuate brightness of original col (i.e light reflected from planet surfaces)
			// 	// This is a hacky mess, TODO: figure out a proper way to do this
			// 	const float brightnessAdaptionStrength = 0.15;
			// 	const float reflectedLightOutScatterStrength = 3;
			// 	float brightnessAdaption = dot (inScatteredLight,1) * brightnessAdaptionStrength;
			// 	float brightnessSum = viewRayOpticalDepth * intensity * reflectedLightOutScatterStrength + brightnessAdaption;
			// 	float reflectedLightStrength = exp(-brightnessSum);
			// 	float hdrStrength = saturate(dot(originalCol,1)/3-1);
			// 	reflectedLightStrength = lerp(reflectedLightStrength, 1, hdrStrength);
			// 	float3 reflectedLight = originalCol * reflectedLightStrength;

			// 	float3 finalCol = reflectedLight + inScatteredLight;

				
			// 	return finalCol;
			// }
            fixed4 frag (v2f i) : SV_Target
            {
                float4 originalCol = tex2D(_MainTex, i.uv);
				float sceneDepthNonLinear = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				float sceneDepth = LinearEyeDepth(sceneDepthNonLinear) * length(i.viewVector);
											
				float3 rayOrigin = _WorldSpaceCameraPos;
				float3 rayDir = normalize(i.viewVector);
				
				float dstToOcean = raySphere(_WorldSpherePos, 0.0, rayOrigin, rayDir);
				float dstToSurface = min(sceneDepth, dstToOcean);
				
				float2 hitInfo = raySphere(_WorldSpherePos, 60.0, rayOrigin, rayDir);
				float dstToAtmosphere = hitInfo.x;
				float dstThroughAtmosphere = min(hitInfo.y, dstToSurface - dstToAtmosphere);
				
				if (dstThroughAtmosphere > 0) {
					const float epsilon = 0.0001;
					float3 pointInAtmosphere = rayOrigin + rayDir * (dstToAtmosphere + epsilon);
					//float3 light = calculateLight(pointInAtmosphere, rayDir, dstThroughAtmosphere - epsilon * 2, originalCol, i.uv);
					return float4(1.0,0.5,0.7,1.0);
				}
				return originalCol;
                
                //
                
            }
            ENDCG
        }
    }
}