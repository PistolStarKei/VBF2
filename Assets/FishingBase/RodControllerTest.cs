using UnityEngine;
using System.Collections;

public class RodControllerTest : PS_SingletonBehaviour<RodControllerTest>  {


    void Start(){
        InitRod();
        if(reelHand!=null)reelHandRot=reelHand.localEulerAngles;
        startPos=transform.localPosition;
        LineScriptTest.Instance.ShowLine();
    }
    public Transform rodlookTarget;
    public Transform rodTip;
    public Transform dummyRodTrans;
    public GameObject elbowTarget;
    public GameObject rodRootOBJ;
    public bool isDefauting=false;
    public MegaBend bend;
    private Vector3 relative;
    private int angle;
    private float sabun=0.0f;
    private float maxAngle;
    private float prevAngle=0.0f;
    public float bendingPower=0.0f;
    public float rodPower=0.0f;
    public bool isMax=false;
    public float bendRitsu=0.0f;
    public Transform reelHand;
    public Transform rotateCenter;//the target object
    private float speed = 10.0f;//a speed modifier
    Vector3 reelHandRot;
    bool isRodEnable=false;


    public void InitRod(){
        bendingPower=0.0f;
        bend.angle=0.0f;
        startPos=transform.localPosition;
    }
    public void RotateRod(Quaternion euler,float localYOffset){
        isDefauting=false;
        transform.localPosition=startPos;
        rodRootOBJ.transform.localRotation=euler;
    }


    void Update(){  

          isRodEnable=true;

        if(isRodEnable && !isDefauting){
            CarlRodAngleWithTarget();

            if(LineScriptTest.Instance.isLineHasTention()){
                OnTention();
            }else{
                //no tention so streigthen rod pole
                OnNoTention();
            }
        }
    }
    public Vector3 startPos;

    private void BendRod(float bendAngle,float max){
      
        if(bendAngle>=max){
            isMax=true;

            if(bend.angle==max)return;

            bend.angle=max;
        }else{
            isMax=false;
            if(bend.angle==bendAngle)return;
            bend.angle=bendAngle;
        }
    }


    private void CarlRodAngleWithTarget(){
        relative = transform.InverseTransformPoint(rodlookTarget.position);
        angle = Mathf.FloorToInt(Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg);

        if(Mathf.Abs(angle)>0.3f){
            prevAngle+=angle;
            transform.localRotation=Quaternion.Euler(transform.localRotation.x,prevAngle,transform.localRotation.z);
        }

        maxAngle = Mathf.Atan2(relative.z, relative.y) * Mathf.Rad2Deg;

    }
    private void OnTention(){
        bendingPower=(LineScriptTest.Instance.lineTention*bendRitsu);
        if(bendingPower>0.8f)bendingPower=0.8f;

        BendRod(180.0f*(LineScriptTest.Instance.lineTention*bendRitsu),maxAngle);
    }

    private void OnNoTention(){
        
        if(bend.angle!=0.0f){
            bendingPower=0.0f;
            bend.angle=0.0f;
        }
    }


}

