using UnityEngine;
using System.Collections;

public enum GUIMODE{ROD,BOAT,NONE};
public class JoystickFloat : PS_SingletonBehaviour<JoystickFloat> 
{

    public GUIMODE guiMode=GUIMODE.ROD;

	public void Show(bool isShow){
        if(this.gameObject.activeSelf!=isShow)NGUITools.SetActiveSelf(this.gameObject,isShow);
        if(joystick.gameObject!=false)NGUITools.SetActive(joystick.gameObject,false);
		isPressed = false;
	}
	public Transform joystick;
	public Transform center;
	/// <summary>
	/// If true, generate message OnJoystickRotate(float rot).
	/// rot = (left) -Pi .. 0 (forward) .. Pi (right)
	/// </summary>

	float joystickRadius = 0f;
	Plane plane;
	int cntFrame;
	int cntFramePressed = 0;
	bool isPressed = false;
	//Vector3 lastPos = Vector3.zero;
	Vector3 prevPos = Vector3.zero;
	Transform mTrans;
    private Vector2 preVDelata=Vector2.zero;

    public bool isRodMoveToTention=false;
   
    bool GetisRodMoveToTention(float prev,float now) {
        if(now<prev){
            return false;
        }
       
        return true;
    }



    float timeToStop=0.0f;
    float prevDist=0.0f;
    float culDist=0.0f;
    public void OnJoystick(Vector2 delta){
        
        if(delta.x>1.0f)delta.x=1.0f;
        if(delta.y>1.0f)delta.y=1.0f;
        if(delta.y<-1.0f)delta.y=-1.0f;
        if(delta.x<-1.0f)delta.x=-1.0f;
       
        if(guiMode==GUIMODE.ROD){
            if(GameController.Instance.currentMode==GameMode.Fight) GameController.Instance.SetCurrentBassJump();

            if(delta==Vector2.zero){
                preVDelata=delta;
                isRodMoveToTention=false;
                return;
            }
            if(GameController.Instance.GetisNegakariOrFoockingState()){
                if( !isNegakariState){
                    OnFoockingJoyStick(delta);
                }else{
                    OnNegakariJoyStick(delta);
                }
            }

               

            culDist=Vector2.Distance(Vector2.zero,delta);

            if(Vector2.Distance(preVDelata,delta)>0.01f){
                isRodMoving=true;
                timeToStop=0.0f;
                isRodMoveToTention=GetisRodMoveToTention(prevDist,culDist);
            }else{
                    timeToStop+=Time.deltaTime;
                    if(timeToStop>0.5f){
                        isRodMoveToTention=false;
                        isRodMoving=false;
                        timeToStop=0.0f;
                    }

                return;
            }

            float offsetY=delta.y;

            if(delta.y>0.0f){
                offsetY=0.5f;
                qtY=Quaternion.Euler(new Vector3((90.0f-(delta.y*90.0f)),0.0f,0.0f));

            }else{

                offsetY=0.5f-(Mathf.Abs(delta.y)*0.5f);
                if(offsetY<0.0f)offsetY=0.0f;
                qtY=Quaternion.Euler(new Vector3((90.0f+(-delta.y*90.0f)),0.0f,0.0f));
            }
            qtX=Quaternion.Euler(new Vector3(0.0f,delta.x*110.0f,0.0f));
                RodController.Instance.RotateRod(qtX*qtY,offsetY);

            preVDelata=delta;
            prevDist=culDist;
        }else{
            if(GameController.Instance.currentMode==GameMode.Move){
                Player.Instance.gameObject.
                transform.Rotate(Vector3.up * (delta.x*Time.deltaTime*20.0f), Space.World);
                preVDelata=delta;
                prevDist=culDist;
                return;

            }
        }


    }



    Quaternion qtX;
    Quaternion qtY;

	public TweenAlpha ta;

	void Awake()
	{
		mTrans = transform;
	}
	
	
	void Start()
	{
		// Create the plane to drag along
		plane = new Plane(transform.forward, transform.position);
		
		if (joystick == null)
		{
			joystick = transform.FindChild("Joystick");
			if (joystick == null)
				Debug.LogWarning("Child object Joystick is not found.");
			else if (center == null)
			{
				center = joystick.FindChild("Center");
				if (center == null)
					Debug.LogWarning("Child object Center is not found.");
			}
		}
		
		if (joystick != null)
		{
			joystickRadius = ((SphereCollider) joystick.GetComponent<Collider>()).radius;
			joystick.GetComponent<Collider>().enabled = false;	// need only for radius
			NGUITools.SetActive(joystick.gameObject,false);
		}
	}

    public bool isRodMoving=false;
	void LateUpdate()
	{
		if (isPressed && cntFramePressed < cntFrame)
		{
			SendMessageOnJoystick(prevPos);
			cntFramePressed = cntFrame;
		}
	}


