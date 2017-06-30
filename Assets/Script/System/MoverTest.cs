﻿using UnityEngine;
using System.Collections;
using Kalagaan;
public class MoverTest : MonoBehaviour {

    public VertExmotionSensor m_bodySensor;

    public float m_jellyfishMoveAmplitude = .1f;
    public float m_bodyPeriod = 1f;
    public float m_bodyMoveAmplitude= .1f;
    public float m_bodyInflateAmplitude = 1f;
    public float m_tentacleMoveAmplitude = 5f;
    public float m_tentacleInflateAmplitude = 1f;
    public float m_inflatePeriode = 2f;
    public Vector3 m_offset;
    public AnimationCurve m_headCurve = new AnimationCurve();
    //public AnimationCurve m_tentacleMoveCurve = new AnimationCurve();
    //public AnimationCurve m_tentacleInflateCurve = new AnimationCurve();

    public float m_speed = 0f;

    public float m_timeOffset =0f;

    //Vector3 m_lastbodyPosition;
    Vector3 m_initialProceduralsensorPos;

    Vector3 target = Vector3.up;
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


        m_bodySensor.transform.localPosition = transform.up * m_headCurve.Evaluate( progress ) * m_bodyMoveAmplitude  + m_offset;
        m_bodySensor.m_params.inflate = m_headCurve.Evaluate( progress ) * m_bodyInflateAmplitude;


        //locomotion
        if( progress > .8f )
            m_speed =  Mathf.Lerp( m_speed, m_jellyfishMoveAmplitude, Time.deltaTime * 1f);
        else
            m_speed =  Mathf.Lerp( m_speed, 0.001f, Time.deltaTime * 1f );

        transform.parent.position += transform.parent.up * m_speed;

      
        //rotation
        if( Time.time - m_lastTargetTime > Random.Range(3f,10f) )
        {
            target = Vector3.up * 5f + Random.insideUnitSphere;
            m_lastTargetTime = Time.time;
        }
        transform.parent.rotation = Quaternion.Lerp( transform.parent.rotation, Quaternion.FromToRotation(transform.parent.up, target.normalized ) * transform.parent.rotation, Time.deltaTime );



    }

    void OnDrawGizmos()
    {

        if( m_bodySensor != null )
            Gizmos.DrawSphere (m_bodySensor.transform.position, .1f);

    }
}
