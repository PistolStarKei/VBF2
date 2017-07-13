using UnityEngine;
using System.Collections;


public class BassAI :FishAiParent {
   

	
	void Start(){
        
        Init(100.0f,transform.position,parameters.range,10,1,EatType.Wait,new Baits[]{Baits.Bait});


	}

	
	//kasseiLevel 0.0-1.0 cautiouslevel=0.0f -1.0f
    public void Init(float size,Vector3 pos,BassRange bassRange,int kasseiLevel ,int cautiousLevel,EatType eatType,Baits[] bate){
        this.parameters.bate=bate;
        isVisible=false;
       
        if(pos.y< Constants.Params.bassVisibleInDepth){
            isVisible=true;
        }
        VisibleBass(isVisible);
		bassState=BassState.Back;
        this.parameters.eatType=eatType;
       
        moveFrequency_still=Equations.EaseInQuad(kasseiLevel,0.07f,0.4f,1.0f);
        parameters.KASSEILEVEL=kasseiLevel;
        parameters.SURELEVEL=cautiousLevel;
        transform.localScale=new Vector3(size* Constants.BassBihaviour.sizeScallingFactor,size* Constants.BassBihaviour.sizeScallingFactor,size* Constants.BassBihaviour.sizeScallingFactor);
        parameters.range=bassRange;
			transform.position=pos;

        sizeNanido=Equations.EaseInQuad(transform.localScale.x,0.0f,2.0f,1.0f);
        if(bassRange==BassRange.Top){
            transform.localRotation =   Quaternion.Euler(new Vector3(0.0f, 0.0f , 0.0f));
        }else{
            transform.localRotation =   Quaternion.Euler(new Vector3(0.0f, 0.0f , Random.Range(-25.0f, 25.0f)));
        }
           
		ChangeState(BassState.Stay);
	}
   



    float dragPower=0.0f;




	public Vector3 positionOffset=Vector3.zero;
    public bool isRippable=false;
	bool isUpState=false;

	
	float bateMatchingFactor=0.0f;



	bool IsUpstate(bool isChase){
	
		float val=0.0f;
		if(isChase){
			//bateMatchingFactor

            val=TackleParams.Instance.tParams.LureApealPower;
            val*=bateMatchingFactor;
            val*=parameters.SURELEVEL;

		}else{
			//attention 
			Debug.LogError("Chusen State Attention");
			val=0.8f;
			//eat type
			//bate match
			bateMatchingFactor=isBateMatched();
			val*=bateMatchingFactor;
            val*=parameters.SURELEVEL;
			//kassei


		}
		Debug.LogError("Chusen State Percentage=="+val);
        if(val>=Random.value){
			return true;
		}


		return false;
	}
    public bool isJumpKaihi=false;
    public void OnJumpKaihi(){
        if(!isJumpKaihi){
            
            if(LineScript.Instance.lineSlack>0.0f){
                isJumpKaihi=true;
            }
        }
    }
	void SetMinMaxSpeed(float minSpeed ,float maxSpeed,float minTurnSpeed,float maxTurnSpeed){
		this.minSpeed=minSpeed;
		this.maxSpeed=maxSpeed;
		this.minTurnSpeed=minTurnSpeed;
		this.maxTurnSpeed=maxTurnSpeed;
	}

	public float moveFrequency_still=0.005f;
	public bool isAttentioned=false;
    bool dragged=false;
	

