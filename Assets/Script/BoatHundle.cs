using UnityEngine;
using System.Collections;

public class BoatHundle : MonoBehaviour {

	Vector2 centerPoint=Vector2.zero;

	public float maximumSteeringAngle = 200f;
	public float wheelReleasedSpeed = 200f;

	float wheelAngle = 0f;
	float wheelPrevAngle = 0f;

	bool wheelBeingHeld = false;

	public float GetClampedValue()
	{
		// returns a value in range [-1,1] similar to GetAxis("Horizontal")
		return wheelAngle / maximumSteeringAngle;
	}

	public float GetAngle()
	{
		// returns the wheel angle itself without clamp operation
		return wheelAngle;
	}

	void  OnPress (bool isDown) {

		if(isDown){
			Vector2 pointerPos = UICamera.lastTouchPosition;

			wheelBeingHeld = true;
			wheelPrevAngle = Vector2.Angle( Vector2.up, pointerPos - centerPoint );

		}else{

			wheelBeingHeld = false;
			preVec=new Vector2(0.0f,0.0f);

		}


	}

	public GameObject hundle;
	Vector2 preVec=new Vector2(0.0f,0.0f);

	float moveX=0.0f;
	float moveY=0.0f;
	void OnDrag (Vector2 delta){
		// Executed when mouse/finger is dragged over the steering wheel

		Debug.Log(""+delta);

		hundle.transform.localEulerAngles = Vector3.back * wheelAngle;
	}
}
