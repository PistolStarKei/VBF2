using UnityEngine;
using System.Collections;

public class LureMover_Soft_RJ : LureMover {

    public bool isLookingDownOnFall=false;
    public Vector3 rotationOnBottm=Vector3.one;
    public float lookDownVal=90.0f;
    public override void OnNoTention(){
        if(lureController.body.isKinematic)return;
       
        if(!isLureHitBottom()){
            if(lureController.body.useGravity)lureController.body.useGravity = false;
            Debug.Log("OnNoTention not btm");
            if(isLookingDownOnFall){
                tempVec.x=lookDownVal;
            }else{
                tempVec.x=lureController.transform.eulerAngles.x;
            }
            tempVec.y=lureController.transform.eulerAngles.y;
            tempVec.z=lureController.transform.eulerAngles.z;
            lookDirection=tempVec;

            skurfMover.OnTention(Player.Instance.transform.InverseTransformDirection(lureController.body.velocity).y);
            if(lureController.constantF.force==Vector3.zero)lureController.SetConstantForce(new Vector3(0.0f,-GetLureParams().sinkingSpeed,0.0f));
            lureController.appeal.SetMoveState(LureAction.sinking);
            ObjToDefaultRot(true,false);
        }else{
            Debug.Log("OnNoTention btm");
            //OnBottom
            Button_Float.Instance.OnLureHitsButtom();
            skurfMover.OnNoTention();
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
        if(lureController.hamonBladeEffect.sys.isPlaying) if(lureController.appeal.isSplashing)lureController.hamonBladeEffect.Show(false);
    }

    public override void OnTention(){

      
        if(lureController.body.isKinematic)return;
        if(lureController.body.useGravity)lureController.body.useGravity=false;
        Debug.Log("OnTention");
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
       
        skurfMover.OnTention(Player.Instance.transform.InverseTransformDirection(lureController.body.velocity).z);
        base.OnTention();
    }

    public SkurfMover skurfMover;

    float kubiDirec=-1.0f;
    public float offSetSuimenEffect=0.05f;
    public override void ActLure(float movingBodyPow){
        if(movingBodyPow>0.0f)return;

        //ここでソフトボディを定義する

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

    public override void OnNoTention_Land(){
        if(lureController.hamonBladeEffect.sys.isPlaying) if(lureController.appeal.isSplashing)lureController.hamonBladeEffect.Show(false);
        base.OnNoTention_Land();
    }

    public override void OnTention_Land(){
        if(lureController.hamonBladeEffect.sys.isPlaying) if(lureController.appeal.isSplashing)lureController.hamonBladeEffect.Show(false);
        base.OnTention_Land();
    }

    public float HonMuchNearToCulSinking=15.0f;

    public override Vector3 GetDirection(Vector3 movetargetPos){
        lureController.dummyTrans.localPosition=Vector3.zero;
        lureController.dummyTrans.localRotation=lureController.lureOBJ.transform.localRotation;
        tempVec=movetargetPos;

        if(WaterPlane.Instance.isOnSuime(lureController.transform.position.y,0.0f)){
            lureController.body.velocity=new Vector3(lureController.body.velocity.x,0.0f,lureController.body.velocity.z);
            if(lureController.transform.position.y>0.0f)lureController.transform.position=new Vector3(lureController.transform.position.x,0.0f,lureController.transform.position.z);
            tempVec.y=0.0f;
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
