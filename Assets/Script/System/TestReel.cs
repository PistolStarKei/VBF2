using UnityEngine;
using System.Collections;

public class TestReel : MonoBehaviour {

    public Transform rotater;
    public float speed=0.0f;
    void LateUpdate()
    {
            rotater.Rotate( 0.0f,0.0f, -(Time.deltaTime*(500.0f+(speed*200.0f))));
    }
}
