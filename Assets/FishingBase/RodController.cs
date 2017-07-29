using UnityEngine;
using System.Collections;

public class RodController : PS_SingletonBehaviour<RodController>  {

	public RodParameter rodParams;
	public void ShowRod(bool isShow){
		gameObject.GetComponent<MeshRenderer>().enabled=isShow;
		rodParams.GetComponent<MeshRenderer>().enabled=isShow;
	}


    public Transform rodlookTarget;
    public Transform rodTip;
    public Transform dummyRodTrans;
    public GameObject rodRootOBJ;
    public bool isDefauting=false;
    public MegaBend bend;
    private Vector3 relative;
    private int angle;
    private float sabun=0.0f;
    private float maxAngle;
    private float prevAngle=0.0f;
    public float bendingPower=0.0f;

    public bool isMax=false;
    public float bendRitsu=0.0f;
    public Transform reelHand;
    public Transform rotateCenter;//the target object
    private float speed = 10.0f;//a speed modifier
    Vector3 reelHandRot;
    public void SetReelHandRot(Vector3 rot){
        reelHandRot=rot;
    }
    bool isRodEnable=false;
    public bool GetIsRodEnabled(){
        return this.isRodEnable;
    }

  

    public void InitRod(){
        bendingPower=0.0f;
        bend.angle=0.0f;

    }
    public void RotateRod(Quaternion euler,float localYOffset){
        isDefauting=false;
        transform.localPosition=castPos;
        rodRootOBJ.transform.localRotation=euler;
    }
    public Vector3 castRot;
    public Vector3 castPos;
    public void RotateRodToDefault(bool isStartPosition){
        if(isStartPosition){
            isDefauting=false;
            transform.localPosition=castPos;
            rodRootOBJ.transform.localRotation=Quaternion.Euler(castRot);
            if(isInvokingMoveRotToCenter && routin!=null) StopCoroutine(routin);
            isInvokingMoveRotToCenter=false;
        }else{
            if(isInvokingMoveRotToCenter && routin!=null) StopCoroutine(routin);
            isInvokingMoveRotToCenter=false;
                routin=StartCoroutine( MoveRotToCenter());
        }
    }

	void Start(){
        InitRod();
        if(reelHand!=null)SetReelHandRot(reelHand.localEulerAngles);
        dummyRodTrans.position=rodTip.position;
	}

	void Update(){	
	
        isRodEnable=GameController.Instance.isRodEnabled()? true:false;
          
        if(isRodEnable ){
            
            CarlRodAngleWithTarget();

            if(LineScript.Instance.isLineHasTention()){
                OnTention();
            }else{
                //no tention so streigthen rod pole
                OnNoTention();
            }

           
            if( Button_Float.Instance.reelAudio.isPlaying) MoveHand();
        }
        preVRodPos=dummyRodTrans.position;
	}
    bool isInvokingMoveRotToCenter=false;
    Coroutine routin;
    IEnumerator MoveRotToCenter(){
        if(isInvokingMoveRotToCenter)yield break;
        isInvokingMoveRotToCenter=true;
        isDefauting=true;
        while(isDefauting){
            rodRootOBJ.transform.localRotation=Quaternion.Slerp(rodRootOBJ.transform.localRotation,Quaternion.Euler(new Vector3(90.0f,0.0f,0.0f)),Time.deltaTime*10.2f);
            if(transform.localPosition!=Vector3.zero)transform.localPosition=castPos;
            if(rodRootOBJ.transform.localRotation==Quaternion.Euler(new Vector3(90.0f,0.0f,0.0f)))isDefauting=false;
            yield return null;
        }
        rodRootOBJ.transform.localRotation=Quaternion.Euler(new Vector3(90.0f,0.0f,0.0f));
        isDefauting=false;
        isInvokingMoveRotToCenter=false;
    }


    private void BendRod(float bendAngle,float max){
        if(GameController.Instance.currentMode==GameMode.Fight){
            GameController.Instance.SetLineDamageOnMax(bendAngle,max);
        }
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
        bendingPower=Mathf.Clamp(LineScript.Instance.lineTention*bendRitsu,0.0f,1.0f);
        BendRod(180.0f*(LineScript.Instance.lineTention*bendRitsu),maxAngle);

    }
    Vector3 preVRodPos=Vector3.zero;
    private void OnNoTention(){
        if( GameController.Instance.currentMode==GameMode.Fight){
            Debug.LogWarning("実装する　テンションない場合のペナ");
        }
        if(bend.angle!=0.0f){
            bendingPower=0.0f;
            bend.angle=0.0f;
        }
    }

    private void MoveHand(){
        if(rotateCenter==null)return;
        speed= Button_Float.Instance.getReelingSpeed()*50.0f;
        reelHand.RotateAround (rotateCenter.position,rotateCenter.right,20 * Time.deltaTime * speed);
        reelHand.localRotation=Quaternion.Euler(reelHandRot);
    }


	IEnumerator SpawnRodInvoke(string name){


		Debug.Log("SpawnRod"+name);

		GameObject pre=Resources.Load("Rods/"+name) as GameObject;

		yield return null;

		if(pre!=null){
			GameObject sp=Instantiate(pre) as GameObject;
			if(sp!=null){
				RodParameter mov=sp.gameObject.GetComponent<RodParameter>();
				if(mov!=null){
					this.rodParams=mov;
					sp.gameObject.transform.parent=this.rodRootOBJ.transform;
					sp.gameObject.transform.localPosition=mov.startLocalPosition;
					sp.gameObject.transform.localRotation=Quaternion.Euler(mov.startLocalRotation);
				}else{
					Debug.LogError("Instatiated obj has no rodParams script");
				}
			}else{
				Debug.LogError("Instantiate failed");
			}

		}else{
			Debug.LogError("Instantiate failed");
		}

	}
   
  
}
