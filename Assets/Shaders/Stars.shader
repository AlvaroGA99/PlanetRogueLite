Shader "Unlit/Stars"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _dirToSun ("LightDir", Vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 col : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 col : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID // use this to access instanced properties in the fragment shader.
            };


            float4 _Color;
            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);

                float4 screenPos = ComputeScreenPos(o.vertex);
					float2 screenSpaceUV = screenPos.xy / screenPos.w;
					
                    float4 backgroundCol = 0;

					float backgroundBrightness = saturate(dot(backgroundCol.rgb, 1) / 3 * 0.2);
					float starBrightness = (1 - backgroundBrightness);
					
					//o.col = lerp(backgroundCol, starCol, starBrightness);
					o.col = float4(_Color.rgb,starBrightness);
					
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                //_Color.a = 1;
				
				return float4(i.col.rgb, i.col.a);
            }
            ENDCG
        }
    }
}