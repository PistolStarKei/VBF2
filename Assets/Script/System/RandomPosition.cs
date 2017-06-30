using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RandomPosition : MonoBehaviour {

    public BoxCollider col;
    void DestroyCollider(){
        Destroy(col);
        col=null;

    }
    void MatchScale(){
        col.size=new Vector3(col.size.x*transform.lossyScale.x,col.size.y*transform.lossyScale.y,col.size.z*transform.lossyScale.z);
        transform.localScale=Vector3.one;
    }
    Vector3 center;
    Vector3 size;
    public void GetColliderBounds(){
        if(col!=null){
            MatchScale();
           
            center=transform.InverseTransformPoint(col.bounds.center);
            size=col.size/2.0f;

            DestroyCollider();
        }
    }
    public Vector3 GetRandomPointInArea(){
        Vector3 rndPosWithin=transform.TransformPoint(center+new Vector3(Random.Range(-size.x, size.x), 
            Random.Range(-size.y, size.y), 
            Random.Range(-size.z, size.z)));;
        return rndPosWithin;
    }

}
