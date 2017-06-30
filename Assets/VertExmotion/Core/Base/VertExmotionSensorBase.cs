using UnityEngine;
using System.Collections;
using Kalagaan;

namespace Kalagaan
{
	public class VertExmotionSensorBase : MonoBehaviour {


		[HideInInspector]
		public string className = "VertExmotionSensor";

		///Parent of the transform \n
		///for skinedMeshRenderer set the bone 
		[HideInInspector] public Transform m_parent;

		public VertExmotionSensorBase m_parentSensor;

		///radius of the sensor \n
		///shader parameter
		/*[HideInInspector]*/ public float m_envelopRadius = 1f;

		///center of the sensor \n
		///shader parameter
		[HideInInspector] public Vector3 m_center;

		///Motion direction \n
		///shader parameter
		[HideInInspector] public Vector3 m_motionDirection;

		///Torque force \n
		///shader parameter \n
		/// WIP
		[HideInInspector] public float m_motionTorqueForce = 0;

		///Torque axis \n
		///shader parameter
		[HideInInspector] public Vector3 m_torqueAxis;

		///Torque axis \n
		///shader parameter
		[HideInInspector] public float m_centripetalForce = 0f;

		///Collision vector \n
		/// Set by VertExmotionCollider \n
		///shader parameter
		[HideInInspector] public Vector3 m_collision = Vector3.zero;

		//[HideInInspector] public int m_motionZoneId = 1;
		//[HideInInspector] public Color m_motionZoneColor = Color.green;

		///Parameters list \n
		/// all parameters needed for sensor behaviour
		/*[HideInInspector]*/ public Parameter m_params = new Parameter();	
		///PID time \n
		///editor only
		[HideInInspector] public float m_pidTime = 2f;
		///PID
		///regulation system
		/*[HideInInspector]*/ public PID_V3 m_pid = new PID_V3();

        ///current sensor direction
        /// used for smooth
        public Vector3 m_sensorDirection;

		///last tranform position
		public Vector3 m_lastPosition;

		public Vector3 m_lastMotionDirection = Vector3.zero;
		public PID m_torqueForcePID = new PID();
		public PID_V3 m_torqueAxisPID = new PID_V3();


		private Vector3 m_lastForward;
		private Vector3 m_lastRight;
		//private Quaternion m_lastRotation;
		//private float m_angleCumul = 0f;
		public Vector3 m_speed = Vector3.zero;
		public Vector3 m_lastSpeed = Vector3.zero;

		public Vector3 m_speedStrech = Vector3.zero;
		public Vector3 m_accStretch = Vector3.zero;
		public float m_stretch = 0;

		public float timeScale
		{
			get
			{
				return m_pid.timeScale;
			}
			set{
				m_pid.timeScale = value;
			}
		}

		[System.Serializable]
		public class Parameter
		{
			[System.Serializable]
			public class Translation
			{
				public float amplitudeMultiplier = 1f;
				public float outerMaxDistance = 1f;
				public float innerMaxDistance = 1f;
				public Vector3 worldOffset = Vector3.zero;
				public Vector3 localOffset = Vector3.zero;
				public Vector2 gravityInOut = Vector2.zero;

			}
			
			[System.Serializable]
			public class Torque
			{
				//torque data
				public float amplitude = 0f;
				public float smooth = 0f;
				public float max = 0f;
				public float damping = 0f;
				public float bouncing = 0f;
			}


			[System.Serializable]
			public class FX
			{
				public float stretch = 0f;
				public float stretchMax = 1f;
				public float stretchMinSpeed = 0f;
				public float squash = 0f;
			}

			//translation data		
			public Translation translation = new Translation();
			//[HideInInspector]
			public Torque torque = new Torque();

			public FX fx = new FX();

			public float inflate = 0f;
			public float damping = 1f;
			public float bouncing = 1f;
			public float globalSmooth = 0f;

		}





		void Awake()
		{
			if( m_parent!= null )
				transform.parent = m_parent;
		}



		void Start () {
			m_lastPosition = transform.position;
			m_sensorDirection = Vector3.zero;
			m_motionDirection = Vector3.zero;
			//m_lastRotation = transform.rotation;
			//m_lastForward = transform.forward;
			//m_lastRight = transform.right;

		}
		


