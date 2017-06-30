using UnityEngine;
using System.Collections;
using Kalagaan;

//[AddComponentMenu("VertExmotion/Demo/Jellyfish")]
public class Jellyfish : MonoBehaviour {

	public VertExmotionSensor m_bodySensor;

	public float m_bodyPeriod = 1f;
	public float m_bodyMoveAmplitude= .1f;
	public float m_bodyInflateAmplitude = 1f;
	public Vector3 m_offset;
	public AnimationCurve m_headCurve = new AnimationCurve();

	public float m_timeOffset =0f;


	float m_lastTargetTime = 0;

	void Start()
	{
		//m_timeOffset = Random.value * 10f;
		//transform.parent.localScale = Random.Range (.95f, 1.1f) *Vector3.one;
		//m_lastbodyPosition = m_bodySensor.transform.position;

		m_bodyPeriod *= Random.Range (.9f, 1.1f);
	}



	void Update () {
	
		//body motion
		m_bodyPeriod = Mathf.Clamp (m_bodyPeriod, .1f, m_bodyPeriod);
		float progress = ( (Time.time+m_timeOffset) % m_bodyPeriod )/m_bodyPeriod;

        m_bodySensor.transform.localPosition = direction * m_headCurve.Evaluate( progress ) * m_bodyMoveAmplitude  + m_offset;

		m_bodySensor.m_params.inflate = m_headCurve.Evaluate( progress ) * m_bodyInflateAmplitude;
       

	}
    public Vector3 direction;
	void OnDrawGizmos()
	{

		if( m_bodySensor != null )
			Gizmos.DrawSphere (m_bodySensor.transform.position, .1f);
        Gizmos.DrawLine(transform.position, m_bodySensor.transform.InverseTransformDirection(direction)*10.0f);

	}
	
}
