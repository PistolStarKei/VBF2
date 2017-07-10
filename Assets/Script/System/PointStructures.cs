using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

public class PointStructures : PS_SingletonBehaviour<PointStructures> {

    public float depth=0.0f;
    public float cleaness=0.0f;
    public Vector3 windAvoidFactor;
    public List<PointStructureObject> areas=new List<PointStructureObject>();

    void Awake(){
        //初期化処理
        foreach(Transform go in transform){
            if(go.gameObject.GetComponent<PointStructureObject>()!=null) {
                areas.Add(go.gameObject.GetComponent<PointStructureObject>());
                go.gameObject.GetComponent<PointStructureObject>().GetAllPointInArea();
            }
        }
    }
    void Start(){
        GameController.Instance.BottomDepth=this.depth;
        Debug.LogWarning("ここでクリアねすなどから、水とバスが見える深さをセットすること");

        SortByEnv();
        Debug.LogWarning("ここでバスをスポーンすること");
        allBassInAreas.Add( Instanciate(areas[0].GetRandomPointInArea(),40.0f,BassRange.Top,100,100,EatType.Anger,new Baits[]{Baits.Shrimp}));
       
    }

    public void SortByEnv(){
        //シェード効果の有無
        //水温安定　温水
        //主なベイト
        //スポーニングエリアか？

    }

    public List<Bass> allBassInAreas=new List<Bass>();
    public void DespawnAllBass(){
        foreach(Bass ba in allBassInAreas){
            LureSpawner.Instance.DespawnBass(ba.gameObject.transform);
        }
    }

    public Bass Instanciate(Vector3 pos,float size,BassRange range,int kassei,int sure,EatType type,Baits[] baits){
       //Instantiate(ObjectToSpawn, areas[0].GetRandomPointInArea(), transform.rotation);

        BassParameters paramsb=new BassParameters();
        paramsb.size=size;
        paramsb.spawnedPosiion=pos;
        if(paramsb.spawnedPosiion.y>depth){
            paramsb.spawnedPosiion.y=depth;
        }
        paramsb.range=range;
        paramsb.KASSEILEVEL=kassei;
        paramsb.SURELEVEL=sure;
        paramsb.eatType=type;
        paramsb.bate=baits;
        return LureSpawner.Instance.SpawnBassOnPoint(paramsb);

    }

    public RandomPosition openWater;
}
