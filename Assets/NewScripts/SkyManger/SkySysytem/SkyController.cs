////////////////////////////////////////////
///                                      ///
///         RealSky Version 1.4          ///
///  Created by: Black Rain Interactive  ///
///                                      ///
//////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using PathologicalGames;
using DynamicFogAndMist;
public enum TimeOfDay {MORNIG,DAY,YU,NIGHT,EMORNING};
public enum FogDensity {NONE,LIGHT,HEAVY};

public class SkyController : PS_SingletonBehaviour<SkyController> {

	public Color[] skyColours;

    public Light sun;
    public void UpdateSky(){
        switch( EnvManager.Instance.skyParams.time){
            case TimeOfDay.EMORNING:
                if( !EnvManager.Instance.skyParams.isCloudy){SetSkyTex("Sky/M",0);}else{SetSkyTex("Sky/M_C",1);}
                break;
            case TimeOfDay.MORNIG:
            if( !EnvManager.Instance.skyParams.isCloudy){SetSkyTex("Sky/M2",0);}else{SetSkyTex("Sky/M2_C",1);}
                break;
            case TimeOfDay.DAY:
            if( !EnvManager.Instance.skyParams.isCloudy){SetSkyTex("Sky/D",2);}else{SetSkyTex("Sky/D_C",3);}
                break;
            case TimeOfDay.YU:
            if( !EnvManager.Instance.skyParams.isCloudy){SetSkyTex("Sky/Y",4);}else{SetSkyTex("Sky/Y_C",5);}
                break;
            case TimeOfDay.NIGHT:
            if( !EnvManager.Instance.skyParams.isCloudy){SetSkyTex("Sky/N",6);}else{SetSkyTex("Sky/N_C" ,7);}
                break;
        }

        Debug.Log("時間"+EnvManager.Instance.skyParams.time);
        //gameObject.transform.localPosition=new Vector3(0.0f,-31.3f,55.65002f);
        //gameObject.transform.localEulerAngles=new Vector3(0.0f, 246.4f,89.99994f);
        skySpeed= EnvManager.Instance.skyParams.windSpeed;

        Debug.Log("風"+EnvManager.Instance.skyParams.windSpeed);

        if( EnvManager.Instance.skyParams.isRainy){
            DespawnRain();
            SetRain();
        }else{
            DespawnRain();
        }
        Debug.Log("フォグ"+EnvManager.Instance.skyParams.fogDensity);
        switch( EnvManager.Instance.skyParams.fogDensity){
             case FogDensity.NONE:
                fog.enabled=false;
                break;
             case FogDensity.LIGHT:
                fog.enabled=true;
                 fog.noiseStrength=0.717f;
                break;
              case FogDensity.HEAVY:
                fog.enabled=true;
                fog.noiseStrength=0.0f;
                break;
        }

    }
    public DynamicFog fog;
    public Transform pre_rain;
    Transform current ;
    string poolName = "Non_GUI_Effects";

    void DespawnRain(){
        if(current!=null) PoolManager.Pools[poolName].Despawn(current);

    }
    void SetRain(){
        current = PoolManager.Pools[poolName].Spawn(pre_rain);
        current.transform.localRotation=Quaternion.Euler(90.0f,0.0f,0.0f);
        current.transform.localPosition=new Vector3(0.0f,9.41f,0.0f);
    }

    void SetSkyTex(string name,int colNum){
        sun.color=skyColours[colNum];
        Texture2D tex = Resources.Load(name) as Texture2D;
        GetComponent<Renderer>().material.mainTexture=tex;
    }
    void SetSkyRotationVector(bool isLeft){

        if(isLeft){
            WindOrientation=Vector3.down;
        }else{
            WindOrientation=-Vector3.down;
        }
    }
    float skySpeed = 0.1f;
    Vector3 WindOrientation=-Vector3.down;
	void FixedUpdate(){
        
		transform.Rotate(WindOrientation * Time.deltaTime * skySpeed, Space.World);

	}

    public void MoveWithPlayer(Transform playerPos){
        transform.position=new Vector3(playerPos.position.x,transform.position.y,playerPos.position.z);

    }






	
}