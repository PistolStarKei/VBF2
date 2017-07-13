using UnityEngine;
using System.Collections;

public class BassAITest : MonoBehaviour {

    public BassState bassState=BassState.Stay;
    public BassRange bassRange=BassRange.Top;
    private Vector3 moveAt;
    public float _speed=0.0f;               //Fish Speed
    private float _turnSpeed=0.0f;                  //Turn speed
    public Transform _model;                //Model with animations
    private float _targetSpeed=0.0f;                    //Fish target speed
    private float tParam =0.0f;                 //
    private float _rotateCounterR=0.0f;             //Used to increase avoidance speed over time
    private float _rotateCounterL=0.0f;             
    public Transform _scanner;              //Scanner object used for push, this rotates to check for collisions
    private bool _scan = true;      //Has this been instantiated
    private static int _updateNextSeed= 0;  //When using frameskip seed will prevent calculations for all fish to be on the same frame
    private int _updateSeed= -1;
    private float _stuckCounter;            //prevents looping around a waypoint
    public float[] _spawnBox_Width=new float[2];
    public float[] _spawnBox_Depth=new float[2];
    public float[] _spawnBox_Height=new float[2];
    public float sizeNanido=0.0f;
  
    // Use this for initialization
    #if UNITY_EDITOR
    static bool _sWarning= true;
    #endif
    public float posOffset=0.0f;
    float posOffsetWhenScaleOne=1.3f;

    void Start(){
        GameController.Instance.BassIsChasing(true,gameObject.transform);

        testDir=Player.Instance.gameObject.transform.forward;
        moveAt=transform.position;

        if(GameController.Instance.isDebugMode){
            Init(100.0f,transform.position,bassRange);
        }

    }
    public bool isJumpKaihi=false;
    public void OnJumpKaihi(){
        if(!isJumpKaihi){
            if(LineScript.Instance.lineSlack>0.0f){
                    isJumpKaihi=true;
            }
        }
    }
    void Jump(){
        isJumpKaihi=false;
        anime.Jump();
        isJumping=true;
    }
    public void OnJumped(){
        Debug.Log("OnJumped");
        AudioController.Play("bassjump");
        WaterController.Instance.SplashAt(new Vector3(transform.position.x,0.0f,transform.position.z),transform.localScale.x);
        isJumping=false;
        if(!isJumpKaihi){
            Debug.Log("Foock Offed");

            if(  JoystickFloat.Instance.foockedPower<(Random.value*50.0f)*sizeNanido){
                Debug.Log("Foock Offed");
                FoockOff();
            }
        }
        if(fightState==FightState.Monkey)SetFightMoveDirection(true);

    }

    public void OnJumpTop(){
        Debug.Log("OnJumpTop");
    }
    Vector3 posCenter=Vector3.zero;
    //kasseiLevel 0.0-1.0 cautiouslevel=0.0f -1.0f
    public void Init(float size,Vector3 pos,BassRange bassRange){

        bassState=BassState.Stay;

        transform.localScale=new Vector3(size*Constants.BassBihaviour.sizeScallingFactor,size*Constants.BassBihaviour.sizeScallingFactor,size*Constants.BassBihaviour.sizeScallingFactor);
        this.bassRange=bassRange;
        transform.position=pos;
        sizeNanido=Equations.EaseInQuad(transform.localScale.x,0.0f,2.0f,1.0f);
        if(bassRange==BassRange.Top){
            _model.localRotation =   Quaternion.Euler(new Vector3(0.0f, 0.0f , 0.0f));
        }else{
            _model.localRotation =   Quaternion.Euler(new Vector3(0.0f, 0.0f , Random.Range(-25.0f, 25.0f)));
        }
        GameObject go=GameObject.FindGameObjectWithTag("FishingController");
        if(go!=null){
        }else{
            Debug.Log("null dontroller!");
        }
        isWithinTargetBounds=true;
        delayStill=2.0f;

        posOffset=transform.localScale.x*posOffsetWhenScaleOne;
        CreateTerritory(bassRange);
        useObstacleAvoidance=false;
        SetMinMaxSpeed(0.01f,0.03f,2.0f,5.0f);
        //dont use delayStill
        //dont use delatReached
        positionOffset=new Vector3(transform.position.x,transform.position.y+Random.Range(-1.0f,1.0f),transform.position.z);
        howLongToDetectReached=(posOffsetWhenScaleOne*transform.lossyScale.x)/2.0f;

    }
    public bool testIsJyosou=true;
    public bool testIsTurning=true;
    public void Test(){
        GameController.Instance.BassIsChasing(true,gameObject.transform);
       
        if(transform.localScale.x>1.0f){
            rodPowerMax=0.8f;
        }else{
            rodPowerMax=0.5f+(0.3f*transform.localScale.x);
        }
        GameController.Instance.ChangeStateTo(GameMode.Fight);
        fightState=FightState.Tukkomi;
        LureController.Instance.gameObject.SetActive(false);
        LureController.Instance.gameObject.transform.parent=rureFoockPosition;
        LureController.Instance.gameObject.transform.localPosition=Vector3.zero;

        isUpState=true;
        //ChangeState(BassState.Fight);
    }
    Vector3 positionOffset=Vector3.zero;

    bool isUpState=false;

  

    void SetMinMaxSpeed(float minSpeed ,float maxSpeed,float minTurnSpeed,float maxTurnSpeed){
        this.minSpeed=minSpeed;
        this.maxSpeed=maxSpeed;
        this.minTurnSpeed=minTurnSpeed;
        this.maxTurnSpeed=maxTurnSpeed;
    }
    public float maxreachDistanceToPlayer=0.0f;
    public float moveFrequency_still=0.005f;
    public bool isAttentioned=false;
    public FightState fightState=FightState.NotFoocked;
    bool isWithinTargetBounds=false;

