using UnityEngine;
using System.Collections;

[System.Serializable]
public class SKY_PARAMS{
	public void SetTimeOfDay(FieldTime ft,FogDensity fog){
		fogDensity=FogDensity.NONE;
		switch(ft){
		case FieldTime.EMorning:
			time=TimeOfDay.EMORNING;
			fogDensity=fog;
			break;
		case FieldTime.Morning:
			time=TimeOfDay.MORNIG;
			break;
		case FieldTime.Noon:
			time=TimeOfDay.DAY;
			break;
		case FieldTime.AfterNoon:
			time=TimeOfDay.DAY;
			break;
		case FieldTime.Evening:
			time=TimeOfDay.YU;
			break;
		case FieldTime.Night:
			time=TimeOfDay.NIGHT;
			break;
		}
	}
	public TimeOfDay time=TimeOfDay.DAY;
	public bool isCloudy=false;
	public bool isRainy=false;
	public float windSpeed=0.1f;
	public FogDensity fogDensity=FogDensity.NONE;
}

public class GameController : PS_SingletonBehaviour<GameController> {


	void Awake () {
		//Startの前に呼ばれる
		Debug.Log("Awake ");


	}

	public bool isPoolMode=false;
	public float time=0.0f;
	public bool isTimeLimitMode=false;
	bool isTimerEnabled=false;
	void SetTimeLimit(float limitTime){
		time=limitTime;
		isTimeLimitMode=true;
		isTimerEnabled=true;
	}



	public SKY_PARAMS skyParams;
	// Use this for initialization
	void Start () {
		Debug.Log("Start ");

		//水中カメラの無効化
		if(isPoolMode)InWaterCamera.Instance.Activate(false);
	
		//タイムリミットの設定 (マルチプレイヤの時だけ)
		//SetTimeLimit();


		//空の設定

		//EnvManager.Instance.skyParams.SetTimeOfDay(LakeEnvironmentalParamas.Instance.weather.fieldTime,PointParameters.Instance.FogOnMorning);
		//EnvManager.Instance.currentWeather=LakeEnvironmentalParamas.Instance.weather.GetTodayWeather();

		Debug.LogError("ここで空のパラメータをまずは設定すること ");
		SkyController.Instance.SetSky(skyParams.time,skyParams.isCloudy,skyParams.isRainy,skyParams.fogDensity);

	}
	
	// Update is called once per frame
	void Update () {
		if(isTimeLimitMode){
			if(isTimerEnabled){
				time-=Time.deltaTime;
				if(time<0.0f){
					isTimerEnabled=false;
					OnTimeUp();
				}
			}
		}
	
	}


	public void OnLureInWater(){
		Debug.Log("OnLureInWater ");
		//水中カメラの有効化
		if(isPoolMode)InWaterCamera.Instance.Activate(true);
	}

	public void OnLureKaishu(){
		Debug.Log("OnLureKaishu ");
		//水中カメラの無効化
		if(isPoolMode)InWaterCamera.Instance.Activate(false);
	}



	void OnTimeUp(){
		Debug.Log("OnTimeUp ");
	}

	void OnPause(){
		Debug.Log("OnPause ");
	}

	void OnResume(){
		Debug.Log("OnResume ");
	}

	void OnDestroy() {
		Debug.Log("OnDestroy ");
	}







	bool isPaused = false;
	void OnApplicationPause(bool pauseStatus)
	{
		isPaused = pauseStatus;
		if(isPaused){
			OnPause();
		}else{
			OnResume();
		}
	}
}
