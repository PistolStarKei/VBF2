using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum StructureType{OpenWater,Rock,Tree,Weed,Lyry,Kui,Gogan,Liver,Sanbashi,Overhang,Bottom,Ashi,Haisuikou};
public enum Baits{Zarigani,Shrimp,KonchuOnWater,Bait,SmallBait,BigBait,Flog,Mimizu};
[System.Serializable]
public class PointData{
    public StructureType type;

    public float depth;
    public List<Baits> baits=new List<Baits>();


}
public class PointStructureObject : MonoBehaviour {

    public PointData data;

    public void GetAllPointInArea(){
        RandomPosition pos;
        foreach(Transform go in transform){
            pos=go.gameObject.GetComponent<RandomPosition>();
             
            if(pos!=null){
                pos.GetColliderBounds();
                colliders.Add(pos);
            }
        }
    }

    [HideInInspector]
    public List<RandomPosition> colliders=new List<RandomPosition>();


    public Vector3 GetRandomPointInArea(){
        if(colliders.Count==0){
            Debug.Log("REctがセットされていない！　先にGetAllPointInArea()を呼べ");
            return Vector3.zero;
        }
        Vector3 rndPosWithin;
        int index=0;
        index=Random.Range(0,colliders.Count);
        rndPosWithin=colliders[index].GetRandomPointInArea();

        if(rndPosWithin.y<-PointStructures.Instance.depth)rndPosWithin.y=-PointStructures.Instance.depth;
        if(rndPosWithin.y>0.0f)rndPosWithin.y=0.0f;

        return  rndPosWithin;
    }


}
