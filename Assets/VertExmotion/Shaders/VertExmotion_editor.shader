// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "Hidden/VertExmotion_editor" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		 //_Weights ("Weights", Range (0, 1)) = 0
		 _SensorId( "SensorId", int ) = 1
	}
	SubShader {
		
		Pass{
	CGPROGRAM
		//#pragma target 3.0
		
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		#include "./VertExmotion.cginc"

		uniform float4 _SensorPositionEditor[NB_SENSORS];
		uniform float4 _RadiusCentripetalTorqueEditor[NB_SENSORS];

		struct vertexToFragment {
		float4 vertex : POSITION;
		float2 uv_MainTex : TEXCOORD0;
		float4 color : COLOR;		
		};


		
		sampler2D _MainTex;
		int _SensorId;

		void vert (appdata_full v, out vertexToFragment o) {

		if( _SensorId == -1 )
		{
			o.color = v.color.gggg ;
		}
		else
		{
			float4 wrldPos = mul( unity_ObjectToWorld, v.vertex  );	
			float dist = distance(wrldPos.xyz, _SensorPositionEditor[_SensorId].xyz) * 1.0;
			
			if( dist < _RadiusCentripetalTorqueEditor[_SensorId].x )
				o.color = lerp( float4(0,1,0,1), v.color.gggg, dist/(_RadiusCentripetalTorqueEditor[_SensorId].x+.0000001f) ) * v.color.g;
			else
				o.color = v.color.gggg;
				
		}		
		o.vertex = mul (UNITY_MATRIX_MVP, v.vertex);	
		o.uv_MainTex = float4( v.texcoord.xy, 0, 0 );
			
		}


		fixed4 frag(vertexToFragment IN) : COLOR {
			float4 col;  	
			
			col = tex2D (_MainTex, IN.uv_MainTex);	
			col.x = col.y = col.z = (col.x + col.y + col.z) / 3;

			return lerp( IN.color, col, .4 );
		}
		ENDCG

	 }
	} 
	//FallBack "Diffuse"
}
