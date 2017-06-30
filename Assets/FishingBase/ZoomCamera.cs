using UnityEngine;
using System.Collections;

public class ZoomCamera :  PS_SingletonBehaviour<ZoomCamera> {

	private Vector3 startRotation=Vector3.zero;
	private Vector3 startPosition=Vector3.zero;
	private float fieldOfView=64.0f;
	bool isDeaultPosition=true;
	public bool isZoom=false;
	float moveSpeed = 5.0f; //move speed
	float rotationSpeed = 4.0f; //speed of turning
	void Update () {

		if(isZoom){
			timeSinceZoomStarted+=Time.deltaTime;
            transform.LookAt(LureController.Instance.transform.position);
		
            if(timeSinceZoomStarted>2.0f)CastBtn.Instance.ShowCastBtn(false);
			//move towards the playe
			transform.position += transform.forward * moveSpeed * Time.deltaTime;
		}else{
            if(!isDeaultPosition){
				timeSinceZoomStarted+=Time.deltaTime/20.5f;

				transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.Euler(startRotation), rotationSpeed*Time.deltaTime);
				transform.localPosition = Vector3.Lerp(transform.localPosition,startPosition, timeSinceZoomStarted);

				if(transform.localPosition==startPosition){
					transform.localRotation=Quaternion.Euler(startRotation);
                    isDeaultPosition=true;
				}

			}
		}
	}
	private float timeSinceZoomStarted = 0.0f;
	public void BackCamera(){
		timeSinceZoomStarted = 0.0f;
		isZoom=false;

	}
	public void SetZoomCamera(){
        startRotation=transform.localEulerAngles;
        startPosition=transform.localPosition;

		timeSinceZoomStarted=0.0f;
        isDeaultPosition=false;
		isZoom = true;
	}

	


}
