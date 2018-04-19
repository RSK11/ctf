Shader "Custom/LWCShader" {
	Properties{
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
	_Color("Color", Color) = (1,1,1,1)
		_SpecularColor("Specular Color", Color) = (1,1,1,1)
		_ShadowColor("Shadow Color", Color) = (1,1,1,1)
		_LineColor("Line Color", Color) = (1,1,1,1)
		_LineWeight("Line Weight", float) = 1.0
		_Cap("Weight Cap", float) = 0.1
		_Cutoff("Cutoff", float) = 0
		_Smooth("Smooth",float) = 0
		_ShadowCutoff("Shadow Threshold", float) = .5
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		Pass{
		Cull Front

		CGPROGRAM

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;

	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
	};

	float _LineWeight;
	float _Cap;
	float4 _LineColor;

	v2f vert(appdata v)
	{
		v2f o;
		float4 act = UnityObjectToClipPos(v.vertex);
		float ratio = 2.0 / (_ScreenParams.x * UNITY_MATRIX_P[0][0]) * act.w;
		o.vertex = UnityObjectToClipPos(v.vertex + min(_Cap,(_LineWeight * ratio)) * v.normal);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		fixed4 col = _LineColor;
	return col;
	}
		ENDCG
	}

		Pass{
		Tags{ "LightMode" = "ForwardBase" }
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "UnityLightingCommon.cginc"
#include "Lighting.cginc"

		// compile shader into multiple variants, with and without shadows
		// (we don't care about any lightmaps yet, so skip these variants)
#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
		// shadow helper functions and macros
#include "AutoLight.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;

	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float4 position : COORDINATE0;
		float2 uv : TEXCOORD0; // texture coordinate
		float3 wnorm : TEXCOORD1;
		SHADOW_COORDS(2) // put shadows data into TEXCOORD1
			fixed3 ambient : COLOR1;
	};

	float4 _Color;
	float4 _ShadowColor;
	float _Cutoff;
	float _Smooth;
	sampler2D _MainTex;
	float _ShadowCutoff;
	float4 _ShadeColor;
	float4 _SpecularColor;
	float _SpecularCutoff;
	float _SpecularPower;
	float _SpecularMult;
	float _ShadowSmooth;

	v2f vert(appdata v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.position = v.vertex;

		half3 worldNormal = UnityObjectToWorldNormal(v.normal);
		o.wnorm = UnityObjectToWorldNormal(v.normal);
		// dot product between normal and light direction for
		// standard diffuse (Lambert) lighting
		o.uv = v.uv;

		o.ambient = ShadeSH9(half4(worldNormal,1));
		// compute shadows data
		TRANSFER_SHADOW(o)
			return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed shadow = -SHADOW_ATTENUATION(i) + 1;
	shadow = (shadow - _ShadowCutoff) / _Smooth;
	shadow = (clamp(shadow, -1.0, 1.0) + 1) / 2;

	float nl = dot(_WorldSpaceLightPos0, i.wnorm);
	nl = (nl - _Cutoff) / _Smooth;
	nl = (-clamp(nl, -1.0, 1.0) + 1) / 2;
	float lev1 = clamp(nl + shadow, 0.0, 1.0);

	fixed4 precol = tex2D(_MainTex, i.uv);
	fixed4 shadcol = _ShadowColor * precol;
	precol = precol * _Color;
	shadcol = shadcol - precol;

	fixed4 col = lev1 * shadcol + precol;

	return col;
	}
		ENDCG
	}

		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

	}
		FallBack "Diffuse"
}
