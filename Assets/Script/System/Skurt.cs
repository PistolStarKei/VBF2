using UnityEngine;
using System.Collections;
using Kalagaan;

public enum SkurtState{NotentionStart,Notention,TentionStart,Tention,None};
public class Skurt : MonoBehaviour {


    public void OnTention(float pow){

    }

    public void OnNoTention(){
        
    }


    public VertExmotionSensor m_bodySensor;

    public float m_bodyPeriod = 1f;
    public float m_bodyMoveAmplitude= .1f;
    public float m_bodyInflateAmplitude = 1f;
    public Vector3 m_offset;
    public AnimationCurve m_headCurve = new AnimationCurve();

    public AnimationCurve m_headCurve2 = new AnimationCurve();

    public float m_timeOffset =0f;


    float m_lastTargetTime = 0;

    void Start()
    {
        
        m_bodyPeriod *= Random.Range (.9f, 1.1f);
    }



    void Update () {
        return;
        //body motion
        m_bodyPeriod = Mathf.Clamp (m_bodyPeriod, .1f, m_bodyPeriod);
        float progress = ( (Time.time+m_timeOffset) % m_bodyPeriod )/m_bodyPeriod;

        m_bodySensor.transform.localPosition = direction * m_headCurve.Evaluate( progress ) * m_bodyMoveAmplitude  + m_offset;

        m_bodySensor.m_params.inflate =m_headCurve.Evaluate( progress ) * m_bodyInflateAmplitude;


    }
    void SetPosition(Vector3 pos){
        m_bodySensor.transform.localPosition =pos;
    }
    void SetInflate(float val){
        m_bodySensor.m_params.inflate =val;
    }

    public float testSpeed;
    public Vector3 startPos;
    public Vector3 endPos;

    IEnumerator Tention(){

        float progress =0.0f;
        yield return new WaitForSeconds(2.0f);
        while(true){
            progress +=Time.time*testSpeed;
            SetPosition(Vector3.Slerp(startPos,endPos,progress));
            SetInflate(m_headCurve.Evaluate( progress ) * m_bodyInflateAmplitude);
            if(progress>1.0f)yield break;
            yield return null;
        }
        Debug.Log("End Anime");
    }


    public Vector3 direction;

    public SkurtState state=SkurtState.Notention;



    public VertExmotion motionBase;


    bool isOn=false;
    public void Tets(){
        isOn=!isOn;
        StartCoroutine(Tention());
    }
    public void Enable(bool isEnable){
        motionBase.enabled=isEnable;
        motionBase.DisableMotion();
       
    }


    void OnDrawGizmos()
    {

        if( m_bodySensor != null )
            Gizmos.DrawSphere (m_bodySensor.transform.position, .1f);

    }




}