	/// <summary>
	/// Press and show the joystick
	/// </summary>
	void OnPress(bool pressed)
	{
		if (joystick != null)
		{
			if (pressed && !isPressed)
			{
				prevPos = Vector3.zero;
				CalcPositionJoystick();
				// Show joystick
				NGUITools.SetActive(joystick.gameObject,true);
				ta.ResetToBeginning();
				ta.Play();
				center.localPosition = Vector3.zero;
				isPressed = true;
			}
			else if (pressed && isPressed)
			{
				CalcPositionCenter();
			}
			else
			{
                OnJoystickOff();
				NGUITools.SetActive(joystick.gameObject,false);
				isPressed = false;
			}
		}
	}
    public void OnJoystickOff(){
        
        Debug.Log("OnJoystickOff"+guiMode);
        if(guiMode==GUIMODE.ROD){
            
           isRodMoving=false;
            isRodMoveToTention=false;
            timeToStop=0.0f;
            prevDist=0.0f;
            culDist=0.0f;
            RodController.Instance.RotateRodToDefault(false);
            preVDelata=Vector2.zero;
        }else if(guiMode==GUIMODE.BOAT){
            ShipControls.Instance.OnJoyStickUp();
        }
    }
	/// <summary>
	/// Drag the center
	/// </summary>
    /// 
    void OnDrag(Vector2 delta)
	{
		prevPos = delta;

		if (center != null)
			CalcPositionCenter();
	}
	
    Ray ray;
    Vector3 newPos1;
	void CalcPositionJoystick()
	{
		ray = UICamera.currentCamera.ScreenPointToRay (UICamera.lastTouchPosition);
		float dist = 0f;
		newPos1 = joystick.position;
		
		if (plane.Raycast(ray, out dist))
			newPos1 = ray.GetPoint(dist);
		
		joystick.localPosition = mTrans.InverseTransformPoint(newPos1);
	}
	

	void CalcPositionCenter()
	{
		ray = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition);
		float dist = 0f;
		newPos1 = center.position;
		
		if (plane.Raycast(ray, out dist))
			newPos1 = ray.GetPoint(dist);
		
		newPos1 = joystick.InverseTransformPoint(newPos1);
		
		if (newPos1.magnitude > joystickRadius)
			newPos1 = newPos1.normalized * joystickRadius;
		
		center.localPosition = newPos1;
		
		SendMessageOnJoystick(newPos1);
	}
	

	/// <summary>
	/// Joystick event.
	/// Return Vector2, x - rotation (-Pi..Pi), y - radius (0..1)
	/// </summary>
	void SendMessageOnJoystick(Vector3 newPos)
	{
		float x = newPos.x / joystickRadius;
		float y = newPos.y / joystickRadius;
		Vector2 delta = new Vector2(x > 1f ? 1f : x, y > 1f ? 1f : y);
			OnJoystick(delta);

		prevPos = newPos;
	}

	/// <summary>
	/// Converting between Cartesian coordinates and polar,
	/// return angle in rad
	/// </summary>
	float Polar(float x, float y)
	{
		float f = 0f;	// if (x == 0 && y == 0)
		if (x > 0)
			f = Mathf.Atan (y / x);
		else if (x < 0 && y >= 0)
			f = Mathf.Atan (y / x) + Mathf.PI;
		else if (x < 0 && y < 0)
			f = Mathf.Atan (y / x) - Mathf.PI;
		else if (x == 0 && y > 0)
			f = Mathf.PI / 2f;
		else if (x == 0 && y < 0)
			f = -Mathf.PI / 2f;
		
		return f;
	}


    void FixedUpdate(){
        cntFrame++;
        if(guiMode==GUIMODE.ROD){
            
                if(!Button_Float.Instance.isDragging){
                    //ドラグ非動作中

                    if( ((LineScript.Instance.lineTention* RodController.Instance.bendRitsu)>Button_Float.Instance.draggingTention)){
                        if(Button_Float.Instance.reelAudio.isPlaying)Button_Float.Instance.ReelDrag(true);
                    }
                 }else{
                        //ドラグ動作中
                        if( ((LineScript.Instance.lineTention* RodController.Instance.bendRitsu)>Button_Float.Instance.draggingTention)){
                        }else{
                            Button_Float.Instance.ReelDrag(false);
                        }
                }

        }
    }


    void OnFoockingJoyStick(Vector2 delta){
       

    }

    void OnNegakariJoyStick(Vector2 delta){
        NegakariMeter.Instance.OnRodMove(delta,preVDelata);
    }


    bool isNegakariState=false;
    public void OnNegakari(){
        Debug.LogError("根掛かり！！！！！");
        NegakariMeter.Instance.Show(1.0f);
        isNegakariState=true;
    }

    public void OnBite(){
        
    }

    public float foockedPower=0.0f;
   void SetFoockTimeStateToDefault(){
        if( GameController.Instance.GetisNegakariOrFoockingState()){
            GameController.Instance.OnFoocked();
        }
    }


}
