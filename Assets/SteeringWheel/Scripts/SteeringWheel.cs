using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UISprite))]
[RequireComponent(typeof(BoxCollider))]
public class SteeringWheel : MonoBehaviour {

	//Returns Input value between -1 and 1 for your car controls;
	public float GetInput()
	{
		return Mathf.Round(wheelAngle / maxAngle * 100) / 100;
	}

	//Returns rotation value;
	public int GetAngle()
	{
		return Mathf.FloorToInt(wheelAngle);
	}

	//Returns whether or not steering wheel is pressed;
	public bool isPressed()
	{
		return isHold;
	}


	public float maxAngle = 500;							//Maximum (positive/negative) rotate angle;
	public float wheelFreeSpeed = 10;						//Wheel turn to default rotation speed;
	public float centerDeadZoneRadius = 5;					//Center dead zone radius;
	public float defaultAlpha = 0.5F, activeAlpha = 1.0F; 	//Wheel image alpha depending whether on pressed or not;
	public bool Interactable = true;						//Is steering wheel interactable or not;

	[HideInInspector]public float wheelAngle;
	private float wheelTempAngle, wheelNewAngle;
	private bool isHold;
	private Vector2 wheelCenter, touchPos;
    private Transform wheelRect;
    private Color col;
    private UISprite sprite;
	
	void Start () 
	{
		wheelRect = this.transform;
        wheelCenter = UICamera.mainCamera.WorldToScreenPoint(wheelRect.position);
        sprite = GetComponent<UISprite>();
        col = sprite.color;
	}	
	
	// Update is called once per frame
	void Update () {
        
        //Debug.Log("Current input value: " + GetInput().ToString("F1") + "  Current angle value: " + GetAngle() + " Is Pressed: " + isPressed());

		if( isHold )
		{
			if(Interactable)
			{
                col.a = activeAlpha;
				wheelNewAngle = Vector2.Angle( Vector2.up, touchPos - wheelCenter );
				
				// If mouse is very close to the steering wheel's center, do nothing
				if( Vector2.Distance( touchPos, wheelCenter ) > centerDeadZoneRadius )
				{
					if( touchPos.x > wheelCenter.x )
						wheelAngle += wheelNewAngle - wheelTempAngle;
					else
						wheelAngle -= wheelNewAngle - wheelTempAngle;
				}
				
				// Make sure that the wheelAngle does not exceed the maximumAngle
				if( wheelAngle > maxAngle )
					wheelAngle = maxAngle;
				else if( wheelAngle < -maxAngle )
					wheelAngle = -maxAngle;
				
				wheelTempAngle = wheelNewAngle;
			}
		}
		else 
		{
            col.a = defaultAlpha;
			// If the wheel is rotated and not being held, rotate it to its default angle (zero)
			if( !Mathf.Approximately( 0f, wheelAngle ) )
			{
				float deltaAngle = wheelFreeSpeed;
				
				if( Mathf.Abs( deltaAngle ) > Mathf.Abs( wheelAngle ) )
				{
					wheelAngle = 0f;
					return;
				}
				
				if( wheelAngle > 0f )
					wheelAngle -= deltaAngle;
				else
					wheelAngle += deltaAngle;
			}
		}

        sprite.color = col;
		wheelRect.eulerAngles = new Vector3 (0, 0, -wheelAngle);
		wheelObj.eulerAngles=new Vector3 (wheelObj.eulerAngles.x, 0, -wheelAngle);
	}

	public Transform wheelObj;

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            isHold = true;
            touchPos = UICamera.lastTouchPosition;
            wheelTempAngle = Vector2.Angle(Vector2.up, touchPos - wheelCenter);
        }
    }

    void OnDrag()
    {
        touchPos = UICamera.lastTouchPosition;
    }

    void OnDragEnd()
    {
        isHold = false;
    }



}
