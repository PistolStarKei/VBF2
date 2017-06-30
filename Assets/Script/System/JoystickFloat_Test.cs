using UnityEngine;
using System.Collections;

public class JoystickFloat_Test :PS_SingletonBehaviour<JoystickFloat_Test> {

    public GUIMODE guiMode=GUIMODE.ROD;

    public void Show(bool isShow){
        NGUITools.SetActiveSelf(this.gameObject,isShow);
        NGUITools.SetActive(joystick.gameObject,false);
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
    Vector2 prevDelta=Vector2.zero;
    private Vector2 preVDelata=Vector2.zero;
    public void OnJoystick(Vector2 delta){

        if(delta.x>1.0f)delta.x=1.0f;
        if(delta.y>1.0f)delta.y=1.0f;
        if(delta.y<-1.0f)delta.y=-1.0f;
        if(delta.x<-1.0f)delta.x=-1.0f;

            if(delta.x==0.0f && delta.y==0.0f){
                preVDelata=delta;
                return;
            }

            if(delta!=preVDelata){
                isRodMoving=true;
            }else{
                isRodMoving=false;
                return;
            }

            float offsetY=delta.y;

            if(delta.y>0.0f){
                offsetY=0.5f;
                qtY=Quaternion.Euler(new Vector3((90.0f-(delta.y*90.0f)),0.0f,0.0f));

            }else{

                offsetY=0.5f-(Mathf.Abs(delta.y)*0.5f);
                if(offsetY<0.0f)offsetY=0.0f;
                qtY=Quaternion.Euler(new Vector3((90.0f+(-delta.y*40.0f)),0.0f,0.0f));
            }
            qtX=Quaternion.Euler(new Vector3(0.0f,delta.x*110.0f,0.0f));
            RodControllerTest.Instance.RotateRod(qtX*qtY,offsetY);


        prevDelta=delta;

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
            joystick.GetComponent<Collider>().enabled = false;  // need only for radius
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
                //OnJoystickOff();
                NGUITools.SetActive(joystick.gameObject,false);
                isPressed = false;
            }
        }
    }
    public void OnJoystickOff(){

            isRodMoving=false;
            preVDelata=Vector2.zero;
    }

    /// <summary>
    /// Drag the center
    /// </summary>
    void OnDrag(Vector2 delta)
    {
        prevPos = delta;
        if (center != null)
            CalcPositionCenter();
    }


    void CalcPositionJoystick()
    {
        Ray ray = UICamera.currentCamera.ScreenPointToRay (UICamera.lastTouchPosition);
        float dist = 0f;
        Vector3 newPos1 = joystick.position;

        if (plane.Raycast(ray, out dist))
            newPos1 = ray.GetPoint(dist);

        joystick.localPosition = mTrans.InverseTransformPoint(newPos1);
    }


    void CalcPositionCenter()
    {
        Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition);
        float dist = 0f;
        Vector3 newPos1 = center.position;

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
        float f = 0f;   // if (x == 0 && y == 0)
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


    void Update(){
        cntFrame++;
    }

}