    public bool isJumping=false;
	public void ChangeState(BassState state){
		Debug.Log("ChangeState"+bassState.ToString()+" →"+state.ToString());
		switch(state){
            case BassState.Result:
                GameController.Instance.ChangeStateTo(GameMode.Result);
                 break;
			case BassState.Stay:
					if(bassState==BassState.Back){
                        VisibleBass(false);
						isAttentioned=false;
                        CreateTerritory(parameters.range);
						useObstacleAvoidance=false;
						delayEvery=0.0f;
						//start at still
                        SetMinMaxSpeed(0.01f,0.03f,2.0f,5.0f);
						isReachedMovePosition=true;
						//dont use delayStill
						delayStill=Random.Range(2.0f,6.0f);
						//dont use delatReached
						delayReached=0.0f;
						//set offset to float up down
				        positionOffset=GetRandomFloatingPosition();
                        howLongToDetectReached=(Constants.BassBihaviour.posOffsetWhenScaleOne*transform.lossyScale.x)/2.0f;
					}else{
						Debug.Log("Inpossible State Change");
						return;
					}
				break;
			case BassState.Chase:
				if(bassState==BassState.Stay){
                GameController.Instance.BassIsChasing(true,gameObject.transform);
                SetMinMaxSpeed(0.05f,0.1f,7.0f,20.0f);
				prevMove=LureController.Instance.appeal.moveState;
                howLongToDetectReached=Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne)*transform.lossyScale.x;
                GetNewPoint(LureController.Instance.gameObject.transform.position,true,true,0.0f,4.0f,0.0f);
				ZeroElapsedTime();
				delayReached =0.5f;
                isUpState=IsUpstate(true);
                SetChusenKakuritu();


                keikaTime=0.0f;
                isJustReeling=true;
				}else{
						Debug.Log("Inpossible State Change");
						return;
				}
				break;
			case BassState.Bite:
				if(bassState!=BassState.Stay && bassState!=BassState.Chase &&bassState!=BassState.Bite){
						Debug.Log("Inpossible State Change");
						return;
					}else{
                GameController.Instance.BassIsChasing(true,gameObject.transform);
                SetMinMaxSpeed(0.05f,0.1f,7.0f,20.0f);
                        howLongToDetectReached=transform.lossyScale.x;
                GetNewPoint(LureController.Instance.gameObject.transform.position,true,true,0.0f,4.0f,0.0f);
                        ZeroElapsedTime();
                        delayReached =0.0f;
                         delayEvery=20.0f;
					}
				break;
			case BassState.Fight:
					if(bassState!=BassState.Bite){
						Debug.Log("Inpossible State Change");
						return;
					}else{
                SetMinMaxSpeed(0.05f,0.1f,7.0f,20.0f);
                isJumping=false;
                stamina2=parameters.stamina;
                delayEvery=0.0f;
                //animator.SetTrigger("bite");

                if(LureController.Instance.mover.IsOnSuimen()){
                    
                    WaterController.Instance.SplashAt(transform.position,transform.localScale.x);
                    AudioController.Play("bite");
                    isReachedMovePosition=false;
                    fightState=FightState.Tukkomi;
                }else{
                    if(parameters.KASSEILEVEL>0.3f){
                        if(Random.value<=parameters.KASSEILEVEL){
                            if(Random.value<0.5f){
                                anime.OneShotAnime("SwimReturnBack");
                            }else{
                                isReachedMovePosition=false;
                                fightState=FightState.Tukkomi;
                            }

                        }else{   
                            isReachedMovePosition=false;
                            fightState=FightState.NotFoocked;
                        }
                    }else{
                        isReachedMovePosition=false;
                        fightState=FightState.NotFoocked;
                    }


                }
                delayReached=1.0f;
                if(transform.localScale.x>1.0f){
                    dragPower=0.01f;
                }else{
                    dragPower=0.01f+((1.0f-transform.localScale.x)*0.04f);
                }
                anime.OneShotAnime("bite");
                delayStill=0.5f;
                GameController.Instance.ChangeStateTo(GameMode.Fight);
                LureController.Instance.gameObject.SetActive(false);
                LureController.Instance.gameObject.transform.parent=rureFoockPosition;
                LureController.Instance.gameObject.transform.localPosition=Vector3.zero;
             

					}
				break;
			case BassState.Back:
				if(bassState==BassState.Stay || bassState==BassState.Back){
					Debug.Log("Inpossible State Change");
					return;
				}else{
                
                howLongToDetectReached=(Constants.BassBihaviour.posOffsetWhenScaleOne*transform.lossyScale.x)/2.0f;
						delayReached=0.0f;
						delayStill=0.0f;
						delayEvery=10.0f;
						useObstacleAvoidance=false;
                GetNewPoint( parameters.spawnedPosiion,true,true,Random.Range(0.3f,0.5f),4.0f,0.0f);
				}
				break;
		}
		if(timeEvery!=0.0f)timeEvery=0.0f;
		ZeroElapsedTime();
		bassState=state;
    }
    float stamina2=0.0f;
    public Transform rureFoockPosition;
	void Update() {
        if(bassState==BassState.Result )return;

		if(delayEvery>0.0f){
			timeEvery +=Time.deltaTime;
			if(timeEvery>delayEvery){
				timeEvery=0.0f;
				OnEveryDelayed();
			}
		}

        if(isReachedMovePosition){
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
            distance=(transform.position - moveTarget).magnitude;
           
            if(bassState==BassState.Chase  ){
				MoveToTarget();
                ChusenBite();

            }else if(bassState==BassState.Fight){
                MoveToTarget();
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
	public float distance=0.0f;

	public void ZeroElapsedTime(){
		timeReached=0.0f;
		timeStill=0.0f;
	}
	float timeStill =0.0f;
	float timeReached =0.0f;
	float timeEvery =0.0f;
	public float delayStill =0.0f;
	float delayReached =0.0f;
	float delayEvery =0.0f;
	void OnEveryDelayed(){

		Debug.Log("OnEveryDelayed"+delayEvery);

		switch(bassState){
			case BassState.Chase:
                sasoiNum++;
                //actioned
                 if(reveallBiteNum>=sasoiNum){
                    Debug.LogError("DelayEveried so I neeed to back");
                    GameController.Instance.BassIsChasing(false,null);

        			ChangeState(BassState.Back);

                    if(isUpState){
                        Debug.Log("I'm not taken anymore iT's delicious bite it ");
                        if(reveallBiteNum>=sasoiNum){
                            ChangeState(BassState.Bite);
                        }else{
                        GameController.Instance.BassIsChasing(false,null);
                            ChangeState(BassState.Back);
                        }

                    }else{

                        if(LureController.Instance.appeal.isReactionByte){
                        if(0.1f>=Random.value)ChangeState(BassState.Bite);
                        }else{
                        GameController.Instance.BassIsChasing(false,null);
                            ChangeState(BassState.Back);
                        }
                    }
                }else{
                GameController.Instance.BassIsChasing(false,null);
                    ChangeState(BassState.Back);
                }

				break;
			case BassState.Back:
				transform.position= parameters.spawnedPosiion;
				ChangeState(BassState.Stay);
				break;
			case BassState.Bite:
            GameController.Instance.BassIsChasing(false,null);
                    ChangeState(BassState.Back);
				break;
			case BassState.Fight:
				break;
			case BassState.Stay:
				if(isAttentioned){
					if(isUpState){
                        isUpState=false;
						Debug.LogError("Time up to change state to chase");
						isAttentioned=false;
                    GameController.Instance.BassIsChasing(true,gameObject.transform);
						ChangeState(BassState.Chase);
					}else{
						Debug.LogError("Time up to back to terrytory");
						isAttentioned=false;
                    VisibleBass(isAttentioned);
                    GetNewPoint(GetRandomPositionInTerritory(),true,true,Random.Range(0.0f,0.5f),4.0f,0.0f);
					}
				}
				break;
		}

	}
    Vector3 tempVec=Vector3.zero;
    public Vector3 GetRandomFloatingPosition(){
        tempVec.x=transform.position.x;tempVec.y=transform.position.y+Random.Range(-1.0f,1.0f); tempVec.z=transform.position.z;
        return tempVec;
    }
	void OnStillDelayed(){
		//set still delay!
        ZeroElapsedTime();
		switch(bassState){
		case BassState.Chase:
            
            positionOffset=PSGameUtils.GetPointAroundPosition(LureController.Instance.gameObject.transform.position,Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne));
            delayStill=Random.Range(1.0f-parameters.KASSEILEVEL,2.0f-parameters.KASSEILEVEL);

			break;
		case BassState.Fight:
            positionOffset=GetRandomFloatingPosition();
			break;
		case BassState.Stay:

			if(isAttentioned){

                positionOffset=PSGameUtils.GetPointAroundPosition(LureController.Instance.gameObject.transform.position,Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne));

                delayStill=Random.Range(1.0f-parameters.KASSEILEVEL,2.0f-parameters.KASSEILEVEL);

			}else{
                if(moveFrequency_still>=Random.value){
                    Debug.Log("decide to move");
                    GetNewPoint(GetRandomPositionInTerritory(),true,true,Random.Range(0.0f,0.1f),4.0f,0.0f);	

				}else{
					delayStill=Random.Range(2.0f,6.0f);
					positionOffset=GetRandomFloatingPosition();

				}
			}
			break;
		}

	}
	Quaternion lookTarget=Quaternion.identity;


	//0 still 1 rure is moving 2 floating 3 sinking 
    LureAction prevMove=0;
    bool isJustReeling=true;
    float timeForBiteChusen=2.0f;
    float keikaTime=0.0f;
    float chusenRituOnTadaMaki=0.0f;
    int reveallBiteNum=0;
    void ChusenBite(){
        keikaTime+=Time.deltaTime;
        if(keikaTime>timeForBiteChusen){
            keikaTime=0.0f;
            Debug.Log("Chusen for bite "+isUpState);
            if(isJustReeling){
                //tada maki
                Debug.Log("Not Lure Actioned");

                //tada maki
                if(chusenRituOnTadaMaki>=Random.value){
                    sasoiNum++;
                    if(isUpState){
                       
                        Debug.Log("I'm not taken anymore iT's delicious bite it ");
                        if(reveallBiteNum>=sasoiNum){
                            isUpState=false;
                            ChangeState(BassState.Bite);
                        }

                    }else{

                        if(LureController.Instance.appeal.isReactionByte){
                            if(0.1f>=Random.value)ChangeState(BassState.Bite);
                        }else{
                            if(reveallBiteNum>=sasoiNum){
                                Debug.Log("I'm back chusen ");
                                GameController.Instance.BassIsChasing(false,null);
                                ChangeState(BassState.Back);
                            }else{
                                if(parameters.SURELEVEL*0.5f>=Random.value){
                                    Debug.Log("I'm back chusen ");
                                    GameController.Instance.BassIsChasing(false,null);
                                    ChangeState(BassState.Back);
                                }
                            }

                        }
                    }
                   
                }else{
                    if(parameters.SURELEVEL*0.5f>=Random.value){
                            Debug.Log("I'm back chusen ");
                        GameController.Instance.BassIsChasing(false,null);
                            ChangeState(BassState.Back);
                        }
                    

                }
            }else{
                //actioned
                Debug.Log("Lure Actioned");
                isJustReeling=true;
                sasoiNum++;
                if(isUpState){
                    Debug.Log("I'm not taken anymore iT's delicious bite it ");
                    if(reveallBiteNum>=sasoiNum){
                        ChangeState(BassState.Bite);
                    }

                }else{

                    if(LureController.Instance.appeal.isReactionByte){
                        if(0.1f>=Random.value)ChangeState(BassState.Bite);
                    }else{
                        if(reveallBiteNum>=sasoiNum){
                            Debug.Log("I'm back chusen ");
                            GameController.Instance.BassIsChasing(false,null);
                            ChangeState(BassState.Back);
                        }else{
                            if(parameters.SURELEVEL*0.5f>=Random.value){
                                Debug.Log("I'm back chusen ");
                                GameController.Instance.BassIsChasing(false,null);
                                ChangeState(BassState.Back);
                            }
                        }

                    }
                }

            }
        }

    }
	public void CheckRureMoveGetAppealled(){
		if(bassState==BassState.Chase ){
			if(prevMove!=LureController.Instance.appeal.moveState){

				Debug.Log("Rure changed it's behaviour");


				switch(prevMove){
                case LureAction.still:
                    if(LureController.Instance.appeal.moveState==LureAction.moving){
						//stop and go
                        if(isJustReeling)isJustReeling=false;
					}
					break;
                case LureAction.moving:
					switch(LureController.Instance.appeal.moveState){
                         case LureAction.still:
    						//go and stop
                            if(isJustReeling)isJustReeling=false;
    						break;
                        case LureAction.floating:
    						//cranky dive to float
                            if(isJustReeling)isJustReeling=false;
    						break;
                        case LureAction.sinking:
    						//lift and fall
                            if(isJustReeling)isJustReeling=false;
    						break;
					}
					break;
                case LureAction.floating:
					switch(LureController.Instance.appeal.moveState){
                    case LureAction.still:
						//crank popper float on suimen
                        if(isJustReeling)isJustReeling=false;
						break;
                    case LureAction.moving:
						//on suimen move or crank sink
                        if(isJustReeling)isJustReeling=false;
						break;
					}
					break;
                case LureAction.sinking:
					switch(LureController.Instance.appeal.moveState){
                    case LureAction.still:
						//to the bottom
                        if(isJustReeling)isJustReeling=false;
						break;
                    case LureAction.moving:
						//fall and lift
                        if(isJustReeling)isJustReeling=false;
						break;
					}
					break;
				}
				prevMove=LureController.Instance.appeal.moveState;
			}else{

			}


		}else{
			if(prevMove!=LureController.Instance.appeal.moveState)prevMove=LureController.Instance.appeal.moveState;
		}

	}



    float stillTimeMax=0.0f;
    float timeBassIsBoring=5.0f;
	void StillAnimate(){
        if(_speed!=0.0f){
            _speed=0.0f;
            anime.SetSpeed(_speed);
        }
		if(bassState==BassState.Chase ){
			if(LureController.Instance.appeal.moveState!=0){
				//move rure
				CheckRureMoveGetAppealled();
				ZeroElapsedTime();
                GetNewPoint(LureController.Instance.gameObject.transform.position,true,true,0.0f,0.0f,0.0f);
                howLongToDetectReached=Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne)*transform.lossyScale.x;
				return;
			}
            stillTimeMax +=Time.deltaTime;
            if(stillTimeMax>timeBassIsBoring){
                stillTimeMax=0.0f;
                Debug.LogError("I'm back to terrior it's not moving anymore boring");
                GameController.Instance.BassIsChasing(false,null);
                ChangeState(BassState.Back);
             }
			//lookTarget=Quaternion.LookRotation(LureController.Instance.gameObject.transform.position-transform.position);
			lookTarget=Quaternion.Euler(new Vector3(0.0f,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z));
		}else if(bassState==BassState.Stay ){
			if(isAttentioned){
				if(LureController.Instance.appeal.moveState!=0){
					//move rure
					CheckRureMoveGetAppealled();
					ZeroElapsedTime();
                    GetNewPoint(LureController.Instance.gameObject.transform.position,true,true,0.0f,0.1f,0.0f);
                    howLongToDetectReached=Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne)*transform.lossyScale.x;
					return;
				}
                stillTimeMax +=Time.deltaTime;
                if(stillTimeMax>timeBassIsBoring){
                    stillTimeMax=0.0f;
                    Debug.LogError("I'm back to terrior it's not moving anymore boring");
                    GameController.Instance.BassIsChasing(false,null);
                    ChangeState(BassState.Back);
                }
				lookTarget=Quaternion.Euler(new Vector3(0.0f,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z));
			}else{
				lookTarget=Quaternion.Euler(new Vector3(0.0f,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z));

			}
		
		}

			
		transform.rotation=Quaternion.Lerp(transform.rotation,lookTarget,Time.deltaTime);

        if(transform.position.y<-(GameController.Instance.BottomDepth)+posOffset){
			return;
		}
           
		if(transform.position.y>0.0f-posOffset){
			return;
		}
		transform.position=
            Vector3.Lerp(transform.position,positionOffset,Time.deltaTime/5.0f);

	}

	void MoveToTarget(){
		switch(bassState){
			
			case BassState.Fight:
            if( Player.Instance.GetDistanceoPlayer()<Constants.Params.kaishuDistance){
					Debug.LogError("very close player ");
				}
				_stuckCounter+=Time.deltaTime*(howLongToDetectReached*0.25f);
				Move(true);
				break;
			case BassState.Bite:
				timeReached=0.0f;
				_stuckCounter=timeReached;
            if( Player.Instance.GetDistanceoPlayer()<Constants.Params.kaishuDistance){
					Debug.LogError("very close player ");
                GameController.Instance.BassIsChasing(false,null);
					ChangeState(BassState.Back);
				}
				moveTarget=LureController.Instance.gameObject.transform.position;
				Move(true);
				break;
			case BassState.Chase:
					//moving
            if( Player.Instance.GetDistanceoPlayer()<Constants.Params.kaishuDistance){
						Debug.LogError("very close player ");
                GameController.Instance.BassIsChasing(false,null);
						ChangeState(BassState.Back);
					}
							if(LureController.Instance.appeal.moveState==0){
										//rure stopped
								Debug.LogError("rure stopped set to still ");
				                  CheckRureMoveGetAppealled();
                                stillTimeMax=0.0f;
                timeBassIsBoring=10.0f*parameters.KASSEILEVEL;
								delayStill=delayEvery*Random.Range(0.5f,0.99f);
								_turnSpeed = Random.Range(minTurnSpeed, maxTurnSpeed);
                positionOffset=PSGameUtils.GetPointAroundPosition(LureController.Instance.gameObject.transform.position,Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne));

								isReachedMovePosition=true;
								return;
							}
			CheckRureMoveGetAppealled();
					timeReached=0.0f;
					_stuckCounter=timeReached;
					
				moveTarget=LureController.Instance.gameObject.transform.position;
				if(distance < howLongToDetectReached*2){
					if(speedWhenDeccerate==0.0f){
						speedWhenDeccerate=_speed;
						tParam=2.0f;

					}
					Move(false);
				}else{
					if(tParam<-1.0f){
						tParam=Mathf.InverseLerp(0.0f, _targetSpeed,_speed);
					}
					speedWhenDeccerate=0.0f;
					Move(true);
				}
				break;
			case BassState.Stay:
				if(isAttentioned){
               
                if( Player.Instance.GetDistanceoPlayer()<Constants.Params.kaishuDistance){
						Debug.LogError("very close player ");
                    GameController.Instance.BassIsChasing(false,null);
						ChangeState(BassState.Back);
					}
						if(LureController.Instance.appeal.moveState==0){
							//rure stopped
            				CheckRureMoveGetAppealled();
                            stillTimeMax=0.0f;
                    timeBassIsBoring=10.0f*parameters.KASSEILEVEL;
                    Debug.LogError("rure stopped wait for "+timeBassIsBoring+" sec");
							delayStill=delayEvery*Random.Range(0.5f,0.99f);
							_turnSpeed = Random.Range(minTurnSpeed, maxTurnSpeed);
                    positionOffset=PSGameUtils.GetPointAroundPosition(LureController.Instance.gameObject.transform.position,Random.Range(Constants.BassBihaviour.minDistanceWhenChaseScaleOne,Constants.BassBihaviour.maxDistanceWhenChaseScaleOne));

							isReachedMovePosition=true;
							return;
						}
					
					//moving
					timeReached=0.0f;
					_stuckCounter=timeReached;
					moveTarget=LureController.Instance.gameObject.transform.position;
				if(distance < howLongToDetectReached*2){

						if(speedWhenDeccerate==0.0f){
							speedWhenDeccerate=_speed;
							tParam=-2.0f;
						}	
					Move(false);
					}else{
						if(tParam<-1.0f){
							tParam=Mathf.InverseLerp(0.0f, _targetSpeed,_speed);
						}
						speedWhenDeccerate=0.0f;
						Move(true);
					}
				}else{
               
					_stuckCounter+=Time.deltaTime*(howLongToDetectReached*0.25f);
				Move(true);
				}
				break;
		}
	}
    Vector3 dragDirection;
	void Move(bool isAccerate){
		//look target and adjust rotation
		SetRotationToWayPoint();
		//move forword
		SetSpeed(isAccerate);
		ForwardMovement();

		RayCastToAwayFromObstacles();
		//set to be random
	}
	void OnReachedWithinBounds(){

		switch(bassState){
			case BassState.Bite:
				// parent lure and state to fight
            if(transform.position.y>-(transform.localScale.x*0.7f)){
                AudioController.Play("jabajaba");
                WaterController.Instance.SplashAt(transform.position,transform.localScale.x);
            }
			    ChangeState(BassState.Fight);
				break;
			case BassState.Fight:
				break;
			case BassState.Stay:
				if(!isAttentioned){
					
					positionOffset=GetRandomFloatingPosition();
				
				}
				break;
			case BassState.Back:
				ChangeState(BassState.Stay);
				break;
		}
        isReachedMovePosition=true;
	}
	bool useObstacleAvoidance=false;
	void RotateScanner() {
		//Scan random if not pushing
		if(_scan){
			_scanner.rotation = Random.rotation;
			return;
		}
		//Scan slow if pushing
		_scanner.Rotate(new Vector3(150*Time.deltaTime,0,0));
	}

	//Uses scanner to push away from obstacles
	void RayCastToAwayFromObstacles() {
		if(!useObstacleAvoidance)return;
			//Scan random if not pushing
			RotateScanner();

			RaycastHit hit;
			float d;
			Vector3 cacheForward = _scanner.forward;
       
        if (Physics.Raycast(transform.position, cacheForward,out hit, 1, Player.Instance.obstacleslayerMask_ForBass)){		

			d = (1 - hit.distance)/1;	// Equals zero to one. One is close, zero is far	
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
		
	//Returns true if there is an obstacle in the way
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
	public int _avoidDistance= 1;		//How far avoid rays travel
	public float _avoidAngle = 0.35f; 		//Angle of the rays used to avoid obstacles left and right
	public float _stopSpeedMultiplier= 2.0f;	//How fast to stop when within stopping distance
	public float _stopDistance= 0.5f;		//How close this can be to objects directly in front of it before stopping and backing up. This will also rotate slightly, to avoid "robotic" behaviour

	void SetSpeed(bool isAccerarate){
		if(isAccerarate){
			//slow to target speed
			if (tParam < 1) {
				//_targetSpeed=minSpeed MxSpeed

                if(isDebugMode){
                    if(_speed > _targetSpeed){
                        tParam += Time.deltaTime * 0.1f;
                    }else{
                        tParam += Time.deltaTime * 0.1f;       
                    }
                }else{
    				if(_speed > _targetSpeed){
    					tParam += Time.deltaTime * 0.025f;
    				}else{
    					tParam += Time.deltaTime * 0.01f;		
    				}
                }
				//make it to target
				
			}
            _speed = Mathf.Lerp(_speed, _targetSpeed,tParam);   
		}else{

				tParamDec=Mathf.Clamp01((distance-howLongToDetectReached)/howLongToDetectReached);
					_speed = Mathf.Lerp(0.0f,speedWhenDeccerate,tParamDec);	
						



		}
        anime.SetSpeed(_speed);
	}
	

	void ForwardMovement(){

		transform.position += transform.TransformDirection(Vector3.forward)*_speed;




	}
	Quaternion rotation;
	void SetRotationToWayPoint (){

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
	public float howLongToDetectReached=2.0f;


	

	
   
	
	




int sasoiNum=0;
    void SetChusenKakuritu(){
        //kasseiLavel batematched rure.appealPower
        Debug.Log("Set Chusen Ritsu");
        delayEvery=20.0f;
       
        int min=1;
        int max=10;

        if(parameters.KASSEILEVEL<=0.1f){
            delayEvery=Random.Range(20.0f,40.0f);
            min=8;
            max=10;
        }else if(parameters.KASSEILEVEL>0.1f && parameters.KASSEILEVEL<=0.4f){
            delayEvery=Random.Range(20.0f,40.0f);
            min=3;
            max=7;
        }else if(parameters.KASSEILEVEL>0.4f && parameters.KASSEILEVEL<=0.6f){
            delayEvery=Random.Range(10.0f,20.0f);
            min=2;
            max=6;
        }else if(parameters.KASSEILEVEL>0.6f && parameters.KASSEILEVEL<=0.9f){
            delayEvery=Random.Range(7.0f,15.0f);
            min=1;
            max=5;
        }else{
            delayEvery=Random.Range(5.0f,10.0f);
            min=1;
            max=3;
        }
        timeForBiteChusen=delayEvery/ 10.0f;
        reveallBiteNum=Random.Range(min,max);
        sasoiNum=0;

        if(parameters.KASSEILEVEL<=0.5f){
           
            chusenRituOnTadaMaki=0.1f;
        }else{
            chusenRituOnTadaMaki=parameters.KASSEILEVEL;
        }
       
        Debug.Log("I will be Back When"+delayEvery+"sec");
    Debug.Log("I will bite if upstate in "+reveallBiteNum);
        Debug.Log("Chusen in every "+timeForBiteChusen+"sec");
        Debug.Log("I will chusen if tadamaki "+(chusenRituOnTadaMaki*100.0f)+"%");


    }


	//return -0.0(not Matched 100%)- 1.0f(matched100%);
	float isBateMatched(){
		float value=0.0f;
     
       
    
        /*if((LureController.Instance.lureParams.lureParamsData.sizeMatchIn* Constants.BassBihaviour.sizeScallingFactor)<gameObject.transform.lossyScale.x){
            float gensan=0.0f;
            gensan=(gameObject.transform.lossyScale.x-(LureController.Instance.lureParams.lureParamsData.sizeMatchIn* Constants.BassBihaviour.sizeScallingFactor))*0.3f;
            if(gensan>0.8f){
                gensan=0.8f;
            }
            value-=gensan;
        }*/



        if(value<0.0f)value=0.0f;
		return value;
        
	}
    bool isMatchBate(){
        bool match=false;
       
        return match;
    }
    
    int GetRureType(){
        int i=0;

        return i;
    }


    public void OnJumped(){
        Debug.Log("OnJumped");
        AudioController.Play("bassjump");
        WaterController.Instance.SplashAt(new Vector3(transform.position.x,0.0f,transform.position.z),transform.localScale.x);
        isJumping=false;

    }

    public void OnJumpTop(){
        Debug.Log("OnJumpTop");
    }
}
