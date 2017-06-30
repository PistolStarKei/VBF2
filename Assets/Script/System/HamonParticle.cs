using UnityEngine;
using System.Collections;

public class HamonParticle : MonoBehaviour {

    public void Show(bool isOn){
        if(isOn){
            if(!sys.isPlaying)
            sys.Play();
        }else{
            if(sys.isPlaying){
                sys.Stop();
            }
        }

    }
    public ParticleScaler scaler;
    public ParticleSystem sys;
    void SetHamonScale(float scale){
        if(scaler.particleScale!=scale)scaler.particleScale=scale;
    }
    void SetHamonRot(float rot){
        if(sys.startRotation!=rot)sys.startRotation=rot;
    }
    public float stillHamonScale=0.5f;
    public float moveScale=0.5f;
    public void OnStill(){
        Show(false);
    }
    Vector3 tPos;

    public void OnMove(float angle,Vector3 pos){
        tPos=pos;
        tPos.y=0.0f;
        transform.position=tPos;
        SetHamonRot(  0.0172f*angle);
        SetHamonScale(moveScale);
    }


}
