using UnityEngine;
using System.Collections;

public class LureMover : MonoBehaviour {



    //Player.Instance.transformから見て、右か？
    public bool isRight(){
        if(Player.Instance.transform.InverseTransformPoint(transform.position).x-Player.Instance.transform.InverseTransformPoint(RodController.Instance.rodTip.transform.position).x>0.0f){
            return true;
        }
        return false;
    }


    [HideInInspector]
    public LureController lureController;
    public LureParamsData GetLureParams(){
        return lureController.lureParams.lureParamsData;
    }
    public float minSpeedWhenTention=0.02f;
    [HideInInspector]
    public Vector3 lookDirection;
    [HideInInspector]
    public Vector3 moveDirection;
    [HideInInspector]
    public float moveSpeed=0.0f;
    public float suimenOffset=0.5f;

    public bool SinkingObj=false;
    public void OnEnterWater(){
        Debug.Log("OnEnterWater");
        //if(SinkingObj){
        lureController.body.isKinematic=true;
            lureController.body.velocity=Vector3.zero;
            lureController.body.useGravity=false;
        lureController.body.isKinematic=false;
        //}
    }
    public void CheckHamon(){
        if(WaterController.Instance.isOnSuime(lureController.transform.position.y,.7f)){
            lureController.hamonEffect.Show(true);
            if( lureController.appeal.moveState==LureAction.still){
                lureController.hamonEffect.OnStill();
            }else{
                lureController.hamonEffect.OnMove( lureController.transform.eulerAngles.y, lureController.lureOBJ.transform.position);
               
            }

        }else{
            lureController.hamonEffect.Show(false);
        }

    }
    public bool isLureHitBottom(){
        return Player.Instance.isHitBottom(lureController.transform);
    }


    public bool IsOnSuimen(){
        return WaterController.Instance.isOnSuime(lureController.gameObject.transform.position.y,suimenOffset);}

    public virtual void MoveByRigidBody(){
        if (lureController.lureParams.buoParams.keepAwake && lureController.body.IsSleeping()){
            lureController.body.WakeUp();
            lureController.body.AddForce(-Vector3.up*2.0f,ForceMode.Force);
            lureController.body.useGravity = true;
        }
        if (lureController.body.mass < 0.1f) lureController.body.mass = 0.1f;
        //水中かどうかで変える
        if (WaterController.Instance.isUnderWater((lureController.transform.position.y-lureController.lureParams.buoParams.buoyancyOffset))){
            // 水中なので浮力を与える
            if (lureController.lureParams.buoParams.buoyancyFactor > 0.0f){
                lureController.body.useGravity = false;
                lureController.body.AddForce((Vector3.up*lureController.lureParams.buoParams.buoyancyFactor),ForceMode.Acceleration);
            }
            lureController.body.mass = lureController.lureParams.buoParams.underwaterMass;
            lureController.body.drag = lureController.lureParams.buoParams.underwaterDrag;
            lureController.body.angularDrag = lureController.lureParams.buoParams.underwaterADrag;

        } else {
            // 水面上なので重力を与える
            lureController. body.useGravity = true;
            lureController.body.mass = lureController.origMass;
            lureController.body.drag = lureController.origDrag;
            lureController.body.angularDrag =lureController.origADrag;
        }
    }
    public void ApplyBuoyancy(){
        if (lureController.body.mass < 0.1f) lureController.body.mass = 0.1f;

        if(!WaterController.Instance.isOnSuime(lureController.transform.position.y- lureController.lureParams.buoParams.buoyancyOffset,0.0f)){
            //underwaterDrag
            if (lureController.lureParams.buoParams.buoyancyFactor > 0.0f){
                lureController.body.useGravity = false;
                lureController.body.AddForce((Vector3.up* lureController.lureParams.buoParams.buoyancyFactor),ForceMode.Acceleration);
            }
        }else{
            //on water
            lureController.body.useGravity = true;
        }

    }

    public virtual void OnNoTention(){
        //アピールを行う
        lureController.appeal.GenerateAppealScale();
        RotateSlerpEuler(Time.deltaTime*rotationSpeedOnNoTention);
        ApplyBuoyancy();
        CheckHamon();
    }
    [HideInInspector]
    public bool isMovedOnce=false;
    public virtual void OnTention(){
        isMovedOnce=true;
        //アピールを行う
        RotateSlerp(Time.deltaTime*rotationSpeedOnTention);
        lureController.appeal.GenerateAppealScale();
        CheckHamon();
    }

    public virtual void OnNoTention_Land(){
        //アピールを行う
        if(lureController.body.isKinematic)return;
        isMovedOnce=false;
        lureController.appeal.SetMoveState(LureAction.onLand);
    }
    float PowerScalerOnLand=3.0f;
    public virtual void OnTention_Land(){
        //アピールを行う
        if(lureController.body.isKinematic)return;

        lureController.appeal.SetMoveState(LureAction.onLand);
        isMovedOnce=false;

        lureController.dummyTrans.localPosition=Vector3.zero;
        lureController.dummyTrans.localRotation=lureController.lureOBJ.transform.localRotation;
        tempVec=RodController.Instance.rodTip.transform.position;

        lureController.dummyTrans.LookAt(tempVec);
        moveDirection=lureController.dummyTrans.forward;

        if(!JoystickFloat.Instance.isRodMoveToTention && !Button_Float.Instance.isReeling()){
            moveSpeed=minSpeedWhenTention;
        }else{
            moveSpeed=AffectPowerFromRodBending();
        }
        ObjToDefaultRot(true,true);
        ApplyForce(moveSpeed*PowerScalerOnLand);
        lookDirection=moveDirection;
        RotateSlerp(Time.deltaTime*rotationSpeedOnTention);

    }
    [HideInInspector]
    public Vector3 tempVec=Vector3.zero;
    public virtual Vector3 GetDirection(Vector3 movetargetPos){

            return lureController.dummyTrans.forward;
    }

