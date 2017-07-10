using UnityEngine;
using System.Collections;

public class LureMover_Krot : LureMover {

    public float customLookDirectionOnFloating=0.0f;
    public override void OnNoTention(){
        if(lureController.body.isKinematic)return;
        lureController.appeal.SetMoveState(IsOnSuimen()?LureAction.still:LureAction.floating);

        tempVec.x=0.0f;
        tempVec.y=lureController.transform.eulerAngles.y;
        tempVec.z=lureController.transform.eulerAngles.z;
        lookDirection=tempVec;
        lureController.hamonBladeEffect.OnStill();
        if(isSplasshableOnSuimen)if(lureController.hamonBladeEffect.sys.isPlaying) if(lureController.appeal.isSplashing)lureController.hamonBladeEffect.Show(false);

        if(isJurkingEnabled){
            if(isJurking){
                time+=Time.deltaTime;
                if(time>1.0f){
                    time=0.0f;
                    isJurking=false;
                }
            }
            ObjToDefaultRotOnWater(customLookDirectionOnFloating);
        }else{
            ObjToDefaultRotOnWater(customLookDirectionOnFloating);
        }
      brade.ActLure(moveSpeed,true);
        base.OnNoTention();
    }

    public override void OnTention(){
        if(lureController.body.isKinematic)return;
        if(lureController.body.useGravity)lureController.body.useGravity=false;
        lureController.appeal.SetMoveState(LureAction.moving);

        moveDirection=GetDirection(RodController.Instance.rodTip.transform.position);


        if(!JoystickFloat.Instance.isRodMoveToTention && !Button_Float.Instance.isReeling()){
            moveSpeed=minSpeedWhenTention;
            Debug.LogWarning("水中抵抗を実装すべき");
        }else{
            moveSpeed=AffectPowerFromRodBending();
        }



        lookDirection=moveDirection;
        if(IsOnSuimen()) moveDirection.y=-1.0f;
        ApplyForce(moveSpeed);
        if(isJurking){
            time+=Time.deltaTime;
            if(time>1.0f){
                time=0.0f;
                isJurking=false;
            }
            ObjToDefaultRotJurked(jurkingDirection,moveSpeed*jurkingSpeed);
        }else{
            ObjToDefaultRot(true,true);
        }

        ActLure(Player.Instance.transform.InverseTransformDirection(lureController.body.velocity).z);

        brade.ActLure(moveSpeed,false);
        base.OnTention();
    }


    float kubiDirec=-1.0f;
    public BradeMover brade;
    public float offSetSuimenEffect=0.05f;
    public override void ActLure(float movingBodyPow){
        if(movingBodyPow>0.0f)return;


        if(isSplasshableOnSuimen){
			if(WaterController.Instance.isOnSuime(lureController.transform.position.y, offSetSuimenEffect)){

                if(movingBodyPow>0.0f){
                    lureController.hamonBladeEffect.Show(false);
                    return;
                }
                lureController.hamonBladeEffect.Show(true);
            }else{
                lureController.hamonBladeEffect.Show(false);
            }
        }else{
            if(isPoppable ){

                if(!lureController.appeal.isSplashing  ){
                    return;
                }else{
                    if(lureController.appeal.isReactionByte )return;
                }
            }
        }


    }
    public bool isSplasshableOnSuimen=false;
    public override void OnNoTention_Land(){
        if(isSplasshableOnSuimen)if(lureController.hamonBladeEffect.sys.isPlaying) if(lureController.appeal.isSplashing)lureController.hamonBladeEffect.Show(false);

        base.OnNoTention_Land();
    }

    public override void OnTention_Land(){
        if(isSplasshableOnSuimen)if(lureController.hamonBladeEffect.sys.isPlaying) if(lureController.appeal.isSplashing)lureController.hamonBladeEffect.Show(false);

        base.OnTention_Land();
    }

    public float HonMuchNearToCulSinking=15.0f;

    public override Vector3 GetDirection(Vector3 movetargetPos){
        lureController.dummyTrans.localPosition=Vector3.zero;
        lureController.dummyTrans.localRotation=lureController.lureOBJ.transform.localRotation;
        tempVec=movetargetPos;

		if(WaterController.Instance.isOnSuime( lureController.transform.position.y,0.0f)){
            lureController.body.velocity=new Vector3(lureController.body.velocity.x,0.0f,lureController.body.velocity.z);
            lureController.transform.position=new Vector3(lureController.transform.position.x,0.0f,lureController.transform.position.z);
            tempVec.y=0.0f;
        }

        lureController.dummyTrans.LookAt(tempVec);
        return lureController.dummyTrans.forward;
    }
    public bool isPoppable=false;
    public bool isJurkingEnabled=false;
    public bool isJurking=false;
    public float jurkingSpeed=2.0f;
    public Vector3 jurkingDirection=new Vector3(0.0f,0.0f,0.0f);
    float time=0.0f;
    public override void  OnSpikeOnRodMove(){
        if(lureController.appeal.isSplashing)return;
        if(isPoppable){
            AudioManager.Instance.OnPopWater();
            LureSpawner.Instance.OnPopWater(new Vector3(lureController.transform.position.x,0.0f,lureController.transform.position.z));
            lureController.OnSplash(false);
            if(addYForceOnSplash!=0.0f)
                moveDirection.y+=addYForceOnSplash;
        }
        if(isJurkingEnabled){
            if(isRight()){
                //右に動くべき
                moveDirection.x+=-2.0f;
                jurkingDirection=new Vector3(0.0f,0.0f,0.0f);
            }else{
                moveDirection.x+=2.0f;
                jurkingDirection=new Vector3(0.0f,180.0f,0.0f);
            }
            time=0.0f;
            isJurking=true;
        }
    }
    public float addYForceOnSplash=0.0f;
    public override void MoveByRigidBody(){
        //アピールを行う
        base.MoveByRigidBody();
    }
}
