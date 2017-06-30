using UnityEngine;
using System.Collections;

public class TestPlayer : MonoBehaviour {

	// Use this for initialization
    void Start(){
        if(reelHand!=null)reelHandRot=reelHand.localEulerAngles;

    }
	
    float speed;
    public Transform reelHand;
    public Transform rotateCenter;
    public Vector3 reelHandRot;
	// Update is called once per frame
	void Update () {
            speed= 2.0f*50.0f;
            reelHand.RotateAround (rotateCenter.position,rotateCenter.right,20 * Time.deltaTime * speed);
            reelHand.localRotation=Quaternion.Euler(reelHandRot);
	}
}
