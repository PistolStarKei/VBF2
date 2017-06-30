using UnityEngine;
using System.Collections;

public class FishAiParent : MonoBehaviour {

    public BassParameters parameters;
    public FightState fightState=FightState.NotFoocked;
    public bool isDebugMode=false;
    public BassState bassState=BassState.Stay;

    public BassAnimeEvenet anime;

   
    public bool isReachedMovePosition=false;


    public void AwayFromLure(){
        if(bassState==BassState.Stay){

            GetNewPoint(new Vector3(Random.Range(_territory_Width[0] ,_territory_Width[1]),transform.position.y-(transform.localScale.x*2.0f),Random.Range(_territory_Depth[0],_territory_Depth[1])),true,true,Random.Range(0.0f,0.1f),4.0f,1.0f);  
        }
    }




   

    public  Vector3 moveTarget;
    public float _speed=0.0f;               //Fish Speed
    public float _turnSpeed=0.0f;                  //Turn speed
    public Transform _model;                //Model with animations
    public float _targetSpeed=0.0f;                    //Fish target speed
    public float tParam =0.0f;                 //
    public float _rotateCounterR=0.0f;             //Used to increase avoidance speed over time
    public float _rotateCounterL=0.0f;             
    public Transform _scanner;              //Scanner object used for push, this rotates to check for collisions
    public  bool _scan = true;      //Has this been instantiated
    public int _updateNextSeed= 0;  //When using frameskip seed will prevent calculations for all fish to be on the same frame
    public int _updateSeed= -1;
    public float _stuckCounter;            //prevents looping around a waypoint

    public  float rodPowerMax=0.0f;
    public float sizeNanido=0.0f;
    public bool isVisible=false; 
    public void VisibleBass(bool isVisible){
        if(isVisible){

            _model.gameObject.SetActive(true);
        }else{
            if(!this.isVisible){
                _model.gameObject.SetActive(false);
            }
        }
    }
    public void CheckVisible(){
        if(transform.position.y> Constants.Params.bassVisibleInDepth){
            if(_model.gameObject.activeSelf!=false)_model.gameObject.SetActive(false);
        }else{
            if(_model.gameObject.activeSelf!=true)_model.gameObject.SetActive(true);
        }

    }



    // Use this for initialization
    #if UNITY_EDITOR
    public  bool _sWarning= true;
    #endif


    public float minTurnSpeed=2.0f;
    public float maxTurnSpeed=30.0f;
    public float minSpeed=2.0f;
    public float maxSpeed=30.0f;
    public Transform waypointVisual;

    public float testSpeed=0.0f;
    public float testTurnSpeed=0.0f;

    public bool isTurningReach=false;
    public float turnigTime=0.0f;
    public float turnMaxTime=0.0f;
    public float turnMaxTimeDelta=0.0f;


    public float testTuringTime=0.0f;
    public float tParamDec=0.0f;
    public float speedWhenDeccerate=0.0f;

    public  void GetNewPoint(Vector3 moveTo,bool isMoveFromStill,bool isTurningReach,float turnigTime,float turnigMax,float speed){

        this.isTurningReach=isTurningReach;;
        turnMaxTimeDelta=0.0f;
        speedWhenDeccerate=0.0f;
        tParam = 0.0f;
        if(isTurningReach){
            _targetSpeed = Random.Range(minSpeed, maxSpeed);
            _turnSpeed = Random.Range(minTurnSpeed, maxTurnSpeed);
            this.turnigTime=Time.timeSinceLevelLoad+turnigTime;

        }else{
            _turnSpeed = Random.Range(minTurnSpeed, maxTurnSpeed);
            _targetSpeed = Random.Range(minSpeed, maxSpeed);


        }
        if(speed!=0.0f){
            _targetSpeed=speed;
            _turnSpeed =_targetSpeed*17.0f;
        }
        _stuckCounter=0.0f;
        this.turnMaxTime=turnigMax+turnigTime;

        if(isMoveFromStill){
            _speed=0.0f;
        }else{
            _speed=_targetSpeed;
        }
        moveTarget=moveTo;

        isReachedMovePosition=false;
    }

    public float posOffset=0.0f;

    //scale1 の時のテリトリーサイズ　基準に大小する
    public float[] _territory_Width=new float[2];
    public float[] _territory_Depth=new float[2];
    public float[] _territory_Height=new float[2];

    public void CreateTerritory(BassRange bassRange){
        
        float terrytorySize=2.0f;
        posOffset=transform.localScale.x*Constants.BassBihaviour.posOffsetWhenScaleOne;
        terrytorySize=posOffset*2;

        _territory_Width[0]=transform.position.x-terrytorySize;
        _territory_Width[1]=transform.position.x+terrytorySize;
        _territory_Depth[0]=transform.position.z-terrytorySize;
        _territory_Depth[1]=transform.position.z+terrytorySize;

        switch(bassRange){
        case BassRange.Top:
            Debug.Log("Create territory top");
             parameters.spawnedPosiion=new Vector3(transform.position.x,0.0f-posOffset,transform.position.z);
            _territory_Height[1]=0.0f-posOffset;
            Debug.Log("Create territory top"+_territory_Height[1]);
            _territory_Height[0]=(0.0f-posOffset)-terrytorySize;
            if(_territory_Height[0]<-(EnvManager.Instance.BottomDepth)+posOffset)_territory_Height[0]=-EnvManager.Instance.BottomDepth+posOffset;
            if(_territory_Height[1]<_territory_Height[0]){
                _territory_Height[1]=_territory_Height[0]+0.2f;
            }
            break;
        case BassRange.Mid:
             parameters.spawnedPosiion=new Vector3(transform.position.x,-Mathf.Abs(EnvManager.Instance.BottomDepth)/2.0f,transform.position.z);
            _territory_Height[1]= parameters.spawnedPosiion.y+(terrytorySize/2.0f);
            if(_territory_Height[1]>-posOffset)_territory_Height[1]=0.0f-posOffset;
            _territory_Height[0]= parameters.spawnedPosiion.y-(terrytorySize/2.0f);
            if(_territory_Height[0]<-(EnvManager.Instance.BottomDepth)+posOffset)_territory_Height[0]=-EnvManager.Instance.BottomDepth+posOffset;
            if(_territory_Height[1]<_territory_Height[0]){
                _territory_Height[0]=0.0f-posOffset;
                _territory_Height[1]=_territory_Height[0]+0.2f;

            }
            break;
        case BassRange.Bottom:
             parameters.spawnedPosiion=new Vector3(transform.position.x,-EnvManager.Instance.BottomDepth+posOffset,transform.position.z);
            _territory_Height[1]= parameters.spawnedPosiion.y+terrytorySize;
            if(_territory_Height[1]>-posOffset)_territory_Height[1]=0.0f-posOffset;
            _territory_Height[0]= parameters.spawnedPosiion.y;
            if(_territory_Height[1]<_territory_Height[0]){
                _territory_Height[0]=0.0f-posOffset;
                _territory_Height[1]=_territory_Height[0]+0.2f;

            }
            break;
        }
        transform.position=new Vector3(transform.position.x,Random.Range(_territory_Height[0],_territory_Height[1]),transform.position.z);
    }


   
    public Vector3 GetRandomPositionInTerritory(){
        return new Vector3 (Random.Range(_territory_Width[0] ,_territory_Width[1]),Random.Range(_territory_Height[0], _territory_Height[1]),Random.Range(_territory_Depth[0],_territory_Depth[1]));  
    }





}
