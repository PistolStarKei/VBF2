using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class Tester : MonoBehaviour {

    public Transform lureOBJ;
    public Transform dummyTrans;

    public void ObjToDefaultRotJurked(Vector3 offset){
        lureOBJ.transform.localRotation=Quaternion.Slerp(lureOBJ.transform.localRotation,
            Quaternion.Euler(lureOBJ.transform.InverseTransformDirection(offset)),Time.deltaTime);


    }
    public Vector3 GetDirection(){
        dummyTrans.localPosition=Vector3.zero;
        dummyTrans.localRotation=lureOBJ.transform.localRotation;

        dummyTrans.LookAt(RodController.Instance.rodTip.transform.position);

        return dummyTrans.forward;
    }

    public void RotateSlerp(float speed){
        transform.rotation=Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(tempVec),Time.deltaTime*speed);

    }

    public void RotateObjSlerp(float speed){
        lureOBJ.transform.forward=dummyTrans.forward;
        return;
        lureOBJ.transform.rotation=Quaternion.Slerp(lureOBJ.transform.rotation,
            Quaternion.LookRotation(transform.forward,up),Time.deltaTime*speed);

    }
    public Vector3 up;
    void Update(){
     

        tempVec= GetDirection();
        RotateSlerp(1.0f);
        RotateObjSlerp(1.0f);
    }
    public Vector3 tempVec;
    void OnDrawGizmos(){
        Gizmos.DrawLine(transform.position,transform.forward);
    }

   
}
