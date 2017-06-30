using UnityEngine;
using System.Collections;
using System;
using PathologicalGames;

public class LureSpawner :PS_SingletonBehaviour< LureSpawner > {

    public Transform TraleBass;
    public void OnTraleBass(Transform bass,float scale){
        Transform trans=  PoolManager.Pools["Non_GUI_Effects"].Spawn(TraleBass);
        trans.gameObject.GetComponent<FollowingTargetHamon>().Follow(bass,scale*.5f);

    }

    public Transform SplashTraleBass;
    public void OnSplashTraleBass(Transform bass,float scale){
        Transform trans=  PoolManager.Pools["Non_GUI_Effects"].Spawn(SplashTraleBass);

        trans.gameObject.GetComponent<ParticleScaler>().AffectScale(scale*.3f);
        trans.gameObject.GetComponent<FollwingTargetFX>().Follow(bass,scale*.5f);

    }

    public void OnBassJumpInWater(Vector3 position,float scale){
        AudioManager.Instance.OnBassJumpInWater();
        Transform trans=  PoolManager.Pools["Non_GUI_Effects"].Spawn(JumpInWater);
        trans.position=new Vector3(position.x,0.2f,position.z);
        trans.gameObject.GetComponent<ParticleScaler>().AffectScale(Mathf.Lerp(scale*1.2f,.1f,2.0f));

    }

    public Transform PopWater;
    public void OnPopWater(Vector3 position){
        Transform trans=  PoolManager.Pools["Non_GUI_Effects"].Spawn(PopWater);
        trans.position=new Vector3(position.x,0.2f,position.z);

        trans.gameObject.GetComponent<ParticleScaler>().AffectScale( UnityEngine.Random.Range(.3f,0.5f));
       
    }



    public Transform JumpInWater;
    public void OnJumpInWater(Vector3 position,float scale){
        Transform trans=  PoolManager.Pools["Non_GUI_Effects"].Spawn(JumpInWater);
        trans.position=new Vector3(position.x,0.2f,position.z);

        trans.gameObject.GetComponent<ParticleScaler>().AffectScale(scale);

    }

    public Transform SplashTrale;
    public void OnSplashTrale(Vector3 position,float scale){
        Transform trans=  PoolManager.Pools["Non_GUI_Effects"].Spawn(SplashTrale);
        trans.position=new Vector3(position.x,0.2f,position.z);

        trans.gameObject.GetComponent<ParticleScaler>().AffectScale(scale);
    }

    public Transform LureInWater;
    public void OnInWater(Vector3 position,float scale){
        Transform trans=  PoolManager.Pools["Non_GUI_Effects"].Spawn(LureInWater);
        trans.position=new Vector3(position.x,0.0f,position.z);

        trans.gameObject.GetComponent<ParticleScaler>().AffectScale(scale);
    }
    public float testScale=0.1f;
    public Transform testPos;
    public void Test(){
        Debug.Log("Tet");
        OnTraleBass(testPos,testPos.lossyScale.x);
    }
   public void SpawnLure(string name){
        if(LureController.Instance.lureParams!=null){

            if(LureController.Instance.lureParams.gameObject.name=="Hard/"+name){
                Debug.LogError("すでに装備されているのと同じ");
                return;
            }

           
        }
        StartCoroutine(SpawnLureInvoke("Hard/"+name));
    }
    public void SpawnLure(string name,int rigNum){
        if(LureController.Instance.lureParams!=null){
            
            if(LureController.Instance.lureParams.gameObject.name=="Soft/"+name+Constants.RigDatas.RigID[rigNum]){
                Debug.LogError("すでに装備されているのと同じ");
                return;
            }
        }
        StartCoroutine(SpawnLureInvoke("Soft/"+name+Constants.RigDatas.RigID[rigNum]));
    }
    IEnumerator SpawnLureInvoke(string name){
       

        Debug.Log("Spawn"+name);

        if(LureController.Instance.lureParams!=null){
            Destroy(LureController.Instance.lureParams.gameObject);
        }

        yield return null;
        GameObject pre=Resources.Load("Lures/"+name) as GameObject;

        if(pre!=null){
            GameObject sp=Instantiate(pre) as GameObject;
            LureMover mov=sp.gameObject.GetComponent<LureMover>();
            if(mov!=null){
                LureController.Instance.mover=mov;
                mov.lureController=LureController.Instance;
            }
            LureParams obj=sp.gameObject.GetComponent<LureParams>();
            if(obj!=null){
                sp.transform.SetParent(LureController.Instance.gameObject.transform,false);
                sp.name=name;
                sp.transform.localPosition=obj.spawnPosInLocal;
                sp.transform.localRotation=Quaternion.Euler(new Vector3(0.0f,-90.0f,0.0f));
                LureController.Instance.lureOBJ=sp;

                LureController.Instance.lureParams=obj;
                LureController.Instance.OnLureChamged();
                Debug.Log("Instantiate successed"+sp.transform.position);
            }else{
                Debug.LogError("Instantiate failed");
            }
        }else{
            Debug.LogError("Instantiate failed");
        }

    }



    public Transform bassObj;
    public Bass SpawnBassOnPoint( BassParameters paramsb){
        Transform trans=  PoolManager.Pools["Non_GUI_Effects"].Spawn(bassObj);
        Bass ba=trans.gameObject.GetComponent<Bass>();
        if(ba!=null)
        {
            
            ba.Init(paramsb);
            return ba;
        }
        return null;
    }

    public void DespawnBass(Transform trans){
        PoolManager.Pools["Non_GUI_Effects"].Despawn(trans);
    }
}
