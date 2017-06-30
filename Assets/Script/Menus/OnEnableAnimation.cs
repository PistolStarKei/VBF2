using UnityEngine;
using System.Collections;

public class OnEnableAnimation : MonoBehaviour {
    
    void OnEnable(){
        Debug.Log("OnEnable");
        gameObject.GetComponent<Animation>().Play();
    }
}
