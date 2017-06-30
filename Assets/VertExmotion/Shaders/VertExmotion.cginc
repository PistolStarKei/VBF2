// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'


#if SHADER_API_MOBILE
	static const int NB_SENSORS = 4;
#elif SHADER_TARGET < 30
	static const int NB_SENSORS = 8;
#else
	static const int NB_SENSORS = 20;
#endif

uniform float4 _SensorPosition[NB_SENSORS];
uniform float4 _MotionDirection[NB_SENSORS];
uniform float4 _MotionAxis[NB_SENSORS];
uniform int _MotionZoneID[NB_SENSORS];
uniform float4 _RadiusCentripetalTorque[NB_SENSORS];
uniform float4 _SquashStretch[NB_SENSORS];
uniform float4 _Speed[NB_SENSORS];



float3 VertExmotionComputeNormal(float3 n, float3 vOffset, float4 tan)
{
	float3 biTan = cross(n, tan.xyz) * tan.w;
	float3x3 rotation = transpose(float3x3(tan.xyz, biTan, n));

	return -n;
}

float4 VertExmotionBase(float4 wrldPos, float4 col)
{
	
	//int sensorId = 0;	
	//compute torque
	float3 torqueDir = float3(0,0,0);
	float4 motionDir = float4(0,0,0,0);
	float3 centripetalDir = float3(0,0,0);
	float dist;
	float3 squash = float3(0,0,0);
	
	for( int i=0; i<NB_SENSORS; ++i )
	{
		dist = distance(wrldPos.xyz,_SensorPosition[i].xyz);
		
		if( dist < _RadiusCentripetalTorque[i].x )
		{
			torqueDir.xyz = cross( normalize( wrldPos.xyz-_SensorPosition[i].xyz).xyz, _MotionAxis[i].xyz ) ;
			torqueDir *= _RadiusCentripetalTorque[i].z * dist;	
			
			centripetalDir = normalize( (wrldPos-_SensorPosition[i]).xyz );			
			motionDir.xyz +=  (_MotionDirection[i].xyz + torqueDir + centripetalDir * _RadiusCentripetalTorque[i].y) * (1.0f - dist/(_RadiusCentripetalTorque[i].x+.0000001f));


#if SHADER_API_MOBILE
#elif SHADER_TARGET < 30
#else
			if( length(_Speed[i].xyz) > 0 )
			{
				//stretch
				float d = dot( _Speed[i].xyz,  centripetalDir );
				if(d>=0)
					motionDir.xyz += d * d * d * _SquashStretch[i].y * _Speed[i].xyz;		
//				else			
//					motionDir.xyz += d * d * d * _SquashStretch[i].y * _Speed[i].xyz * .1f;
			
				//stretch reduce volume
				float3 c1 = cross( normalize( _Speed[i].xyz ), centripetalDir );
				float3 c2 = cross( normalize( _Speed[i].xyz ), c1 );
				float d2 = dot( (wrldPos-_SensorPosition[i]).xyz, c2 );
				
				if( length(c2)>0 )
				{  
				motionDir.xyz -= normalize(c2)* length(_Speed[i].xyz) * d2 * _SquashStretch[i].y * .8f;				
				motionDir.xyz += normalize(c2)* length(_Speed[i].xyz)* d2 * _SquashStretch[i].x;
				}
			}
#endif				
		
		}		
	}	 

	//vpos.xyz = mul(_World2Object, wrldPos + motionDir *  (col.g)).xyz * 1.0;
	//return vpos;
	return (wrldPos + motionDir *  (col.g));
}


float4 VertExmotion(float4 vpos, float4 col)
{
	float4 wrldPos = mul(unity_ObjectToWorld, vpos); 
		
	wrldPos = VertExmotionBase(wrldPos, col);

	vpos.xyz = mul(unity_WorldToObject, wrldPos);
	return vpos;
}


float4 VertExmotion(float4 vpos, float3 n, float3 t, float4 col)
{
	float4 wrldPos = mul(unity_ObjectToWorld, vpos);

	wrldPos = VertExmotionBase(wrldPos, col);

	vpos.xyz = mul(unity_WorldToObject, wrldPos);
	return vpos;
}

float4 VertExmotionUV( float4 vpos, float4 uv )
{	
	half4 wrldPos = mul( unity_ObjectToWorld, vpos  );	
	int sensorId = 0;
	
	//compute torque
	half3 torqueDir = half3(0,0,0);
	half4 motionDir = half4(0,0,0,0);
	half3 centripetalDir = half3(0,0,0);
	half dist;
	
	for( int i=0; i<NB_SENSORS; ++i )
	{
		sensorId = i;

		torqueDir.xyz = cross( normalize( wrldPos.xyz-_SensorPosition[sensorId].xyz).xyz, _MotionAxis[sensorId].xyz ) ;
		torqueDir *= _RadiusCentripetalTorque[sensorId].z;	
		
		centripetalDir = normalize( (wrldPos-_SensorPosition[sensorId]).xyz ) * _RadiusCentripetalTorque[sensorId].y;			
		motionDir.xyz += _MotionDirection[sensorId].xyz + torqueDir + centripetalDir;
	}
		
	vpos.xyz = mul( unity_WorldToObject, wrldPos + motionDir *  uv.y *  uv.y * uv.y).xyz;
	vpos.w = vpos.w;
	return vpos;
}


void VertExmotion( inout appdata_full v )
{			
	 float4 v2 = VertExmotion( v.vertex, v.color );		 
	 v.normal = VertExmotionComputeNormal( v.normal, v.tangent , v2-v.vertex);
	 v.vertex = v2;
}



void VertExmotionUV( inout appdata_full v )
{			
	v.vertex = VertExmotionUV( v.vertex, v.texcoord );	
}


float3 VertExmotionSF(float3 wrldXYZ, float wrldW, float3 col)
{
	return VertExmotionBase( float4(wrldXYZ, wrldW), float4(col,0)) - wrldXYZ;
	//return wrldXYZ;
}

