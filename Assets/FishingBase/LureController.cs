using UnityEngine;
using System.Collections;

public enum ActionWhenNoTention{Float,Sinking,SinkingSlow,StayAtDepth};
public enum ActionWhenDrag{Kubi,Splash,wakingDog,none,Pop};
public enum LureAction{still,moving,floating,sinking,onLand};


public class LureController :PS_SingletonBehaviour< LureController > {
    public bool isOnLand=false;
    public LureParams lureParams;
    //0 still 1 rure is moving 2 floating 3 sinking 
   
    public Transform dummyTrans;
    float force=0.0f;
    public AppealTrigger appeal;
    public GameObject lureOBJ;
    public void SetConstantForce(Vector3 force){
        constantF.force=force;
    }
    public void ClearConstantForce(){
        constantF.force=Vector3.zero;
    }

    [HideInInspector]
    public float origMass = 0.0f;
    [HideInInspector]
    public float origDrag = 0.0f;
    [HideInInspector]
    public float origADrag = 0.0f;
    void Start(){
        body=gameObject.GetComponent<Rigidbody>();
        constantF= gameObject.GetComponent<ConstantForce>();
        origMass = body.mass;
        origDrag = body.drag;
        origADrag = body.angularDrag;
       
    }



    [HideInInspector]
    public  Rigidbody body;
    [HideInInspector]
    public  ConstantForce constantF;

    public void CastLure(Vector3 force,Vector3 windAffects){
        mover.isMovedOnce=false;
        if(body!=null) body=gameObject.GetComponent<Rigidbody>();
        body.useGravity=true;
        isOnLand=false;
        body.isKinematic=false;
        LineScript.Instance.ShowLine();;
        body.AddForce(force);
        SetConstantForce(windAffects);
    }

    public void SetToDefaultPosition(Transform pos){
        hamonEffect.Show(false);
        hamonBladeEffect.Show(false);

        if(mover!=null)mover.ObjToDefaultRot(false,false);
        if(!body.isKinematic)body.velocity=Vector3.zero;
        body.isKinematic=true;
        appeal.SetDefault();
        GetComponent<ConstantForce>().force=Vector3.zero;
        transform.position=pos.position;
        transform.rotation=pos.rotation;

    }
    public void OnLureChamged(){
        RodController.Instance.rodlookTarget=lureOBJ.transform;
        LineScript.Instance.LurePosition=lureOBJ.transform;
        SetToDefaultPosition(Player.Instance.lureDefaultPos);

    }
    public bool isLureActive(){
        if(lureParams!=null){
            return  lureParams.gameObject.activeSelf;
        }

        return true;
    }
    public void HideLure(){
        if(lureParams!=null){
            appeal.SetDefault();
            lureParams.gameObject.SetActive(false);
        }
    }
    public void ShowLure(){
        if(lureParams!=null){
            lureParams.gameObject.SetActive(true);
        }
    }
    //Lure Movement
    void FixedUpdate(){  
       

        if(GameController.Instance.isStateWithin(GameMode.Cast) || lureOBJ==null){
            transform.position=Player.Instance.lureDefaultPos.position;
            transform.rotation=Player.Instance.lureDefaultPos.rotation;
            if(mover!=null)mover.SetPreviousRodPosition(RodController.Instance.rodTip.position);
            return;
        }
        if(GameController.Instance.GetisNegakariOrFoockingState()){
            if(mover!=null)mover.SetPreviousRodPosition(RodController.Instance.rodTip.position);
            return;
        }

        if(RodController.Instance.GetIsRodEnabled()){
            if(body.isKinematic)return;
            //プレイヤとルアーの距離を更新する
            Player.Instance.SetDistancePlayer(transform);
            //ルアー回収判定する
            if(!GameController.Instance.isStateWithin(GameMode.Throwing) &&  Player.Instance.GetDistanceoPlayer()<Constants.Params.kaishuDistance){
                OnReachedPlayer();return;
            }

             HandleByRod();

        }else{
            if(GameController.Instance.currentMode==GameMode.Throwing)mover.MoveByRigidBody();
        }

        if(mover!=null)mover.SetPreviousRodPosition(RodController.Instance.rodTip.position);

    }

    public LureMover mover;
    void HandleByRod(){
        if(body.isKinematic)return;

        if(!isOnLand){
           //水中にある状態
            if(constantF.force!=Vector3.zero)ClearConstantForce();
            if(LineScript.Instance.isLineHasTention()){
                    //テンションにより力を与える
                mover.OnTention();
            }else{
                mover.OnNoTention();
               
            }
        }else{
            //陸上にある状態
            if(constantF.force==Vector3.zero)SetConstantForce(new Vector3(0.0f,-lureParams.lureParamsData.sinkingSpeed,0.0f));

            if(LineScript.Instance.isLineHasTention()){
                mover.OnTention_Land();
            }else{
                mover.OnNoTention_Land();
            }

        }

    }
       

    //Evenets
    public void OnReachedPlayer(){
		//ルアーの回収イベントを呼ぶ
		if(GameController.Instance!=null)GameController.Instance.OnLureKaishu();

        Debug.LogError("OnReachedPlayer");
        GameController.Instance.ChangeStateTo(GameMode.Cast);
    }
    public void OnWater(bool isOnWater){
        if(isOnWater){
            
            mover.OnEnterWater();
            ClearConstantForce();

        }else{
            ClearConstantForce();
        }
    }



    public AnimationCurve SinkingCurb = new AnimationCurve();

	
	void OnCollisionEnter(Collision collision) {
       
       
        /*if(collision.gameObject.layer==LayerMask.NameToLayer("ReflectSubstance")|| collision.gameObject.layer==LayerMask.NameToLayer("NoReflectSubstance")){
            Debug.Log("OnCollisionEnter"+collision.gameObject.name);
           
           
                if(lureParams.lureParamsData.isReactionByteEnabled){
    				OnSplash(true);
    			}
		}else{
        */
        if(collision.gameObject.layer==LayerMask.NameToLayer("Terrain")){

            if(GameController.Instance.currentMode!=GameMode.Reeling)GameController.Instance.OnTerrain();
        }
        if(collision.gameObject.layer==LayerMask.NameToLayer("ReflectSubstanceCanLand")){
            AudioManager.Instance.Equip();
            if( GameController.Instance.currentMode==GameMode.ReelingOnLand)return;
            AudioManager.Instance.Contact();
            GameController.Instance.ChangeStateTo(GameMode.ReelingOnLand);
        }

        if(GameController.Instance.currentMode!=GameMode.Reeling && GameController.Instance.currentMode!=GameMode.ReelingOnLand){
            return;
        }
        if(collision.gameObject.layer==LayerMask.NameToLayer("Bass")){
            Debug.Log("OnCollisionEnter"+collision.gameObject.name);

            if(!Button_Float.Instance.isCovered){
                collision.gameObject.GetComponent<Bass>().AwayFromLure();
            }
        }
		//}
	}
   

    public void OnEnterWater(){
        if( GameController.Instance.currentMode==GameMode.Reeling)return;
        isOnLand=false;
        AudioController.Play("chapun");
        GameController.Instance.ChangeStateTo(GameMode.Reeling);
    }

	public void OnSplash(bool isReactionByte){
        appeal.OnSplash(isReactionByte);
    }

    public AnimationCurve m_RodBaseCurve = new AnimationCurve();
    public AnimationCurve m_RodMoveCurve = new AnimationCurve();
    public AnimationCurve m_Dive_Curve = new AnimationCurve();
    public HamonParticle hamonEffect;
    public HamonParticle hamonBladeEffect;
   


}
