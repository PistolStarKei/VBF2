using UnityEngine;
using System.Collections;
public enum LureActionAppeal{STOP_GO,GO_STOP,MOVE_FLOAT,LIFT_FALL,FLOAT_SUIMEN,FLOAT_GO,SINKING_BTM,SINKING_GO
    ,JUST_STILL,JUST_MOVE,JUST_FLOAT,JUST_SINKING,NONE};
public enum BassState{Stay,Chase,Bite,Back,Fight,Result};
public enum FightState{NotFoocked,Dragged,Monkey,Tukkomi,Stay};
public enum BassRange{Top,Mid,Bottom};
public enum EatType{Anger,Boil,Tired,Wait};
[System.Serializable]
public class BassParameters{
    public bool isEcoMode=false;
    //1-10まで
    public int KASSEILEVEL=0; 
    public int SURELEVEL=0;
    public BassRange range=BassRange.Top;
    public EatType eatType=EatType.Wait;

    public float size=10.0f;
    public Vector3 spawnedPosiion=Vector3.zero;

    public float GetMoveFrequency_Still(){
        if(KASSEILEVEL==1)return 0.06f;
        return 0.06f+KASSEILEVEL*0.024f;
    }
    public float GetDelayStill(){
        //min .9 max .1
        return Random.Range(0.7f,1.2f);
    }
    public float stamina=20.0f;
   
    public Baits[] bate;
    //チェイス移行率 活性に応じて、50-99% -スレに応じて
    public bool StateChangeToChase(){
        float val=0.99f;
        val-=(SURELEVEL/10.0f);

        if(val>Random.value){
            return true;
        }
        return false;
    }

}
public class Bass : StatefulObjectBase<Bass, BassState> {

    public bool isJumping=false;
    public bool isJumpKaihi=false;

    public BassParameters parameters;
    public BassState bassState=BassState.Stay;
    public FightState fightState=FightState.NotFoocked;
    public BassAnimeEvenet anime;
    public bool isDebugMode=false;

    public bool useObstacleAvoidance=false;
    //kasseiLevel 0.0-1.0 cautiouslevel=0.0f -1.0f
    public void Init(BassParameters parameters){

		//プール以外ではバスのテクスチャをLowResにする
		if(GameController.Instance.isPoolMode){
			if(_model.material!=GameController.Instance.bassMatFight)_model.material=GameController.Instance.bassMatFight;
		}else{
			if(_model.material!=GameController.Instance.bassMatInWater)_model.material=GameController.Instance.bassMatInWater;
		}

        isBassActive=false;
        this.parameters=parameters;
        float size=parameters.size* Constants.BassBihaviour.sizeScallingFactor;
        transform.localScale=new Vector3(size,size,size);
        transform.position=parameters.spawnedPosiion;
        transform.localRotation =   Quaternion.Euler(new Vector3(0.0f, Random.Range(-25.0f, 25.0f),0.0f));

        stateList.Add(new stateStay(this));
        stateList.Add(new stateChase(this));
        stateList.Add(new stateBite(this));
        stateList.Add(new stateBack(this));
        stateList.Add(new stateFight(this));
        stateList.Add(new stateResult(this));


        stateMachine = new StateMachine<Bass>();
        CreateTerritory();
        ChangeState(BassState.Stay);
    }

    Vector3 moveTarget;
    bool isReachedMovePosition=false;
    public float howLongToDetectReached=2.0f;
    float timeStill =0.0f;
    float timeReached =0.0f;
    float timeEvery =0.0f;
    public float delayStill =0.0f;
    float delayReached =0.0f;
    float delayEvery =0.0f;

    float minTurnSpeed=2.0f;
    float maxTurnSpeed=30.0f;
    float minSpeed=2.0f;
    float maxSpeed=30.0f;
    bool isTurningReach=false;
    float turnigTime=0.0f;
    float turnMaxTime=0.0f;
    float turnMaxTimeDelta=0.0f;
    float tParamDec=0.0f;
    float speedWhenDeccerate=0.0f;
    public float _targetSpeed=0.0f;                    //Fish target speed
    public float tParam =0.0f; 
    public float _speed=0.0f;
    float _stuckCounter;
    public float _turnSpeed=0.0f;
    Quaternion lookTarget=Quaternion.identity;
    public void SetMinMaxSpeed(float minSpeed ,float maxSpeed,float minTurnSpeed,float maxTurnSpeed){
        this.minSpeed=minSpeed;
        this.maxSpeed=maxSpeed;
        this.minTurnSpeed=minTurnSpeed;
        this.maxTurnSpeed=maxTurnSpeed;
    }

    //上下に浮くもの
    public Vector3 positionOffset=Vector3.zero;
    void ResetElapsedTime(){
        timeReached=0.0f;
        timeStill=0.0f;
    }
   
    public float distance=0.0f;
    Vector3 tempVec=Vector3.zero;
    public Vector3 GetRandomFloatingPosition(){
        if(transform.position.y>=_territory_Height[1]){
            tempVec.x=transform.position.x;
            tempVec.y=transform.position.y-0.1f; 
            tempVec.z=transform.position.z;
            return tempVec;
        }
        tempVec.x=transform.position.x;
        tempVec.y=transform.position.y+Random.Range(-0.1f,0.5f); 
        tempVec.z=transform.position.z;
        return tempVec;
    }


