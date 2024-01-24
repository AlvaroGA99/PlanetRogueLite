Shader "Unlit/Stars"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _dirToSun ("LightDir", Vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"RenderType"="Opaque" }
        LOD 100
        Blend SrcAlpha SrcAlpha
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
            float4 _dirToSun;
            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);

                // float3 viewDir = WorldSpaceViewDir(v.vertex);
                // float starBrightness = dot(_dirToSun.xyz-_WorldSpaceCameraPos,viewDir);
                o.col = float4(_Color.rgb,1);
					
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                //_Color.a = 1;
				
				return float4(i.col.rgb,i.col.a );
            }
            ENDCG
        }
    }
}