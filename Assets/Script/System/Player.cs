using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;


public class Player : PS_SingletonBehaviour<Player> {

	void Start(){
		//FreePlayer
		//rodParams.gameObject.transformをDespawn
		//ロードして、rodParamsを得て設定する　
		//RodRootを初期位置にセットする
		//rodParams.gameObjectをRodRootの子供にして、rodParamsのStartTransに設定する
		// UpdatePlayerConstrainsする
		//Move か　Castに応じてPositionする
		UpdatePlayerConstrains();
	}

    public bool showPlayerModel =true;
    public FullBodyBipedIK playerIK;
    public HandPoser rightHandGrip;
    public HandPoser leftHandGrip;
    public void FreePlayer(){
        if(!showPlayerModel)return;
        playerIK.solver.IKPositionWeight=0.0f;
        playerIK.solver.leftHandEffector.positionWeight=0.0f;
        playerIK.solver.leftHandEffector.rotationWeight=0.0f;
        playerIK.solver.leftArmChain.bendConstraint.weight=0.0f;
        playerIK.solver.rightHandEffector.positionWeight=0.0f;
        playerIK.solver.rightHandEffector.rotationWeight=0.0f;
        playerIK.solver.rightArmChain.bendConstraint.weight=0.0f;
    }
    void SetPlayerConstrains(Transform grip , Transform ReelHundleHand,Transform ReelHundleHandToRotate,Transform reelCenter){
        FreePlayer();
        if(!showPlayerModel||rodParams==null){
            return;
        }
        bool error=false;
        if(grip!=null){
            playerIK.solver.leftHandEffector.target=grip;
            playerIK.solver.leftHandEffector.positionWeight=1.0f;
            playerIK.solver.leftHandEffector.rotationWeight=1.0f;
            playerIK.solver.leftArmChain.bendConstraint.weight=1.0f;
        }else{
            Debug.LogError("Grip==null");
            error=true;
        }
       

        if(ReelHundleHand!=null){
            playerIK.solver.rightHandEffector.target=ReelHundleHand;
            playerIK.solver.rightHandEffector.positionWeight=1.0f;
            playerIK.solver.rightHandEffector.rotationWeight=1.0f;
            playerIK.solver.rightArmChain.bendConstraint.weight=1.0f;
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
    public void UpdatePlayerConstrains(){
        SetPlayerConstrains(rodParams.gripController,rodParams.reelHundleToGrab,rodParams.reelHundleToRotate,rodParams.reelCenter);
    }
    public RodParameter rodParams;
    public void Test(){
        SetPlayerState(false);
    }
    public void SetPlayerState(bool isDefault){
        StartCoroutine(HandAnim(isDefault));
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
            rightHandGrip.weight=1.0f;
            RodParent.parent=gameObject.transform;
            RodParent.localPosition=RodController.Instance.castPos;
            RodParent.localRotation= Quaternion.Euler(RodController.Instance.castRot);
            playerIK.solver.rightHandEffector.positionWeight=1.0f;
            playerIK.solver.rightHandEffector.rotationWeight=1.0f;
            playerIK.solver.rightArmChain.bendConstraint.weight=1.0f;
            playerIK.solver.leftHandEffector.positionWeight=1.0f;
            playerIK.solver.leftHandEffector.rotationWeight=1.0f;
            playerIK.solver.leftArmChain.bendConstraint.weight=0.395f;
        }else{
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
        AudioManager.Instance.Equip();
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
