using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    public Transform target;
    public float front=0f;
    public float right=0f;
    public float up=0f;
    void Update(){
        
        transform.position = target.position+(Player.Instance.gameObject.transform.up*up)+(Player.Instance.gameObject.transform.forward*front)+(Player.Instance.gameObject.transform.right*right);

        transform.LookAt(target);
    }
}
