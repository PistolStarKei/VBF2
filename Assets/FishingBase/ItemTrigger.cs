using UnityEngine;
using System.Collections;

public class ItemTrigger : MonoBehaviour {

    void Start(){
        
    }
    bool isRight=true;
    void Update () {
        if(isRight){
            transform.Translate(Vector3.right*0.02f);
            if( transform.position.x>-15.0f){
                isRight=false;
        }
        }else{
            transform.Translate(-Vector3.right*0.02f);
            if( transform.position.x<-19.0f){
                isRight=true;
            }
        }
	}
	
	// Update is called once per frame
    void OnTriggerEnter (Collider col) {
        Debug.Log(""+col.gameObject.name);
	}
}
