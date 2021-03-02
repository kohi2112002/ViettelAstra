#include "UnityStandardInput.cginc"

// just for NormalizePerPixelNormal()
#include "UnityStandardCore.cginc"

// LightingStandard(), LightingStandard_GI(), LightingStandard_Deferred() and
// struct SurfaceOutputStandard are defined here.
#include "UnityPBSLighting.cginc"

struct appdata_vert {
	float4 vertex : POSITION;
	half3 normal : NORMAL;
	float2 uv0 : TEXCOORD0;
	float2 uv1 : TEXCOORD1;
	float4 texcoord1 : TEXCOORD2; // lightmaps and meta pass (not sure)
	float4 texcoord2 : TEXCOORD3; // dynamig GI and meta pass (not sure)
	UNITY_VERTEX_INPUT_INSTANCE_ID
#ifdef _TANGENT_TO_WORLD
	float4 tangent : TANGENT;
#endif
};

struct Input {
	float4 texcoords;
#if defined(_PARALLAXMAP)
	half3 viewDirForParallax;
#endif
};
float _CurveStrength;
void vert (inout appdata_vert v, out Input o) {
	UNITY_INITIALIZE_OUTPUT(Input,o);
	UNITY_SETUP_INSTANCE_ID(v);
	o.texcoords.xy = TRANSFORM_TEX(v.uv0, _MainTex); // Always source from uv0
	o.texcoords.zw = TRANSFORM_TEX(((_UVSec == 0) ? v.uv0 : v.uv1), _DetailAlbedoMap);
	float4 worldPosition = mul(unity_ObjectToWorld, v.vertex); // get world space position of vertex
	float wpToCam = _WorldSpaceCameraPos.z - worldPosition.z; // get vector to camera and dismiss vertical component
	float distance = wpToCam*wpToCam; // distance squared from vertex to the camera, this power gives the curvature
	worldPosition.y -= distance * _CurveStrength; // offset vertical position by factor and square of distance.
	// the default 0.01 would lower the position by 1cm at 1m distance, 1m at 10m and 100m at 100m
	v.vertex = mul(unity_WorldToObject, worldPosition); // reproject position into object space
#ifdef _PARALLAXMAP
	TANGENT_SPACE_ROTATION; // refers to v.normal and v.tangent
	o.viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
#endif
}

void surf (Input IN, inout SurfaceOutputStandard o) {
#ifdef _PARALLAXMAP
	half3 viewDirForParallax = NormalizePerPixelNormal(IN.viewDirForParallax);
#else
	half3 viewDirForParallax = half3(0,0,0);
#endif
	float4 texcoords = IN.texcoords;
	texcoords = Parallax(texcoords, viewDirForParallax);
	half alpha = Alpha(texcoords.xy);
#if defined(_ALPHATEST_ON)
	clip (alpha - _Cutoff);
#endif
	o.Albedo = Albedo(texcoords);
#ifdef _NORMALMAP
	o.Normal = NormalInTangentSpace(texcoords);
	o.Normal = NormalizePerPixelNormal(o.Normal);
#endif
	o.Emission = Emission(texcoords.xy);
	half2 metallicGloss = MetallicGloss(texcoords.xy);
	o.Metallic = metallicGloss.x; // _Metallic;
	o.Smoothness = metallicGloss.y; // _Glossiness;
	o.Occlusion = Occlusion(texcoords.xy);
	o.Alpha = alpha;
}

void final (Input IN, SurfaceOutputStandard o, inout fixed4 color)
{
	color = OutputForward(color, color.a);
}
