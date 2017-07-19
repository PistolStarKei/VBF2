using UnityEngine;
using System.Collections;

public class BoatControllers : PS_SingletonBehaviour<BoatControllers> {

	public SteeringWheel steeringWeel;
	public ButtonFloat_BoatLever lever;
	void Awake(){
		Show(false);
	}
	public void Show(bool isShow){
		if(isShow){
			steeringWeel.gameObject.SetActive(true);
			lever.Show(true);
		}else{
			steeringWeel.gameObject.SetActive(false);
			lever.Show(false);
		}

	}

	public float GetAngle(){
		return steeringWeel.GetAngle();
	}

}
