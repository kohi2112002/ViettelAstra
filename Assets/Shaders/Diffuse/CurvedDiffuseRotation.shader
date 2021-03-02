Shader "Lit/CurvedDiffuseRotation"
{
	Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            // indicate that our pass is the "base" pass in forward
            // rendering pipeline. It gets ambient and main directional
            // light data set up; light direction in _WorldSpaceLightPos0
            // and color in _LightColor0
            Tags {"LightMode"="ForwardBase"}
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc" // for UnityObjectToWorldNormal
            #include "UnityLightingCommon.cginc" // for _LightColor0

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 diff : COLOR0; // diffuse lighting color
                float4 vertex : SV_POSITION;
            };
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _CurveStrength;
            v2f vert (appdata_base v)
            {
                v2f o;
                float4 rotVert = v.vertex;
	            rotVert.z = v.vertex.z * cos(_Time.y * 3.14f) - v.vertex.x * sin(_Time.y * 3.14f);
	            rotVert.x = v.vertex.z * sin(_Time.y * 3.14f) + v.vertex.x * cos(_Time.y * 3.14f);
                o.vertex = UnityObjectToClipPos(rotVert);
				float dist = UNITY_Z_0_FAR_FROM_CLIPSPACE(o.vertex.z);
				o.vertex.y -= _CurveStrength * dist * dist * _ProjectionParams.x;
                o.uv = v.texcoord;
                // get vertex normal in world space
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                // dot product between normal and light direction for
                // standard diffuse (Lambert) lighting
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                // factor in the light color
                o.diff = nl * _LightColor0;
				o.diff.rgb += ShadeSH9(half4(worldNormal,1));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // multiply by lighting
                col *= i.diff;
                return col;
            }
            ENDCG
        }
    }
}
