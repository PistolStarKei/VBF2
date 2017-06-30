using UnityEngine;
using System.Collections;
using Kalagaan;

public class SkurfMover : MonoBehaviour {

    public VertExmotion motion;

    public void SetEnable(bool isdisable){
        motion.enabled = isdisable;
        motion.DisableMotion();
    }

    public VertExmotionSensor m_bodySensor;
   


    public float OpenVal=0.13f;
    public float CloseVal=-0.05f;

    public void MoveSkurf(float pow){
        m_bodySensor.m_params.inflate =pow;
    }
    bool isClose=false;
    public void OnTention(float moveSpeed){
        if(moveSpeed>0.0f)return;
        moveSpeed=Mathf.Abs(moveSpeed);
        if(isClose){
            Debug.Log("最初に入る　OnTention");
            isClose=!isClose;
            isClosed_Stay=false;
        }
        if(isClosed_Stay){
            Rotate(moveSpeed);
        }else{
            MoveSkurf( Mathf.Lerp( m_bodySensor.m_params.inflate,CloseVal,Time.deltaTime*moveSpeed*affectorTention));
            if(m_bodySensor.m_params.inflate<=CloseVal*0.95f){
                Debug.Log("最初に入る　Stayへ");
                isClosed_Stay=true;
            }
        }

    }

    void Rotate(float movingBodyPow){
        if(!isUseKubi)return;

        tempVec.x=0.0f;
        tempVec.y=0.0f;
        tempVec.z+=kubiDirec*(kubiSpeed*movingBodyPow* Time.deltaTime*affectorKubi);

       transform.localRotation=Quaternion.Euler(tempVec);
        //交互に動かす
        if(kubiDirec==-1f){
            if(tempVec.z<-rotMax)kubiDirec=1f;
        }else{
            if(tempVec.z>rotMax)kubiDirec=-1f;
        }
    }
    Vector3 tempVec;
    public float kubiSpeed=20.0f;
    public bool isUseKubi=false;
    public float rotMax=20.0f;
     bool isClosed=false;
    bool isClosed_Stay=false;
    public float affectorTention=1.0f;
    public float affectorKubi=1.0f;
    bool flag=false;
    float timeVal=0.0f;

    public float kubiDirec=-1.0f;
    public AnimationCurve openCurve = new AnimationCurve();
    public void OnNoTention(){
        if(!isClose){
            Debug.Log("最初に入る　OnNoTention");
            isClose=!isClose;
            isClosed_Stay=false;
            timeVal=0.0f;
        }
        timeVal+=Time.deltaTime;
        if(isClosed_Stay){
            Rotate(.05f);
        }else{
            MoveSkurf( Mathf.Lerp( m_bodySensor.m_params.inflate,OpenVal,openCurve.Evaluate(timeVal)));
            if(m_bodySensor.m_params.inflate>=OpenVal*0.95f){
                Debug.Log("最初に入る　Stayへ");
                isClosed_Stay=true;
            }
        }
       
    }

}
