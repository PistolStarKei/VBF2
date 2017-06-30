using UnityEngine;
using System.Collections;

public class Button_Float : PS_SingletonBehaviour<Button_Float>  {

	public void Show(bool isShow){
        if(this.gameObject.activeSelf!=isShow)NGUITools.SetActiveSelf(this.gameObject,isShow);
        if(btnParent.gameObject.activeSelf!=false)NGUITools.SetActive(btnParent.gameObject,false);
		isPressed = false;
        isDragging=false;
	}
    public AudioSource reelAudio;
   public void StopReelAudio(){
        if(reelAudio.isPlaying)reelAudio.Stop();
    }

    //ドラグ作動のテンション
    public float draggingTention=3.0f;
    public bool isDragging=false;
    public bool isCovered=false;



    float reelSpeed=0.0f;

    public float getReelingSpeed(){
            return reelSpeed;
    }

    public bool isReeling(){
        if(reelSpeed>0.0f){
            return true;
        }else{
            return false;
        }
    }

    void OnReel(float power){
        //0 1 2

        if(isDragging)return;
        CheckCancelSinking();
        reelSpeed=(power+1.0f)/2.0f;

        if(!reelAudio.isPlaying){
            reelAudio.Play();
        }else{
            //0.9-1.9
            reelAudio.pitch=0.9f+((power+1.0f)*0.7f);
        }
        LineScript.Instance.Length-=TackleParams.Instance.tParams.ReelSpeed*(reelSpeed);

    }

    void CheckCancelSinking(){
        if(!isCovered){
            isCovered=true;
            AudioController.Play("cover");
            LineScript.Instance.InvokeFuke(false,false,false);
        }
    }

    public void ReelDrag(bool isOn){

        if(isOn){
            isDragging=true;

            if(reelAudio.isPlaying)reelAudio.Stop();

            if(!AudioController.IsPlaying("drag"))AudioController.Play("drag");

        }else{
            isDragging=false;
            if(AudioController.IsPlaying("drag"))AudioController.Stop("drag");
        }

    }


	public Transform btnParent;
	public Transform center;

	float btnParentRadius = 0f;
	Plane plane;
	int cntFrame;
	int cntFramePressed = 0;
	bool isPressed = false;
	//Vector3 lastPos = Vector3.zero;
	Vector3 prevPos = Vector3.zero;
	Transform mTrans;

	Vector2 prevDelta=Vector2.zero;
	void OnButton(float power){
		speed=power;
	}
		
	void Awake()
	{
		mTrans = transform;
	}


	void Start()
	{
		// Create the plane to drag along
		plane = new Plane(transform.forward, transform.position);

		if (btnParent == null)
		{
			btnParent = transform.FindChild("Joystick");
			if (btnParent == null)
				Debug.LogWarning("Child object Joystick is not found.");
			else if (center == null)
			{
				center = btnParent.FindChild("Center");
				if (center == null)
					Debug.LogWarning("Child object Center is not found.");
			}
		}

		if (btnParent != null)
		{
			btnParentRadius = ((SphereCollider) btnParent.GetComponent<Collider>()).radius;
			btnParent.GetComponent<Collider>().enabled = false;	// need only for radius
			NGUITools.SetActive(btnParent.gameObject,false);
		}
	}


    void Update(){
        cntFrame++;


    }

    public float speed=0.0f;
	void LateUpdate()
	{
        if (isPressed && JoystickFloat.Instance.guiMode==GUIMODE.ROD)
		{
			rotater.Rotate( 0.0f,0.0f, -(Time.deltaTime*(500.0f+(speed*200.0f))));
            OnReel(speed);

			if(cntFramePressed < cntFrame){
				SendMessageOnFloatButton(prevPos);
				cntFramePressed = cntFrame;
			}
		}
	}

	/// <summary>
	/// Press and show the joystick
	/// </summary>
	void OnPress(bool pressed)
	{
        if (JoystickFloat.Instance.guiMode!=GUIMODE.ROD)return;
		if (btnParent != null)
		{
			if (pressed && !isPressed)
			{
				speed=0.0f;
				prevPos = Vector3.zero;
				CalcPositionParent();
				// Show btnParent
				NGUITools.SetActive(btnParent.gameObject,true);

				center.localPosition = Vector3.zero;
				isPressed = true;
			}
			else if (pressed && isPressed)
			{
				CalcPositionCenter();
			}
			else
			{
                
                    if(reelAudio.isPlaying){
                        reelSpeed=0.0f;
                        reelAudio.Stop();
                    }
                    ReelDrag(false);

				NGUITools.SetActive(btnParent.gameObject,false);
				isPressed = false;
			}
		}
        if(!isPressed)StopReelAudio();
	}


	/// <summary>
	/// Drag the center
	/// </summary>
	void OnDrag(Vector2 delta)
	{
        if (JoystickFloat.Instance.guiMode!=GUIMODE.ROD)return;
		prevPos = delta;
		if (center != null)
			CalcPositionCenter();
	}


	void CalcPositionParent()
	{
		Ray ray = UICamera.currentCamera.ScreenPointToRay (UICamera.lastTouchPosition);
		float dist = 0f;
		Vector3 newPos1 = btnParent.position;

		if (plane.Raycast(ray, out dist))
			newPos1 = ray.GetPoint(dist);

		btnParent.localPosition = mTrans.InverseTransformPoint(newPos1);
	}


	void CalcPositionCenter()
	{
		Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition);
		float dist = 0f;
		Vector3 newPos1 = center.position;
		//tweek yajirusi
		if (plane.Raycast(ray, out dist))
			newPos1 = ray.GetPoint(dist);

		newPos1 = btnParent.InverseTransformPoint(newPos1);

		if (newPos1.magnitude > btnParentRadius)
			newPos1 = newPos1.normalized * btnParentRadius;

		center.localPosition = new Vector3(0.0f,newPos1.y,0.0f);

		SendMessageOnFloatButton(newPos1);
	}

	/// <summary>
	/// Joystick event.
	/// Return Vector2, x - rotation (-Pi..Pi), y - radius (0..1)
	/// </summary>
	void SendMessageOnFloatButton(Vector3 newPos)
	{
		float x = newPos.x / btnParentRadius;
		float y = newPos.y / btnParentRadius;
		Vector2 delta = new Vector2(x > 1f ? 1f : x, y > 1f ? 1f : y);

		OnButton(delta.y);

		prevPos = newPos;
	}
	public Transform rotater;
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

   
    public void OnLureHitsButtom(){
        if(!isCovered){
            isCovered=true;
            LineScript.Instance.InvokeFuke(true,true,false);
        }
    }

}
