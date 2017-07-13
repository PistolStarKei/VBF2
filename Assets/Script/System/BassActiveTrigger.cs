using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BassActiveTrigger : MonoBehaviour {

    public List<Bass> bassList=new List<Bass>();

    public void DisableAllBass(){
        foreach(Bass bs in bassList){
            if(GameController.Instance.currentFightingBass!=bs)bs.SetActiveBass(false);
        }
    }
    public void AbleAllBass(){
        foreach(Bass bs in bassList){
            bs.SetActiveBass(true);
        }
    }

    void OnTriggerEnter(Collider other) {
        //if(GameController.Instance.currentMode==GameMode.Move){
            Debug.Log(""+other.name);
            if(other.gameObject.layer==LayerMask.NameToLayer("Bass")){
                bassList.Add( other.gameObject.GetComponent<Bass>());
                other.gameObject.GetComponent<Bass>().OnEnterEnableTrigger(true);
            }
        //}
    }
    void OnTriggerExit(Collider other) {
        Debug.Log(""+other.name);
        //if(GameController.Instance.currentMode==GameMode.Move){
            if(other.gameObject.layer==LayerMask.NameToLayer("Bass")){
                bassList.Remove( other.gameObject.GetComponent<Bass>());
                other.gameObject.GetComponent<Bass>().OnEnterEnableTrigger(false);
            }
       // }
    }
}
