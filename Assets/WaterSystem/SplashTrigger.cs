using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplashTrigger : PS_SingletonBehaviour<SplashTrigger> {

   

	void OnTriggerEnter(Collider other ) {
        
        Debug.Log("OnTriggerExit"+other.gameObject.name);
        if( other.gameObject.layer==LayerMask.NameToLayer("Rure")){
            if(FishingStateManger.Instance.currentMode==GameMode.Throwing||FishingStateManger.Instance.currentMode==GameMode.ReelingOnLand)
                SplashAt(other.gameObject.transform.position,other.gameObject.transform.localScale.x/2.0f);
            
            if(!FishingStateManger.Instance.isStateWithin(GameMode.Cast)) LureController.Instance.OnEnterWater();
		}

    }

    public void SplashAt(Vector3 position,float scale){
            AudioManager.Instance.OnLureInWater();
            LureSpawner.Instance.OnJumpInWater(position,scale);
    }



    void OnTriggerExit  ( Collider other) {
        Debug.Log("OnTriggerExit"+other.gameObject.name);
      }

}
