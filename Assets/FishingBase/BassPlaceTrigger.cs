using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BassPlaceTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(this.gameObject.layer!=18){
			this.gameObject.layer=18;
		}
	}
	List<GameObject> basses= new List<GameObject>();
	List<PointObject> points= new List<PointObject>();


	// Update is called once per frame
	void OnTriggerEnter(Collider other) {
		Debug.Log("Detect Point");

		PointObject po=other.gameObject.GetComponent<PointObject>();
		points.Add(po);

		//points.Clear();
	}

	public void Detectpoint(){
		this.gameObject.SetActive(true);
		StartCoroutine("DetectInvoke");
	}
	IEnumerator DetectInvoke(){
		yield return new WaitForSeconds(1.0f);
		Debug.Log(points.Count.ToString()+"Point Detcted");
	}
	public void HideTrigger(){
		this.gameObject.SetActive(false);
	}
}