    float RodTipMoveSpeed=0.0f;
    Vector3 prevRodTipPos=Vector3.zero;
    public void SetPreviousRodPosition(Vector3 pos){
        prevRodTipPos=pos;
    }

    public float minPower=0.02f;
    public float maxPower=2.0f;
    public float rodBendingOnReel=0.2f;
    public float reelingPower=0.25f;
    float tempPow=0.0f;
    public float AffectPowerFromRodBending(){
        tempPow=0.0f;
        tempPow=lureController.m_RodBaseCurve.Evaluate( RodController.Instance.bendingPower );

        if(JoystickFloat.Instance.isRodMoveToTention){
            RodTipMoveSpeed=Vector3.Distance(RodController.Instance.rodTip.position,prevRodTipPos)/Time.deltaTime;
            RodTipMoveSpeed=Mathf.Clamp(RodTipMoveSpeed/10.0f,0.0f,10.0f);

                if(RodTipMoveSpeed>2.0f){
                    OnSpikeOnRodMove();
                }

            tempPow+= RodTipMoveSpeed*lureController.m_RodMoveCurve.Evaluate(RodTipMoveSpeed);

        }else{
            if(Button_Float.Instance.isReeling() ){
                if(rodBendingOnReel<RodController.Instance.bendingPower){
                    tempPow+=Button_Float.Instance.getReelingSpeed()*reelingPower;
                }
            }
        }
        tempPow=Mathf.Clamp(tempPow,minPower,maxPower);
        return tempPow;
    }

    public virtual void  OnSpikeOnRodMove(){


    }
    public void ApplyForce(float pow){
        lureController.body.AddForce(moveDirection * pow,ForceMode.VelocityChange);
    }
    public virtual void ActLure(float movingBodyPow){


    }
    public float rotationSpeedOnTention=1.0f;
    public float rotationSpeedOnNoTention=1.0f;
    public void RotateSlerp(float speed){
        lureController.transform.rotation=Quaternion.Slerp(lureController.transform.rotation,
            Quaternion.LookRotation(lookDirection),Time.deltaTime*speed);

    }

   
    public void RotateSlerpEuler(float speed){
        
        lureController.transform.rotation=Quaternion.Slerp(lureController.transform.rotation,Quaternion.Euler(lookDirection),
            Time.deltaTime*speed);
    }

    public Vector3 defaultLureObjRot=new Vector3(-900.0f,-900.0f,-900.0f);
    public virtual void OnDefaultPos(){
        if(defaultLureObjRot.x==-900.0f)defaultLureObjRot=lureController.lureOBJ.transform.localEulerAngles;

    }



    public float rotationSpeed_OBJOnNoTention=1.0f;
    public float rotationSpeed_OBJOnTention=1.0f;
    public void ObjToDefaultRot(bool isLerp,bool isTention){
        OnDefaultPos();
        if(isLerp){

            if(isTention){
                lureController.lureOBJ.transform.localRotation=Quaternion.Slerp(lureController.lureOBJ.transform.localRotation,Quaternion.Euler(defaultLureObjRot),
                    Time.deltaTime* rotationSpeed_OBJOnTention);
            }else{
                lureController.lureOBJ.transform.localRotation=Quaternion.Slerp(lureController.lureOBJ.transform.localRotation,Quaternion.Euler(defaultLureObjRot),
                    Time.deltaTime*rotationSpeed_OBJOnNoTention);
            }



        }else{
            if(defaultLureObjRot!=lureController.lureOBJ.transform.localEulerAngles)lureController.lureOBJ.transform.localRotation=Quaternion.Euler(defaultLureObjRot);
        }

    }

    public void ObjToDefaultRot(Vector3 offset){
        OnDefaultPos();
       lureController.lureOBJ.transform.localRotation=Quaternion.Slerp(lureController.lureOBJ.transform.localRotation,
            Quaternion.Euler(new Vector3(defaultLureObjRot.x+offset.x,defaultLureObjRot.y,defaultLureObjRot.z+offset.z)),Time.deltaTime*rotationSpeed_OBJOnNoTention);


    }

    public void ObjToDefaultRotOnWater(float offsetz){
        OnDefaultPos();

        lureController.lureOBJ.transform.localRotation=Quaternion.Slerp(lureController.lureOBJ.transform.localRotation,
            Quaternion.Euler(new Vector3(lureController.lureOBJ.transform.localRotation.eulerAngles.x
                ,lureController.lureOBJ.transform.localRotation.eulerAngles.y,
                defaultLureObjRot.z+offsetz)),Time.deltaTime*rotationSpeed_OBJOnNoTention);
        

    }

    public void ObjToDefaultRotJurked(Vector3 offset,float speed){
        lureController.lureOBJ.transform.localRotation=Quaternion.Slerp(lureController.lureOBJ.transform.localRotation,
            Quaternion.Euler(offset),Time.deltaTime*speed);


    }


}
