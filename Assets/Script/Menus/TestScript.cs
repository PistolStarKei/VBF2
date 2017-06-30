using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

    public float sped=0.0f;
    public Transform target;

    Vector3 GetDirection(){
        dires=(target.position-transform.position).normalized;
        Debug.Log("GetD"+dires);
        return dires;
    }
    Vector3 dires;
    void Update(){
        transform.position += GetDirection() * Time.deltaTime * sped;
    }
}