    //テリトリー関係
    //scale1 の時のテリトリーサイズ　基準に大小する
    public float posOffset=0.0f;
    public  float[] _territory_Width=new float[2];
    public float[] _territory_Depth=new float[2];
    public float[] _territory_Height=new float[2];
    public void CreateTerritory(){
        float terrytorySize=2.0f;
        posOffset=transform.localScale.x*Constants.BassBihaviour.posOffsetWhenScaleOne;
        terrytorySize=posOffset*2;

        _territory_Width[0]=transform.position.x-terrytorySize;_territory_Width[1]=transform.position.x+terrytorySize;
        _territory_Depth[0]=transform.position.z-terrytorySize; _territory_Depth[1]=transform.position.z+terrytorySize;
       
        switch(parameters.range){
        case BassRange.Top:
             parameters.spawnedPosiion=new Vector3(transform.position.x,0.0f-posOffset,transform.position.z);
            _territory_Height[1]=0.0f-posOffset;
            _territory_Height[0]=(0.0f-posOffset)-terrytorySize;
            if(_territory_Height[0]<-(GameController.Instance.BottomDepth)+posOffset)_territory_Height[0]=-GameController.Instance.BottomDepth+posOffset;
            if(_territory_Height[1]<_territory_Height[0]){
                _territory_Height[1]=_territory_Height[0]+0.2f;
            }
            break;
        case BassRange.Mid:
             parameters.spawnedPosiion=new Vector3(transform.position.x,-Mathf.Abs(GameController.Instance.BottomDepth)/2.0f,transform.position.z);
            _territory_Height[1]= parameters.spawnedPosiion.y+(terrytorySize/2.0f);
            if(_territory_Height[1]>-posOffset)_territory_Height[1]=0.0f-posOffset;
            _territory_Height[0]= parameters.spawnedPosiion.y-(terrytorySize/2.0f);
            if(_territory_Height[0]<-(GameController.Instance.BottomDepth)+posOffset)_territory_Height[0]=-GameController.Instance.BottomDepth+posOffset;
            if(_territory_Height[1]<_territory_Height[0]){
                _territory_Height[0]=0.0f-posOffset;
                _territory_Height[1]=_territory_Height[0]+0.2f;

            }
            break;
        case BassRange.Bottom:
             parameters.spawnedPosiion=new Vector3(transform.position.x,-GameController.Instance.BottomDepth+posOffset,transform.position.z);
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

    public bool IsWithinInTerritory(Transform trans){
        if(trans.position.x<_territory_Width[0]){
            return false;
        }
        if(trans.position.x>_territory_Width[1]){
            return false;
        }
        if(trans.position.y<_territory_Height[0]){
            return false;
        }
        if(trans.position.y>_territory_Height[1]){
            return false;
        }
        if(trans.position.z<_territory_Depth[0]){
            return false;
        }
        if(trans.position.z>_territory_Depth[1]){
            return false;
        }
        return true;
    }


    public SkinnedMeshRenderer _model;
    public void CheckVisible(){
        if(transform.position.y> GameController.Instance.BassVisibleDepth){
            if(!_model.enabled)_model.enabled=true;
        }else{
            if(_model.enabled)_model.enabled=false;
        }

    }
    public void OnEnterEnableTrigger(bool Enter){
        SetActiveBass(Enter);
    }

    public void SetActiveBass(bool isOn){
        if(_model.enabled!=isOn)_model.enabled=isOn;
        isBassActive=isOn;
    }


    //移動する場所　0からスタートかTargetSpeedからスタートか　ターンするか　その時間　ターン最大値　ターゲットスピード指定　falseで即最高速度へ
    public  void GetNewPoint(Vector3 moveTo,bool isTurningReach,float turnigTime,float turnigMax,float speed,bool isAccerarate){
     
        this.isTurningReach=isTurningReach;;
        turnMaxTimeDelta=0.0f;
       

        if(isTurningReach){
            _targetSpeed = Random.Range(minSpeed, maxSpeed);
            _turnSpeed = Random.Range(minTurnSpeed, maxTurnSpeed);
            this.turnigTime=Time.timeSinceLevelLoad+turnigTime;

        }else{
            _turnSpeed = Random.Range(minTurnSpeed, maxTurnSpeed);
            _targetSpeed = Random.Range(minSpeed, maxSpeed);


        }
        startSpeed=this._speed;
        if(speed!=0.0f){
           
            _targetSpeed=speed;
           
            _turnSpeed =_targetSpeed*17.0f;
        }

        _stuckCounter=0.0f;
        this.turnMaxTime=turnigMax+turnigTime;

        if(isAccerarate){
            tParam = 0.0f;
            _speed=startSpeed;
        }else{
            tParam=1.0f;
            _speed=_targetSpeed;

        }
        moveTarget=moveTo;
        isReachedMovePosition=false;
    }

    public  void Move(bool isAccerate){
        //look target and adjust rotation
        SetRotationToTargetPoint();
        //move forword
        SetSpeed(isAccerate);
        transform.position += transform.TransformDirection(Vector3.forward)*_speed;

        RayCastToAwayFromObstacles();
        //set to be random
    }
    Quaternion rotation;
    void SetRotationToTargetPoint (){

        //look to target
        rotation = Quaternion.LookRotation(moveTarget - transform.position);

        if(!Avoidance()){

            if(isTurningReach){
                if(Time.timeSinceLevelLoad>turnigTime){

                    if(transform.rotation!=rotation){
                        turnMaxTimeDelta+=Time.deltaTime;
                        if(turnMaxTimeDelta>turnMaxTime){
                            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1.0f);
                        }else{
                            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _turnSpeed);
                        }
                    }
                }
            }else{
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _turnSpeed);
            }
        }
    }
    bool Avoidance () {
        if(!useObstacleAvoidance)return false;
        RaycastHit hit ;
        float d;
        Quaternion  rx = transform.rotation;
        Vector3 ex = transform.rotation.eulerAngles;
        Vector3 cacheForward = transform.forward;
        Vector3 cacheRight = transform.right;
        //Up / Down avoidance
        if (transform.position.y<-(GameController.Instance.BottomDepth)+posOffset){          

            d = (posOffset -  Mathf.Abs(transform.position.y-(-(GameController.Instance.BottomDepth)+posOffset)))/posOffset;
            ex.x -= _avoidSpeed*d*Time.deltaTime*(_speed +1);
            rx.eulerAngles = ex;
            transform.rotation = rx;
        }
        if (transform.position.y>0.0f-posOffset){
            d = (posOffset - Mathf.Abs(transform.position.y))/posOffset;         
            ex.x += _avoidSpeed*d*Time.deltaTime*(_speed +1);   
            rx.eulerAngles = ex;
            transform.rotation = rx;    
        }

        //Crash avoidance //Checks for obstacles forward
        if (Physics.Raycast(transform.position, cacheForward+(cacheRight*Random.Range(-0.1f, 0.1f)),out hit, _stopDistance, Player.Instance.obstacleslayerMask_ForBass)){
            Debug.Log("Found Obstacles forword");
            Debug.DrawLine(transform.position,hit.point);
            d = (_stopDistance - hit.distance)/_stopDistance;               
            ex.y -= _avoidSpeed*d*Time.deltaTime*(_targetSpeed +3);
            rx.eulerAngles = ex;
            transform.rotation = rx;
            _speed -= d*Time.deltaTime*_stopSpeedMultiplier*_speed;             
            if(_speed < 0.01f){
                _speed = 0.01f; 
            }
            return true;
        }else if (Physics.Raycast(transform.position, cacheForward+(cacheRight*(_avoidAngle+_rotateCounterL)),out hit, _avoidDistance, Player.Instance.obstacleslayerMask_ForBass)){
            Debug.Log("Found Obstacles Left");
            Debug.DrawLine(transform.position,hit.point);
            d = (_avoidDistance - hit.distance)/_avoidDistance;             
            _rotateCounterL+=0.1f;
            ex.y -= _avoidSpeed*d*Time.deltaTime*_rotateCounterL*(_speed +1);
            rx.eulerAngles = ex;
            transform.rotation = rx;                
            if(_rotateCounterL > 1.5f)
                _rotateCounterL = 1.5f;             
            _rotateCounterR = 0;
            return true;        
        }else if (Physics.Raycast(transform.position, cacheForward+(cacheRight*-(_avoidAngle+_rotateCounterR)),out hit, _avoidDistance, Player.Instance.obstacleslayerMask_ForBass)){
            Debug.Log("Found Obstacles Right");
            Debug.DrawLine(transform.position,hit.point);
            d = (_avoidDistance - hit.distance)/_avoidDistance;
            if(hit.point.y < transform.position.y){
                ex.y -= _avoidSpeed*d*Time.deltaTime*(_speed +1);
            }
            else{
                ex.x += _avoidSpeed*d*Time.deltaTime*(_speed +1);
            }
            _rotateCounterR +=0.1f;
            ex.y += _avoidSpeed*d*Time.deltaTime*_rotateCounterR*(_speed +1);
            rx.eulerAngles = ex;
            transform.rotation = rx;    
            if(_rotateCounterR > 1.5f)
                _rotateCounterR = 1.5f; 
            _rotateCounterL = 0;
            return true;
        }else{
            _rotateCounterL = 0;
            _rotateCounterR = 0;
        }
        return false;                                                                                                                                                                                                                                                                                                           
    }
    public float _avoidSpeed=75.0f;//How fast this turns around when avoiding   
    public int _avoidDistance= 1;       //How far avoid rays travel
    public float _avoidAngle = 0.35f;       //Angle of the rays used to avoid obstacles left and right
    public float _stopSpeedMultiplier= 2.0f;    //How fast to stop when within stopping distance
    public float _stopDistance= 0.5f;     
    float _rotateCounterR=0.0f;             //Used to increase avoidance speed over time
    float _rotateCounterL=0.0f;             
    public Transform _scanner;              //Scanner object used for push, this rotates to check for collisions
    bool _scan = true; 
    float startSpeed=0.0f;
    void SetSpeed(bool isAccerarate){
        if(isAccerarate){
            //slow to target speed
            if (tParam < 1) {
                //_targetSpeed=minSpeed MxSpeed
                    if(_speed > _targetSpeed/5.0f){
                        tParam += Time.deltaTime * 0.2f;
                        
                    }else{
                        tParam += Time.deltaTime * 0.8f;  

                    }
                //make it to target
                _speed = Mathf.Lerp(startSpeed, _targetSpeed,tParam);   
            }else{
                _speed =  _targetSpeed;
            }
        }else{

            if (tParam > 0.0f) {
                tParam -= Time.deltaTime * 0.2f; 
                //make it to target
                _speed = Mathf.Lerp(0.0f, _targetSpeed,tParam);   
            }else{
                _speed =  0.01f;
            }

        }
        anime.SetSpeed(_speed);
    }
    void RayCastToAwayFromObstacles() {
        if(!useObstacleAvoidance)return;
        //Scan random if not pushing
        if(_scan){
            _scanner.rotation = Random.rotation;
            return;
        }
        //Scan slow if pushing
        _scanner.Rotate(new Vector3(150*Time.deltaTime,0,0));

        RaycastHit hit;
        float d;
        Vector3 cacheForward = _scanner.forward;

        if (Physics.Raycast(transform.position, cacheForward,out hit, 1, Player.Instance.obstacleslayerMask_ForBass)){      

            d = (1 - hit.distance)/1;   // Equals zero to one. One is close, zero is far    
            _speed -= 0.01f*Time.deltaTime;
            if(_speed < 0.1f)
                _speed = 0.1f;
            transform.position -= cacheForward*Time.deltaTime*d*5*2;
            //Tell scanner to rotate slowly
            _scan = false;
        }else{
            //Tell scanner to rotate randomly
            _scan = true;
        }

    }


    public void OnLureKaihsu(){
        if(stateMachine.CurrentState!=null)stateMachine.CurrentState.OnLureKaishu();
    }
    #if UNITY_EDITOR
    void Start(){
        if(isDebugMode){
            BassParameters param=new BassParameters();
            param.size=100.0f;
            param.spawnedPosiion=transform.position;
            param.range=BassRange.Top;
            param.KASSEILEVEL=10;
            param.SURELEVEL=0;
            param.eatType=EatType.Wait;
            param.bate=new Baits[]{Baits.Shrimp};

            Init(param);
        }
    }
    #endif

    void Update() {
        
        if(!isBassActive || bassState==BassState.Result )return;
       
        if(delayEvery>0.0f){
            timeEvery +=Time.deltaTime;
            if(timeEvery>delayEvery){
                timeEvery=0.0f;
                if (stateMachine.CurrentState != null)stateMachine.CurrentState.OnEveryDelayed();
            }
        }
        if(isReachedMovePosition){
            if(delayStill>0.0f){
                timeStill +=Time.deltaTime;
                if(timeStill>delayStill){
                    ResetElapsedTime();
                    if (stateMachine.CurrentState != null)stateMachine.CurrentState.OnStillDelayed();
                    return;
                }
            }
            if (stateMachine.CurrentState != null)stateMachine.CurrentState.StillAnimate();
        }else{
            distance=(transform.position - moveTarget).magnitude;
            if (stateMachine.CurrentState != null)stateMachine.CurrentState.MoveToTarget();
        }
    }


    //ルアーの動きのセンサー
    LureAction prevLureMove=0;


    public LureActionAppeal CheckRureMoveGetAppealled(){
        if(prevLureMove!=LureController.Instance.appeal.moveState){
                switch(prevLureMove){
                case LureAction.still:
                if(LureController.Instance.appeal.moveState==LureAction.moving)
                    return LureActionAppeal.STOP_GO;
                    break;
                case LureAction.moving:
                switch(LureController.Instance.appeal.moveState){
                    case LureAction.still:
                        //go and stop
                        return LureActionAppeal.GO_STOP;
                        break;
                    case LureAction.floating:
                        //cranky dive to float
                        return LureActionAppeal.MOVE_FLOAT;
                        break;
                    case LureAction.sinking:
                        //lift and fall
                        return LureActionAppeal.LIFT_FALL;
                        break;
                    }
                    break;
                case LureAction.floating:
                switch(LureController.Instance.appeal.moveState){
                    case LureAction.still:
                        //crank popper float on suimen
                        return LureActionAppeal.FLOAT_SUIMEN;
                        break;
                    case LureAction.moving:
                        //on suimen move or crank sink
                        return LureActionAppeal.FLOAT_GO;
                        break;
                    }
                    break;
                case LureAction.sinking:
                switch(LureController.Instance.appeal.moveState){
                    case LureAction.still:
                        //to the bottom
                        return LureActionAppeal.SINKING_BTM;
                        break;
                    case LureAction.moving:
                        //fall and lift
                        return LureActionAppeal.SINKING_GO;
                        break;
                    }
                    break;
                }
            prevLureMove=LureController.Instance.appeal.moveState;
            }else{
               
            switch(LureController.Instance.appeal.moveState){
                    case LureAction.still:
                        //crank popper float on suimen
                        return LureActionAppeal.JUST_STILL;
                        break;
                    case LureAction.moving:
                        //on suimen move or crank sink
                        return LureActionAppeal.JUST_MOVE;
                        break;
                    case LureAction.floating:
                        //on suimen move or crank sink
                        return LureActionAppeal.JUST_FLOAT;
                        break;
                    case LureAction.sinking:
                        //on suimen move or crank sink
                        return LureActionAppeal.JUST_SINKING;
                        break;
                }

            }

        return LureActionAppeal.NONE;

    }


    public void AwayFromLure(){
        if(bassState==BassState.Stay){
            //if (stateMachine.CurrentState != null)stateMachine.CurrentState.OnLureTrigger();

            ResetElapsedTime();
            Vector3 pos=GetRandomPositionInTerritory();
            pos.y=_territory_Width[0]-(transform.localScale.x*2.0f);

            GetNewPoint(pos,true,0.0f,0.0f,0.1f,false);  
        }
    }
    public void OnEnterLureTrigger(){
        if(bassState==BassState.Stay){
            if (stateMachine.CurrentState != null)stateMachine.CurrentState.OnLureTrigger();
         }

    }

    /// <summary>
    /// ステート: ステイ
    /// </summary>
    private class stateStay : State<Bass>
    {
        public stateStay(Bass owner) : base(owner) {}
        bool isAttentioned=false;
        bool isUpState=false;
        public override void Enter() {
            
            owner.useObstacleAvoidance=false;
            owner.delayEvery=0.0f;

            owner.SetMinMaxSpeed(Constants.BassBihaviour.bassSpeed_Stay[0],Constants.BassBihaviour.bassSpeed_Stay[1],Constants.BassBihaviour.bassSpeed_Stay[2],Constants.BassBihaviour.bassSpeed_Stay[3]);
            owner.isReachedMovePosition=true;
            //dont use delayStill
            owner.delayStill=GetDelayStill();
            //dont use delatReached
            owner.delayReached=0.0f;
            //set offset to float up down
            owner.positionOffset=owner.GetRandomFloatingPosition();
            owner.howLongToDetectReached=(Constants.BassBihaviour.posOffsetWhenScaleOne*owner.transform.lossyScale.x)/2.0f;
            owner.CheckVisible();
            isAttentioned=false;
            isUpState=false;
            if(owner.timeEvery!=0.0f)owner.timeEvery=0.0f;
            owner.ResetElapsedTime();
            owner.bassState=BassState.Stay;
            Debug.Log("Enter Stay");
        }

        public override void OnEveryDelayed() {
            if(isAttentioned){
                if(isUpState){
                    isUpState=false;

                    isAttentioned=false;

                    owner.ChangeState(BassState.Chase);
                }else{
                    DisableAttention();
                }
            }
        }
        // このステートである間、毎フレーム呼ばれる
        public override void OnStillDelayed() {
            if(isAttentioned){
                owner.positionOffset=PSGameUtils.GetPointAroundPosition(LureController.Instance.gameObject.transform.position,Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne));
                owner.delayStill=GetDelayStill();
            }else{
                if(owner.parameters.GetMoveFrequency_Still()>=Random.value){
                    owner.GetNewPoint(owner.GetRandomPositionInTerritory(),true,Random.Range(0.0f,0.1f),4.0f,0.0f,true);  
                }else{
                    owner.delayStill=GetDelayStill();
                    owner.positionOffset=owner.GetRandomFloatingPosition();
                }
            }
        }
        // このステートである間、毎フレーム呼ばれる
        public override void StillAnimate() {
            if(owner._speed!=0.0f){
                owner._speed=0.0f;
                owner.anime.SetSpeed(owner._speed);
            }
            if(isAttentioned){
                if(LureController.Instance.appeal.moveState!=LureAction.still){
                    //ルアー動いた
                    if(owner.distance>4.0f){
                        //ルアーと離れた場合
                        OnMeExitTerittory();
                        return;
                    }
                    if(owner.prevLureMove!=LureController.Instance.appeal.moveState)owner.prevLureMove=LureController.Instance.appeal.moveState;
                    owner.ResetElapsedTime();
                    owner.GetNewPoint(LureController.Instance.gameObject.transform.position,true,0.0f,0.1f,0.0f,true);  
                    owner.howLongToDetectReached=Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne)*owner.transform.lossyScale.x;
                    return;
                }
                owner.lookTarget=Quaternion.LookRotation(LureController.Instance.gameObject.transform.position-owner.transform.position);
                //owner.lookTarget=Quaternion.Euler(new Vector3(0.0f,owner.transform.rotation.eulerAngles.y,owner.transform.rotation.eulerAngles.z));
            }else{
               
                owner.lookTarget=Quaternion.Euler(new Vector3(0.0f,owner.transform.rotation.eulerAngles.y,owner.transform.rotation.eulerAngles.z));

            }

            owner.transform.rotation=Quaternion.Lerp(owner.transform.rotation,owner.lookTarget,Time.deltaTime);

            if(owner.transform.position.y<-(GameController.Instance.BottomDepth)+owner.posOffset){
                return;
            }

            if(owner.transform.position.y>0.0f-owner.posOffset){
                owner.posOffset-=0.1f;
                //return;
            }
            owner.transform.position=
                Vector3.Lerp(owner.transform.position,owner.positionOffset,Time.deltaTime/5.0f);
        }
        // このステートである間、毎フレーム呼ばれる
        public override void MoveToTarget() {
            
            if(owner.distance < owner.howLongToDetectReached+owner._stuckCounter){

                if(owner.delayReached>0.0f){

                    owner.timeReached+=Time.deltaTime;
                    if(owner.timeReached>owner.delayReached){
                        owner.ResetElapsedTime();
                        OnReachedTarget();
                    }
                }else{
                    OnReachedTarget();
                }
            }else{
                //以下移動
                if(isAttentioned){
                    if(!owner.IsWithinInTerritory(owner.transform)){
                        //自分がトリトリーの外へ出た場合
                        OnMeExitTerittory();
                        return;
                    }
                    if(owner.distance>4.0f){
                        //ルアーと離れた場合
                        OnMeExitTerittory();
                        return;
                    }
                    if( Player.Instance.GetDistanceoPlayer()<Constants.Params.kaishuDistance+0.1f){
                        DisableAttention();
                    }

                    if(LureController.Instance.appeal.moveState==LureAction.still){
                        //rure stopped
                        if(owner.prevLureMove!=LureController.Instance.appeal.moveState)owner.prevLureMove=LureController.Instance.appeal.moveState;
                        owner.delayStill=GetDelayStill();
                        owner._turnSpeed = Random.Range(owner.minTurnSpeed, owner.maxTurnSpeed);
                        owner.positionOffset=PSGameUtils.GetPointAroundPosition(LureController.Instance.gameObject.transform.position,Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne));

                        owner.isReachedMovePosition=true;
                        return;
                    }
                    owner.CheckVisible();
                    //moving
                    owner.timeReached=0.0f;
                    owner._stuckCounter=owner.timeReached;
                    owner.moveTarget=LureController.Instance.gameObject.transform.position;
                    if(owner.distance < owner.howLongToDetectReached*2){

                        if(owner.speedWhenDeccerate==0.0f){
                            owner.speedWhenDeccerate=owner._speed;
                            owner.tParam=-2.0f;
                        }   
                        owner.Move(false);
                    }else{
                        if(owner.tParam<-1.0f){
                            owner.tParam=Mathf.InverseLerp(0.0f, owner._targetSpeed,owner._speed);
                        }
                        owner.speedWhenDeccerate=0.0f;
                        owner.Move(true);
                    }
                }else{

                    owner._stuckCounter+=Time.deltaTime*(owner.howLongToDetectReached*0.25f);
                    owner.Move(true);
                }
            }



        }
        // このステートである間、毎フレーム呼ばれる
        public override void OnReachedTarget() {
            
            if(!isAttentioned) owner.positionOffset=owner.GetRandomFloatingPosition();
            owner.isReachedMovePosition=true;
        }
        public void OnMeExitTerittory() {
            if(!isUpState){
                Debug.LogError("OnMeExitTerittory");
                DisableAttention();
            }else{
                Debug.LogError("チェイスへ");
                isUpState=false;
                isAttentioned=false;
                owner.ChangeState(BassState.Chase);
            }

        }
        public override void OnLureKaishu() {
            DisableAttention();
        }
        void DisableAttention(){
            //アテンションやめた
            Debug.LogError("アテンションやめた");
            Enter();
        }
        public float GetDelayStill(){
            return owner.parameters.GetDelayStill();
        }
        public override void OnLureTrigger() {
         
            if(!isAttentioned){
                Debug.LogError("ルアーみっけ！");
                owner.SetMinMaxSpeed(Constants.BassBihaviour.bassSpeed_Attention[0],Constants.BassBihaviour.bassSpeed_Attention[1],Constants.BassBihaviour.bassSpeed_Attention[2],Constants.BassBihaviour.bassSpeed_Attention[3]);


                owner.howLongToDetectReached=Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne)*owner.transform.lossyScale.x;
                owner.GetNewPoint(LureController.Instance.gameObject.transform.position,true,0.0f,4.0f,0.0f,true);  
                owner.timeEvery =0.0f;
                owner.ResetElapsedTime();
                owner.delayReached =0.5f;

                owner.delayStill=GetDelayStill();

                owner.prevLureMove=LureController.Instance.appeal.moveState;

                //delayEveryで結果を出すので、早くする。
                if(owner.isDebugMode){
					GameController.Instance.BassIsChasing(true,owner.gameObject.transform);
                    isUpState=true;
                }else{
                    
					if(GameController.Instance.currentChasingBass==null && owner.parameters.StateChangeToChase()){
                        isUpState=true;
						GameController.Instance.BassIsChasing(true,owner.gameObject.transform);
                    }else{
                        isUpState=false;
                    }
                }
                if(isUpState){
                    owner.delayEvery =Random.Range(0.1f,0.5f);
                }else{
                    owner.delayEvery =Random.Range(0.5f,2.5f);
                }
                owner.isReachedMovePosition=false;
                isAttentioned=true;
            }
        }
       
        public override void Exit() {

            Debug.LogError("Exit stay");
        }
    }

    /// <summary>
    /// ステート: チェイス
    /// </summary>
    private class stateChase : State<Bass>
    {
        public stateChase(Bass owner) : base(owner) {}

        bool isJustReeling=true;
        float moveFrequency_still=0.0f;
        float chusenFrequency=0.2f;
        float chusenTime=0.0f;

        float byteRitsu=0.0f;
        float ridatsuRitsu=0.0f;
        public override void Enter() {
			GameController.Instance.BassIsChasing(true,owner.gameObject.transform);
            if(owner.timeEvery!=0.0f)owner.timeEvery=0.0f;
            moveFrequency_still=owner.parameters.GetMoveFrequency_Still();
            owner.SetMinMaxSpeed(Constants.BassBihaviour.bassSpeed_Chase[0],Constants.BassBihaviour.bassSpeed_Chase[1],Constants.BassBihaviour.bassSpeed_Chase[2],Constants.BassBihaviour.bassSpeed_Chase[3]);

            owner.prevLureMove=LureController.Instance.appeal.moveState;
            owner.howLongToDetectReached=Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne)*owner.transform.lossyScale.x*Constants.BassBihaviour.DistanceWhenChaseScale;
            //owner.GetNewPoint(LureController.Instance.gameObject.transform.position,true,true,0.0f,4.0f,0.0f,true);  

            owner.GetNewPoint(LureController.Instance.gameObject.transform.position,true,0.0f,4.0f,0.0f,false);  
            owner.ResetElapsedTime();
            owner.delayReached =0.5f;
            chusenTime=0.0f;
            SetChusenKakuritu();
            owner.ResetElapsedTime();
            owner.bassState=BassState.Chase;
            owner.delayStill=0.0f;


        }

        public override void OnEveryDelayed() {
                    Debug.Log("時間切れ ");
                    ExitChase();
        }
        public override void OnLureKaishu() {
            Debug.Log("ルアー回収 ");
            ExitChase();
        }
        void ExitChase(){
            Debug.LogError("チェイスやめ");

            owner.ChangeState(BassState.Back);
        }
        // このステートである間、毎フレーム呼ばれる
        public override void OnStillDelayed() {
            Debug.LogError("OnStillDelayed");
            owner.ResetElapsedTime();
            if(moveFrequency_still>=Random.value){
                owner.ResetElapsedTime();
                owner.delayStill=Random.Range(2.0f,6.0f);
                owner.positionOffset=PSGameUtils.GetPointAroundPosition(LureController.Instance.gameObject.transform.position,Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne));
                ;
            }else{
                owner.delayStill=Random.Range(2.0f,6.0f);
                owner.positionOffset=owner.GetRandomFloatingPosition();
            }

        }
        // ルアーに到着したとき
        public override void StillAnimate() {
           
            chusenTime+=Time.deltaTime;
            if(chusenTime>chusenFrequency){
                chusenTime=0.0f;
                OnChusenBite();
                if(owner.distance>1.5f){
                    if(LureController.Instance.appeal.moveState!=LureAction.still){
                        if(moveFrequency_still>=Random.value){
                        //move rure

                            owner.ResetElapsedTime();
                            owner.delayStill=0.0f;
                            owner.SetMinMaxSpeed(Constants.BassBihaviour.bassSpeed_Chase[0],Constants.BassBihaviour.bassSpeed_Chase[1],Constants.BassBihaviour.bassSpeed_Chase[2],Constants.BassBihaviour.bassSpeed_Chase[3]);

                            //owner.GetNewPoint(LureController.Instance.gameObject.transform.position,true,true,0.0f,0.0f,0.0f,true);  
                            owner.GetNewPoint(LureController.Instance.gameObject.transform.position,true,0.0f,4.0f,0.0f,false);  
                            owner.howLongToDetectReached=Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne)*owner.transform.lossyScale.x*Constants.BassBihaviour.DistanceWhenChaseScale;
                            return;
                        }
                    }
                }
            }


            if(owner._speed!=0.0f){
                owner._speed=0.0f;
                owner.anime.SetSpeed(owner._speed);
            }
            owner.lookTarget=Quaternion.LookRotation(LureController.Instance.gameObject.transform.position-owner.transform.position);
            owner.transform.rotation=Quaternion.Lerp(owner.transform.rotation,owner.lookTarget,Time.deltaTime);

            if(owner.transform.position.y<-(GameController.Instance.BottomDepth)+owner.posOffset){
                return;
            }

            if(owner.transform.position.y>0.0f-owner.posOffset){
                return;
            }
            owner.transform.position=
                Vector3.Lerp(owner.transform.position,owner.positionOffset,Time.deltaTime/5.0f);
        }
        // このステートである間、毎フレーム呼ばれる
        public override void MoveToTarget() {
            
            if( Player.Instance.GetDistanceoPlayer()<Constants.Params.kaishuDistance+0.1f){
                Debug.LogError("釣り人発見！　逃げろ");
                ExitChase();
                return;
            }
            chusenTime+=Time.deltaTime;
            if(chusenTime>chusenFrequency){
                chusenTime=0.0f;
                OnChusenBite();
            }


            if(owner.distance < owner.howLongToDetectReached+owner._stuckCounter){
                    OnReachedTarget();
                    return;
            }
            owner.CheckVisible();
            owner.timeReached=0.0f;
            owner._stuckCounter=owner.timeReached;

            owner.moveTarget=LureController.Instance.gameObject.transform.position;
            if(owner.distance < owner.howLongToDetectReached*2){
                if(owner.speedWhenDeccerate==0.0f){
                    owner.speedWhenDeccerate=owner._speed;
                    owner.tParam=2.0f;

                }
                owner.Move(false);
            }else{
                if(owner.tParam<-1.0f){
                    owner.tParam=Mathf.InverseLerp(0.0f, owner._targetSpeed,owner._speed);
                }
                owner.speedWhenDeccerate=0.0f;
                owner.Move(true);
            }
        }
        // このステートである間、毎フレーム呼ばれる
        public override void OnReachedTarget() {
                owner.SetMinMaxSpeed(Constants.BassBihaviour.bassSpeed_Stay[0],Constants.BassBihaviour.bassSpeed_Stay[1],Constants.BassBihaviour.bassSpeed_Stay[2],Constants.BassBihaviour.bassSpeed_Stay[3]);

                owner.delayStill=1.0f;
                owner.isReachedMovePosition=true;
           
        }
        public void OnChusenBite() {
			if(GameController.Instance.currentChasingBass==owner.gameObject.transform){
                

                lureActionFactor=Constants.BassBihaviour.lureApealFactor[(int)owner.CheckRureMoveGetAppealled()];
                Debug.Log("OnChusenBite"+lureActionFactor);

                if(lureActionFactor<0.1f && LureController.Instance.lureParams.lureParamsData.isShugyozai)lureActionFactor=0.05f;

				if(!GameController.Instance.isPoolMode){
					if(PSGameUtils.Chusen(chusenRitsu*lureActionFactor)){
						Debug.Log("バイトへ");
						owner.ChangeState(BassState.Bite);
					}else{

					}
				}else{
					Debug.Log("プールモードなので、バイトしない");
				}
               


            }else{
                Debug.Log("Other bass is chasing");
                ExitChase();
            }
        }
        float lureActionFactor=0.0f;
        float chusenRitsu=0.0f;

        void SetChusenKakuritu(){
            
            //抽選時間　 なら100回　　確率に当てはめる
            //6.5秒 -20秒
            owner.delayEvery=5.0f+(owner.parameters.KASSEILEVEL*1.5f);
            owner.delayEvery+=Random.Range(0.5f,5.0f);

            //20分割
            chusenFrequency=1.2f;
            chusenFrequency-=owner.parameters.KASSEILEVEL*0.1f;
            if(chusenFrequency<0.2f)chusenFrequency=0.2f;

            //%をベイトマッチとスレから決める。
            int biteRitsu=0;

            //活性とルアーアピールパワー 5-50%
            biteRitsu=owner.parameters.KASSEILEVEL*5;

            //アピールパワー　0-1まで段階　　＋　2-20%を追加
            float num=TackleParams.Instance.tParams.LureApealPower*20.0f;

            //3-30%を追加
            biteRitsu+=GameController.Instance.GetEnvFactor()*3;

            //最大４０％減
             biteRitsu-=owner.parameters.SURELEVEL*4;


            if(owner.parameters.size>TackleParams.Instance.tParams.sizeMatchIn){
                biteRitsu=0;
            }else{
                if(biteRitsu<0) biteRitsu=0;
                if(biteRitsu>99) biteRitsu=99;
            }

            Debug.Log("Bite率"+biteRitsu);

            //5%になるように調整
            chusenRitsu= PSGameUtils.GetChusenRitsuFromKakuritsu(owner.delayEvery/chusenFrequency,(float)biteRitsu);

            //1/50

           


        }

        public override void Exit() {
            Debug.Log("Exit Chase");
        }
    }

    /// <summary>
    /// ステート: バイト
    /// </summary>
    private class stateBite : State<Bass>
    {
        public stateBite(Bass owner) : base(owner) {}

        public override void Enter() {
            Debug.Break();
            owner.isReachedMovePosition=false;
            if(owner.timeEvery!=0.0f)owner.timeEvery=0.0f;
            owner.ResetElapsedTime();
            owner.bassState=BassState.Bite;
			GameController.Instance.BassIsChasing(true,owner.gameObject.transform);
            owner.SetMinMaxSpeed(Constants.BassBihaviour.bassSpeed_Bite[0],Constants.BassBihaviour.bassSpeed_Bite[1],Constants.BassBihaviour.bassSpeed_Bite[2],Constants.BassBihaviour.bassSpeed_Bite[3]);
            owner.howLongToDetectReached=owner.transform.lossyScale.x*1.3f;
            owner.GetNewPoint(LureController.Instance.gameObject.transform.position,true,0.0f,4.0f,0.0f,false);  

            owner.delayReached =0.0f;
            owner.delayEvery=20.0f;
        }

        public override void OnEveryDelayed() {
            Debug.Log("Enter Chase");
            ExitBite();
        }
        void ExitBite(){
            Debug.LogError("バイトやめ");
			GameController.Instance.BassIsChasing(false,null);
            owner.ChangeState(BassState.Back);
        }
        // このステートである間、毎フレーム呼ばれる
        public override void OnStillDelayed() {}
        // このステートである間、毎フレーム呼ばれる
        public override void StillAnimate() {
            if(owner._speed!=0.0f){
                owner._speed=0.0f;
                owner.anime.SetSpeed(owner._speed);
            }
            owner.transform.rotation=Quaternion.Lerp(owner.transform.rotation,owner.lookTarget,Time.deltaTime);

            if(owner.transform.position.y<-(GameController.Instance.BottomDepth)+owner.posOffset){
                return;
            }

            if(owner.transform.position.y>0.0f-owner.posOffset){
                return;
            }
            owner.transform.position=
                Vector3.Lerp(owner.transform.position,owner.positionOffset,Time.deltaTime/5.0f);
        }
        // このステートである間、毎フレーム呼ばれる
        public override void MoveToTarget() {
            if(owner.distance < owner.howLongToDetectReached+owner._stuckCounter){
                if(owner.delayReached>0.0f){
                    owner.timeReached+=Time.deltaTime;
                    if(owner.timeReached>owner.delayReached){
                        owner.ResetElapsedTime();
                        OnReachedTarget();
                        return;
                    }
                }else{
                    OnReachedTarget();
                    return;
                }
            }
            //以下移動
            owner.CheckVisible();
            owner.timeReached=0.0f;
            owner._stuckCounter=owner.timeReached;
            if( Player.Instance.GetDistanceoPlayer()<Constants.Params.kaishuDistance){
                Debug.LogError("very close player ");
                ExitBite();
            }
            owner.moveTarget=LureController.Instance.gameObject.transform.position;
            owner.Move(true);
        }
        public override void OnLureKaishu() {
            ExitBite();

        }
        // このステートである間、毎フレーム呼ばれる
        public override void OnReachedTarget() {
            if(owner.transform.position.y>-(owner.transform.localScale.x*0.7f)){
                AudioController.Play("jabajaba");
				WaterController.Instance.SplashAt(owner.transform.position,owner.transform.localScale.x);
            }
            Debug.Break();
            owner.ChangeState(BassState.Fight);
        }

        public override void Exit() {
            Debug.Log("Enter Chase");
        }
    }


    private class stateBack : State<Bass>
    {
        public stateBack(Bass owner) : base(owner) {}

        public override void Enter() {
            if(owner._model.material!=GameController.Instance.bassMatInWater)owner._model.material=GameController.Instance.bassMatInWater;
            GameController.Instance.BassIsChasing(false,null);
            Debug.Log("Enter Back");
            owner.isReachedMovePosition=false;
            owner.SetMinMaxSpeed(Constants.BassBihaviour.bassSpeed_Back[0],Constants.BassBihaviour.bassSpeed_Back[1],Constants.BassBihaviour.bassSpeed_Back[2],Constants.BassBihaviour.bassSpeed_Back[3]);
            owner.howLongToDetectReached=(Constants.BassBihaviour.posOffsetWhenScaleOne*owner.transform.lossyScale.x)/2.0f;
            owner.delayReached=0.0f;
            owner.delayStill=0.0f;
            owner.delayEvery=50.0f;
            owner.useObstacleAvoidance=false;  
            owner.GetNewPoint( owner.parameters.spawnedPosiion,true,Random.Range(0.3f,0.5f),4.0f,0.0f,true);  
            if(owner.timeEvery!=0.0f)owner.timeEvery=0.0f;
            owner.ResetElapsedTime();

            owner.bassState=BassState.Back;
        }

        public override void OnEveryDelayed() {
            Debug.Log("Enter Back");
            owner.transform.position= owner.parameters.spawnedPosiion;
            owner.ChangeState(BassState.Stay);
        }
        // このステートである間、毎フレーム呼ばれる
        public override void OnStillDelayed() {}
        // このステートである間、毎フレーム呼ばれる
        public override void StillAnimate() {
            if( owner._speed!=0.0f){
                owner._speed=0.0f;
                owner.anime.SetSpeed(owner._speed);
            }
            owner.transform.rotation=Quaternion.Lerp( owner.transform.rotation, owner.lookTarget,Time.deltaTime);

            if( owner.transform.position.y<-(GameController.Instance.BottomDepth)+ owner.posOffset){
                return;
            }

            if( owner.transform.position.y>0.0f- owner.posOffset){
                return;
            }
            owner.transform.position=
                Vector3.Lerp( owner.transform.position, owner.positionOffset,Time.deltaTime/5.0f);
        }
        // このステートである間、毎フレーム呼ばれる
        public override void MoveToTarget() {
            if(owner.distance < owner.howLongToDetectReached+owner._stuckCounter){
                
                    OnReachedTarget();
                return;
            }
            //moving
            owner.CheckVisible();
            if( owner.distance <  owner.howLongToDetectReached*2){

                if(owner.speedWhenDeccerate==0.0f){
                    owner.speedWhenDeccerate=owner._speed;
                    owner.tParam=-2.0f;
                }   
                owner.Move(false);
            }else{
                if(owner.tParam<-1.0f){
                    owner.tParam=Mathf.InverseLerp(0.0f, owner._targetSpeed,owner._speed);
                }
                owner.speedWhenDeccerate=0.0f;
                owner.Move(true);
            }
        }
        // このステートである間、毎フレーム呼ばれる
        public override void OnReachedTarget() {
            owner.isReachedMovePosition=true;
            owner.ChangeState(BassState.Stay);
        }
        public override void OnLureKaishu() {
        }
        public override void Exit() {
            Debug.Log("Exit Back");
        }
    }
    /// <summary>
    /// ステート: ファイト
    /// </summary>
    private class stateFight : State<Bass>
    {
        public stateFight(Bass owner) : base(owner) {}

        public override void Enter() {
            owner._model.material=GameController.Instance.bassMatFight;

            Debug.Log("Enter Fight");
            if(owner.timeEvery!=0.0f)owner.timeEvery=0.0f;
            owner.ResetElapsedTime();
            owner.bassState=BassState.Fight;


        }
        public override void OnLureKaishu() {
        }
        public override void OnEveryDelayed() {
            Debug.Log("Enter Chase");
        }
        // このステートである間、毎フレーム呼ばれる
        public override void OnStillDelayed() {}
        // このステートである間、毎フレーム呼ばれる
        public override void StillAnimate() {}
        // このステートである間、毎フレーム呼ばれる
        public override void MoveToTarget() {
            owner.CheckVisible();
        }
        // このステートである間、毎フレーム呼ばれる
        public override void OnReachedTarget() {}

        public override void Exit() {
            Debug.Log("Enter Chase");
        }
    }

    /// <summary>
    /// ステート: リザルト
    /// </summary>
    private class stateResult : State<Bass>
    {
        public stateResult(Bass owner) : base(owner) {}

        public override void Enter() {
            Debug.Log("Enter Result");
            if(owner.timeEvery!=0.0f)owner.timeEvery=0.0f;
            owner.ResetElapsedTime();
            owner.bassState=BassState.Result;
        }

        public override void OnEveryDelayed() {
            Debug.Log("Enter Chase");
        }
        // このステートである間、毎フレーム呼ばれる
        public override void OnStillDelayed() {}
        // このステートである間、毎フレーム呼ばれる
        public override void StillAnimate() {}
        // このステートである間、毎フレーム呼ばれる
        public override void MoveToTarget() {
        }
        // このステートである間、毎フレーム呼ばれる
        public override void OnReachedTarget() {}
       
        public override void Exit() {
            Debug.Log("Enter Chase");
        }
    }
   

}

public class State<T>
{
    // このステートを利用するインスタンス
    protected T owner;

    public State(T owner)
    {
        this.owner = owner;
    }

    // このステートに遷移する時に一度だけ呼ばれる
    public virtual void Enter() {}
    public virtual void OnLureKaishu() {
    }
    // このステートである間、毎フレーム呼ばれる
    public virtual void OnEveryDelayed() {}
    // このステートである間、毎フレーム呼ばれる
    public virtual void OnStillDelayed() {}
    // このステートである間、毎フレーム呼ばれる
    public virtual void StillAnimate() {}
    // このステートである間、毎フレーム呼ばれる
    public virtual void MoveToTarget() {}
    // このステートである間、毎フレーム呼ばれる
    public virtual void OnReachedTarget() {}
    public virtual void OnLureTrigger() {}
    public virtual void TestEvent() {}
    public virtual void Exit() {}
}

