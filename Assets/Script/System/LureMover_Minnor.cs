using UnityEngine;
using System.Collections;

public class LureMover_Minnor : LureMover {

    public bool isSplasshableOnSuimen=false;
    public bool isSinking=false;

    public bool isLookingDownOnFall=false;
    public Vector3 rotationOnBottm=Vector3.one;

    public override void OnNoTention(){
        if(lureController.body.isKinematic)return;

        if(isSinking){
            if(!isLureHitBottom()){
                if(lureController.body.useGravity)lureController.body.useGravity = false;
                lookDirection=lureController.transform.forward;
                if(isLookingDownOnFall){
                    tempVec.x=10.0f;
                    tempVec.y=lureController.transform.eulerAngles.y;
                    tempVec.z=lureController.transform.eulerAngles.z;
                    lookDirection=tempVec;
                }else{
                    tempVec.x=0.0f;
                    tempVec.y=lureController.transform.eulerAngles.y;
                    tempVec.z=lureController.transform.eulerAngles.z;
                    lookDirection=tempVec;
                }

                if(lureController.constantF.force==Vector3.zero)lureController.SetConstantForce(new Vector3(0.0f,-GetLureParams().sinkingSpeed,0.0f));
                lureController.appeal.SetMoveState(LureAction.sinking);

                ObjToDefaultRot(true,false);
                base.OnNoTention();
            }else{

                //OnBottom
                Button_Float.Instance.OnLureHitsButtom();

                lureController.appeal.SetMoveState(LureAction.still);
                lureController.ClearConstantForce();
                if(lureController.body.velocity!=Vector3.zero)lureController.body.velocity=Vector3.zero;
                tempVec.x=0.0f;
                tempVec.y=lureController.transform.eulerAngles.y;
                tempVec.z=lureController.transform.eulerAngles.z;
                lookDirection=tempVec;

                if(rotationOnBottm!=Vector3.one){
                    ObjToDefaultRot(rotationOnBottm);
                }else{
                    ObjToDefaultRot(true,false);
                }
            }

            lureController.appeal.GenerateAppealScale();
            RotateSlerpEuler(Time.deltaTime*rotationSpeedOnNoTention);
            CheckHamon();
        }else{
            lureController.appeal.SetMoveState(IsOnSuimen()?LureAction.still:LureAction.floating);
            tempVec.x=0.0f;
            tempVec.y=lureController.transform.eulerAngles.y;
            tempVec.z=lureController.transform.eulerAngles.z;
            lookDirection=tempVec;
            ObjToDefaultRot(true,false);
            base.OnNoTention();
        }
        if(isSplasshableOnSuimen)if(lureController.hamonBladeEffect.sys.isPlaying) if(lureController.appeal.isSplashing)lureController.hamonBladeEffect.Show(false);

    }

    public override void OnTention(){
        if(lureController.body.isKinematic)return;
        if(lureController.body.useGravity)lureController.body.useGravity=false;

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
    public float offSetSuimenEffect=0.05f;
    public override void ActLure(float movingBodyPow){
        if(movingBodyPow>0.0f)return;

        tempVec.x=Mathf.LerpAngle(lureController.lureOBJ.transform.localEulerAngles.x,0.0f,Time.deltaTime);
        tempVec.y=lureController.lureOBJ.transform.localEulerAngles.y+(kubiDirec*(GetLureParams().kubiSpeed*Mathf.Clamp(-movingBodyPow/10.0f,.1f,.6f))* Time.deltaTime*300.0f);
        tempVec.z=-(GetLureParams().kubiRotdownMax*Mathf.Clamp(-movingBodyPow/5.0f,.1f,.6f));

        //空中なので前傾させない
        if( GetLureParams().kubiRotdownMax==0.0f || WaterPlane.Instance.isOnSuime(lureController.transform.position.y,0.2f))tempVec.z=0.0f;

        lureController.lureOBJ.transform.localRotation=Quaternion.Euler(tempVec);
        //交互に動かす
        if(kubiDirec==-1f){
            if(lureController.lureOBJ.transform.localEulerAngles.y<(defaultLureObjRot.y-GetLureParams().kubiRotMax))kubiDirec=1f;
        }else{
            if(lureController.lureOBJ.transform.localEulerAngles.y>(defaultLureObjRot.y+GetLureParams().kubiRotMax))kubiDirec=-1f;
        }

        if(isSplasshableOnSuimen){
            if(WaterPlane.Instance.isOnSuime(lureController.transform.position.y, offSetSuimenEffect)){
                if(movingBodyPow>0.0f){
                    lureController.hamonBladeEffect.Show(false);
                    return;
                }
                lureController.hamonBladeEffect.Show(true);
            }else{
                lureController.hamonBladeEffect.Show(false);
            }
        }

    }

    public override void OnNoTention_Land(){
        if(isSplasshableOnSuimen)if(lureController.hamonBladeEffect.sys.isPlaying) if(lureController.appeal.isSplashing)lureController.hamonBladeEffect.Show(false);

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

        if(isSinking){
            if(WaterPlane.Instance.isOnSuime(lureController.transform.position.y,0.0f)){
                lureController.body.velocity=new Vector3(lureController.body.velocity.x,0.0f,lureController.body.velocity.z);
                if(lureController.transform.position.y>0.0f)lureController.transform.position=new Vector3(lureController.transform.position.x,0.0f,lureController.transform.position.z);
                tempVec.y=0.0f;
            }
        }else{
            if(Player.Instance.distanceToLure<HonMuchNearToCulSinking){
                //近いのでロッドに寄る
                tempVec.y+=(20.0f*-GetLureParams().sinkingSpeed*lureController.SinkingCurb.Evaluate(Mathf.Abs(lureController.transform.position.y)/GetLureParams().sinkingDepth))
                    *lureController.m_Dive_Curve.Evaluate(Player.Instance.distanceToLure);

            }else{
                tempVec.y+=(20.0f*-GetLureParams().sinkingSpeed*lureController.SinkingCurb.Evaluate(Mathf.Abs(lureController.transform.position.y)/GetLureParams().sinkingDepth));
            }
        }

        lureController.dummyTrans.LookAt(tempVec);
        return lureController.dummyTrans.forward;
    }

    public override void  OnSpikeOnRodMove(){
        if(lureController.appeal.isSplashing)return;
    }
    public override void MoveByRigidBody(){
        //アピールを行う
        base.MoveByRigidBody();
    }
}
