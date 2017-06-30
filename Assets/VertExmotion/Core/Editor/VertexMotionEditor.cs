//#define KVTM_DEBUG


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Kalagaan
{
//[CanEditMultipleObjects]
[CustomEditor(typeof(VertExmotionBase),true)]
public class VertExmotionEditor : Editor {


	public enum eMode
	{
		INFO,
		PAINT,
		SENSORS
	}

    public enum eRendererType
    {
        NONE,
        MESH,
        SKINNEDMESH,
        SPRITE
    }



        Vector3[] m_vtx;
	Vector3[] m_nrml;


	public eMode m_mode = eMode.INFO;
    public eRendererType m_rendererType = eRendererType.NONE;

    float m_brushSize;
	float m_drawRadius = .05f;
	float m_drawIntensity = 1f;
	float m_drawFalloff = 1f;

	//Vector3 m_camLockPosition;
	//Quaternion m_camLockRotation;

	public static GUIStyle m_styleBold;
	public static GUIStyle m_styleTitle;
	public static GUIStyle m_styleHighLight;
	public static float m_dt = 0;
	Material m_paintMaterial;

	bool m_eraseMode = false;
	bool m_modeTrigger = false;
	bool m_drawBrushSettings = false;
	bool m_enableBrushMenuContextual = false;
	Rect m_brushMenuRect;




	//bool m_shiftDown = false;
	bool m_ctrlDown = false;
	bool m_initialized = false;
	public static bool m_editorInitialized = false;

    public bool m_editorInstanceInitialized = false;

	int m_sensorId = -1;
	int m_sensorToDelete = -1;
	int m_sensorToRemove = -1;

	Tool m_lastTool = Tool.None;
	bool m_lastOrthoMode = false;

	public static string m_exportMeshName = "";
    public static bool m_exportFlag = false;

	bool m_useUV1 = true;


	public static VertExmotionBase m_lastVTMSelected;
    //public static eMode m_lastMode = eMode.INFO;

	VertExmotionSensorBase m_externalSensor;

    MaterialPropertyBlock m_matPropBlk;
    Vector4[] m_sensorpos = new Vector4[50];
    Vector4[] m_RadiusCentripetalTorque = new Vector4[50];

        //bool m_distributionClassFound = false; 


        public override void OnInspectorGUI () 
	{

		if (!m_editorInitialized)
			InitializeEditorParameters ();

			if( m_exportFlag )
			{
				ExportMesh();
				m_exportFlag = false;
			}

		//GUI.backgroundColor = grey;
		//GUI.contentColor = Color.cyan;
		GUI.color = Color.white;


		//GUI.skin = (GUISkin)Resources.Load("VertExmotionSkin",typeof(GUISkin));
		/*
		 * //show all style in editor
		int id = 0;
		foreach(GUIStyle style in GUI.skin.customStyles)
		{
			if( style.name.Contains("ack") )
				GUILayout.Button("" + id + " " + style.name,style);
			id++;
		}*/


		VertExmotionBase vtm = (target) as VertExmotionBase;

		//clean sensor list
		while( vtm.m_VertExmotionSensors.Contains(null) )
			vtm.m_VertExmotionSensors.Remove( null );
		
		GUILayout.BeginHorizontal ();
		GUILayout.Space(30);
		DrawLogo ();
		GUILayout.Label ( "v"+VertExmotionBase.version );
		GUILayout.EndHorizontal ();

		if( GUILayout.Button( m_showPanel?"Hide panel":"Show panel" ) )
			m_showPanel = !m_showPanel;

	/*
        if( m_mode == eMode.SENSORS )
		    DrawSensorsList ( vtm );
    */

		if( !m_showPanel )
			m_mode = eMode.INFO;


		


		#if KVTM_DEBUG
		GUILayout.Label ( "--------------\nDEBUG\n--------------" );
		DrawDefaultInspector ();
		#endif

		if( Application.isPlaying )
			GUI.enabled = false;
		

		
//		if( m_distributionClassFound )
//			{
//				if (GUILayout.Button ("Pack"))
//					VertExmotionPacker.Pack ( vtm );
//			}

			Repaint ();
		
	}



        public static void InitializeEditorParameters()
        {
            //if( m_styleBold == null  )
            {


                m_styleBold = new GUIStyle();
                if (EditorGUIUtility.isProSkin)
                    m_styleBold.normal.textColor = Color.white;
                m_styleBold.fontStyle = FontStyle.Bold;

                m_styleTitle = new GUIStyle();
                m_styleTitle.normal.textColor = orange;
                m_styleTitle.normal.background = new Texture2D(1, 1);
                m_styleTitle.normal.background.SetPixel(0, 0, grey);
                m_styleTitle.normal.background.Apply();
                m_styleTitle.fontStyle = FontStyle.Bold;

                m_styleHighLight = new GUIStyle();
                m_styleHighLight.normal.textColor = Color.white;
                m_styleHighLight.fontStyle = FontStyle.Bold;
                m_styleHighLight.normal.background = new Texture2D(1, 1);
                m_styleHighLight.normal.background.SetPixel(0, 0, orange);
                m_styleHighLight.normal.background.Apply();
                m_styleHighLight.alignment = TextAnchor.MiddleCenter;

                m_bgStyle.normal.background = new Texture2D(1, 1);
                m_bgStyle.normal.background.SetPixel(0, 0, grey);
                m_bgStyle.normal.background.Apply();


            }
        }

        public void InitializeEditorInstance()
        {
            //Debug.Log("InitializeEditorInstance");           

            VertExmotionBase vtm = (target) as VertExmotionBase;

            if (vtm.GetComponent<MeshRenderer>())            
                m_rendererType = eRendererType.MESH;            

            if ( vtm.GetComponent<SkinnedMeshRenderer>() )            
                m_rendererType = eRendererType.SKINNEDMESH;

            if (vtm.GetComponent<SpriteRenderer>())
                m_rendererType = eRendererType.SPRITE;

            if (vtm.GetComponent<TextMesh>())
                m_rendererType = eRendererType.SPRITE;

            m_editorInstanceInitialized = true;
            //Debug.Log("" + m_rendererType);

            m_matPropBlk = new MaterialPropertyBlock();
            vtm.renderer.GetPropertyBlock(m_matPropBlk);
            vtm.CleanShaderProperties();
            
            if (EditorPrefs.HasKey("VertExmotion_LastMode"))
            {
                Debug.Log((eMode)EditorPrefs.GetInt("VertExmotion_LastMode"));
                m_mode = (eMode)EditorPrefs.GetInt("VertExmotion_LastMode");
                EditorPrefs.DeleteKey("VertExmotion_LastMode");
            }

            //m_lastMode = eMode.INFO;

            

        }





	GUISkin m_skin;
	void OnSceneGUI()
	{
//			if (Application.isPlaying)
//								return;

		GUI.backgroundColor = grey;

		//if (Application.isPlaying)
		//	return;

		if (!m_editorInitialized)
				InitializeEditorParameters ();

            if (!m_editorInstanceInitialized)
                InitializeEditorInstance();

		Event e = Event.current;

		VertExmotionBase vtm = (target) as VertExmotionBase;

		if( !m_initialized && !Application.isPlaying && m_rendererType!= eRendererType.SPRITE )
		{
			vtm.InitVertices ();
			//vtm.InitMesh();
			InitVerticesPosDictionary ();
			m_initialized = true;

			if( m_vtx == null )
				m_vtx = vtm.m_mesh.vertices.Clone () as Vector3[];
			if( m_nrml == null )
				m_nrml = vtm.m_mesh.normals.Clone () as Vector3[];
		}

		Camera svCam = SceneView.currentDrawingSceneView.camera;


		
/*
		if( m_initialShader == null )
			m_initialShader = vtm.renderer.sharedMaterial.shader;
		if( m_initialMaterial == null )
			m_initialMaterial = vtm.renderer.sharedMaterial;*/

		Vector2 mp2d = Event.current.mousePosition;
		mp2d.y = svCam.pixelHeight - mp2d.y;		
		Vector3 mp = svCam.ScreenToWorldPoint ( mp2d );
		mp +=  svCam.transform.forward; 

		//Debug.Log ("mouse pos "+ mp);





		//-----------------------------------------------------------------------
		//draw motion sensor position
		bool useHandle = false;
		if( m_mode== eMode.SENSORS && m_sensorId != -1 && m_sensorId<vtm.m_VertExmotionSensors.Count )
			useHandle = VertExmotionSensorEditor.DrawSensorHandle( vtm.m_VertExmotionSensors[m_sensorId] );

		//-----------------------------------------------------------------------
		//Paint Vertices
		if( !useHandle && m_mode == eMode.PAINT && !Application.isPlaying )
			PaintVertice( vtm, svCam, mp );




		//-----------------------------------------------------------------------


		if (e.control )
		{
			//toggle paint mode with shift
			if( !m_ctrlDown )
				m_eraseMode = !m_eraseMode;
			m_ctrlDown = true;
		}
		else
		{
			m_ctrlDown = false;
		}








		//switch render mode & shader
		if ( (m_mode == eMode.PAINT || m_mode == eMode.SENSORS) && !Application.isPlaying )
		{

			Selection.activeGameObject = vtm.gameObject;

			//disable drag selection
			HandleUtility.AddDefaultControl( GUIUtility.GetControlID( GetHashCode(), FocusType.Passive ) );

			if( !m_modeTrigger )
			{
				for( int i=0; i<vtm.renderer.sharedMaterials.Length; ++i )
					vtm.renderer.sharedMaterials[i].shader = Shader.Find ( "Hidden/VertExmotion_editor" );
				//m_lastTool = Tools.current;
				m_lastOrthoMode = SceneView.lastActiveSceneView.orthographic;
			}

			//SceneView.currentDrawingSceneView.camera.isOrthoGraphic = true;
			SceneView.lastActiveSceneView.orthographic = true;
			Tools.current = Tool.None;
			m_modeTrigger = true;
			//vtm.Update();
			//Event.current.Use();
		}
		else
		{
			if( m_modeTrigger )
			{
				for(int i=0;i<vtm.renderer.sharedMaterials.Length;++i)
					vtm.renderer.sharedMaterials[i].shader = vtm.m_initialShaders[i];
				Tools.current = m_lastTool;
				SceneView.lastActiveSceneView.orthographic = m_lastOrthoMode;
			}
			m_modeTrigger = false;
		}

		DrawMenu ( vtm, e );

		if (e.alt)
		{
			if( !m_drawBrushSettings )
				m_brushMenuRect = new Rect( Event.current.mousePosition.x, Event.current.mousePosition.y, 150f, 200f  );
			DrawBrushMenuContextual (e);
			m_drawBrushSettings = true;
		}
		else
		{
			m_drawBrushSettings = false;
		}


		if (m_mode == eMode.PAINT )
			DrawCursor( mp, svCam );


		SceneView.currentDrawingSceneView.Repaint ();
	}



	public static bool m_showPanel = true;
	public static float m_showPanelProgress = 0f;
	static Rect m_menuRect;
	static PID m_panelPID = new PID( 5f, .5f, 0f );
	static System.DateTime m_lastTime = System.DateTime.Now;

	static public void UpdateShowPanel()
	{
		//m_showPanelProgress += m_showPanel?.05f:-.05f;
		m_panelPID.m_target = m_showPanel ? 1f : 0f;
		m_panelPID.m_params.limits.x = 0;
		m_panelPID.m_params.limits.y = 1;
		System.TimeSpan dt = System.DateTime.Now - m_lastTime;
		m_lastTime = System.DateTime.Now;
		m_dt = (float)dt.TotalSeconds;
			m_showPanelProgress = m_panelPID.Compute (m_showPanelProgress, m_dt);
		//m_showPanelProgress = Mathf.Clamp01( m_showPanelProgress );

		m_menuRect = new Rect(-(1f-m_showPanelProgress)*m_menuWidth, 0, m_menuWidth, Screen.height);
		//Debug.Log ( "ShowPanel " + m_showPanelProgress );
	}



	//-----------------------------------------------------
	//Paint function
	//-----------------------------------------------------
	void PaintVertice( VertExmotionBase vtm, Camera svCam, Vector3 mp )
	{
        if (m_rendererType == eRendererType.SPRITE )
            return;

		//this is a screen base unit, don't change on zoom
		float constUnit = (svCam.ViewportToWorldPoint (Vector3.zero) - svCam.ViewportToWorldPoint (Vector3.one)).magnitude;
		constUnit = HandleUtility.GetHandleSize(vtm.transform.position) * 10f;
		m_brushSize = m_drawRadius * constUnit;
		float paintRange = ( svCam.WorldToScreenPoint (mp) - svCam.WorldToScreenPoint (mp + svCam.transform.right * m_brushSize ) ).magnitude;
            //Debug.Log(""+ m_rendererType);
            //if( false )
            for ( int i=0; i< m_vtx.Length; ++i )
			//foreach( KeyValuePair<Vector3, List<int>> kvp in m_posToVertices )
		{
			
			//Handles.DrawLine( vtm.transform.InverseTransformPoint( vtm.m_mesh.vertices[i] ), vtm.transform.InverseTransformPoint( Vector3.zero ) );
			if( Vector3.Dot( vtm.transform.TransformDirection(m_nrml[i]).normalized, -svCam.transform.forward ) > 0 )
			{
                    Vector3 localpos = m_vtx[i];
                    if (m_rendererType == eRendererType.SKINNEDMESH)
                    {
                        //Debug.Log("eRendererType.SKINNEDMESH");
                        localpos.x /= vtm.transform.localScale.x;
                        localpos.y /= vtm.transform.localScale.y;
                        localpos.z /= vtm.transform.localScale.z;
                    }

                    Vector3 worldPos = vtm.transform.TransformPoint(localpos);
				//Vector3 worldPos = vtm.transform.TransformPoint( kvp.Key );
                //if (vtm.GetComponent<SkinnedMeshRenderer>())//sometime meshes are scaled
                


				Vector3 vertex2D = svCam.WorldToScreenPoint( worldPos );
				vertex2D.z=0;
				Vector3 mouse2D = svCam.WorldToScreenPoint( mp );
				mouse2D.z=0;
				float dist = ( vertex2D - mouse2D ).magnitude;
				
/*
				//test----------
				if( i==0 )
				{				
					Handles.color = Color.green;
					Handles.DrawWireDisc( worldPos, -svCam.transform.forward, constUnit*.01f );	
						//Debug.Log( "dist : " + dist + " < " + paintRange );
						Debug.Log( "point :" + svCam.WorldToScreenPoint( worldPos ) + "  mouse :" + svCam.WorldToScreenPoint( mp ) );
				}
				//--------------
*/

				if( dist < paintRange  )
				{
					float falloff = m_drawFalloff * (dist/paintRange);
					float intensity =  m_drawIntensity - m_drawIntensity*falloff;
					
					
					Handles.color = Color.Lerp(m_eraseMode?Color.red:Color.green, Color.blue, m_drawIntensity * falloff);
					Handles.DrawSolidDisc( worldPos, -svCam.transform.forward, ( constUnit*.001f + constUnit*.0015f * intensity  ) );		
					
					if( !Event.current.alt && Event.current.type == EventType.MouseDrag &&  Event.current.button == 0 /*&& !useHandle*/ )
					{
						//int i=kvp.Value[0];
						Color vc = vtm.m_vertexColors[i];
						
						//Update color
						if( ! m_eraseMode )
							vc = Color.Lerp( vtm.m_vertexColors[i], Color.green, intensity * .2f );
						else
							vc = Color.Lerp( vtm.m_vertexColors[i], Color.black, intensity * .2f );
						
						//for( int j=1; j<kvp.Value.Count; ++j )
						//	vtm.m_vertexColors[kvp.Value[j]] = vc;
						
						//Debug.Log ( "paint " + vc );
						//update all vertices at same position
						for( int vid=0; vid<m_posToVertices[m_vtx[i]].Count; ++vid )
							vtm.m_vertexColors[m_posToVertices[m_vtx[i]][vid]] = vc;
						
					}
				}
			}		
			
		}
		//-----------------------------------------------------------------------
		//update vertices
		if( Event.current.type == EventType.MouseDrag && Event.current.button < 2 )
		{
			//refresh colors
			vtm.m_mesh.colors32 = vtm.m_vertexColors;
			vtm.ApplyMotionData();
			EditorUtility.SetDirty( vtm );
			//Debug.Log( "update vertices" );
		}
	}
	
	
	
	
	
	//-----------------------------------------------------
	//Draw Menu
	//-----------------------------------------------------
	static float m_menuWidth = 220f;

	public static Color orange = new Color (240f/255f, 158f/255f, 0);
	public static Color grey = new Color (41f/255f, 41f/255f, 41f/255f, .9f);

	Vector2 m_scrollViewPos;
	void DrawMenu( VertExmotionBase vtm, Event e )
	{
		UpdateShowPanel ();
		
		Handles.BeginGUI ();

		//GUI.matrix.SetTRS (Vector3.right * (-200f), Quaternion.identity, Vector3.one); 

		DrawBackground ();

		GUI.backgroundColor = orange;
		if( GUI.Button( new Rect( m_menuWidth *m_showPanelProgress-12,0,24,35 ), m_showPanel?"x":">" ) )
			m_showPanel = !m_showPanel;
		GUI.backgroundColor = Color.white;

		GUILayout.BeginArea( m_menuRect );


		m_scrollViewPos = GUILayout.BeginScrollView (m_scrollViewPos, GUILayout.Width (m_menuWidth));


		GUI.backgroundColor = Color.gray;
/*
		GUI.color = Color.yellow;
		GUILayout.Label ("VertExmotion", m_styleHighLight, GUILayout.Width(menuWidth));
		GUI.color = Color.white;
*/
		DrawLogo();

		GUI.backgroundColor = Color.clear;
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace();

		if( DrawInfoIcon ( m_mode==eMode.INFO ) )
			m_mode = eMode.INFO;


		GUILayout.FlexibleSpace();
		if(warning)
			GUI.color = Color.gray;

		if( DrawPaintIcon ( m_mode==eMode.PAINT ) && !warning )
			m_mode = eMode.PAINT;

		GUILayout.FlexibleSpace();

		if( DrawSensorIcon ( m_mode==eMode.SENSORS ) && !warning )
			m_mode = eMode.SENSORS;

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal ();
		GUI.backgroundColor = Color.white;

		GUI.color = Color.white;


		//GUILayout.BeginHorizontal();
		//GUILayout.FlexibleSpace();
		switch( m_mode )
		{
		case eMode.INFO:
			DrawInfo ( vtm );
			break;
		case eMode.PAINT:
			DrawPaint( vtm );
			break;
		case eMode.SENSORS:
			DrawSensors( vtm );
			break;
		}
		//GUILayout.EndHorizontal();

		GUILayout.Space (100);

		GUILayout.EndScrollView ();
		GUILayout.EndArea();
		Handles.EndGUI ();

	}


	//------------------------------------------------------------------------------------------------------
	//Draw Menu mode
	//------------------------------------------------------------------------------------------------------
	public bool warning = false;
	void DrawInfo( VertExmotionBase vtm )
	{
		warning = false;
		GUILayout.Label( "Info", m_styleHighLight, GUILayout.ExpandWidth(true), GUILayout.Height(30) );

		int fixMaterialID = -1;
			for (int i=0; i<vtm.renderer.sharedMaterials.Length; ++i)
				if (!vtm.renderer.sharedMaterials[i].shader.name.Contains ("VertExmotion/"))
			{
				fixMaterialID = i;
				break;
			}


		//---------------------------------------------------------
		//material check
		//---------------------------------------------------------
		if(vtm.renderer.sharedMaterial == null || vtm.renderer.sharedMaterial.name == "Default-Diffuse"  || vtm.renderer.sharedMaterial.name == "Default-Material" || vtm.renderer.sharedMaterial.name == "Sprites-Default")
		{

			GUILayout.Label ("\nWarning!\n", m_styleTitle);
			GUILayout.Label ("Object is using default material,", m_styleTitle);
			GUILayout.Label ("please assign a new material", m_styleTitle);
			GUILayout.Label ("with a VertExmotion shader\n", m_styleTitle);
			warning = true;

		}
			else if( m_mode == eMode.INFO && fixMaterialID != -1 )
		{
			GUILayout.Label ("\nWarning!\n", m_styleTitle);
			GUILayout.Label ("material "+ fixMaterialID +" doesn't use", m_styleTitle);
			GUILayout.Label ("a VertExmotion shader.\n", m_styleTitle);
			warning = true;

			if( GUILayout.Button("Fix material") )
			{
				//Selection.activeObject = vtm.renderer.sharedMaterial;
				//InitMaterialEditor( vtm );
				string shaderName = vtm.renderer.sharedMaterials[fixMaterialID].shader.name;
				if( Shader.Find ( "VertExmotion/" + shaderName ) != null )
				{
					vtm.renderer.sharedMaterials[fixMaterialID].shader = Shader.Find ( "VertExmotion/" + shaderName );	
					vtm.m_initialShaders[fixMaterialID] = vtm.renderer.sharedMaterials[fixMaterialID].shader;
				}
				else
				{
					Debug.LogError( "Material use a non compatible shader ("+shaderName+"), please select a VertexMotion shader in your material properties or add VertExmotion function to your custom shader." );

				}

			}
		}
		else
		{
			GUILayout.Label ("Help :", m_styleTitle);
			GUILayout.Space( 10 );
			GUILayout.Label ("1. Paint vertices", m_styleBold);
			GUILayout.Label ("   white for motion");
			GUILayout.Label ("   black for static");
			GUILayout.Space( 5 );
			GUILayout.Label ("2. Add a sensor", m_styleBold);
			GUILayout.Label ("   set position and range");
			GUILayout.Label ("   vertices in range are highlighted");
			GUILayout.Label ("   set bouncing properties");
			GUILayout.Space( 5 );
			GUILayout.Label ("3. Add more sensors if needed", m_styleBold);
			GUILayout.Label ("   6 sensors max");
			GUILayout.Space( 5 );
			GUILayout.Label ("4. Try it in play mode", m_styleBold);
			GUILayout.Label ("   Move your gameobject");
			GUILayout.Label ("   Have fun!");
		}

		vtm.renderer.sharedMaterial.SetInt( "_SensorId", -1 );
	
	}



	Texture2D m_paintMap = null;
	Mesh m_importMesh = null;

	void DrawPaint( VertExmotionBase vtm )
	{
		GUILayout.Label( "Paint", m_styleHighLight, GUILayout.ExpandWidth(true), GUILayout.Height(30) );

		if( !Application.isPlaying && m_rendererType!= eRendererType.SPRITE )
		{

			DrawBrushMenu ();

			//if( GUILayout.Button ("Erase (ctrl)" ) )
			//	m_eraseMode = !m_eraseMode;

			m_eraseMode = EditorGUILayout.ToggleLeft( "Erase mode (ctrl)", m_eraseMode );
			m_enableBrushMenuContextual = EditorGUILayout.ToggleLeft( "contexual brush settings", m_enableBrushMenuContextual );

			GUILayout.Space(20);
			if( GUILayout.Button("Paint all") )
				PaintAll(true);

			
			if( GUILayout.Button("Unpaint all") )
				PaintAll(false);

				//paint map
				m_paintMap = EditorGUILayout.ObjectField( "Drag & drop paint map", m_paintMap, typeof(Texture2D), false, GUILayout.Height(15), GUILayout.Width(m_menuWidth+10) ) as Texture2D;

				GUILayout.Label( AssetPreview.GetAssetPreview( m_paintMap ) );
			

			
			if( m_paintMap == null )
			{
			GUI.enabled = false;
			}
			else
			{
				GUILayout.Label( "Paint map must be readable" );
				GUILayout.Label( "texture inspector settings :" );
				GUILayout.Label( "  - Texture Type : Advanced" );
				GUILayout.Label( "  - Enable Read/Write" );

				if( vtm.m_mesh.uv2.Length> 0 )
				{
					//choose UV1 or UV2
					GUILayout.BeginHorizontal();
					m_useUV1 = EditorGUILayout.ToggleLeft( "UV1", m_useUV1, GUILayout.Width(50) );
					m_useUV1 = !EditorGUILayout.ToggleLeft( "UV2", !m_useUV1, GUILayout.Width(50) );
					GUILayout.EndHorizontal();
				}
			

				if( GUILayout.Button("Paint from map") )
				{
					//Texture2D txtr = Instantiate( m_paintMap ) as Texture2D;
					Vector2[] uv = vtm.m_mesh.uv;
					if( !m_useUV1 )
						uv = vtm.m_mesh.uv2;

					for( int i=0; i<vtm.m_vertexColors.Length; ++i )
					{
						vtm.m_vertexColors[i]=m_paintMap.GetPixel((int)(uv[i].x * m_paintMap.width), (int)(uv[i].y * m_paintMap.height) );

					}
					vtm.ApplyMotionData();

				}
			}
			GUI.enabled = true;
				
			
			

			//-----------------------------------
			//Export mesh data
			GUILayout.Space( 20 );
			GUILayout.Label( "Export painting data", m_styleTitle );
			GUILayout.Label( "Create a new mesh reference\nincluding painting data" );			

			string assetName = GetAssetName();
			if ( m_exportMeshName == "" )
					m_exportMeshName = assetName.Replace("_VertExmotion","");

			GUILayout.Label( "template name", m_styleBold );
			m_exportMeshName = EditorGUILayout.TextField(  m_exportMeshName );

			
			if( m_exportMeshName == assetName.Replace("_VertExmotion","") && assetName.Contains("_VertExmotion") )
				GUI.enabled = false;

			if( GUILayout.Button("Save as new mesh") )
			{
				//ExportMesh();
				m_exportFlag = true;
					Repaint();
			}


			GUI.enabled = true;


			GUILayout.Label( "Import mesh", m_styleTitle );
			m_importMesh = EditorGUILayout.ObjectField( m_importMesh, typeof( Mesh ), false ) as Mesh;
			
			GUI.enabled = m_importMesh != null;
			if( GUILayout.Button("Import") )
			{
				//apply new mesh
				vtm.SetMesh (m_importMesh);
				vtm.InitMesh();
				vtm.m_params.usePaintDataFromMeshColors = true;//enable mesh data driven
			}
				/*
			GUILayout.Label( "To restore a template :" );
			if( vtm.GetComponent<MeshFilter>() )
				GUILayout.Label( "drag & drop mesh in MeshFilter" );
			else
				GUILayout.Label( "drag & drop mesh in SkinMeshRenderer" );
*/
			

			GUI.enabled = true;
			if( GUILayout.Button( "Find mesh reference" ) )
					EditorGUIUtility.PingObject ( vtm.m_mesh );

//tips


			if( m_enableBrushMenuContextual )
			{
				GUILayout.Space(20);
				GUILayout.Label ("tips :", m_styleTitle );
				GUILayout.Space(10);

				GUILayout.Label ("Alt : contextual brush settings" );
			}

		}
		else
		{
            if(m_rendererType != eRendererType.SPRITE )
			    GUILayout.Label ("Paint disabled in Play mode", m_styleBold );
            else
                GUILayout.Label("Paint disabled for Sprite and TextMesh", m_styleBold);
            }



		vtm.renderer.sharedMaterial.SetInt( "_SensorId", -1 );

	}




	void DrawSensors( VertExmotionBase vtm )
	{
		GUILayout.Label( "Sensors", m_styleHighLight, GUILayout.ExpandWidth(true), GUILayout.Height(30) );
		DrawSensorsList ( vtm );

		vtm.renderer.sharedMaterial.SetInt( "_SensorId", m_sensorId );


		/*
		if( Event.current.isKey && Event.current.keyCode == KeyCode.F && m_sensorId>-1 )
		{
			//SceneView.currentDrawingSceneView.LookAt( vtm.m_VertExmotionSensors[m_sensorId].transform.position );
			SceneView.currentDrawingSceneView.camera.transform.position = vtm.m_VertExmotionSensors[m_sensorId].transform.position + (SceneView.currentDrawingSceneView.camera.transform.position-vtm.m_VertExmotionSensors[m_sensorId].transform.position) * vtm.m_VertExmotionSensors[m_sensorId].m_envelopRadius;
			Event.current.Use();
		}*/

	}


	//------------------------------------------------------------------------------------------------------
	//Draw menu parts
	//------------------------------------------------------------------------------------------------------


	void DrawSensorsList( VertExmotionBase vtm )
	{
		GUILayout.Label( "Sensors list", m_styleTitle );
		//m_paintMode = GUILayout.Toggle (m_paintMode, "paint mode");

		if( vtm.m_VertExmotionSensors.Count== 0 )
			m_sensorId = -1;
		else if( m_sensorId == -1 )
			m_sensorId = 0;
		
		for( int i=0; i<vtm.m_VertExmotionSensors.Count; ++i )
		{
                if (vtm.m_VertExmotionSensors[i] == null)
                    continue;

			if( i==m_sensorId )
				GUI.color = orange;

			GUILayout.BeginHorizontal();
			if( GUILayout.Toggle( i==m_sensorId, "" + (i+1) ) )
			{
				m_sensorId = i;
			}
			vtm.m_VertExmotionSensors[i].name = EditorGUILayout.TextField( vtm.m_VertExmotionSensors[i].name );
			GUILayout.EndHorizontal();

			GUI.color = Color.white;

			if( i==3 )
				GUILayout.Label( "      max sensor limit for mobile" );

			if( i==7 )
				GUILayout.Label( "      max sensor limit for SM2 shaders" );
		}

		
		
		if( vtm.m_VertExmotionSensors.Count<VertExmotionBase.MAX_SENSOR )
		{
			if( GUILayout.Button("New motion sensor") )
			{
                    EditorPrefs.SetInt("VertExmotion_LastMode", (int)m_mode);
                    VertExmotionSensorBase sensor = vtm.CreateSensor();
                    GameObject go = sensor.gameObject;
                    ReplaceBaseClass(sensor);
                    EditorUtility.SetDirty(go);
                    sensor = go.GetComponent<VertExmotionSensorBase>();
                    //EditorUtility.SetDirty(sensor);
                    //EditorUtility.SetDirty( vtm );
                    vtm.m_VertExmotionSensors.Add( sensor );
                    m_sensorId = vtm.m_VertExmotionSensors.Count - 1;

                    //m_lastMode = m_mode;//Editor come back to the current mode
                    

                    //vtm.m_VertExmotionSensors[vtm.m_VertExmotionSensors.Count-1] = sensor as VertExmotionSensorBase;

                }

			GUILayout.Label( "Add existing sensor" );
			m_externalSensor = EditorGUILayout.ObjectField( m_externalSensor, typeof(VertExmotionSensorBase), true ) as VertExmotionSensorBase;
			GUI.enabled = m_externalSensor != null && !vtm.m_VertExmotionSensors.Contains(m_externalSensor);
			if( GUILayout.Button("Add motion sensor") )
				vtm.m_VertExmotionSensors.Add( m_externalSensor );
			GUI.enabled = true;

			
		}
		else
		{
			GUILayout.Label( "      max sensor limit" );
		}


		if (!Application.isPlaying)
		{
			for( int i=0;i<vtm.renderer.sharedMaterials.Length; ++i )
				vtm.renderer.sharedMaterials[i].SetInt ("_SensorId", m_sensorId);
		}

		if(m_sensorId>-1 && m_sensorId<vtm.m_VertExmotionSensors.Count && vtm.m_VertExmotionSensors[m_sensorId] != null )
		{

                if (!Application.isPlaying )
                {

                    if (m_mode == eMode.SENSORS)
                    {
                        /*
                        // < unity 5.4
                        for (int i = 0; i < vtm.renderer.sharedMaterials.Length; ++i)
                        {

                            vtm.renderer.sharedMaterials[i].SetVector("_SensorPosition" + m_sensorId, vtm.m_VertExmotionSensors[m_sensorId].transform.position);

                            Vector4 radiusCentripetalTorque = Vector4.zero;
                            radiusCentripetalTorque.x = vtm.m_VertExmotionSensors[m_sensorId].m_envelopRadius;
                            radiusCentripetalTorque.y = vtm.m_VertExmotionSensors[m_sensorId].m_centripetalForce;
                            radiusCentripetalTorque.z = vtm.m_VertExmotionSensors[m_sensorId].m_motionTorqueForce;

                            vtm.renderer.sharedMaterials[i].SetVector("_RadiusCentripetalTorque" + m_sensorId, radiusCentripetalTorque);

                        }*/

                        //unity 5.4


                        m_sensorpos[m_sensorId] = vtm.m_VertExmotionSensors[m_sensorId].transform.position;

                        Vector4 radiusCentripetalTorque = Vector4.zero;
                        radiusCentripetalTorque.x = vtm.m_VertExmotionSensors[m_sensorId].m_envelopRadius;
                        radiusCentripetalTorque.y = vtm.m_VertExmotionSensors[m_sensorId].m_centripetalForce;
                        radiusCentripetalTorque.z = vtm.m_VertExmotionSensors[m_sensorId].m_motionTorqueForce;
                        m_RadiusCentripetalTorque[m_sensorId] = radiusCentripetalTorque;

                        //vtm.renderer.GetPropertyBlock(m_matPropBlk);
                        m_matPropBlk.SetVectorArray("_SensorPositionEditor", m_sensorpos);
                        m_matPropBlk.SetVectorArray("_RadiusCentripetalTorqueEditor", m_RadiusCentripetalTorque);

                        vtm.renderer.SetPropertyBlock(m_matPropBlk);
                    }                    
                }

			VertExmotionSensorEditor.DrawSensorSettings( vtm.m_VertExmotionSensors[m_sensorId] );

			GUILayout.Space( 20 );

			if( GUILayout.Button( "Show in Inspector" ) )
			{
				//m_paintMode = false;
				m_mode = eMode.INFO;
				Selection.activeGameObject = vtm.m_VertExmotionSensors[m_sensorId].gameObject;
			}
		}


		if( m_sensorId != -1 && m_sensorToRemove == -1 && m_sensorToDelete == -1 && GUILayout.Button("Delete sensor") )
		{
			m_sensorToDelete = m_sensorId;
		}

		if( m_sensorId != -1 && m_sensorToRemove == -1 && m_sensorToDelete == -1 && GUILayout.Button("Remove sensor") )
		{
			m_sensorToRemove = m_sensorId;
		}



		if( m_sensorToDelete != -1 )
		{
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Delete sensor ?\ndestroy gameobject", m_styleTitle);
			if( GUILayout.Button("Yes") )
			{
				if( Application.isPlaying )
					Destroy( vtm.m_VertExmotionSensors[m_sensorToDelete].gameObject );
				else
					DestroyImmediate( vtm.m_VertExmotionSensors[m_sensorToDelete].gameObject );

				vtm.m_VertExmotionSensors.RemoveAt( m_sensorToDelete );

				m_sensorId = -1;
				m_sensorToDelete = -1;
			}

			if( GUILayout.Button("No") )
				m_sensorToDelete = -1;

			GUILayout.EndHorizontal();
		}



		if( m_sensorToRemove != -1 )
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Remove sensor ?", m_styleTitle);
			if( GUILayout.Button("Yes") )
			{
				vtm.m_VertExmotionSensors.RemoveAt( m_sensorToRemove );
				m_sensorId = -1;
				m_sensorToRemove = -1;
			}
			
			if( GUILayout.Button("No") )
				m_sensorToRemove = -1;
			
			GUILayout.EndHorizontal();
		}




	}


	void DrawBrushMenu()
	{
		GUILayout.Label ("Brush settings", m_styleTitle, GUILayout.ExpandWidth(true));	
		GUILayout.Space (10);

		GUILayout.Label ("Size : "+m_drawRadius*100f/25f, m_styleBold, GUILayout.ExpandWidth(true));	
		m_drawRadius = GUILayout.HorizontalSlider ( m_drawRadius, .01f, .25f );
		GUILayout.Label ("Intensity : "+ Mathf.Round( m_drawIntensity *100f )/100f, m_styleBold, GUILayout.ExpandWidth(true));	
		m_drawIntensity = GUILayout.HorizontalSlider ( m_drawIntensity*100f/100, 0.01f, 1f );
		GUILayout.Label ("Falloff : "+  Mathf.Round( m_drawFalloff *100f ) / 100f, m_styleBold, GUILayout.ExpandWidth(true));	
		m_drawFalloff = GUILayout.HorizontalSlider ( m_drawFalloff, 0f, 1f );

	}


	void DrawBrushMenuContextual( Event e )
	{
		//float menuWidth = 150f;
		if( m_enableBrushMenuContextual )
		{
			Handles.BeginGUI ();

			GUI.Box (m_brushMenuRect, "", m_bgStyle );
			GUILayout.BeginArea( m_brushMenuRect );

			DrawBrushMenu ();

			GUILayout.EndArea ();
			Handles.EndGUI ();
		}
	}






	void DrawCursor( Vector3 mp, Camera svCam )
	{
		Color c = m_eraseMode ? Color.red : Color.gray;
		c.a = .05f + .1f * m_drawIntensity;
		Handles.color = c;
		Handles.DrawSolidDisc( mp, -svCam.transform.forward, m_brushSize );	

		c = orange;
		c.a = 1f;
		Handles.color = c;
		Handles.DrawWireDisc( mp, -svCam.transform.forward, m_brushSize );	

		c.a = .5f;
		Handles.color = c;
		Handles.DrawWireDisc( mp, -svCam.transform.forward, m_brushSize * (1f-m_drawFalloff) );	
	}





	//------------------------------------------------------------------------------------------------------
	//On enable / disable / destroy
	//------------------------------------------------------------------------------------------------------

	void OnEnable()
	{

		
		

		m_editorInitialized = false;
            m_editorInstanceInitialized = false;

            InitializeEditorInstance();

		//Debug.Log ("OnEnable");
		VertExmotionBase vtm = (target) as VertExmotionBase;


		


		vtm.m_params.version = VertExmotionBase.version;

		if( vtm.GetComponent<MeshFilter>() == null 
                && vtm.GetComponent<SkinnedMeshRenderer>() == null 
                && vtm.GetComponent<SpriteRenderer>() == null
                && vtm.GetComponent<TextMesh>() == null
                )
        //if(m_rendererType==eRendererType.NONE)
		{
			Debug.LogError( "VertExmotion need a MeshFilter, a SkinnedMeshRenderer or a SpriteRenderer component" );
			if( !Application.isPlaying ) 
				DestroyImmediate( vtm );
			else
				Destroy ( vtm );

			return;
		}


		//m_paintMode = false;
		//m_mode = eMode.INFO;
		m_lastTool = Tools.current;
		m_showPanelProgress = 0;

		if( !Application.isPlaying && vtm.m_meshCopy )
			DestroyImmediate ( vtm.m_mesh );
		vtm.m_mesh = null;

		//FIX when script compile while edition
		if( vtm.m_initialShaders != null && vtm.renderer.sharedMaterials.Length == vtm.m_initialShaders.Length )
		{
			for( int i=0; i<vtm.renderer.sharedMaterials.Length; ++i )
			{
				if( vtm.renderer.sharedMaterials[i].shader.name.StartsWith("Hidden" ) )
				{
					vtm.renderer.sharedMaterials[i].shader = vtm.m_initialShaders[i];	
						EditorUtility.SetDirty( vtm.renderer.sharedMaterials[i] );
				}
			}
		}

		//update shader list
		vtm.m_initialShaders = new Shader[vtm.renderer.sharedMaterials.Length];

		for( int i=0; i<vtm.renderer.sharedMaterials.Length; ++i )
		{
			vtm.m_initialShaders[i] = vtm.renderer.sharedMaterials[i].shader;			
		}

		m_initialized = false;
/*		string shaderName = vtm.renderer.sharedMaterial.shader.name;

		if( ( vtm.m_initialShaders == null || vtm.m_initialShaders != vtm.renderer.sharedMaterial.shader ) && !shaderName.Contains("Hidden/") )
		{
			vtm.m_initialShaders = vtm.renderer.sharedMaterial.shader;
			EditorUtility.SetDirty( vtm.renderer.sharedMaterial );
		}
*/



/*
		if (!vtm.renderer.sharedMaterial.shader.name.Contains ("VertExmotion/"))
			Debug.LogError( "Material use a non compatible shader ("+shaderName+"), please select a VertexMotion shader in your material properties or add VertExmotion function to your custom shader." );
*/


			//check if Distrubution packege is deployed
			//m_distributionClassFound = VertExmotionBase.ClassExists ( "VertExmotionPacked" );

	}


	void OnDestroy()
	{
		//Debug.Log ("OnDestroy");
		OnDisable ();
	}

	void OnDisable()
	{
       m_exportMeshName = "";

       if (Application.isPlaying)
	        return;

		if (target == null)
			return;
		
		VertExmotionBase vtm = (target) as VertExmotionBase;
		if( vtm == null )
			return;

        /*
		for( int i=0; i<vtm.m_VertExmotionSensors.Count; ++i )
		{
			VertExmotionColliderBase vtmCol = vtm.m_VertExmotionSensors[i].GetComponentInChildren<VertExmotionColliderBase>();
                if (vtmCol != null)
                {
                    VertExmotionEditor.ReplaceBaseClass(vtmCol);
                    //EditorUtility.SetDirty(vtmCol);
                }

			GameObject go = vtm.m_VertExmotionSensors[i].gameObject;
			ReplaceBaseClass(vtm.m_VertExmotionSensors[i]);
			vtm.m_VertExmotionSensors[i] = go.GetComponent<VertExmotionSensorBase>();
		}
        */

		for( int i=0; i<vtm.renderer.sharedMaterials.Length; ++i )
		{
			if( vtm.renderer.sharedMaterials[i].shader.name == "Hidden/VertExmotion_editor" )
				vtm.renderer.sharedMaterials[i].shader = vtm.m_initialShaders[i];
			EditorUtility.SetDirty( vtm.renderer.sharedMaterials[i] );
                vtm.DisableMotion();

        }
	
		Tools.current = m_lastTool;

		if( SceneView.lastActiveSceneView != null )
			SceneView.lastActiveSceneView.orthographic = m_lastOrthoMode;
		//m_mode = eMode.INFO;

		m_lastVTMSelected = vtm;

		//ReplaceBaseClass(vtm);
        //EditorUtility.SetDirty(vtm);
        }


	public static GUIStyle m_winBg = null;
	public static GUIStyle m_bgStyle = new GUIStyle();
	public static void DrawBackground()
	{

		if( m_winBg == null )
		{
			foreach(GUIStyle style in GUI.skin.customStyles)
				if( style.name == "WindowBackground" )
					m_winBg = style;
		}

		GUI.color = Color.white;
		GUI.backgroundColor = Color.white;
		GUI.Label (m_menuRect,"", m_winBg);
		
	}











	//------------------------------------------------------------------------------------------------------
	//Logo & icons
	//------------------------------------------------------------------------------------------------------

	static Texture2D m_logo;
	public static void DrawLogo()
	{
		if( m_logo==null )
		{
			m_logo = (Texture2D)Resources.Load("Icons/VertExmotion_banner_200",typeof(Texture2D));
		}

		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.Label(m_logo, GUILayout.MaxWidth(200f) );		
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
	}




	static Texture2D[] m_infoIcons = new Texture2D[2];
	public static bool DrawInfoIcon( bool enable )
	{
		for(int i=0; i<2; ++i)
			if( m_infoIcons[i]==null )		
				m_infoIcons[i] = (Texture2D)Resources.Load( "Icons/info_icon_"+(i==0?"off":"on"),typeof(Texture2D));
		if (GUILayout.Button (enable ? m_infoIcons [1] : m_infoIcons [0], GUILayout.Width (50), GUILayout.Height (50)))
			return !enable;
		
		return enable;
	}


	static Texture2D[] m_paintIcons = new Texture2D[2];
	public static bool DrawPaintIcon( bool enable )
	{
		for(int i=0; i<2; ++i)
			if( m_paintIcons[i]==null )		
				m_paintIcons[i] = (Texture2D)Resources.Load( "Icons/paint_icon_"+(i==0?"off":"on"),typeof(Texture2D));

		if (GUILayout.Button (enable ? m_paintIcons [1] : m_paintIcons [0], GUILayout.Width (50), GUILayout.Height (50)))
			return !enable;

		return enable;
	}

	static Texture2D[] m_sensorIcons = new Texture2D[2];
	public static bool DrawSensorIcon( bool enable )
	{
		for(int i=0; i<2; ++i)
			if( m_sensorIcons[i]==null )		
				m_sensorIcons[i] = (Texture2D)Resources.Load( "Icons/sensor_icon_"+(i==0?"off":"on"),typeof(Texture2D));
		
		if (GUILayout.Button (enable ? m_sensorIcons [1] : m_sensorIcons [0], GUILayout.Width (50), GUILayout.Height (50)))
			return !enable;
		
		return enable;
	}



	//------------------------------------------------------------------------------------------------------
	//check vertices on same position
	//------------------------------------------------------------------------------------------------------
	//Vector3[] m_verticesPos;
	Dictionary<Vector3,List<int>> m_posToVertices = new Dictionary<Vector3,List<int>>();
	void InitVerticesPosDictionary()
	{
            if ((target as VertExmotionBase).m_mesh == null)
                return;

		foreach( KeyValuePair<Vector3,List<int>> kvp in m_posToVertices )
			kvp.Value.Clear();
		m_posToVertices.Clear ();
		m_vtx = (target as VertExmotionBase).m_mesh.vertices; 

		for( int i=0; i<m_vtx.Length; ++i )
		{
			if( !m_posToVertices.ContainsKey( m_vtx[i] ) )
				m_posToVertices.Add( m_vtx[i], new List<int>() );

			m_posToVertices[m_vtx[i]].Add( i );
		}
	}



		void PaintAll( bool paintWhite )
		{
			VertExmotionBase vtm = (target as VertExmotionBase); 
			for( int i=0; i<vtm.m_vertexColors.Length; ++i )
			{
				vtm.m_vertexColors[i] = paintWhite ? Color.white : Color.black;
			}
			//refresh colors
			vtm.m_mesh.colors32 = vtm.m_vertexColors;
			vtm.ApplyMotionData();
			EditorUtility.SetDirty( vtm );
		}


		/// <summary>
		/// Gets the name of the asset.
		/// </summary>
		/// <returns>The asset name.</returns>
		string GetAssetName()
		{
			VertExmotionBase vtm = (target as VertExmotionBase);
			string path = AssetDatabase.GetAssetPath ( vtm.m_mesh );

			SkinnedMeshRenderer smr = vtm.GetComponent<SkinnedMeshRenderer>();
			if (smr)			
				path = AssetDatabase.GetAssetPath ( smr.sharedMesh );


			//Debug.Log ("path " + path);
			string[] pathParts = path.Split('/');
			return pathParts [pathParts.Length - 1].Split('.')[0];
		}

		//-------------------------------------------------------------------
		void ExportMesh()
		{
			VertExmotionBase vtm = (target as VertExmotionBase);
			//Mesh initMesh = vtm.GetMesh ();
			Mesh m = (Mesh)Instantiate( vtm.m_mesh );
			m.name = vtm.m_mesh.name;
			string path = AssetDatabase.GetAssetPath ( vtm.m_mesh );

			//SkinnedMesh Setttings
			SkinnedMeshRenderer smr = vtm.GetComponent<SkinnedMeshRenderer>();
			if (smr)
			{
				path = AssetDatabase.GetAssetPath ( smr.sharedMesh );
				m = (Mesh)Instantiate( smr.sharedMesh );
				m.colors32 = vtm.m_mesh.colors32;
				m.name = smr.sharedMesh.name;
			}

			string[] pathParts = path.Split('/');
			path = "";
			for(int i=0; i<pathParts.Length-1; ++i)
				path += pathParts[i]+"/";

			if( path == "" )//built-in models
				path = "Assets/";

			string prefabName = path + m_exportMeshName + "_VertExmotion.prefab";



			Object prefab = AssetDatabase.LoadAssetAtPath (prefabName, typeof(Object) );

			if( prefab == null )
			{
				prefab = PrefabUtility.CreateEmptyPrefab ( prefabName  );
				AssetDatabase.AddObjectToAsset( m, prefabName );
				AssetDatabase.SaveAssets();

				//apply new mesh
				vtm.SetMesh (m);
				vtm.InitMesh();
				EditorGUIUtility.PingObject ( prefab );
				Debug.Log ( "Export mesh : " + prefabName );
			}
			else
			{
				Debug.LogError ( "Export mesh failed : " + prefabName + " exists\nPlease change template name" );
			}




			vtm.m_params.usePaintDataFromMeshColors = true;//enable mesh data driven


			//GUIUtility.ExitGUI ();

			//AssetDatabase.CreateAsset ( m, path + m.name);
			//AssetDatabase.Refresh ();
		}



		public static void ReplaceBaseClass( MonoBehaviour obj )
		{
			if( obj == null )
				return;
            /*
			if (Application.platform == RuntimePlatform.OSXEditor)
				return;//Unity is no more stable when changing script
            */

			var so = new SerializedObject( obj );
			var sp = so.FindProperty("m_Script");
			MonoScript script = GetScriptFromBase( obj.GetType().Name );
			if( script != null )  
			{
				sp.objectReferenceValue = script;
				so.ApplyModifiedProperties();                
			}            
		}




		static MonoScript GetScriptFromBase( string type )
		{
            if (!type.Contains("Base"))
                return null;

			var types = Resources
				.FindObjectsOfTypeAll (typeof(MonoScript))
					.Where (x => x.name == type.Replace("Base","") )
					//.Where (x => x.GetType () == typeof(MonoScript))
					.Cast<MonoScript> ()
					.Where (x => x.GetClass () != null)
					//.Where( x => x.GetClass().Assembly.FullName.Split(',')[0] == "" )
					.ToList ();


			if( types.Count == 1 )
			{
				//Debug.Log ("class found " + type );
				return types[0];

			}


			//Debug.Log ("class not found " + type );
			return null;
		}




}
}
