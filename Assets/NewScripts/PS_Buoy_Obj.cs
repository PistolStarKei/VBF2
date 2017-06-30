using UnityEngine;
using System.Collections;

public class PS_Buoy_Obj : MonoBehaviour {

    public BuoyancyParams buoParams;
    public void SetBuoParams(BuoyancyParams buoParams){
        this.buoParams=buoParams;
    }

    //Define private variables
    private float origMass = 0.0f;
    private float origDrag = 0.0f;
    private float origADrag = 0.0f;

    private Vector2 waterForce  = new Vector2(0.0f,0.0f);
     bool  isUnderwater = false;

    public bool handledByLure=false;

    private Rigidbody body;

    void Start () {
        body=gameObject.GetComponent<Rigidbody>();
        origMass = body.mass;
        origDrag = body.drag;
        origADrag = body.angularDrag;

    }

    void Update(){

        if (buoParams.keepAwake && body.IsSleeping()){
                body.WakeUp();
                body.AddForce(-Vector3.up*2.0f,ForceMode.Force);
                body.useGravity = true;
        }


        //Check for underwater
        isUnderwater=CheckIsUnderWater();
       
        if (body.mass < 0.1f) body.mass = 0.1f;

        if (isUnderwater){
            // this obj is under water surface to float
            ApplyBuoForceInWater();
        } else {
            // this obj is over water surface to fall in
            ApplyGravityOnOverWater();
        }

    }

    private void ApplyBuoForceInWater(){
        if (buoParams.buoyancyFactor > 0.0f){
            body.useGravity = false;
            body.AddForce((Vector3.up*buoParams.buoyancyFactor),ForceMode.Acceleration);
        }
        body.mass = buoParams.underwaterMass;
        body.drag = buoParams.underwaterDrag;
        body.angularDrag = buoParams.underwaterADrag;

        //apply water force if applicable
        if (waterForce.x != 0.0f || waterForce.y != 0.0f){
            waterForce.x = Random.Range(waterForce.x,(waterForce.x*0.1f));
            waterForce.y = Random.Range(waterForce.y,(waterForce.y*0.1f));
            vec.x=waterForce.x;
            vec.z= waterForce.y;
            body.AddForce(vec,ForceMode.Force);
        }




    }

    Vector3 vec=Vector3.zero;
    private void ApplyGravityOnOverWater(){
        body.useGravity = true;

        body.mass = origMass;
        body.drag = origDrag;
        body.angularDrag = origADrag;

    }

    private bool CheckIsUnderWater(){
        //Check for underwater

        if( WaterPlane.Instance.isUnderWater((transform.position.y-buoParams.buoyancyOffset))){

            return true;
        }else{
            return false;
        }
    }
}
