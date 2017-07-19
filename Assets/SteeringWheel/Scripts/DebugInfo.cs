using UnityEngine;
using System.Collections;

public class DebugInfo : MonoBehaviour {
	public UILabel debugLabel;
	public SteeringWheel steeringWheelController;

	void Update () 
	{
        debugLabel.text = "Input value: " + steeringWheelController.GetInput() +    //Input value;
                         " Angle value: " + steeringWheelController.GetAngle() +   //Wheel angle;
                         " Pressed: " + steeringWheelController.isPressed();       //Pressed or not;
	}
}