		void Update () {


			float sf = VertExmotionBase.GetScaleFactor (transform);

			m_center = transform.position;
			m_pid.m_params.kp = m_params.damping;
			m_pid.m_params.ki = m_params.bouncing;




/*
			if (Time.deltaTime > 0)
				m_pid.m_target = (m_lastPosition - transform.position) * m_params.translation.amplitudeMultiplier / Time.deltaTime * .1f;
*/
			if ( Time.deltaTime * m_pid.m_pidX.m_timeScale > 0)
				m_speed = (m_lastPosition - transform.position) / ( Time.deltaTime * m_pid.m_pidX.m_timeScale );


			//Debug.Log ("speed " + m_speed.y);


			/*
			if ( Time.deltaTime * m_pid.m_pidX.m_timeScale > 0)
				m_accSquash = Vector3.Lerp ( m_accSquash, ( m_lastSpeed - m_speed )/ ( Time.deltaTime * m_pid.m_pidX.m_timeScale ), Time.deltaTime * 10f  );;
*/

			//Debug.Log ("squash min " + m_speed.magnitude );
			if( m_speed.magnitude > m_params.fx.stretchMinSpeed )
				m_speedStrech = Vector3.Lerp ( m_speedStrech, m_speed.normalized * (m_speed.magnitude-m_params.fx.stretchMinSpeed), Time.deltaTime * 2f * timeScale  );
			else
				m_speedStrech = Vector3.Lerp ( m_speedStrech, Vector3.zero, Time.deltaTime * 2f * timeScale );		

			m_speedStrech = Vector3.ClampMagnitude (m_speedStrech, m_params.fx.stretchMax);

			if( Vector3.Dot(m_speed.normalized,m_lastSpeed.normalized) > 0 )
				m_stretch = Mathf.Lerp( m_stretch, m_params.fx.stretch, Time.deltaTime * 10f * timeScale );
			else
				m_stretch = Mathf.Lerp( m_stretch, 0, Time.deltaTime * 1f * timeScale );


			//m_speedSquash = m_accSquash;
			//m_speedSquash = -transform.parent.rigidbody.velocity;

			m_pid.m_target = m_speed  * m_params.translation.amplitudeMultiplier * .1f;

			//m_pid.m_target = Vector3.up * Mathf.Sin (Time.time) * .5f;

			//Debug.Log ( "target : " + m_pid.m_target );


			//compute translation
			float lerpFactor = ( Vector3.Dot (transform.forward, m_sensorDirection.normalized) + 1f ) * .5f;

			//sensor limits
			float clampMag = ( Mathf.Lerp (m_params.translation.innerMaxDistance, m_params.translation.outerMaxDistance, lerpFactor) ) * sf;
			m_pid.m_params.limits.x = -clampMag;
			m_pid.m_params.limits.y = clampMag;

			//compute sensor position
			m_sensorDirection = m_pid.Compute (m_sensorDirection);



			//----------------------------------------------------------------------------
			//compute torque Force (need more work...)
			m_torqueAxis = Vector3.zero;
			m_motionTorqueForce = 0;


			//m_motionTorqueForce = Mathf.Lerp (m_motionTorqueForce, ( 1f - Quaternion.Dot (transform.rotation, m_lastRotation) ) *m_torqueAmplitude, Time.time * m_torqueSpeed);
			float angleSpeed = 0f;// = (1f - Quaternion.Dot (transform.rotation, m_lastRotation)) * m_params.torque.amplitude;// + m_motionTorqueForce;
			//angle = ( Quaternion.Angle (transform.rotation, m_lastRotation)  );

			if( Time.deltaTime > 0 )
				angleSpeed = Vector3.Angle (m_lastRight , transform.right )/ Time.deltaTime;


			//m_angleCumul += angle;

			//Debug.Log ( angle );

			m_torqueForcePID.m_params.kp = m_params.torque.damping;
			m_torqueForcePID.m_params.ki = m_params.torque.bouncing;


			//newTorque = Mathf.Lerp (newTorque, m_motionTorqueForce, Time.time * m_params.torque.smooth);
			//m_torqueForcePID.m_target = Mathf.Lerp ( m_torqueForcePID.m_target, angle * Time.deltaTime * m_params.torque.amplitude, Time.deltaTime * m_params.torque.smooth );
			//m_torqueForcePID.m_target = Mathf.Clamp (m_torqueForcePID.m_target, -m_params.torque.max, m_params.torque.max);

			m_torqueForcePID.m_target = angleSpeed * m_params.torque.amplitude;
			m_torqueForcePID.m_target = Mathf.Clamp (m_torqueForcePID.m_target, -m_params.torque.max, m_params.torque.max);

			m_motionTorqueForce = m_torqueForcePID.Compute ( m_motionTorqueForce );
			//m_motionTorqueForce += newTorque;

			m_motionTorqueForce = Mathf.Clamp (m_motionTorqueForce, -m_params.torque.max, m_params.torque.max);

			Vector3 newAxis = Vector3.Cross ( m_lastForward, transform.forward ) + Vector3.Cross ( m_lastRight, transform.right );
			newAxis.Normalize ();

			//move forward on torque
			//m_sensorDirection += transform.forward * Mathf.Abs( Vector3.Dot (newAxis, transform.forward) ) * m_motionTorqueForce * .1f;

			//m_torqueAxis = Vector3.Lerp (m_torqueAxis, newAxis, Time.deltaTime * 10f );
			

			m_torqueAxisPID.m_params.kp = m_params.torque.damping;
			m_torqueAxisPID.m_params.ki = m_params.torque.bouncing;

			m_torqueAxisPID.m_target = newAxis;

			m_torqueAxis = m_torqueAxisPID.Compute ( m_torqueAxis );

			//m_torqueAxis = newAxis;


			m_motionTorqueForce *= Vector3.Dot ( m_torqueAxis, transform.forward );//limit torque on Z axis


			//init for next frame

			//m_lastRotation = transform.rotation;
			m_lastForward = transform.forward;
			m_lastRight = transform.right;

			//----------------------------------------------------------------------------

	
			m_lastPosition = transform.position;
			m_lastSpeed = m_speed;

			if( m_collision.magnitude < .0001f )
			{
				m_motionDirection = Vector3.zero;
			}
			else
			{
				m_sensorDirection = Vector3.Lerp( m_sensorDirection, m_collision, Time.deltaTime );
				m_motionDirection = m_collision;
			}

			m_motionDirection += m_sensorDirection + ( m_params.translation.worldOffset + transform.TransformDirection( m_params.translation.localOffset ) ) * sf;

			//gravity
			Vector3 gravityTorqueAxis = Vector3.zero;
			float gravityTorqueForce = 0;
			Vector3 gravityDirection = Vector3.zero;
			if( m_params.translation.gravityInOut.magnitude != 0 )
			{
				Vector3 g = Physics.gravity.normalized;
				float dot = Vector3.Dot(g,transform.forward);
				gravityDirection = g * Mathf.Lerp( m_params.translation.gravityInOut.x, m_params.translation.gravityInOut.y, (dot+1f)*.5f ) * Physics.gravity.magnitude;// * ( Vector3.Dot(g,transform.forward )>0 ? Vector3.Dot(g,transform.forward ) : 0 );
				gravityTorqueAxis = Vector3.Cross( g, transform.forward );
				//m_motionTorqueForce = 1f;// m_params.translation.gravityFactor * 2f;
				gravityTorqueForce = ( 1f - dot*dot ) * Mathf.Lerp(dot,1f,dot*dot);//(dot<0?dot:1f);
				//m_motionTorqueForce = ( 1f - dot*dot );//(dot<0?dot:1f);
			}/*
			else
			{
				//m_torqueAxis = 
				m_motionTorqueForce = 0f;
			}*/


			m_motionDirection += gravityDirection*sf;
			//m_motionDirection = Vector3.ClampMagnitude (m_motionDirection, clampMag);

			//Debug.DrawLine (transform.position, transform.position + m_motionDirection.normalized * 10f, Color.red);

			m_centripetalForce = m_params.inflate;//todo add torque force

			//Add gravity
			m_torqueAxis += gravityTorqueAxis;
			m_motionTorqueForce += gravityTorqueForce;

			/*
			if( m_torqueAxis.magnitude == 0 )
				m_torqueAxis = transform.forward;
			else
				m_torqueAxis.Normalize();*/


			//apply global smooth
			m_motionDirection = Vector3.Lerp( m_motionDirection, m_lastMotionDirection, Time.deltaTime * m_params.globalSmooth );

			if( m_parentSensor != null )
				m_motionDirection += m_parentSensor.m_motionDirection;

			m_lastMotionDirection = m_motionDirection;



		}


