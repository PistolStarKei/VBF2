using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;


public class Player : PS_SingletonBehaviour<Player> {

	void Awake(){
		//FreePlayer
		//rodParams.gameObject.transformをDespawn
		//ロードして、rodParamsを得て設定する　
		//RodRootを初期位置にセットする
		//rodParams.gameObjectをRodRootの子供にして、rodParamsのStartTransに設定する
		// UpdatePlayerConstrainsする
		//Move か　Castに応じてPositionする
		PlayerConstrainsToFishing();
	}

	public Transform playerPositionBoat;
	public Transform playerPositionFishing;
	//プレイヤの位置をシートからリアへ
	public void ChangePositionTo(bool isFishingPosition){
		if(isFishingPosition){
			playerIK.transform.position=playerPositionFishing.position;
			playerIK.transform.rotation=playerPositionFishing.rotation;
			this.transform.parent=null;
			ShipControls.Instance.gameObject.transform.parent=this.transform;
		}else{
			playerIK.transform.position=playerPositionBoat.position;
			playerIK.transform.rotation=playerPositionBoat.rotation;
			ShipControls.Instance.gameObject.transform.parent=null;
			this.transform.parent=ShipControls.Instance.gameObject.transform;
		}
	}
	public Transform boatHundleLeft;
	public Transform boatHundleRight;
	public Transform boatPedalLeft;
	public Transform boatPedalRight;



    public bool showPlayerModel =true;
    public FullBodyBipedIK playerIK;
    public HandPoser rightHandGrip;
    public HandPoser leftHandGrip;
	public Transform bendGoalRightArm;
	public Transform bendGoalLeftArm;
    public void FreePlayer(){
        if(!showPlayerModel)return;
        playerIK.solver.IKPositionWeight=0.0f;
        playerIK.solver.leftHandEffector.positionWeight=0.0f;
        playerIK.solver.leftHandEffector.rotationWeight=0.0f;
        playerIK.solver.leftArmChain.bendConstraint.weight=0.0f;
        playerIK.solver.rightHandEffector.positionWeight=0.0f;
        playerIK.solver.rightHandEffector.rotationWeight=0.0f;
        playerIK.solver.rightArmChain.bendConstraint.weight=0.0f;
		playerIK.solver.rightFootEffector.positionWeight=0.0f;
		playerIK.solver.leftFootEffector.positionWeight=0.0f;
    }
    void SetPlayerConstrains(Transform grip , Transform ReelHundleHand,Transform ReelHundleHandToRotate,Transform reelCenter){
        FreePlayer();
        if(!showPlayerModel||rodParams==null){
			Debug.LogError("SetPlayerConstrains null Error");
            return;
        }
        bool error=false;
        if(grip!=null){
            playerIK.solver.leftHandEffector.target=grip;
            playerIK.solver.leftHandEffector.positionWeight=1.0f;
            playerIK.solver.leftHandEffector.rotationWeight=1.0f;
            playerIK.solver.leftArmChain.bendConstraint.weight=1.0f;
			playerIK.solver.leftArmChain.bendConstraint.bendGoal=bendGoalLeftArm;
        }else{
            Debug.LogError("Grip==null");
            error=true;
        }
       

        if(ReelHundleHand!=null){
            playerIK.solver.rightHandEffector.target=ReelHundleHand;
            playerIK.solver.rightHandEffector.positionWeight=1.0f;
            playerIK.solver.rightHandEffector.rotationWeight=1.0f;
            playerIK.solver.rightArmChain.bendConstraint.weight=1.0f;
			playerIK.solver.rightArmChain.bendConstraint.bendGoal=bendGoalRightArm;
        }else{
            Debug.LogError("ReelHundleHand==null");
            error=true;
        }

        if(ReelHundleHandToRotate!=null){
            RodController.Instance.reelHand=ReelHundleHandToRotate;
            RodController.Instance.SetReelHandRot(RodController.Instance.reelHand.localEulerAngles);
            RodController.Instance.rotateCenter=reelCenter;
        }else{
            Debug.LogError("ReelHundleHandToRotate==null");
            error=true;
        }


        if(!error){
            playerIK.solver.IKPositionWeight=1.0f;
            rightHandGrip.weight=1.0f;
        }else{
            Debug.LogError("Error on UpdatePlayerConstrains");
        }
    }

