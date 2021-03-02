// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Beffio/Standard/Car Paint Opaque"
{
	Properties
	{
		_BaseColor("Base Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_DetailColor("Detail Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_DetailMap("Detail Map", 2D) = "white" {}
		_DetailMapDepthBias("Detail Map Depth Bias", Float) = 1.0
		_DiffuseColor("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_DiffuseMap("Diffuse Map", 2D) = "white" {}
		_MatCapLookup("MatCap Lookup", 2D) = "white" {}
		_ReflectionColor("Reflection Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_ReflectionMap("Reflection Map", Cube) = "" {}
		_ReflectionStrength("Reflection Strength", Range(0.0, 1.0)) = 0.5
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
		}

		Pass
		{
			Blend Off
			Cull Back
			ZWrite On

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma fragment FragmentMain
			#pragma vertex VertexMain

			float4 _BaseColor;
			float4 _DetailColor;
			sampler2D _DetailMap;
			float4 _DetailMap_ST;
			float _DetailMapDepthBias;
			float4 _DiffuseColor;
			sampler2D _DiffuseMap;
			float4 _DiffuseMap_ST;
			sampler2D _MatCapLookup;
			float4 _ReflectionColor;
			samplerCUBE _ReflectionMap;
			float _ReflectionStrength;

			struct VertexInput
			{
				float3 normal : NORMAL;
				float4 position : POSITION;
				float2 UVCoordsChannel1: TEXCOORD0;
			};

			struct VertexToFragment
			{
				float3 detailUVCoordsAndDepth : TEXCOORD0;
				float4 diffuseUVAndMatCapLookupCoords : TEXCOORD1;
				float4 position : SV_POSITION;
				float3 worldSpaceReflectionVector : TEXCOORD2;
			};

			float4 FragmentMain(VertexToFragment input) : COLOR
			{
				float3 reflectionColor = texCUBE(_ReflectionMap, input.worldSpaceReflectionVector).rgb * _ReflectionColor.rgb;
				float4 diffuseColor = tex2D(_DiffuseMap, input.diffuseUVAndMatCapLookupCoords.xy) * _DiffuseColor;
				float3 finalColor = lerp(lerp(_BaseColor.rgb, diffuseColor.rgb, diffuseColor.a), reflectionColor, _ReflectionStrength);

				float3 detailMask = tex2D(_DetailMap, input.detailUVCoordsAndDepth.xy).rgb;
				float3 detailColor = lerp(_DetailColor.rgb, finalColor, detailMask);
				finalColor = lerp(detailColor, finalColor, saturate(input.detailUVCoordsAndDepth.z * _DetailMapDepthBias));

				float3 matCapColor = tex2D(_MatCapLookup, input.diffuseUVAndMatCapLookupCoords.zw).rgb;
				return float4(finalColor * matCapColor * 2.0, _BaseColor.a);
			}
			float _CurveStrength;
			VertexToFragment VertexMain(VertexInput input)
			{
				VertexToFragment output;

				output.diffuseUVAndMatCapLookupCoords.xy = TRANSFORM_TEX(input.UVCoordsChannel1, _DiffuseMap);

				output.diffuseUVAndMatCapLookupCoords.z = dot(normalize(UNITY_MATRIX_IT_MV[0].xyz), normalize(input.normal));
				output.diffuseUVAndMatCapLookupCoords.w = dot(normalize(UNITY_MATRIX_IT_MV[1].xyz), normalize(input.normal));
				output.diffuseUVAndMatCapLookupCoords.zw = output.diffuseUVAndMatCapLookupCoords.zw * 0.5 + 0.5;

				float4 worldPosition = mul(unity_ObjectToWorld, input.position); // get world space position of vertex
				float wpToCam = _WorldSpaceCameraPos.z - worldPosition.z; // get vector to camera and dismiss vertical component
				float distance = wpToCam*wpToCam; // distance squared from vertex to the camera, this power gives the curvature
				worldPosition.y -= distance * _CurveStrength; // offset vertical position by factor and square of distance.
				// the default 0.01 would lower the position by 1cm at 1m distance, 1m at 10m and 100m at 100m
				input.position = mul(unity_WorldToObject, worldPosition); // reproject position into object space
				output.position = UnityObjectToClipPos(input.position);
				//output.position = mul(unity_ObjectToWorld, input.position);;

				output.detailUVCoordsAndDepth.xy = TRANSFORM_TEX(input.UVCoordsChannel1, _DetailMap);
				output.detailUVCoordsAndDepth.z = output.position.z;

				float3 worldSpacePosition = mul(unity_ObjectToWorld, input.position).xyz;
				float3 worldSpaceNormal = normalize(mul((float3x3)unity_ObjectToWorld, input.normal));
				output.worldSpaceReflectionVector = reflect(worldSpacePosition - _WorldSpaceCameraPos.xyz, worldSpaceNormal);

				//float dist = UNITY_Z_0_FAR_FROM_CLIPSPACE(output.position.z);
				//output.position.y -= _CurveStrength * dist * dist * _ProjectionParams.x;
				return output;
			}
			ENDCG
		}
	}

	Fallback "VertexLit"
}