		///Convert a position by applying sensor deformation.
		///@param pos position to convert
		///@param weight 0->1
		public Vector3 TransformPosition( Vector3 pos, float weight )
		{
			float dist = Vector3.Distance(pos,m_center);
			Vector3 torqueDir;
			Vector3 centripetalDir;
			Vector3 motionDir = Vector3.zero;
			float scaleFactor = VertExmotionBase.GetScaleFactor ( transform );

			if( dist < m_envelopRadius* scaleFactor )
			{
				torqueDir = Vector3.Cross( ( pos-m_center).normalized, m_torqueAxis ) ;
				torqueDir *= m_motionTorqueForce * dist;	
				
				centripetalDir = (pos-m_center).normalized * m_centripetalForce * .89f;

				motionDir =  (m_motionDirection + torqueDir + centripetalDir) * (1.0f - dist/(m_envelopRadius*scaleFactor+.0000001f));
				
			}	

			return pos + motionDir * weight;
		}



        ///Ignore current frame motion.
        public void IgnoreFrame()
        {
            m_pid.IgnoreFrame();
        }


        ///Reset motion.
        public void ResetMotion()
        {
            m_pid.Init();
            m_sensorDirection = Vector3.zero;
            m_motionDirection = Vector3.zero;
            m_lastMotionDirection = m_motionDirection;
        }


        /// <summary>
        /// Ignore collider from colliding with sensor's collision zones
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="ignore"></param>
        public void IgnoreCollision(Collider collider, bool ignore)
        {
            VertExmotionColliderBase[] colliders = GetComponentsInChildren<VertExmotionColliderBase>();

            for (int i = 0; i < colliders.Length; ++i)
                colliders[i].IgnoreCollision(collider, ignore);
        }

    }
}
