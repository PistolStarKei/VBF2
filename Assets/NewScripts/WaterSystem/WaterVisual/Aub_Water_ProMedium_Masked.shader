#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Aubergine/Object/NoLights/Water/ProMedium_Masked" {
	Properties {
		_Color("Main Color", Color) = (0, 0.15, 0.115, 1)
		_WaveTex("Wave Texture", 2D) = "bump" {}
		_WaveSpeed("Wave Speed", Range(0.0, 0.1)) = 0.01
		_FresnelTex("Fresnel Mask", 2D) = "white" {}
		_Refraction("Refraction Amount", Range(0.0, 1.0)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 400

		Pass {
			Lighting Off Fog { Mode Off }

			CGPROGRAM
				#include "UnityCG.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest

				fixed4 _Color;
				sampler2D _WaveTex, _FresnelTex, _ReflectionTex, _RefractionTex;
				fixed4 _WaveTex_ST;
				fixed _WaveSpeed, _Refraction;

				struct v2f {
					float4 Pos : SV_POSITION;
					fixed4 UvBump : TEXCOORD0;
					fixed2 UvFresnel : TEXCOORD1;
					fixed3 View : TEXCOORD2;
					fixed4 SPos : TEXCOORD3;
					fixed3 T2W0 : TEXCOORD4;
					fixed3 T2W1 : TEXCOORD5;
					fixed3 T2W2 : TEXCOORD6;
				};

				v2f vert (appdata_tan v) {
					v2f o;
					o.Pos = mul(UNITY_MATRIX_MVP, v.vertex);
					fixed2 uv = TRANSFORM_TEX(v.texcoord, _WaveTex);
					o.UvBump.xy = uv + (_Time.y * _WaveSpeed);
					o.UvBump.zw = uv - (_Time.y * _WaveSpeed);
					o.UvFresnel = v.texcoord;
					o.View = normalize(ObjSpaceViewDir(v.vertex));
					o.SPos = ComputeScreenPos(o.Pos);
					TANGENT_SPACE_ROTATION;
					o.T2W0 = mul(rotation, unity_ObjectToWorld[0].xyz * 1.0);
					o.T2W1 = mul(rotation, unity_ObjectToWorld[1].xyz * 1.0);
					o.T2W2 = mul(rotation, unity_ObjectToWorld[2].xyz * 1.0);
					return o;
				}

				fixed4 frag(v2f i) : COLOR {
					//Tangent Space Normals
					fixed4 bump = tex2D(_WaveTex, i.UvBump.xy) + tex2D(_WaveTex, i.UvBump.zw);
					bump *= 0.5;
					fixed3 normal = UnpackNormal(bump);
					fixed3 normalW;
					normalW.x = dot(i.T2W0, normal.xyz);
					normalW.y = dot(i.T2W1, normal.xyz);
					normalW.z = dot(i.T2W2, normal.xyz);
					normalW = normalize(normalW);

					//Reflection
					fixed4 projUv = i.SPos;
					projUv.xz += normalW.rb;
					fixed3 reflection = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(projUv)).rgb;
					//Refraction
					fixed3 refraction = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(projUv)).rgb;
					//Fresnel term
					fixed fresnel = tex2D(_FresnelTex, i.UvFresnel).r * _Refraction;

					fixed EdotN = max(dot(i.View, normalW), 0);
					fixed facing = (1.0 - EdotN);
					fixed3 deepCol = (reflection * (1 - fresnel) + _Color * fresnel);
					fixed3 waterCol = (_Color * (1.0 - facing) + deepCol * facing);
					fixed3 color = fresnel * refraction + waterCol;
					return fixed4(color, 1);
				}
			ENDCG
		}
	} 
	FallBack Off
}