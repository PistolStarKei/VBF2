using UnityEngine;
using System.Collections;

public class LureMover_SuspendShad : LureMover {

    float kijyun=0.0f;
    public override void OnNoTention(){
        if(lureController.body.isKinematic)return;

        if(kijyun==0.0f)kijyun=lureController.transform.position.y;
        if(-transform.position.y>GetLureParams().sinkingDepth){
            lureController.appeal.SetMoveState(LureAction.floating);
            ApplyBuoyancy();
        }else{
            if(!isMovedOnce){
                lureController.appeal.SetMoveState(LureAction.floating);
                ApplyBuoyancy();
            }else{
                lureController.appeal.SetMoveState(LureAction.still);
                if (lureController.body.mass < 0.1f) lureController.body.mass = 0.1f;

                Debug.Log("ステイ");

                if(WaterController.Instance.isOnSuime(lureController.transform.position.y- lureController.lureParams.buoParams.buoyancyOffset,0.0f)){
                    //on water
                    lureController.body.useGravity = true;
                }else{
                    if(lureController.body.useGravity){
                        lureController.body.useGravity = false;
                    }
                    if(Mathf.Abs(lureController.transform.position.y-kijyun)<0.3f){
                        moveDirection=Random.value<=0.5f? Vector3.up:Vector3.down;
                    }else{
                        if(lureController.transform.position.y>kijyun){
                            moveDirection=Vector3.down;
                        }else{
                            moveDirection=Vector3.up;
                        }
                    }

                    moveSpeed=Random.Range(0.005f,0.01f);

                    ApplyForce(moveSpeed);
                }
            }
        }


        tempVec.x=0.0f;
        tempVec.y=lureController.transform.eulerAngles.y;
        tempVec.z=lureController.transform.eulerAngles.z;
        lookDirection=tempVec;
        ObjToDefaultRot(true,true);

        lureController.appeal.GenerateAppealScale();
        RotateSlerpEuler(Time.deltaTime*rotationSpeedOnNoTention);
        CheckHamon();
    }

    public override void OnTention(){
        if(lureController.body.isKinematic)return;
        if(lureController.body.useGravity)lureController.body.useGravity=false;
        kijyun=0.0f;
        lureController.appeal.SetMoveState(LureAction.moving);

        moveDirection=GetDirection(RodController.Instance.rodTip.transform.position);
        if(IsOnSuimen()) moveDirection.y=-1.0f;

        if(!JoystickFloat.Instance.isRodMoveToTention && !Button_Float.Instance.isReeling()){
            moveSpeed=minSpeedWhenTention;
        }else{
            moveSpeed=AffectPowerFromRodBending();
        }

        ApplyForce(moveSpeed);
        lookDirection=moveDirection;
        ObjToDefaultRot(true,true);

        ActLure(Player.Instance.transform.InverseTransformDirection(lureController.body.velocity).z);

        base.OnTention();
    }

    float kubiDirec=-1.0f;

    public override void ActLure(float movingBodyPow){
        if(movingBodyPow>0.0f)return;

        tempVec.x=Mathf.LerpAngle(lureController.lureOBJ.transform.localEulerAngles.x,0.0f,Time.deltaTime);
        tempVec.y=lureController.lureOBJ.transform.localEulerAngles.y+(kubiDirec*(GetLureParams().kubiSpeed*Mathf.Clamp(-movingBodyPow/10.0f,.1f,.6f))* Time.deltaTime*300.0f);
        tempVec.z=-(GetLureParams().kubiRotdownMax*Mathf.Clamp(-movingBodyPow/5.0f,.1f,.6f));

        //空中なので前傾させない
        if( GetLureParams().kubiRotdownMax==0.0f || WaterController.Instance.isOnSuime(lureController.transform.position.y,0.2f))tempVec.z=0.0f;

        lureController.lureOBJ.transform.localRotation=Quaternion.Euler(tempVec);
        //交互に動かす
        if(kubiDirec==-1f){
            if(lureController.lureOBJ.transform.localEulerAngles.y<(defaultLureObjRot.y-GetLureParams().kubiRotMax))kubiDirec=1f;
        }else{
            if(lureController.lureOBJ.transform.localEulerAngles.y>(defaultLureObjRot.y+GetLureParams().kubiRotMax))kubiDirec=-1f;
        }

    }

    public override void OnNoTention_Land(){
        base.OnNoTention_Land();
    }

    public override void OnTention_Land(){
        base.OnTention_Land();
    }

    public float HonMuchNearToCulSinking=15.0f;

    public override Vector3 GetDirection(Vector3 movetargetPos){
        lureController.dummyTrans.localPosition=Vector3.zero;
        lureController.dummyTrans.localRotation=lureController.lureOBJ.transform.localRotation;
        tempVec=movetargetPos;

        if(Player.Instance.distanceToLure<HonMuchNearToCulSinking){
            //近いのでロッドに寄る
            tempVec.y+=(20.0f*-GetLureParams().sinkingSpeed*lureController.SinkingCurb.Evaluate(Mathf.Abs(lureController.transform.position.y)/GetLureParams().sinkingDepth))
                *lureController.m_Dive_Curve.Evaluate(Player.Instance.distanceToLure);

        }else{
            tempVec.y+=(20.0f*-GetLureParams().sinkingSpeed*lureController.SinkingCurb.Evaluate(Mathf.Abs(lureController.transform.position.y)/GetLureParams().sinkingDepth));
        }

        lureController.dummyTrans.LookAt(tempVec);
        return lureController.dummyTrans.forward;
    }

    public override void  OnSpikeOnRodMove(){
        if(lureController.appeal.isSplashing)return;
        /*AudioManager.Instance.OnPopWater();
        LureSpawner.Instance.OnPopWater(new Vector3(lureController.transform.position.x,0.0f,lureController.transform.position.z));
        lureController.OnSplash(false);*/
    }
    public override void MoveByRigidBody(){
        //アピールを行う
        base.MoveByRigidBody();
    }
}
