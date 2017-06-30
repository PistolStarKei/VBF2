using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Kalagaan
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(VertExmotionSensorBase),true)]
	public class VertExmotionSensorEditor : Editor {


		public enum eSettingsMode
		{
			NONE,
			SENSORS,
			FX,
			COLLIDERS
		}


		//VertExmotionCollider m_vtmcol;
		static bool m_removeCollider = false;
		//static bool m_showSensorSettings = true;
		//static bool m_showColliderSettings = false;
		static eSettingsMode m_settingMode = eSettingsMode.SENSORS;


		static Color m_collisionZone = new Color(.8f,.1f,.2f,.5f);
		static Color m_collisionZoneAlpha = new Color(.8f,.1f,.2f,.08f);

		private static float[] m_fadePanel = {0,0,0};


		public void OnEnable()
		{
			//VertExmotionSensor vms = target as VertExmotionSensor;
			//m_vtmcol = vms.GetComponent<VertExmotionCollider> ();
			m_removeCollider = false;

		}

		public void OnDisable()
		{
			if (Application.isPlaying)
				return;

			if (target == null)
				return;

            /*
			VertExmotionColliderBase vtmCol = (target as VertExmotionSensorBase).GetComponentInChildren<VertExmotionColliderBase>();
            if (vtmCol != null)
            {
                VertExmotionEditor.ReplaceBaseClass(vtmCol);
                EditorUtility.SetDirty(vtmCol);
            }*/

			//VertExmotionEditor.ReplaceBaseClass ( (target as VertExmotionSensorBase) );
            //EditorUtility.SetDirty(target);
            
    
        }


		public override void OnInspectorGUI()
		{
			if (!VertExmotionEditor.m_editorInitialized)
				VertExmotionEditor.InitializeEditorParameters ();


			VertExmotionSensorBase vms = target as VertExmotionSensorBase;

			//DrawDefaultInspector ();
			DrawSensorSettings (vms);

			//vms.m_params.damping = Mathf.Clamp( EditorGUILayout.FloatField( "damping", vms.m_params.damping ), 0, 30f );
			//vms.m_params.bouncing = Mathf.Clamp( EditorGUILayout.FloatField( "bouncing", vms.m_params.bouncing ), 0, 30f );
	/*
			PID pid = new PID ();
			pid.m_params = new PID.Parameters( vms.m_pid.m_params );


			GUILayout.Box( " ", VertExmotionEditor.m_bgStyle, GUILayout.Height(50f), GUILayout.ExpandWidth(true) );
			pid.GUIDrawResponse ( GUILayoutUtility.GetLastRect(), m_timeUnit );
			m_timeUnit = EditorGUILayout.FloatField ("time unit (s)",m_timeUnit);
			m_timeUnit = Mathf.Clamp ( m_timeUnit, 1f, 10f);
	*/

			if (VertExmotionEditor.m_lastVTMSelected != null)
				if (GUILayout.Button ("Select " + VertExmotionEditor.m_lastVTMSelected.gameObject.name))
					Selection.activeGameObject = VertExmotionEditor.m_lastVTMSelected.gameObject;


			#if KVTM_DEBUG
			GUILayout.Label ( "--------------\nDEBUG\n--------------" );
			DrawDefaultInspector ();
			#endif

		}


		Vector2 m_scrollViewPos;
		//Rect m_bgRect = ;
		void OnSceneGUI()
		{
			if (!VertExmotionEditor.m_editorInitialized)
				VertExmotionEditor.InitializeEditorParameters ();

			VertExmotionEditor.m_showPanelProgress = 1f;
			VertExmotionEditor.m_showPanel = true;
			VertExmotionEditor.UpdateShowPanel ();
			float menuWidth = 215f;
			
			Handles.BeginGUI ();

			VertExmotionEditor.DrawBackground ();

			GUILayout.BeginHorizontal ();
			GUILayout.Space (5);

			m_scrollViewPos = GUILayout.BeginScrollView (m_scrollViewPos, GUILayout.Width (menuWidth));

			DrawSensorSettings (target as VertExmotionSensorBase);

			GUILayout.Space (10f);
			if (VertExmotionEditor.m_lastVTMSelected != null)
				if (GUILayout.Button ("Select " + VertExmotionEditor.m_lastVTMSelected.gameObject.name))
					Selection.activeGameObject = VertExmotionEditor.m_lastVTMSelected.gameObject;

			GUILayout.EndScrollView ();
			

			GUILayout.EndHorizontal ();




			Handles.EndGUI ();


			DrawSensorHandle ( target as VertExmotionSensorBase );

		}


		public static void DrawSensorSettings( VertExmotionSensorBase sensor )
		{

			PID pid = new PID ();
			pid.m_params = new PID.Parameters( sensor.m_pid.m_params );
			pid.m_params.limits.x = -float.MaxValue;
			pid.m_params.limits.y = float.MaxValue;

			
			//float timeUnit = EditorGUILayout.FloatField ("time unit (s)",m_timeUnit);
			//timeUnit = Mathf.Clamp ( timeUnit, 0.1f, 30f);
			
			GUILayout.Space (10);
			
			//if( GUILayout.Button( (m_showSensorSettings?"-":"+") + "Sensor Settings", VertExmotionEditor.m_styleTitle ) )
			if( GUILayout.Button( (m_settingMode==eSettingsMode.SENSORS?"-":"+") + "Sensor Settings", VertExmotionEditor.m_styleTitle ) )
			{
				//m_showSensorSettings = !m_showSensorSettings;
				m_settingMode= m_settingMode == eSettingsMode.SENSORS ? eSettingsMode.NONE : eSettingsMode.SENSORS ;
				//if( m_showSensorSettings ) m_showColliderSettings = false;
			}



			//if( m_showSensorSettings )
			//if( m_settingMode == eSettingsMode.SENSORS )
			if( BeginFadeGroup (eSettingsMode.SENSORS) )
			{
				GUILayout.Label( "Lock your sensor to a gameobject" );
				GUILayout.Label( "Set a bone for skinnedMesh" );
				GUILayout.BeginHorizontal ();
				GUILayout.Label( "parent", GUILayout.Width(75f) );
				sensor.m_parent = EditorGUILayout.ObjectField ( sensor.m_parent, typeof(Transform), true) as Transform;
				GUILayout.EndHorizontal ();


				GUILayout.Space (5);
				GUILayout.Label( "motion settings : " , VertExmotionEditor.m_styleBold );

				//sensor.m_params.translation.innerAmplitude = Mathf.Clamp( EditorGUILayout.FloatField( "outer amplitude", sensor.m_params.translation.innerAmplitude ), 0, float.MaxValue );
				//sensor.m_params.translation.amplitudeMultiplier = sensor.m_params.translation.amplitude;
				sensor.m_params.translation.amplitudeMultiplier = Mathf.Clamp( EditorGUILayout.FloatField( "Amplitude multiplier", sensor.m_params.translation.amplitudeMultiplier ), 0, float.MaxValue );
				//sensor.m_params.translation.amplitude = sensor.m_params.translation.amplitudeMultiplier;

				sensor.m_params.translation.outerMaxDistance = Mathf.Clamp( EditorGUILayout.FloatField( "Outer max distance", sensor.m_params.translation.outerMaxDistance ), 0, float.MaxValue );
				sensor.m_params.translation.innerMaxDistance = Mathf.Clamp( EditorGUILayout.FloatField( "Inner max distance", sensor.m_params.translation.innerMaxDistance ), 0, float.MaxValue );
				sensor.m_params.inflate = EditorGUILayout.FloatField( "Inflate", sensor.m_params.inflate );
				sensor.m_params.damping = Mathf.Clamp( EditorGUILayout.FloatField( "Damping", sensor.m_params.damping ), 0, 30f );
				sensor.m_pid.m_params.kp = sensor.m_params.damping;
				sensor.m_params.bouncing = Mathf.Clamp( EditorGUILayout.FloatField( "Bouncing", sensor.m_params.bouncing ), 0, 30f );
				sensor.m_pid.m_params.ki = sensor.m_params.bouncing;
				//GUILayout.Label( "", GUILayout.Height(50f) );
				GUILayout.Box( " ", VertExmotionEditor.m_bgStyle, GUILayout.Height(50f), GUILayout.ExpandWidth(true) );
				GUIDrawPidResponse ( pid, GUILayoutUtility.GetLastRect(), sensor.m_pidTime );
				
				sensor.m_pidTime = EditorGUILayout.Slider ("t (s)", sensor.m_pidTime, 1f, 10f);

				EditorGUILayout.EndFadeGroup ();
			}




			//-----------------------------------------------
			//FX settings
			//-----------------------------------------------
			GUILayout.Space (10);	
			if( GUILayout.Button( (m_settingMode==eSettingsMode.FX?"-":"+") + "FX Settings", VertExmotionEditor.m_styleTitle ) )			
				m_settingMode= m_settingMode == eSettingsMode.FX ? eSettingsMode.NONE : eSettingsMode.FX ;

			//if( m_settingMode == eSettingsMode.FX )
			if( BeginFadeGroup (eSettingsMode.FX) )
			{
				sensor.m_params.translation.gravityInOut = EditorGUILayout.Vector2Field( "Gravity in/out", sensor.m_params.translation.gravityInOut );
				sensor.m_params.translation.localOffset = EditorGUILayout.Vector3Field( "Local offset", sensor.m_params.translation.localOffset );
				sensor.m_params.translation.worldOffset = EditorGUILayout.Vector3Field( "World offset", sensor.m_params.translation.worldOffset );

				sensor.m_params.fx.squash = EditorGUILayout.FloatField( "Stretch", sensor.m_params.fx.squash );
				sensor.m_params.fx.stretchMax = EditorGUILayout.FloatField( "Stretch max dist", sensor.m_params.fx.stretchMax );
				sensor.m_params.fx.stretchMinSpeed = EditorGUILayout.FloatField( "Stretch min speed", sensor.m_params.fx.stretchMinSpeed );

				EditorGUILayout.EndFadeGroup ();
			}



			//-----------------------------------------------
			//collider settings
			//-----------------------------------------------

			GUILayout.Space (10);	
			if( GUILayout.Button( (m_settingMode==eSettingsMode.COLLIDERS?"-":"+") + "Collider Settings", VertExmotionEditor.m_styleTitle ) )			
				m_settingMode = m_settingMode == eSettingsMode.COLLIDERS ? eSettingsMode.NONE : eSettingsMode.COLLIDERS;

			//if( m_settingMode == eSettingsMode.COLLIDERS )
			if( BeginFadeGroup (eSettingsMode.COLLIDERS) )
			{

				VertExmotionColliderBase vtmCol = sensor.GetComponentInChildren<VertExmotionColliderBase>();
				if( vtmCol == null )
				{
					if( GUILayout.Button("Add collider") )
					{
                        EditorPrefs.SetInt("VertExmotion_LastMode", (int)VertExmotionEditor.eMode.SENSORS);
                        GameObject go = new GameObject("VMCollider");
						go.transform.parent = sensor.transform;
						go.transform.localScale = Vector3.one;
						go.transform.localPosition = Vector3.zero;
						go.transform.localRotation = Quaternion.identity;
						VertExmotionColliderBase collider = go.AddComponent<VertExmotionColliderBase>();
						VertExmotionEditor.ReplaceBaseClass( collider );                        
                        m_removeCollider = false;

					}
				}
				else
				{
					//draw collider GUI
					string[] options = new string[32];
					for( int i=0; i<32; ++i )
						options[i] = LayerMask.LayerToName(i);

					vtmCol.m_layerMask = EditorGUILayout.MaskField( "layer mask", vtmCol.m_layerMask, options );



					//show collision zone param
					int colZoneToDelete = -1;
					for( int j=0; j<vtmCol.m_collisionZones.Count; ++j )
					{
						GUILayout.BeginHorizontal();
						GUILayout.Label( "Zone " + j, VertExmotionEditor.m_styleBold );
						GUILayout.FlexibleSpace();
						if( GUILayout.Button("delete") )
							colZoneToDelete = j;
						GUILayout.EndHorizontal();
						vtmCol.m_collisionZones[j].positionOffset = EditorGUILayout.Vector3Field("position", vtmCol.m_collisionZones[j].positionOffset);
						vtmCol.m_collisionZones[j].radius = EditorGUILayout.FloatField("radius", vtmCol.m_collisionZones[j].radius );
						EditorUtility.SetDirty( vtmCol );

					}
					if( colZoneToDelete != -1 )
						vtmCol.m_collisionZones.RemoveAt(colZoneToDelete);
					




					if( GUILayout.Button("Add collision zone") )
					{
						vtmCol.m_collisionZones.Add( new VertExmotionColliderBase.CollisionZone() );
					}

					GUILayout.Space( 10 );

					//remove collider

					if( !m_removeCollider )
					{
						if( GUILayout.Button("Remove collider") )
						{
							m_removeCollider = true;
						}
					}
					else
					{
						GUILayout.BeginHorizontal();
						GUILayout.Label("remove collider?");

						if( GUILayout.Button("yes") )
						{
							if(Application.isPlaying)
								Destroy ( vtmCol.gameObject );
							else
								DestroyImmediate( vtmCol.gameObject );
						}

						if( GUILayout.Button("no") )
							m_removeCollider = false;

						GUILayout.EndHorizontal();
					}
				}

				EditorGUILayout.EndFadeGroup ();
			}


			EditorUtility.SetDirty (sensor);

			
		}





		public static bool BeginFadeGroup( eSettingsMode mode )
		{
//			m_fadePanel [(int)mode - 1] += ( (m_settingMode == mode) ? VertExmotionEditor.m_dt : -VertExmotionEditor.m_dt ) * 10f;
//			m_fadePanel [(int)mode - 1] = Mathf.Clamp (m_fadePanel [(int)mode - 1], 0.001f, 1f);

			m_fadePanel [(int)mode - 1] = (m_settingMode == mode) ? 1f : 0f;

			if( m_fadePanel [(int)mode - 1] != 0f )
				EditorGUILayout.BeginFadeGroup (m_fadePanel [(int)mode - 1]);

			//return true;
			return m_fadePanel [(int)mode - 1] != 0f;
		}




		
		
		
		public static void GUIDrawPidResponse(PID pid,  Rect area, float timeUnit )
		{
			
			Color c = new Color (1f, 1f, 1f, .1f); 
			
			pid.Init ();
			//unit step
			pid.m_target = 1;
			float r = 0;
			Vector2 start = new Vector2( area.x, area.y+area.height );
			Vector2 end = start;
			
			Handles.color = c;
			for (int i=0; i<timeUnit; ++i)
			{
				start = new Vector2( (float)i*area.width / timeUnit, area.y+area.height );
				end = new Vector2( (float)i*area.width / timeUnit, area.y );
				//GLDraw.DrawLine (start, end, c, 1f);
				Handles.DrawLine(start, end);
			}
			
			start = new Vector2( area.x, area.y+area.height*.5f );
			end = new Vector2( area.x+area.width, area.y+area.height*.5f );
			//GLDraw.DrawLine (start, end, c, 1f);
			Handles.DrawLine (start, end);
			
			
			
			
			start = new Vector2( area.x, area.y+area.height );
			end = start;
			
			for( int i=0; i<area.width; ++i )
			{
				float dt = (float)timeUnit / (float)area.width;
				for( int j=0; j<10f; ++j )
					r = pid.Compute( r , dt*.1f );			
				end.x++;			
				end.y = area.height-r*area.height*.5f + area.y;
				end.y = Mathf.Clamp( end.y,  area.y,  area.y+area.height );
				
				//GLDraw.DrawLine (start, end, Color.green, 1f);
				Handles.color = VertExmotionEditor.orange;
				Handles.DrawLine (start, end);
				start = end;
				
				//			//draw error
				//			errEnd.x++;
				//			errEnd.y = area.height - (float) lastErr;
				//			errEnd.y = Mathf.Clamp( errEnd.y,  area.y,  area.y+area.height );
				//			GLDraw.DrawLine (errStart, errEnd, Color.red, 1f);
				//			errStart = errEnd;
			}
			
		}




		public static bool DrawSensorHandle( VertExmotionSensorBase sensor )
		{
			bool useHandle = false;
			Color handleColor = VertExmotionEditor.orange;
			Handles.color = handleColor;
			Camera svCam = SceneView.currentDrawingSceneView.camera;
			float constUnit = (svCam.ViewportToWorldPoint (Vector3.zero) - svCam.ViewportToWorldPoint (Vector3.one)).magnitude;
			constUnit = HandleUtility.GetHandleSize(sensor.transform.position) * 10f;

			if( m_settingMode == eSettingsMode.SENSORS )
			{

				Handles.DrawSolidDisc( sensor.transform.position , -svCam.transform.forward, ( constUnit*.01f ) );
				Handles.DrawWireDisc( sensor.transform.position , -svCam.transform.forward, sensor.m_envelopRadius*VertExmotionBase.GetScaleFactor(sensor.transform) );	
				
				for( int i=0; i<10; ++i )
				{
					handleColor.a = (float)(10-i)/10f * .5f;
					float f = (float)i/11f * (float)i/11f;
					Handles.color = handleColor;
					Handles.DrawWireDisc( sensor.transform.position , -svCam.transform.forward, sensor.m_envelopRadius*VertExmotionBase.GetScaleFactor(sensor.transform) * f );
				}
				
				handleColor = VertExmotionEditor.orange;
				Handles.color = handleColor;
				
				Vector3 lastPos = sensor.transform.position;
				sensor.transform.position = 
					Handles.FreeMoveHandle(  sensor.transform.position, Quaternion.identity, ( constUnit*.02f ), Vector3.zero, Handles.CircleCap );
				if( lastPos != sensor.transform.position )
					useHandle = true;
				
				float lastRadius = sensor.m_envelopRadius;

				if( VertExmotionBase.GetScaleFactor(sensor.transform) > 0 )
				{
				sensor.m_envelopRadius =
					Vector3.Distance( 
						                 Handles.FreeMoveHandle(  sensor.transform.position + svCam.transform.right * sensor.m_envelopRadius* VertExmotionBase.GetScaleFactor(sensor.transform ) , Quaternion.identity, ( constUnit*.02f ), Vector3.zero, Handles.CubeCap )
						                 , sensor.transform.position ) / VertExmotionBase.GetScaleFactor( sensor.transform );
				}

				if( lastRadius != sensor.m_envelopRadius )
					useHandle = true;
				
				
				Handles.color = Color.cyan;

				//draw direction

				lastPos = sensor.transform.position + sensor.transform.forward * constUnit * .1f;
				Handles.DrawLine( sensor.transform.position, lastPos );

				lastPos = Handles.FreeMoveHandle(  lastPos, Quaternion.identity, ( constUnit*.01f ), Vector3.zero, Handles.CircleCap );
				if( lastPos != sensor.transform.position + sensor.transform.forward * constUnit * .1f )
				{
					sensor.transform.LookAt( lastPos );
					useHandle = true;
				}
			



				//--------------------------------------------------
				//draw sensors limits
				//--------------------------------------------------
				Color col = Color.blue;
				col.a = .01f;
				Handles.color = col;

				Vector3[] limitAxis = new Vector3[4];
				limitAxis [0] = sensor.transform.right;
				limitAxis [1] = -sensor.transform.right;
				limitAxis [2] = sensor.transform.up;
				limitAxis [3] = -sensor.transform.up;

				float sf = VertExmotionBase.GetScaleFactor (sensor.transform);
				for( int n=0; n<4; n++ )
				{
					col.a = .3f;
					Handles.color = col;

					float max = 20f;
					Vector3[] points = new Vector3[(int)max+1];
					for( float i=0; i<=max; ++i )
					{

						Vector3 p1 = Quaternion.AngleAxis( i/max * 180f, limitAxis[n] ) *  sensor.transform.forward * Mathf.Max(sensor.m_params.translation.innerMaxDistance,sensor.m_params.translation.outerMaxDistance) * sf;
						float lerpFactor = ( Vector3.Dot (sensor.transform.forward, p1.normalized) + 1f ) * .5f;
						float clampMag = ( Mathf.Lerp (sensor.m_params.translation.innerMaxDistance, sensor.m_params.translation.outerMaxDistance, lerpFactor) * sf );

						p1 = Vector3.ClampMagnitude( p1,clampMag );
						p1 += sensor.transform.position;

						if( i%4==0 )
							Handles.DrawDottedLine (sensor.transform.position, p1, 3f);
						points[(int)i] = p1;
					}

					col.a = .5f;
					Handles.color = col;
					Handles.DrawPolyLine ( points);
				}
				col.a = .03f;
				Handles.color = col;
				Handles.DrawSolidDisc( sensor.transform.position, sensor.transform.forward, (sensor.m_params.translation.innerMaxDistance+sensor.m_params.translation.outerMaxDistance) * .5f *sf );
				col.a = .5f;
				Handles.color = col;
				Handles.DrawWireDisc( sensor.transform.position, sensor.transform.forward, (sensor.m_params.translation.innerMaxDistance+sensor.m_params.translation.outerMaxDistance) * .5f *sf );
			}


			//--------------------------------------------------
			//draw collider handles
			//--------------------------------------------------
			if( m_settingMode == eSettingsMode.COLLIDERS )
			{
				VertExmotionColliderBase vtmCol = sensor.GetComponentInChildren<VertExmotionColliderBase>();

				Handles.color = Color.cyan;

				if( vtmCol != null)
				{
					for(int i=0; i<vtmCol.m_collisionZones.Count;++i)
					{
						Vector3 worldColZonePos = vtmCol.transform.TransformPoint( vtmCol.m_collisionZones[i].positionOffset );

						float radius = vtmCol.m_collisionZones[i].radius* VertExmotionBase.GetScaleFactor( vtmCol.transform );

						//Handles.DrawSolidDisc( vtmCol.m_collisionZones[i].positionOffset + sensor.transform.position , -svCam.transform.forward, ( constUnit*.01f ) );
						Handles.color = m_collisionZoneAlpha;
						Handles.DrawSolidDisc( worldColZonePos , -svCam.transform.forward, radius );	
						Handles.color = m_collisionZone;
						Handles.DrawWireDisc( worldColZonePos , sensor.transform.forward, radius );	
						Handles.DrawWireDisc( worldColZonePos , sensor.transform.up, radius );
						Handles.DrawWireDisc( worldColZonePos , sensor.transform.right, radius );


						Handles.DrawSolidDisc( worldColZonePos , -svCam.transform.forward, ( constUnit*.01f ) );	

						if( VertExmotionBase.GetScaleFactor( vtmCol.transform )>0 )
						{
							vtmCol.m_collisionZones[i].radius =
								Vector3.Distance( 
								                 Handles.FreeMoveHandle(  worldColZonePos + svCam.transform.right * radius , Quaternion.identity, ( constUnit*.02f ), Vector3.zero, Handles.CubeCap )
								                 , worldColZonePos ) / VertExmotionBase.GetScaleFactor( vtmCol.transform );
						}

						vtmCol.m_collisionZones[i].positionOffset = vtmCol.transform.InverseTransformPoint( Handles.FreeMoveHandle( worldColZonePos, Quaternion.identity, ( constUnit*.02f ), Vector3.zero, Handles.CircleCap ) );
					}
				}
			}


			if( useHandle )
				EditorUtility.SetDirty( sensor );


			return useHandle;
		}



	}
}