    void ChangeState(BassState state){
        Debug.Log("ChangeState"+bassState.ToString()+" →"+state.ToString());
        //animator.SetTrigger("bite");
        /*if(LureController.Instance.IsOnSuimen()){
            isWithinTargetBounds=false;
            isUpState=true;
        }else{
            isUpState=false;
            isWithinTargetBounds=true;

        }*/
        GameController.Instance.BassIsChasing(true,gameObject.transform);
        SetMinMaxSpeed(0.05f,0.1f,7.0f,20.0f);
        if(GameController.Instance.isDebugMode){
            isWithinTargetBounds=false;
           
            //ChangeFightState(FightState.Monkey);
            ChangeFightState(FightState.Tukkomi);
        }else{
            if(LureController.Instance.mover.IsOnSuimen()){
                WaterController.Instance.SplashAt(transform.position,transform.localScale.x);
                AudioController.Play("bite");
                isWithinTargetBounds=false;
                ChangeFightState(FightState.Tukkomi);
            }else{

                isWithinTargetBounds=false;
                fightState=FightState.NotFoocked;

            }
            delayReached=2.0f;
        }
        useObstacleAvoidance=true;
        delayEvery=0.0f;
        stamina2=stamina;
       
        delayStill=0.5f;
        if(transform.localScale.x>1.0f){
            rodPowerMax=0.8f;
        }else{
            rodPowerMax=0.5f+(0.3f*transform.localScale.x);
        }
        GameController.Instance.ChangeStateTo(GameMode.Fight);

                LureController.Instance.gameObject.SetActive(false);
                LureController.Instance.gameObject.transform.parent=rureFoockPosition;
                LureController.Instance.gameObject.transform.localPosition=Vector3.zero;
       

       
        if(timeEvery!=0.0f)timeEvery=0.0f;
        ZeroElapsedTime();
        bassState=state;
    }
    void FoockOff(){
        if(LineScript.Instance.lineTention>=0.0f){
            LineScript.Instance.Length+=LineScript.Instance.lineTention+1.0f;
        }
        GameController.Instance.ChangeStateTo(GameMode.Reeling);
        LureController.Instance.gameObject.transform.parent=null;
        LureController.Instance.gameObject.SetActive(true);

        ChangeState(BassState.Back);
    }
    public bool isRippable=false;
    public Transform rureFoockPosition;
    public BassAnimeEvenet anime;
    public float stamina=20.0f;
    float stamina2=0.0f;
    public Vector3 testDir;
    bool isRight=true;
    void Update() {
        if(bassState==BassState.Result ){

            return;

        }



        if(delayEvery>0.0f){
            timeEvery +=Time.deltaTime;
            if(timeEvery>delayEvery){
                timeEvery=0.0f;
                OnEveryDelayed();
            }
        }

        if(isWithinTargetBounds){
            if(delayStill>0.0f){
                timeStill +=Time.deltaTime;
                if(timeStill>delayStill){
                    ZeroElapsedTime();
                    OnStillDelayed();
                }else{
                    StillAnimate();
                }
            }else{

                StillAnimate();
            }

        }else{
            distance=(transform.position - moveAt).magnitude;

            if(bassState==BassState.Chase ){
                MoveToTarget();

            }else if(bassState==BassState.Fight){
                CheckRippable();
                if(isJumping){

                }

                if(fightState==FightState.NotFoocked){
                   
                    if(GameController.Instance.isShowedFightToUser){
                            ChangeFightState(FightState.Tukkomi);
                        }else{
                        
                            if( RodController.Instance.bendingPower>0.2f){
                                ChangeFightState(FightState.Tukkomi);
                            }
                        }
                    StillAnimate();
                }else if(fightState==FightState.Dragged){
                   


                    if(ShipControls.Instance.gameObject.transform.InverseTransformPoint(transform.position).z<1.0f){
                        //close to player
                        bassState=BassState.Result;
                        GameController.Instance.ChangeStateTo(GameMode.Result);

                    }
                    if(isJumping)return;
                    

                    if(RodController.Instance.bend.angle>0.3f){

                        if(transform.position.y>0.0f-posOffset){
                            dragDirection=  new Vector3(RodController.Instance.transform.position.x,-posOffset,RodController.Instance.transform.position.z)-transform.position;
                            dragDirection=dragDirection/dragDirection.magnitude;
                            if(delayReached>0.0f){

                                timeReached+=Time.deltaTime;
                                if(timeReached>delayReached){
                                    ZeroElapsedTime();
                                    if(Random.value<=0.4f){
                                        if(!isJumping){
                                            Debug.Log("SwimReturnBack"+delayReached);
                                            if(Random.value<=testTairyokubar.value/2.0f){
                                                Jump();

                                            }else{
                                                anime.OneShotAnime("SwimReturnBack");
                                            }
                                        }else{
                                            anime.OneShotAnime("SwimReturnBack");
                                        }
                                    }

                                }
                            }
                            //jump chusen
                        }else{
                            dragDirection=RodController.Instance.transform.position-transform.position;
                            dragDirection=dragDirection/dragDirection.magnitude;
                            if(delayReached>0.0f){

                                timeReached+=Time.deltaTime;
                                if(timeReached>delayReached){
                                    ZeroElapsedTime();
                                    if(Random.value<=0.4f){
                                        Debug.Log("SwimReturnBack"+delayReached);

                                        anime.OneShotAnime("SwimReturnBack");
                                    }

                                }
                            }
                        }

                        _speed= Equations.EaseInSine( RodController.Instance.bendingPower,0.0f,dragPower,rodPowerMax);
                        if( RodController.Instance.bendingPower>0.4f){
                            dragged=true;
                            anime.StartKubifuri(true);
                        }else{
                            anime.StartKubifuri(false);
                        }
                        transform.position += dragDirection*_speed;

                        transform.rotation= Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(RodController.Instance.transform.position), Time.deltaTime * _speed*300.0f);

                    }else{

                        if(dragged){
                            //dragged
                            dragged=false;
                            //set as tairyoku
                            timeEvery=0.0f;
                            if(testTairyokubar.value<=0.2f){
                                delayEvery=Random.Range(2.0f,4.0f);
                            }else if(testTairyokubar.value>0.2f && testTairyokubar.value<=0.5f){
                                delayEvery=Random.Range(1.0f,3.0f);
                            }else if(testTairyokubar.value>0.5f && testTairyokubar.value<=0.8f){
                                delayEvery=Random.Range(0.2f,1.0f);
                            }else{
                                delayEvery=0.0f;
                            }
                            fightState=FightState.Stay;
                            //positionOffset=new Vector3(transform.position.x,transform.position.y+Random.Range(-1.0f,1.0f),transform.position.z);
                            positionOffset=new Vector3(transform.position.x,transform.position.y+1.0f,transform.position.z);
                            anime.StartKubifuri(true);
                        }
                        if(delayReached>0.0f){

                            timeReached+=Time.deltaTime;
                            if(timeReached>delayReached){
                                ZeroElapsedTime();
                                if(Random.value<=0.4f){
                                        anime.OneShotAnime("KubifuriTrigger");

                                }

                            }
                        }
                        StillAnimate();
                    }
                }else if(fightState==FightState.Tukkomi){
                    if(isJumping)return;
                    if(delayReached>0.0f){

                        timeReached+=Time.deltaTime;
                        if(timeReached>delayReached){
                            if(dragged){
                                Debug.LogError("Dragged false");
                                dragged=false;
                                anime.StartKubifuri(false);
                                if(LineScript.Instance.lineSlack<0.0f){
                                    if(JoystickFloat.Instance.foockedPower<(Random.value*30.0f)*sizeNanido){
                                        Debug.Log("Foock Offed");
                                        FoockOff();
                                    }
                                }
                            }
                            ZeroElapsedTime();

                            if( RodController.Instance.bendingPower>=rodPowerMax)if(Random.value<=0.2f)anime.OneShotAnime("SwimReturnBack");

                        }

                    }
                    Debug.DrawLine(transform.position,transform.position+(moveAt*10.0f));

                    if(dragged){
                        if( RodController.Instance.bendingPower<backMax){
                            dragged=false;
                            anime.StartKubifuri(false);
                        }
                        if(transform.position.y<0.0f-posOffset){
                            transform.position +=  moveAt*-0.001f;
                        }
                       
                        transform.rotation=Quaternion.Lerp(transform.rotation,lookTarget,Time.deltaTime*2.0f);
                        return;
                    }
                    lookTarget=Quaternion.LookRotation(transform.position+(2.0f*moveAt)-transform.position);

                    transform.rotation=Quaternion.Lerp(transform.rotation,lookTarget,Time.deltaTime*10.0f);
                    if(transform.position.y<-(GameController.Instance.BottomDepth)+posOffset){
                        
                        moveAt=Player.Instance.gameObject.transform.transform.forward;
                    }
                    if( RodController.Instance.bendingPower>0.0f){
                        if( RodController.Instance.bendingPower>=rodPowerMax){
                            if(!dragged){
                                timeFunbari+=Time.deltaTime;
                                if(timeFunbari>timeFunbariTime){
                                    ZeroElapsedTime();
                                    //ikou to 
                                    timeFunbari=0.0f;
                                    if(delayEvery==0.0f){
                                        if(testTairyokubar.value<=0.2f){
                                            delayEvery=Random.Range(1.0f,2.5f);
                                        }else if(testTairyokubar.value>0.2f && testTairyokubar.value<=0.5f){
                                            delayEvery=Random.Range(2.5f,4.0f);
                                        }else if(testTairyokubar.value>0.5f && testTairyokubar.value<=0.8f){
                                            delayEvery=Random.Range(4.0f,6.0f);
                                        }else{
                                            delayEvery=Random.Range(6.0f,10.0f);
                                        }
                                    }
                                    timeFunbariTime=Random.Range(0.5f,1.0f);
                                    delayReached=5.0f;
                                    backMax=Random.Range(0.1f,0.5f);
                                    _speed=0.0f;
                                    lookTarget=Quaternion.LookRotation(transform.position+(2.0f*(-Player.Instance.gameObject.transform.transform.right))-transform.position);
                                    tParam=0.0f;
                                    dragged=true; 
                                }

                                anime.StartKubifuri(true);
                            }
                        
                        }else{
                            if(tParam<1.0f)SetSpeed(true);
                            float gensan=_targetSpeed* RodController.Instance.bendingPower;
                            if(_speed>gensan)_speed=gensan;

                            //_speed=_targetSpeed-Equations.EaseInSine(fishingController.rodController.bendingPower,0.0f,_targetSpeed,rodPowerMax);
                            transform.position +=  moveAt*_speed;
                        }
                    }else{

                        SetSpeed(true);
                        transform.position +=  moveAt*_speed;
                    }
                  

                }else if(fightState==FightState.Stay){
                    if(isJumping)return;
                    if(delayReached>0.0f){

                        timeReached+=Time.deltaTime;
                        if(timeReached>delayReached){
                            ZeroElapsedTime();
                            if(transform.position.y>0.0f-posOffset){
                                if(!isJumping){
                                    if(Random.value<=testTairyokubar.value/2.0f){
                                        Jump();
                                            
                                        
                                    }
                                }
                            }
                            positionOffset=new Vector3(transform.position.x,transform.position.y+1.0f,transform.position.z);
                        }

                    }

                    if(_speed!=0.0f){
                        _speed=0.0f;
                        anime.SetSpeed(_speed);
                    }
                    if(transform.position.y>0.0f-posOffset){
                        Debug.Log("suimen return");
                        return;
                    }
                    Debug.Log("floating");
                    transform.position=
                        Vector3.Lerp(transform.position,positionOffset,Time.deltaTime/3.0f);
                }else if(fightState==FightState.Monkey){
                    if(isJumping)return;
                    if(delayReached>0.0f){

                        timeReached+=Time.deltaTime;
                        if(timeReached>delayReached){
                            Debug.Log("Monkey Reach Delayed");
                            ZeroElapsedTime();
                            SetFightMoveDirection(true);
                            anime.StartKubifuri(false);
                            if(LineScript.Instance.lineSlack<0.0f){
                                if(JoystickFloat.Instance.foockedPower<(Random.value*30.0f)*sizeNanido){
                                    Debug.Log("Foock Offed");
                                    FoockOff();
                                }
                            }
                        }

                    }
                    if(IsAvoidanceForward()){
                        // set new directin

                        ZeroElapsedTime();
                        anime.StartKubifuri(false);
                        SetFightMoveDirection(true);
                        return;
                    }


                        lookTarget=Quaternion.LookRotation(transform.position+(2.0f*moveAt)-transform.position);
                        transform.rotation=Quaternion.Lerp(transform.rotation,lookTarget,Time.deltaTime* _turnSpeed);
                    Debug.DrawLine(transform.position,transform.position+(moveAt*10.0f));

                    if( RodController.Instance.bendingPower>0.0f){
                        if( RodController.Instance.bendingPower>=rodPowerMax){
                            Debug.Log("Tention is Max ");
                            _speed=-0.01f;

                            transform.position +=  transform.TransformDirection(Vector3.forward)*_speed;
                            anime.StartKubifuri(true);
                            if(delayReached>1.0f)delayReached=1.0f;
                        }else{
                            Debug.Log("Tentioned");

                            _speed=_targetSpeed-Equations.EaseInSine( RodController.Instance.bendingPower,0.0f,_targetSpeed,rodPowerMax);
                            transform.position +=  transform.TransformDirection(Vector3.forward)*_speed;
                        }


                    }else{
                        SetSpeed(true);
                        anime.StartKubifuri(false);
                        transform.position +=  transform.TransformDirection(Vector3.forward)*_speed;
                    }
                }
                testTairyokubar.value=stamina2/stamina;
                //MoveToTarget();
            }else{

                if(distance < howLongToDetectReached+_stuckCounter){

                    if(delayReached>0.0f){

                        timeReached+=Time.deltaTime;
                        if(timeReached>delayReached){
                            ZeroElapsedTime();
                            OnReachedWithinBounds();
                        }
                    }else{
                        OnReachedWithinBounds();
                    }
                }else{
                    MoveToTarget();
                }
            }
        }
       
    }

    public void AwayFromLure(){
        if(bassState==BassState.Stay){

            GetNewPoint(new Vector3(Random.Range(_spawnBox_Width[0] ,_spawnBox_Width[1]),transform.position.y-(transform.localScale.x*2.0f),Random.Range(_spawnBox_Depth[0],_spawnBox_Depth[1])),true,true,Random.Range(0.0f,0.1f),4.0f,1.0f);  
        }
    }
    int isPlayerRight(){
        float num=Player.Instance.gameObject.transform.transform.InverseTransformPoint(transform.position).x;

        if(Mathf.Abs(num)<2.0f){

            return 0;
        }
        if(num>0.0f){
            return 1;
        }else{
            return -1;
        }
    }
    void SetFightMoveDirection(bool isMonkey){
      
        if(isMonkey){
            dragged=false;
           
            Debug.Log("SetFightMoveDirection");
           
            moveAt=GenerateRandomVector(GetMobableDirection());

            rodPowerMax=1.0f;

        }else{
            dragged=false;
            delayReached=1.0f;
            tParam=0.0f;
            moveAt=(Player.Instance.gameObject.transform.transform.forward*Random.Range(0.5f,70.0f))+(-Player.Instance.gameObject.transform.transform.up);
            MovingDirForHantei.z=1;
            MovingDirForHantei.y=-1;
            _turnSpeed=Random.Range(minTurnSpeed, maxTurnSpeed);
            SetSpeedByTairyoku();
            rodPowerMax=1.0f;
        }




    }
    float backMax=0.0f;
    void CheckRippable(){
        float angleFactor=ClampAngle(transform.eulerAngles.x,0.0f,360.0f);
        if(angleFactor>90.0f) angleFactor=360.0f-angleFactor;

        float y=(transform.position.y+_model.transform.localPosition.y);
        if(y+(angleFactor*(posOffset/90.0f))>0.0f-posOffset/2.5f && y<0.7f+(angleFactor*(posOffset/90.0f))){

                isRippable=true;
            
        }else{
            isRippable=false;
        }

    }
    public UISlider testTairyokubar;
    bool dragged=false;
    public bool isJumping=false;
    float distance=0.0f;
    float dragPower=0.02f;
    public float rodPowerMax=0.0f;
    Vector3 dragDirection;
    void ZeroElapsedTime(){
        timeReached=0.0f;
        timeStill=0.0f;
    }
    float timeStill =0.0f;
    float timeReached =0.0f;
    float timeEvery =0.0f;
    float delayStill =0.0f;
    float delayReached =0.0f;
    float delayEvery =0.0f;
    void OnEveryDelayed(){


        return;
        if(bassState==BassState.Fight){
            if(fightState==FightState.Stay){
                //
                if(testTairyokubar.value<=0.2f){
                    if(Random.value<=0.8f){
                        ChangeFightState(FightState.Dragged);
                    }else if(Random.value>0.8f && Random.value<=0.9f){
                        ChangeFightState(FightState.Monkey);
                    }else{
                        ChangeFightState(FightState.Tukkomi);
                    }
                }else if(testTairyokubar.value>0.2f && testTairyokubar.value<=0.5f){
                    if(Random.value<=0.3f){
                        ChangeFightState(FightState.Monkey);
                    }else{
                        ChangeFightState(FightState.Tukkomi);
                    }
                }else if(testTairyokubar.value>0.5f && testTairyokubar.value<=0.8f){
                    if(Random.value<=0.5f){
                        ChangeFightState(FightState.Monkey);
                    }else{
                        ChangeFightState(FightState.Tukkomi);
                    }
                }else{
                    ChangeFightState(FightState.Tukkomi);

                }


            }else if(fightState==FightState.Tukkomi){
                if(testTairyokubar.value<=0.2f){
                    if(Random.value<=0.8f){
                    }else if(Random.value>0.8f && Random.value<=0.9f){

                    }else{
                    }
                }else if(testTairyokubar.value>0.2f && testTairyokubar.value<=0.5f){
                    if(Random.value<=0.3f){
                    }else{

                    }
                }else if(testTairyokubar.value>0.5f && testTairyokubar.value<=0.8f){
                    if(Random.value<=0.5f){
                    }else{

                    }
                }else{
                }

            }else if(fightState==FightState.Monkey){
             
                if(testTairyokubar.value<=0.2f){
                    if(Random.value<=0.8f){
                    }else if(Random.value>0.8f && Random.value<=0.9f){

                    }else{
                    }
                }else if(testTairyokubar.value>0.2f && testTairyokubar.value<=0.5f){
                    if(Random.value<=0.3f){
                    }else{

                    }
                }else if(testTairyokubar.value>0.5f && testTairyokubar.value<=0.8f){
                    if(Random.value<=0.5f){
                    }else{

                    }
                }else{
                }

            }
        }
    }
    void ChangeFightState(FightState newstate){
        ZeroElapsedTime();
        anime.StartKubifuri(false);
        if(newstate==FightState.Dragged){
            anime.SetSpeed(0.0f);
            timeReached=0.0f;
            delayEvery=0.0f;
            dragged=false;
            stamina2-=2.0f;
        }else if(newstate==FightState.Monkey){
            anime.SetSpeed(0.0f);
            timeReached=1.0f;
                tParam=1.0f;
            dragged=false;

            //more tairoku  more time
            delayEvery=30.0f;
            dragged=false;
            stamina2-=1.0f;
            SetFightMoveDirection(true);
            fightState=FightState.Monkey;
        }else if(newstate==FightState.Stay){

        }else if(newstate==FightState.Tukkomi){
            anime.SetSpeed(0.0f);
            timeFunbariTime=Random.Range(0.5f,1.0f);
            timeFunbari=0.0f;
            anime.SetSpeed(maxSpeed);
            timeReached=0.0f;
            delayEvery=0.0f;
            tParam=0.0f;
            dragged=false;
            stamina2-=2.0f;
            SetFightMoveDirection(false);
            fightState=FightState.Tukkomi;
        }
        fightState=newstate;
    }
    float timeFunbari=0.0f;
    float timeFunbariTime=0.0f;
    public float minDistanceWhenChaseScaleOne=4.0f;
    public float maxDistanceWhenChaseScaleOne=6.0f;
    void OnStillDelayed(){
        Debug.Log("OnStillDelayed"+delayStill);
        //set still delay!
        positionOffset=new Vector3(transform.position.x,transform.position.y+Random.Range(-1.0f,1.0f),transform.position.z);
        ZeroElapsedTime();


    }
    Quaternion lookTarget=Quaternion.identity;


    //0 still 1 rure is moving 2 floating 3 sinking 
    int prevMove=0;
    bool isJustReeling=true;
    float timeForBiteChusen=2.0f;
    float keikaTime=0.0f;
    float chusenRituOnTadaMaki=0.0f;
    int reveallBiteNum=0;


    float stillTimeMax=0.0f;
    float timeBassIsBoring=5.0f;
    void StillAnimate(){
        if(_speed!=0.0f){
            _speed=0.0f;
            anime.SetSpeed(_speed);
        }
        transform.rotation=Quaternion.Lerp(transform.rotation,lookTarget,Time.deltaTime);

       
        if(transform.position.y<-(GameController.Instance.BottomDepth)+posOffset){
            return;
        }
        if(transform.position.y>0.0f-posOffset){
            return;
        }
        transform.position=
            Vector3.Lerp(transform.position,positionOffset,Time.deltaTime/3.0f);

    }

    void MoveToTarget(){
        if( Player.Instance.GetDistanceoPlayer()<maxreachDistanceToPlayer){
            Debug.LogError("very close player ");
        }
        _stuckCounter+=Time.deltaTime*(howLongToDetectReached*0.25f);
        Move(true);

    }
    void Move(bool isAccerate){
        //look target and adjust rotation
        SetRotationToWayPoint();
        //move forword
        SetSpeed(isAccerate);
        ForwardMovement();

    }
    void OnReachedWithinBounds(){
        Debug.Log("OnReachedWithinBounds");

        isWithinTargetBounds=true;
    }
    bool useObstacleAvoidance=true;

    void SetSpeedByTairyoku(){
        if(testTairyokubar.value<=0.2f){
            _targetSpeed=Random.Range(minSpeed, minSpeed*1.2f);
        }else if(testTairyokubar.value>0.2f && testTairyokubar.value<=0.5f){
            _targetSpeed=Random.Range(minSpeed, maxSpeed);
        }else if(testTairyokubar.value>0.5f && testTairyokubar.value<=0.8f){
            _targetSpeed=Random.Range(maxSpeed*0.7f, maxSpeed);
        }else{
            _targetSpeed=Random.Range(maxSpeed*0.9f, maxSpeed);
        }
        Debug.LogError("targetSpeed="+_targetSpeed);
    }
    Vector3 MovingDirForHantei=Vector3.zero;
    Vector3 GenerateRandomVector(bool[] movable){

        //Debug.Log("GenerateRandomVector : ");
        Vector3 dir=Vector3.zero;
        string str="";
        tParam=0.0f;
        MovingDirForHantei=Vector3.zero;
        bool canJump=false;
        if(!movable[0]){
            //no down

            // Debug.Log("GenerateRandomVector :no dowb ");
            if(Random.value<=0.5f){
                MovingDirForHantei.y=1;
                str+="up :";
                dir+=Player.Instance.gameObject.transform.transform.up*Random.value;
            }

        }else{
            if(!movable[1]){
                //no up
                //str+="no up";
                //Debug.Log("GenerateRandomVector : no up");
                if(Random.value<=0.5f){
                    MovingDirForHantei.y=-1;
                    //Debug.Log("-up");
                    str+="-up : ";
                    dir+=-Player.Instance.gameObject.transform.transform.up*Random.value;
                }else{
                    //jumP?
                        canJump=true;

                }
            }else{
                if(Random.value<=0.5f){
                    //Debug.Log("+up");
                    MovingDirForHantei.y=1;
                    str+="up : ";
                    dir+=Player.Instance.gameObject.transform.transform.up*Random.value;
                }else{
                    //Debug.Log("-up");
                    MovingDirForHantei.y=-1;
                    str+="-up : ";
                    dir+=-Player.Instance.gameObject.transform.transform.up*Random.value;
                }
            }
        }
        if(!movable[2]){
            //no forwads
            // str+="no forwars";
            //Debug.Log("GenerateRandomVector : no forward");
            /*if(Random.value<=0.1f){
                // Debug.Log("-forward");
                    str+="-forward : ";
                delayReached=Random.Range(0.5f,1.0f);
                dir+=-Player.Instance.gameObject.transform.transform.forward*Random.value;
                _targetSpeed=minSpeed;

            }else{*/
                delayReached=Random.Range(5.0f,10.0f);
                SetSpeedByTairyoku();
            //}
        }else{
            if(Random.value<=0.33f){
                // Debug.Log("+forward");
                MovingDirForHantei.z=1;
                str+="forward : ";
                delayReached=Random.Range(5.0f,10.0f);
                dir+=Player.Instance.gameObject.transform.transform.forward*Random.value;
                SetSpeedByTairyoku();
            }else{
                /* if(Random.value<=0.1f){
                        str+="-forward : ";
                    delayReached=Random.Range(0.5f,1.0f);
                    _targetSpeed=minSpeed;

                    dir+=-Player.Instance.gameObject.transform.transform.forward*Random.value;

                    //Debug.Log("-forward");
                }else{*/
                    delayReached=Random.Range(5.0f,10.0f);
                    SetSpeedByTairyoku();
                // }
            }
        }

        _turnSpeed=Random.Range(minTurnSpeed, maxTurnSpeed);

        if(dir==Vector3.zero){
            delayReached=20.0f;

            SetSpeedByTairyoku();
            //Debug.Log("GenerateRandomVector :go open");

            if(!movable[3]){
                //no left
                //str+="no left";
                //Debug.Log("no left");
                MovingDirForHantei.x=1;
                str+="right : ";
                canJump=false;
                dir=Vector3.zero;
                dir+=Player.Instance.gameObject.transform.transform.right*Random.value;

            }else{
                if(!movable[4]){
                    //no left
                    // str+="no right";
                    // Debug.Log("no right");
                    MovingDirForHantei.x=-1;
                    canJump=false;
                    dir=Vector3.zero;
                    dir+=-Player.Instance.gameObject.transform.transform.right*Random.value;
                    //Debug.Log("-right");
                    str+="-right : ";

                }else{
                    if(Random.value<=0.5f){
                        // Debug.Log("+right");
                        MovingDirForHantei.x=1;
                        str+="right : ";
                        dir+=Player.Instance.gameObject.transform.transform.right*Random.value;
                    }else{
                        //Debug.Log("-right");
                        MovingDirForHantei.x=-1;
                        str+="-right : ";
                        dir+=-Player.Instance.gameObject.transform.transform.right*Random.value;

                    }
                }
            }
        }else{

           
            if(!movable[3]){
                //no left
                //str+="no left";
                canJump=false;
                if(Random.value<=0.5f){
                    // Debug.Log("+right");
                    MovingDirForHantei.x=1;
                    str+="right : ";
                    dir+=Player.Instance.gameObject.transform.transform.right*Random.value;
                }
            }else{
                if(!movable[4]){
                    //no left
                    //str+="no right";
                        canJump=false;
                        if(Random.value<=0.5f){
                            str+="-right : ";
                        MovingDirForHantei.x=-1;
                        dir+=-Player.Instance.gameObject.transform.transform.right*Random.value;
                            //Debug.Log("-right");
                        }

                }else{
                    if(Random.value<=0.33f){
                        // Debug.Log("+right");
                            canJump=false;
                            if(Random.value<=0.5f){
                            dir+=Player.Instance.gameObject.transform.transform.right*Random.value;
                                str+="right : ";
                            MovingDirForHantei.x=1;
                                //Debug.Log("-right");
                            }


                    }else{
                        if(Random.value<=0.5f){
                            //Debug.Log("-right");
                            MovingDirForHantei.x=-1;
                            dir+=-Player.Instance.gameObject.transform.transform.right*Random.value;
                            str+="-right : ";
                        }
                    }
                }
            }
        }

        if(canJump){
            if(Random.value<=0.3f)Jump();
        }
        if(str!="")Debug.Log(str);
        // Debug.Log(dir.ToString());
        return dir;

    }
    bool[] directions=new bool[6];
    //Returns true if there is an obstacle in the way
    bool[] GetMobableDirection () {
        RaycastHit hit ;

        transform.rotation=Quaternion.Euler(new Vector3(0.0f,transform.eulerAngles.y,transform.eulerAngles.z));
        Vector3 cacheForward = Player.Instance.gameObject.transform.transform.forward;
        Vector3 cacheRight = Player.Instance.gameObject.transform.transform.right;
        directions[0]=true;
        directions[1]=true;
        directions[2]=true;
        directions[3]=true;
        directions[4]=true;
        //down up 
        string str="";
        string str2="";
        if (transform.position.y<-(GameController.Instance.BottomDepth)+posOffset){          
            str2+=" : near bottom";
            directions[0]=false;
        }else{
            if(Player.Instance.gameObject.transform.transform.InverseTransformDirection(moveAt).y<0.0f){
                if( RodController.Instance.bendingPower>=rodPowerMax*0.7f){
                    str2+=" : almost near bottom but tention";
                    directions[0]=false; 
                }else{

                    directions[0]=true;
                }
            }else{
               
                directions[0]=true;
            }
        }
        if (transform.position.y>0.0f-posOffset){
            str2+=" : near suimen";
            directions[1]=false;  
        }else{

            directions[1]=true;
        }

        //Crash avoidance //Checks for obstacles forward
        if (Physics.Raycast(transform.position, cacheForward,out hit, posOffset*2.0f, Player.Instance.obstacleslayerMask_ForBass)){
            str2+=" : terrain forward";
            directions[2]=false;  
        }else{
            if( RodController.Instance.bendingPower>=rodPowerMax*0.7f){
                str2+=" : not terrain forward but tention";
                directions[2]=false; 
            }else{

                directions[2]=true;
            }
        }

        if (Physics.Raycast(transform.position,-cacheRight,out hit,posOffset*2.0f, Player.Instance.obstacleslayerMask_ForBass)){
            str2+=" : terrain left";
            directions[3]=false;        
        }else{

            directions[3]=true; 
        }
        if (Physics.Raycast(transform.position,cacheRight,out hit, posOffset*2.0f, Player.Instance.obstacleslayerMask_ForBass)){
            str2+=" : terrain right";
            directions[4]=false;
        }else{

            directions[4]=true;
        }
        float num=Player.Instance.gameObject.transform.transform.InverseTransformDirection(moveAt).x;
        float num2=ShipControls.Instance.gameObject.transform.InverseTransformPoint(transform.position).x;

        if(num2<0.0f){
            //bass is left
            if(Mathf.Abs(num2)>2.0f*0.8f){
                str2+=" : left limit";
                directions[3]=false;   
            }else{
                if(num<0.0f){
                    //moving left

                    if( RodController.Instance.bendingPower>=rodPowerMax*0.7f){
                        str2+=" : not left limit but tention";
                            directions[3]=false; 
                        }else{

                            directions[3]=true;  
                        }

                }
            }
        }else{
            //bass is right
            if(num2>2.0f*0.8f){
                str2+=" : right limit";
                directions[4]=false;   
            }else{
                if(num>0.0f){
                    //moving right
                    if( RodController.Instance.bendingPower>=rodPowerMax*0.7f){
                        str2+=" : not right limit but tention";
                            directions[4]=false; 
                        }else{
                            directions[4]=true;  
                        }
                    
                }
            }
        }

        if(directions[0])str+="↓";
        if(directions[1])str+="↑";
        if(directions[2])str+="⇧";
        if(directions[3])str+="←";
        if(directions[4])str+="→";
        Debug.Log("Movable=="+str);

        return directions;                                                                                                                                                                                                                                                                                                           
    }
    //Returns true if there is an obstacle in the way
    bool IsAvoidanceForward () {
        RaycastHit hit ;
        //Up / Down avoidance -10 -9
        if(Player.Instance.gameObject.transform.transform.InverseTransformDirection(moveAt).y<0.0f){
            //moving down
            if (transform.position.y<-(GameController.Instance.BottomDepth)+posOffset){          
                //if downWard
                Debug.Log("Found Avaidance bottom down move");
                return true;
            }
        }else if(Player.Instance.gameObject.transform.transform.InverseTransformDirection(moveAt).y>0.0f){
            //moving up
            if (transform.position.y>0.0f-posOffset){
                //if upward
                Debug.Log("Found Avaidance suimen up move");
                return true;
            }
        } //if upward

        float num=Player.Instance.gameObject.transform.transform.InverseTransformDirection(moveAt).x;
        float num2=ShipControls.Instance.gameObject.transform.InverseTransformPoint(transform.position).x;

        if(num2<0.0f){
            //bass is left
            if(num<0.0f){
                //moving left
                if(Mathf.Abs(num2)>2.0f){
                    Debug.Log("Found Avaidance left limit left move");
                    return true;
                }
            }
        }else{
            //bass is right
            if(num>0.0f){
                //moving right
                if(num2>2.0f){
                    Debug.Log("Found Avaidance forward rught kimit right move");
                    return true;
                }
            }
        }
            
       

        //Crash avoidance //Checks for obstacles forward
        if (Physics.Raycast(transform.position, moveAt,out hit,posOffset, Player.Instance.obstacleslayerMask_ForBass)){
            Debug.Log("Found Avaidance forward terrain");
            return true;
        }
        return false;                                                                                                                                                                                                                                                                                                           
    }
    //Returns true if there is an obstacle in the way
    bool Avoidance () {
        if(!useObstacleAvoidance)return false;
        RaycastHit hit ;
        float d;
        Quaternion  rx = transform.rotation;
        Vector3 ex = transform.rotation.eulerAngles;
        Vector3 cacheForward = transform.forward;
        Vector3 cacheRight = transform.right;
        //Up / Down avoidance -10 -9
        if (transform.position.y<-(GameController.Instance.BottomDepth)+posOffset){          

           
            d = (posOffset -  Mathf.Abs(transform.position.y-(-(GameController.Instance.BottomDepth)+posOffset)))/posOffset;
            ex.x -= _avoidSpeed*d*Time.deltaTime*(_speed +1);
            rx.eulerAngles = ex;
            transform.rotation = rx;
        }
        if (transform.position.y>0.0f-posOffset){

            d = (posOffset - Mathf.Abs(transform.position.y))/posOffset; 
            Debug.Log("SUIMEN!!!!"+d);
            ex.x += _avoidSpeed*d*Time.deltaTime*(_speed +1);  

            rx.eulerAngles = ex;
            transform.rotation = rx;    
        }

        //Crash avoidance //Checks for obstacles forward
        if (Physics.Raycast(transform.position, cacheForward+(cacheRight*Random.Range(-0.1f, 0.1f)),out hit, _stopDistance, Player.Instance.obstacleslayerMask_ForBass)){
            Debug.Log("forward!!!!");
            d = (_stopDistance - hit.distance)/_stopDistance;               
            ex.y -= _avoidSpeed*d*Time.deltaTime*(_targetSpeed +3);
            rx.eulerAngles = ex;
            transform.rotation = rx;

            return true;
        }else if (Physics.Raycast(transform.position, cacheForward+(cacheRight*(_avoidAngle+_rotateCounterL)),out hit, _avoidDistance, Player.Instance.obstacleslayerMask_ForBass)){
            Debug.Log("left!!!!");
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
            Debug.Log("right!!!!");
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
    public float _stopDistance= 0.5f;       //How close this can be to objects directly in front of it before stopping and backing up. This will also rotate slightly, to avoid "robotic" behaviour

    void SetSpeed(bool isAccerarate){
        if(isAccerarate){
            //slow to target speed
            if (tParam < 1) {
                //_targetSpeed=minSpeed MxSpeed
                    if(_speed > _targetSpeed){
                        tParam += Time.deltaTime * 0.025f;
                    }else{
                        tParam += Time.deltaTime * 0.01f;       
                    }

                //make it to target

            }
            _speed = Mathf.Lerp(_speed, _targetSpeed,tParam);   
        }else{

            tParamDec=Mathf.Clamp01((distance-howLongToDetectReached)/howLongToDetectReached);
            _speed = Mathf.Lerp(0.0f,speedWhenDeccerate,tParamDec); 




        }
       
    }
    float tParamDec=0.0f;
    float speedWhenDeccerate=0.0f;


    void ForwardMovement(){

        transform.position += transform.TransformDirection(Vector3.forward)*_speed;




    }
    Quaternion rotation;
    void SetRotationToWayPoint (){

        //look to target
        rotation = Quaternion.LookRotation(moveAt - transform.position);

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
    public float howLongToDetectReached=2.0f;


    static float ClampAngle (float angle,float min ,float max) {
        if (angle < -360.0f)angle += 360.0f;
        if (angle > 360.0f)angle -= 360.0f;
        return Mathf.Clamp (angle, min, max);
    }

    public float minTurnSpeed=2.0f;
    public float maxTurnSpeed=30.0f;
    public float minSpeed=2.0f;
    public float maxSpeed=30.0f;
    public Transform waypointVisual;

    public float testSpeed=0.0f;
    public float testTurnSpeed=0.0f;

    bool isTurningReach=false;
    float turnigTime=0.0f;
    float turnMaxTime=0.0f;
    float turnMaxTimeDelta=0.0f;

    Vector3 GetRandomPositionInTerritory(){
        return new Vector3 (Random.Range(_spawnBox_Width[0] ,_spawnBox_Width[1]),Random.Range(_spawnBox_Height[0], _spawnBox_Height[1]),Random.Range(_spawnBox_Depth[0],_spawnBox_Depth[1]));

    }
    public float testTuringTime=0.0f;
    void GetNewPoint(Vector3 moveTo,bool isMoveFromStill,bool isTurningReach,float turnigTime,float turnigMax,float speed){

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
        moveAt=moveTo;
        /*if(fishingController.isDebugMode){
            moveAt=waypointVisual.transform.position;
        }
        if(fishingController.isDebugMode){
            Debug.Log(""+_turnSpeed);
            Debug.Log(""+_targetSpeed);
            Debug.Log(""+turnigTime);
        }*/
        isWithinTargetBounds=false;
    }

    public bool drawGizmo=false;
    void OnDrawGizmos () {

        //Gizmos.DrawLine(transform.position,transform.TransformDirection(Vector3.forward)*10.0f);
        //Gizmos.DrawLine(transform.position,(moveAt - transform.position)*10.0f);

        Gizmos.DrawLine(new Vector3(_spawnBox_Width[0],_spawnBox_Height[0],_spawnBox_Depth[0]),new Vector3(_spawnBox_Width[1],_spawnBox_Height[0],_spawnBox_Depth[0]));
        Gizmos.DrawLine(new Vector3(_spawnBox_Width[0],_spawnBox_Height[1],_spawnBox_Depth[0]),new Vector3(_spawnBox_Width[1],_spawnBox_Height[1],_spawnBox_Depth[0]));
        Gizmos.DrawLine(new Vector3(_spawnBox_Width[0],_spawnBox_Height[0],_spawnBox_Depth[0]),new Vector3(_spawnBox_Width[0],_spawnBox_Height[1],_spawnBox_Depth[0]));
        Gizmos.DrawLine(new Vector3(_spawnBox_Width[1],_spawnBox_Height[0],_spawnBox_Depth[0]),new Vector3(_spawnBox_Width[1],_spawnBox_Height[1],_spawnBox_Depth[0]));

        Gizmos.DrawLine(new Vector3(_spawnBox_Width[1],_spawnBox_Height[0],_spawnBox_Depth[0]),new Vector3(_spawnBox_Width[1],_spawnBox_Height[0],_spawnBox_Depth[1]));
        Gizmos.DrawLine(new Vector3(_spawnBox_Width[1],_spawnBox_Height[1],_spawnBox_Depth[0]),new Vector3(_spawnBox_Width[1],_spawnBox_Height[1],_spawnBox_Depth[1]));
        Gizmos.DrawLine(new Vector3(_spawnBox_Width[1],_spawnBox_Height[0],_spawnBox_Depth[1]),new Vector3(_spawnBox_Width[1],_spawnBox_Height[1],_spawnBox_Depth[1]));
        Gizmos.DrawLine(new Vector3(_spawnBox_Width[0],_spawnBox_Height[0],_spawnBox_Depth[1]),new Vector3(_spawnBox_Width[1],_spawnBox_Height[0],_spawnBox_Depth[1]));
        Gizmos.DrawLine(new Vector3(_spawnBox_Width[0],_spawnBox_Height[1],_spawnBox_Depth[1]),new Vector3(_spawnBox_Width[1],_spawnBox_Height[1],_spawnBox_Depth[1]));
        Gizmos.DrawLine(new Vector3(_spawnBox_Width[0],_spawnBox_Height[0],_spawnBox_Depth[1]),new Vector3(_spawnBox_Width[0],_spawnBox_Height[1],_spawnBox_Depth[1]));

        Gizmos.DrawLine(new Vector3(_spawnBox_Width[0],_spawnBox_Height[0],_spawnBox_Depth[0]),new Vector3(_spawnBox_Width[0],_spawnBox_Height[0],_spawnBox_Depth[1]));
        Gizmos.DrawLine(new Vector3(_spawnBox_Width[0],_spawnBox_Height[1],_spawnBox_Depth[0]),new Vector3(_spawnBox_Width[0],_spawnBox_Height[1],_spawnBox_Depth[1]));

    }
    void CreateTerritory(BassRange bassRange){

        float terrytorySize=2.0f;


        posOffset=transform.localScale.x*posOffsetWhenScaleOne;
        terrytorySize=posOffset*2;

        _spawnBox_Width[0]=transform.position.x-terrytorySize;
        _spawnBox_Width[1]=transform.position.x+terrytorySize;
        _spawnBox_Depth[0]=transform.position.z-terrytorySize;
        _spawnBox_Depth[1]=transform.position.z+terrytorySize;

        switch(bassRange){
        case BassRange.Top:
            Debug.Log("Create territory top");
            posCenter=new Vector3(transform.position.x,0.0f-posOffset,transform.position.z);
            _spawnBox_Height[1]=0.0f-posOffset;
            Debug.Log("Create territory top"+_spawnBox_Height[1]);
            _spawnBox_Height[0]=(0.0f-posOffset)-terrytorySize;
            if(_spawnBox_Height[0]<-(GameController.Instance.BottomDepth)+posOffset)_spawnBox_Height[0]=-GameController.Instance.BottomDepth+posOffset;
            if(_spawnBox_Height[1]<_spawnBox_Height[0]){
                _spawnBox_Height[1]=_spawnBox_Height[0]+0.2f;
            }
            break;
        case BassRange.Mid:
            posCenter=new Vector3(transform.position.x,-Mathf.Abs(GameController.Instance.BottomDepth)/2.0f,transform.position.z);
            _spawnBox_Height[1]=posCenter.y+(terrytorySize/2.0f);
            if(_spawnBox_Height[1]>-posOffset)_spawnBox_Height[1]=0.0f-posOffset;
            _spawnBox_Height[0]=posCenter.y-(terrytorySize/2.0f);
            if(_spawnBox_Height[0]<-(GameController.Instance.BottomDepth)+posOffset)_spawnBox_Height[0]=-GameController.Instance.BottomDepth+posOffset;
            if(_spawnBox_Height[1]<_spawnBox_Height[0]){
                _spawnBox_Height[0]=0.0f-posOffset;
                _spawnBox_Height[1]=_spawnBox_Height[0]+0.2f;

            }
            break;
        case BassRange.Bottom:
            posCenter=new Vector3(transform.position.x,-GameController.Instance.BottomDepth+posOffset,transform.position.z);
            _spawnBox_Height[1]=posCenter.y+terrytorySize;
            if(_spawnBox_Height[1]>-posOffset)_spawnBox_Height[1]=0.0f-posOffset;
            _spawnBox_Height[0]=posCenter.y;
            if(_spawnBox_Height[1]<_spawnBox_Height[0]){
                _spawnBox_Height[0]=0.0f-posOffset;
                _spawnBox_Height[1]=_spawnBox_Height[0]+0.2f;

            }
            break;
        }
        transform.position=new Vector3(transform.position.x,Random.Range(_spawnBox_Height[0],_spawnBox_Height[1]),transform.position.z);
    }
    int sasoiNum=0;

  
}
