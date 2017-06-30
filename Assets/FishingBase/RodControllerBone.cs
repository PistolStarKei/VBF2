using UnityEngine;
using System.Collections;

public class RodControllerBone : MonoBehaviour {

	public Transform[] Bones;
	void Start(){
		prevEuler = Bones [0].localEulerAngles;
		prevEuler2 = Bones [0].localEulerAngles;
	}
	Vector3 prevEuler=Vector3.zero;
	Vector3 prevEuler2=Vector3.zero;
	Vector3 euler;
	void BendRod(Vector3 angle){
		if (angle == prevEuler)return;
		Bones [0].localRotation= Quaternion.Euler(angle);
		euler=Bones [0].localEulerAngles-prevEuler2;
		for(int i=1;i<Bones.Length;i++){
			Bones [i].localRotation= Quaternion.Euler(euler);
		}
		prevEuler = angle;
	}
	public float angleOffSet=0.0f;
	Vector3 prevPos=Vector3.zero;
	private Vector3 relative;
	private float angle;
	void Update(){	
		//face rod to target
       
        relative = transform.InverseTransformPoint( RodController.Instance.rodlookTarget.position);
		angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
		transform.RotateAround(transform.position, transform.forward,angle);
		//Debug.DrawRay(transform.position,transform.forward,Color.blue);
		BendRod (Bones [0].transform.right*angleOffSet);



		return;
        if(LineScript.Instance.lineTention>=0.0f){
			BendRod (Bones [0].transform.right*angleOffSet);

		}else{
			//no tention so streigthen rod pole
			BendRod(prevEuler2);
		}
	}
}
