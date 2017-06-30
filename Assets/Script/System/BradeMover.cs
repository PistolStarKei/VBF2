using UnityEngine;
using System.Collections;

public class BradeMover : MonoBehaviour {

  
    public Vector3 lookDirection;
    Vector3 tempVec;
    bool isLeft=false;
    public float noTensionSpeed=2.0f;
    public float bradeSpeed=20.0f;
    public void ActLure(float movingBodyPow,bool isNotention){

        //Debug.Log("ActLure"+movingBodyPow);
        if(movingBodyPow<0.02f){
            isNotention=false;
        }
        if(isNotention){
            if(tempVec.z<lookDirection.z)tempVec.z+=Time.deltaTime*noTensionSpeed;
        }else{
            if(tempVec.z>0.0f){
                tempVec.z-=Time.deltaTime*movingBodyPow*bradeSpeed;
            }else{
                tempVec.z=0.0f;
            }
            if(isLeft){
               
                tempVec.y+=Time.deltaTime*movingBodyPow*bradeSpeed;
                if(tempVec.y>lookDirection.y){
                    isLeft=isLeft?false:true;
                }
            }else{
                tempVec.y-=Time.deltaTime*movingBodyPow*bradeSpeed;
                if(tempVec.y<-lookDirection.y){
                    isLeft=isLeft?false:true;
                }
            }
        }


        transform.localRotation= Quaternion.Euler(tempVec);
    }
}
