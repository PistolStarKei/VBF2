using UnityEngine;
using System.Collections;

public class InWaterCamera : PS_SingletonBehaviour<InWaterCamera> {

	public Camera cameraObj;

	public void Activate(bool isActive){
		if(isActive!=cameraObj.enabled)cameraObj.enabled=isActive;
	}

    public Transform target;
    public float front=0f;
    public float right=0f;
    public float up=0f;

    void Update(){
		if(cameraObj.enabled){
	        transform.position = target.position+(Player.Instance.gameObject.transform.up*up)+(Player.Instance.gameObject.transform.forward*front)+(Player.Instance.gameObject.transform.right*right);

	        transform.LookAt(target);
		}
    }
}
