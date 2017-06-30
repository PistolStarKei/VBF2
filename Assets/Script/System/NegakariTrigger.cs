using UnityEngine;
using System.Collections;

public class NegakariTrigger : MonoBehaviour {

    float dist=0.0f;
    float kakuritsu=0.0f;

    void OnTriggerEnter(Collider col){
        if(col.gameObject.layer==LayerMask.NameToLayer("Rure")){
            kakuritsu=PSGameUtils.GetChusenRitsuFromKakuritsu(GetLargestBounds()/0.2f,(negakarikakuritsu-TackleParams.Instance.tParams.negakariKaihi));
            lure=LureController.Instance.gameObject;
            Player.Instance.SetDistancePlayer(LureController.Instance.gameObject.transform);
            dist=Player.Instance.distanceToLure;
        }
    }
    float GetLargestBounds(){
        float num=1f;
        if(transform.lossyScale.x>transform.lossyScale.y){
            num=transform.lossyScale.x;
            if(transform.lossyScale.z>transform.lossyScale.x){
                num=transform.lossyScale.z;
            }
        }else{
            num=transform.lossyScale.y;
            if(transform.lossyScale.y<transform.lossyScale.z){
                num=transform.lossyScale.z;
            }
        }
        return num;

    }
   
    GameObject lure;
    public  float negakarikakuritsu=0.0f;

    void Update(){
        if(lure!=null ){
            if(FishingStateManger.Instance.currentMode!=GameMode.Reeling && FishingStateManger.Instance.currentMode!=GameMode.ReelingOnLand){
                return;
            }
                if(FishingStateManger.Instance.GetisNegakariOrFoockingState() || kakuritsu<=0.0f)return;
                if(Player.Instance.distanceToLure-dist<-0.2f){
                    dist=Player.Instance.distanceToLure;
                   if(PSGameUtils.Chusen(kakuritsu)){
                        lure=null;
                        FishingStateManger.Instance.OnNegakari();

                        return;
                    }
                }
        }
    }
    void OnTriggerExit(Collider col){
        if(col.gameObject.layer==LayerMask.NameToLayer("Rure")){
            lure=null;
        }
    }


}
