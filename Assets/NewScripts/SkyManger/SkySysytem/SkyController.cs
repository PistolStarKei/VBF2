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

	bool isRainy=false;
	FogDensity fogDensity;

	public void SetSky(TimeOfDay timeOfDay,bool isCloudySky,bool isRainy,FogDensity fogType){
		//空のテクスチャをアプライ
		SetSkyTexture(timeOfDay,isCloudySky);

		//雨パーティクルの設定
		DespawnRain();
		this.isRainy=isRainy;


		//フォグの設定
		SetFog(fogType);

	}

	void SetFog(FogDensity foge){
		fog.enabled=false;
		switch(foge){
		case FogDensity.NONE:
			fog.enabled=false;
			break;
		case FogDensity.LIGHT:
			fog.noiseStrength=0.0f;
			fog.enabled=true;
			break;
		case FogDensity.HEAVY:
			fog.noiseStrength=0.717f;
			fog.enabled=true;
			break;
		}
		fogDensity=foge;
	}
	void SetSkyTexture(TimeOfDay timeOfDay,bool isCloudySky){
		switch(timeOfDay){
			case TimeOfDay.EMORNING:
				if( !isCloudySky){SetSkyTex("Sky/M",0);}else{SetSkyTex("Sky/M_C",1);}
				break;
			case TimeOfDay.MORNIG:
				if( !isCloudySky){SetSkyTex("Sky/M2",0);}else{SetSkyTex("Sky/M2_C",1);}
				break;
			case TimeOfDay.DAY:
				if( !isCloudySky){SetSkyTex("Sky/D",2);}else{SetSkyTex("Sky/D_C",3);}
				break;
			case TimeOfDay.YU:
				if( !isCloudySky){SetSkyTex("Sky/Y",4);}else{SetSkyTex("Sky/Y_C",5);}
				break;
			case TimeOfDay.NIGHT:
				if( !isCloudySky){SetSkyTex("Sky/N",6);}else{SetSkyTex("Sky/N_C" ,7);}
				break;
		}
	}


	void Awake(){
		sun=gameObject.GetComponentInChildren<Light>();	
		fog=Camera.main.gameObject.GetComponent<DynamicFog>();
	}




	public Color[] skyColours;
    Light sun;
   /* public void UpdateSky(){
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
   */
	DynamicFog fog;
    public Transform pre_rain;
    Transform current ;
    string poolName = "Non_GUI_Effects";

    void DespawnRain(){
		if(current!=null) {
			PoolManager.Pools[poolName].Despawn(current);
			isRainEffect=false;
			current=null;
		}

    }
	bool isRainEffect=false;
    void SetRain(){
		if(current==null) {
	        current = PoolManager.Pools[poolName].Spawn(pre_rain);
	        current.transform.localRotation=Quaternion.Euler(90.0f,0.0f,0.0f);
	        current.transform.localPosition=new Vector3(0.0f,9.41f,0.0f);
			isRainEffect=true;
		}else{
			Debug.Log("雨をだせす");
		}
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

    public float skySpeed = 0.1f;
    Vector3 WindOrientation=-Vector3.down;
	void FixedUpdate(){
		transform.Rotate(WindOrientation * Time.deltaTime * skySpeed, Space.World);
		if(GameController.Instance.skyParams.isRainy!=this.isRainy){
			this.isRainy=GameController.Instance.skyParams.isRainy;
			if(isRainy!=isRainEffect){
				Debug.Log("雨パラメータが変更された");
				if(isRainy){
					Debug.Log("雨をだす");
					SetRain();
				}else{
					Debug.Log("雨を消す");
					DespawnRain();
				}

			}
		}

		if(GameController.Instance.skyParams.fogDensity!=fogDensity){
			fogDensity=GameController.Instance.skyParams.fogDensity;
			SetFog(fogDensity);
		}
	}
    public void MoveWithPlayer(Transform playerPos){
        transform.position=new Vector3(playerPos.position.x,transform.position.y,playerPos.position.z);
    }
}