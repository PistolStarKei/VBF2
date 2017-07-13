﻿using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
// Put this on a rigidbody object and instantly
// have 2D spaceship controls like OverWhelmed Arena
// that you can tweak to your heart's content.

[RequireComponent (typeof (Rigidbody))]
public class ShipControls : PS_SingletonBehaviour<ShipControls>
{
	public Animator playerAnimater;
	public GameObject playerOBJ;
	public FullBodyBipedIK playerIk;
	public Vector3 boatPos;
	public Vector3 castPos;
	public Transform[] effecters_boat;
	public Transform[] effecters_cast;
	public HandPoser[] hands;
	public void SetPlayerPositionAndIK(bool isBoat){

		if(isBoat){
			playerOBJ.transform.localPosition=boatPos;
			playerIk.solver.leftHandEffector.target=effecters_boat[0];
			playerIk.solver.leftArmChain.bendConstraint.bendGoal=effecters_boat[1];
			playerIk.solver.rightHandEffector.target=effecters_boat[2];
			playerIk.solver.rightArmChain.bendConstraint.bendGoal=effecters_boat[3];
			playerIk.solver.leftFootEffector.target=effecters_boat[4];
			playerIk.solver.rightFootEffector.target=effecters_boat[5];
			hands[0].poseRoot=effecters_boat[0];
			hands[1].poseRoot=effecters_boat[2];
		}else{
			playerOBJ.transform.localPosition=castPos;
			playerIk.solver.leftHandEffector.target=effecters_cast[0];
			playerIk.solver.leftArmChain.bendConstraint.bendGoal=effecters_cast[1];
			playerIk.solver.rightHandEffector.target=effecters_cast[2];
			playerIk.solver.rightArmChain.bendConstraint.bendGoal=effecters_cast[3];
			playerIk.solver.leftFootEffector.target=effecters_cast[4];
			playerIk.solver.rightFootEffector.target=effecters_cast[5];
			hands[0].poseRoot=effecters_cast[0];
			hands[1].poseRoot=effecters_cast[2];
		}
	}

	public float hoverHeight = 3F;
	public float hoverHeightStrictness = 1F;
	public float forwardThrust = 5000F;
	public float backwardThrust = 2500F;
	public float bankAmount = 0.1F;
	public float bankSpeed = 0.2F;
	public Vector3 bankAxis = new Vector3(-1F, 0F, 0F);
	public float turnSpeed = 8000F;

	public Vector3 forwardDirection = new Vector3(1F, 0F, 0F);

	public float mass = 5F;

	// positional drag
	public float sqrdSpeedThresholdForDrag = 25F;
	public float superDrag = 2F;
	public float fastDrag = 0.5F;
	public float slowDrag = 0.01F;

	// angular drag
	public float sqrdAngularSpeedThresholdForDrag = 5F;
	public float superADrag = 32F;
	public float fastADrag = 16F;
	public float slowADrag = 0.1F;

	public bool playerControl = false;

	float bank = 0F;

	void Start()
	{
		boatWakeRotation=boatWake.transform.eulerAngles;
		GetComponent<Rigidbody>().mass = mass;
	}

	void FixedUpdate()
	{

		/*transform.position = 
			RotatePointAroundPivot(transform.position,
				transform.parent.position,
				Quaternion.Euler(0, OrbitDegrees * Time.deltaTime, 0));*/

        if( GameController.Instance.currentMode==GameMode.Move){
			if (Mathf.Abs(thrust) > 0.01F)
			{
				if (GetComponent<Rigidbody>().velocity.sqrMagnitude > sqrdSpeedThresholdForDrag)
					GetComponent<Rigidbody>().drag = fastDrag;
				else
					GetComponent<Rigidbody>().drag = slowDrag;
			}
			else
				GetComponent<Rigidbody>().drag = superDrag;

			if (Mathf.Abs(turn) > 0.01F)
			{
				if (GetComponent<Rigidbody>().angularVelocity.sqrMagnitude > sqrdAngularSpeedThresholdForDrag)
					GetComponent<Rigidbody>().angularDrag = fastADrag;
				else
					GetComponent<Rigidbody>().angularDrag = slowADrag;
			}
			else
				GetComponent<Rigidbody>().angularDrag = superADrag;

			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hoverHeight, transform.position.z), hoverHeightStrictness);

			float amountToBank = GetComponent<Rigidbody>().angularVelocity.y * bankAmount;

			bank = Mathf.Lerp(bank, amountToBank, bankSpeed);

			Vector3 rotation = transform.rotation.eulerAngles;
			rotation *= Mathf.Deg2Rad;
			rotation.x = 0F;
			rotation.z = 0F;
			rotation += bankAxis * bank;
			//transform.rotation = Quaternion.EulerAngles(rotation);
			rotation *= Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(rotation);
			boatWake.transform.rotation=Quaternion.Euler(boatWakeRotation.x,boatWake.transform.eulerAngles.y,boatWake.transform.eulerAngles.z);

		}
	}
	public float thrust = 0F;
	public float turn = 0F;

	void Thrust(float t)
	{
		thrust = Mathf.Clamp(t, -1F, 1F);
	}

	void Turn(float t)
	{
		turn = Mathf.Clamp(t, -1F, 1F) * turnSpeed;
	}

	bool thrustGlowOn = false;
	public GameObject boatWake;
	private Vector3 boatWakeRotation;
	float theThrust=1.0f;

	public void OnJoyStickUp(){
		thrust=0.0f;
		turn=0.0f;
		hundle.localRotation=Quaternion.Euler(new Vector3(hundle.localRotation.x,hundle.localRotation.y,0.0f));
		NGUITools.SetActive(boatWake,false);
		playerControl=false;
		AudioController.Stop("boatLoop");
		AudioController.Play("boatOutro");
		AudioController.Stop("boatWave");
	}
	public Transform hundle;
	public void OnJoyStick(Vector2 delta){
		if(delta.x==0.0f && delta.y==0.0f)return;
		hundle.localRotation=Quaternion.Euler(new Vector3(hundle.localRotation.x,hundle.localRotation.y,-(delta.x*60.0f)));
		thrust=delta.y;
		turn=delta.x*turnSpeed;
		if(!boatWake.activeSelf)NGUITools.SetActive(boatWake,true);
		if(!playerControl){
			AudioController.Play("boatLoop");
			AudioController.Play("boatWave");
			playerControl=true;
		}
	}
	void Update ()
	{
        if( GameController.Instance.currentMode==GameMode.Move){
			if(GetComponent<Rigidbody>().isKinematic)GetComponent<Rigidbody>().isKinematic=false;
			theThrust = thrust;

			if (!playerControl)return;


			if (thrust > 0F)
			{
				theThrust *= forwardThrust;
				if (!thrustGlowOn)
				{
					thrustGlowOn = !thrustGlowOn;
					//BroadcastMessage("SetThrustGlow", thrustGlowOn, SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				theThrust *= backwardThrust;
				if (thrustGlowOn)
				{
					thrustGlowOn = !thrustGlowOn;
					//BroadcastMessage("SetThrustGlow", thrustGlowOn, SendMessageOptions.DontRequireReceiver);
				}
			}
			GetComponent<Rigidbody>().AddTorque(Vector3.up * turn * Time.deltaTime);
			GetComponent<Rigidbody>().AddForce(transform.forward * theThrust * Time.deltaTime);
		}else{
			if(!GetComponent<Rigidbody>().isKinematic)GetComponent<Rigidbody>().isKinematic=true;
		}
	}
}