	public void PlayerConstrainsToFishing(){
        SetPlayerConstrains(RodController.Instance.rodParams.gripController,RodController.Instance.rodParams.reelHundleToGrab,RodController.Instance.rodParams.reelHundleToRotate,RodController.Instance.rodParams.reelCenter);
    }
	public void PlayerConstrainsToBoat(){
		FreePlayer();
		playerIK.solver.IKPositionWeight=1.0f;
		playerIK.solver.leftHandEffector.target=boatHundleLeft;
		playerIK.solver.leftHandEffector.positionWeight=1.0f;
		playerIK.solver.leftHandEffector.rotationWeight=1.0f;
		playerIK.solver.leftArmChain.bendConstraint.weight=1.0f;
		playerIK.solver.leftArmChain.bendConstraint.bendGoal=bendGoalLeftArm;

		playerIK.solver.rightHandEffector.target=boatHundleRight;
		playerIK.solver.rightHandEffector.positionWeight=1.0f;
		playerIK.solver.rightHandEffector.rotationWeight=1.0f;
		playerIK.solver.rightArmChain.bendConstraint.weight=1.0f;
		playerIK.solver.rightArmChain.bendConstraint.bendGoal=bendGoalRightArm;

		playerIK.solver.rightFootEffector.target=boatPedalRight;
		playerIK.solver.rightFootEffector.positionWeight=1.0f;

		playerIK.solver.leftFootEffector.target=boatPedalLeft;
		playerIK.solver.leftFootEffector.positionWeight=1.0f;


	}

    public RodParameter rodParams;
   
    public void ChangeRodState(bool isHandOff){
		StartCoroutine(HandAnim(isHandOff));
    }
    public Transform RodParent;
    public Transform PlayerLeftHand;
    public Transform leftHandDef;
    public Transform leftHandFishing;
    IEnumerator HandAnim(bool isToDefault){
        if(!showPlayerModel)yield break;
        WaitAndCover.Instance.CoverAll(false);

        RodController.Instance.RotateRodToDefault(true);
        yield return null;
        if(!isToDefault){
			//こっちでバグる
			playerIK.solver.IKPositionWeight=1.0f;
            rightHandGrip.weight=1.0f;
            RodParent.parent=gameObject.transform;
			RodParent.localPosition=new Vector3(0.0f,0.38f,0.0f);
			RodParent.localRotation= Quaternion.Euler(Vector3.zero);
            playerIK.solver.rightHandEffector.positionWeight=1.0f;
            playerIK.solver.rightHandEffector.rotationWeight=1.0f;
            playerIK.solver.rightArmChain.bendConstraint.weight=1.0f;
            playerIK.solver.leftHandEffector.positionWeight=1.0f;
            playerIK.solver.leftHandEffector.rotationWeight=1.0f;
            playerIK.solver.leftArmChain.bendConstraint.weight=0.395f;
        }else{
			playerIK.solver.IKPositionWeight=0.0f;
            RodParent.parent=PlayerLeftHand;
            RodParent.localPosition=new Vector3(-0.00095f,0.00026f,-0.00048f);
            RodParent.localRotation=Quaternion.Euler(new Vector3(-6.695496f,-97.12259f,119.9135f));

            rightHandGrip.weight=0.0f;
            playerIK.solver.rightHandEffector.positionWeight=0.0f;
            playerIK.solver.rightHandEffector.rotationWeight=0.0f;
            playerIK.solver.rightArmChain.bendConstraint.weight=0.0f;

            playerIK.solver.leftHandEffector.positionWeight=0.0f;
            playerIK.solver.leftHandEffector.rotationWeight=0.0f;
            playerIK.solver.leftArmChain.bendConstraint.weight=0.0f;

        }
        WaitAndCover.Instance.UnCoverAll();
    }
    


    public BassActiveTrigger bassEnable;
    public void ActiveBassEnabler(bool isOn){
       
        if( bassEnable.transform.gameObject.activeSelf!=isOn) bassEnable.transform.gameObject.SetActive(isOn);
    }


    [SerializeField]
    public LayerMask obstacleslayerMask_ForBass;
   
    RaycastHit hit;
    Ray ray;
    [SerializeField]
    public LayerMask layerMask_ForLureHitBottom;
    public float distanceToHitBottom=0.2f;
    public bool dynamicTerrainDetection=false;
    public bool isHitBottom(Transform trans){
        if(dynamicTerrainDetection){
            ray = new Ray(trans.position, -Vector3.up);
            if (Physics.Raycast(ray, out hit, 1,layerMask_ForLureHitBottom))if(hit.distance<distanceToHitBottom)return true;
        }else{
            if(-trans.position.y>GameController.Instance.BottomDepth)return true;
        }
        return false;
    }



    public Transform lureDefaultPos;

    //Params affected by tackles
    public float distanceToLure=0.0f;

    public float GetDistanceoPlayer(){
        return distanceToLure;
    }
    public void SetDistancePlayer(Transform trans){
        distanceToLure= transform.InverseTransformPoint(trans.position).z;

    }
   

    public Vector3 GetRelativeDirection(Vector3 windDrection){
        return transform.TransformDirection(new Vector3 (windDrection.x, 0.0f, windDrection.z));
    }


}
