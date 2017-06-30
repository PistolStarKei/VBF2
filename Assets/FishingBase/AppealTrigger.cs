using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppealTrigger : MonoBehaviour{

    void Update(){
        if(isSplashing){
            time+=Time.deltaTime;
            if(time>1.5f){
                SetDefault();
            }
        }
    }
     public void SetDefault(){
        if(scaler.gameObject.activeSelf)scaler.gameObject.SetActive(false);
        isSplashing=false;
        isReactionByte=false;
        moveState=LureAction.still;
    }
    public void OnSplash(bool isReactionByte){
        if(!isSplashing){
            isSplashing=true;
            isReactionByte=isReactionByte;
            if(isReactionByte){
                AudioController.Play("contact_wood");
            }else{
                AudioController.Play("pop");
            }
            time=0.0f;
        }
    }


    float time=0.0f;


    public LureAction moveState=LureAction.still;
    public void SetMoveState(LureAction moveState){
        if(this.moveState!=moveState)this.moveState=moveState;
    }
    public bool isSplashing=false;
    public bool isReactionByte=false;
    float appealScale=0.0f;

	// Use this for initialization
    public void GenerateAppealScale(){


        if(! LureController.Instance.isOnLand){
            if(moveState==0){
                if(!isSplashing){

                    appealScale=LureController.Instance.lureParams.lureParamsData.appealScale_still*TackleParams.Instance.tParams.appealFactor;

                    SetScale(appealScale);
                    return;
                }
            }
            if(isSplashing){
                if(isReactionByte){
                    appealScale=(LureController.Instance.lureParams.lureParamsData.appealScale_reaction*TackleParams.Instance.tParams.appealFactor)-Equations.EaseInOutBack(time,0.0f,(LureController.Instance.lureParams.lureParamsData.appealScale_reaction*TackleParams.Instance.tParams.appealFactor),1.5f);

                }else{
                    appealScale=(LureController.Instance.lureParams.lureParamsData.appealScale_splash*TackleParams.Instance.tParams.appealFactor)-Equations.EaseInOutBack(time,0.0f,(LureController.Instance.lureParams.lureParamsData.appealScale_splash*TackleParams.Instance.tParams.appealFactor),1.5f);

                }
            }else{
                switch(moveState){
                case LureAction.moving:
                    appealScale=LureController.Instance.lureParams.lureParamsData.appealScale_move*TackleParams.Instance.tParams.appealFactor;;
                    //move

                    break;
                case LureAction.floating:
                    appealScale=LureController.Instance.lureParams.lureParamsData.appealScale_float*TackleParams.Instance.tParams.appealFactor;;
                    //float

                    break;
                case LureAction.sinking:
                    appealScale=LureController.Instance.lureParams.lureParamsData.appealScale_sinking*TackleParams.Instance.tParams.appealFactor;;
                    //sinking
                    break;
                }
            }

            SetScale(appealScale);

        }

    }

	bool isEnable=false;
    public LureController lure;
	void OnTriggerEnter(Collider other ) {

            if(other.gameObject.layer==LayerMask.NameToLayer("Bass")){
                if(!Button_Float.Instance.isCovered){
                    other.gameObject.GetComponent<Bass>().AwayFromLure();
                }else{
                 other.gameObject.GetComponent<Bass>().OnEnterLureTrigger();
                }
                
            }
	}

	public ParticleScaler scaler;
	public bool isVisibleAppeal=true;

	void SetEnable(bool isEnable){

		this.GetComponent<Collider>().enabled=isEnable;
	}
    public void SetToDefaultState(bool isOn){
		SetEnable(isOn);
		if(gameObject.activeSelf!=isOn)gameObject.SetActive(isOn);
		if(scaler.gameObject.activeSelf!=isOn)scaler.gameObject.SetActive(isOn);
	
	}


	public void SetScale(float scale){
		if(isVisibleAppeal){
			if(!scaler.gameObject.activeSelf)scaler.gameObject.SetActive(true);
			transform.localScale=new Vector3(scale,scale,scale);
            if(scale<=0.1f){
                if(scaler.gameObject.activeSelf)
                    scaler.gameObject.SetActive(false);
                scaler.particleScale=scale;
            }else{
                if(!scaler.gameObject.activeSelf)
                    scaler.gameObject.SetActive(true);
                scaler.particleScale=scale;
            }
                
			
		}else{
			if(scaler.gameObject.activeSelf)scaler.gameObject.SetActive(false);
			transform.localScale=new Vector3(scale,scale,scale);
		}
	}


}
