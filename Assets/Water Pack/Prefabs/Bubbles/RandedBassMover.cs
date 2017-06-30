using UnityEngine;
using System.Collections;

public class RandedBassMover : MonoBehaviour {

	// Use this for initialization
	void Start () {
		swimingPositon=transform.position;
		InvokeRepeating("StateManager",0.0f,2.0f);
		wantedRotation=Quaternion.LookRotation(Random.insideUnitSphere);
	}
	bool isSwiming=false;
	// Update is called once per frame
	float dist=0.0f;
	float speed=0.01f;
	Vector3 swimingPositon=Vector3.zero;
	Quaternion wantedRotation=Quaternion.identity;
	bool isTurning=false;
	Vector3 minimum=Vector3.zero;
	Vector3 maximun=Vector3.zero;
	public void StateManager(){
		if (!isSwiming & !isTurning) {
			//@CHUSEN
			
			if (Random.Range (1, 7) == 1) {
				//isSwiming=true;
				//swimingPositon = Random.insideUnitSphere;
				wantedRotation=Quaternion.LookRotation(Random.insideUnitSphere);
				//SwimRandom ();
			}
		}
	}
	void SwimRandom(){
		//anim.Play("bass_bite");
		//anim.Play("bass_turn");
		isTurning=true;
	
	}
	public Animation anim;
	void Swim(){
		
		isSwiming=true;
		anim.Play("bass_swim");
		
		
	}
	public void AfterLook(){
		isTurning=false;
		Swim();
		
		
	}
	void Waite(){
		if(isSwiming)isSwiming=false;
		anim.Play("bass_stay");
		
	}
	float damping=0.01f;
	void Update () {
		if(isSwiming){
			
			dist=Vector3.Distance(gameObject.transform.position,swimingPositon);
			
			if(dist>0.01f){
				if(dist>=0.3f){
					speed=0.05f;
				}else{
					speed=0.01f;
				}
				transform.Translate((swimingPositon-gameObject.transform.position ).normalized * speed, Space.World);
			}else{
				
				wantedRotation = Quaternion.Euler(new Vector3(0.0f,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z)); 
				swimingPositon=Random.insideUnitSphere;
				
				Waite();
			}
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(swimingPositon - transform.position), Time.deltaTime * damping);
			
		
			
		}else{
			
			if(wantedRotation!=Quaternion.identity& !isTurning)transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.time * 0.0001f);
			transform.Translate((swimingPositon-gameObject.transform.position ).normalized * 0.001f, Space.World);
			
		}
	
	}
}
