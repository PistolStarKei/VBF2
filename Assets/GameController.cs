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
public enum WAVETYPE{STILL,NORMAL,TALL,NONE};
public enum WAVETYPE_COLOR{CLEAR,BLUE,SAND,GREEN};
[System.Serializable]
public class WaveParames{
	public float waveSpeed;
	//0.0が透明　1.0がマッディ
	public float waveClearness;
	public WAVETYPE waveType=WAVETYPE.NONE;
	public WAVETYPE_COLOR waveType_color=WAVETYPE_COLOR.CLEAR;
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
	public WaveParames waveParams;
	public float BottomDepth = 5.0f;
	public float BassVisibleDepth = -3.0f;

	public WeatherType GetCurrentWeather(){
		WeatherType wea=WeatherType.Sunny;

		if(skyParams.isRainy){
			wea=WeatherType.Rain;
		}else{
			if(skyParams.isCloudy){
				wea=WeatherType.Cloudy;
			}else{
				wea=WeatherType.Sunny;
			}
		}

		return wea;
	}

	//1-10
	public int GetEnvFactor(){


		//天候とルアーとの一致
		if(GameController.Instance.skyParams.isRainy){
			//雨の日
		}
		switch(GameController.Instance.waveParams.waveType){
		case WAVETYPE.NONE:
			break;
		case WAVETYPE.NORMAL:
			break;
		case WAVETYPE.STILL:
			break;
		case WAVETYPE.TALL:
			break;
		}

		if(GameController.Instance.waveParams.waveClearness<=0.2f){

		}
		//LineScript.Instance.SetLineWidth();

		//ラインの太さがどう影響するか？

		return 0;
	}



	//0-100 50は中間値 ルアーカラーに影響するパラメータ
	public int GetInWaterBrightness(){
		//水の色　1０％
		int num=50;

		//時間と天候　20％
		int num3=0;
		switch(GameController.Instance.skyParams.time){
		case TimeOfDay.MORNIG:
			//天気　10％ よければー　悪ければ＋に
			if(GetCurrentWeather()==WeatherType.Sunny){
				num=55;
			}else{
				num=60;
			}
			num3=40;
			break;
		case TimeOfDay.DAY:
			num3=50;
			break;
		case TimeOfDay.YU:
			//天気　10％ よければー　悪ければ＋に
			if(GetCurrentWeather()==WeatherType.Sunny){
				num=65;
			}else{
				num=70;
			}
			num3=30;
			break;
		case TimeOfDay.NIGHT:
			num3=10;
			num=90;
			break;

		}

		float val=num3/100.0f;
		//水の色　15％ よければー　悪ければ＋に
		if(GameController.Instance.waveParams.waveType_color==WAVETYPE_COLOR.SAND){
			num+=((int)((val-GameController.Instance.waveParams.waveClearness)*100.0f));
		}else if(GameController.Instance.waveParams.waveType_color==WAVETYPE_COLOR.GREEN){
			num+=((int)((val-GameController.Instance.waveParams.waveClearness)*100.0f));
		}else if(GameController.Instance.waveParams.waveType_color==WAVETYPE_COLOR.BLUE){
			num+=((int)((val-GameController.Instance.waveParams.waveClearness)*100.0f));
		}else if(GameController.Instance.waveParams.waveType_color==WAVETYPE_COLOR.CLEAR){
			num+=((int)((val-GameController.Instance.waveParams.waveClearness)*100.0f));
		}

		num=PSGameUtils.ClampInte(num,0,100);

		return num;
	}

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

		//水質の初期設定
		Debug.LogError("ここで水のパラメータをまずは設定すること ");
		/*
		 int windPow=(int)((LakeEnvironmentalParamas.Instance.weather.WindDirection.x*PointParameters.Instance.WindMiness.x)
            +(LakeEnvironmentalParamas.Instance.weather.WindDirection.z*PointParameters.Instance.WindMiness.z));
        if(windPow>8)windPow=8;
        EnvManager.Instance.waveParams.waveType=getWaveType(windPow);

        //still normal tall none
        EnvManager.Instance.waveParams.waveClearness=GetWaterCleaness();
        //透明度が高いほど、見える -4 - 0
        if(PointParameters.Instance.depth>-1.0f){
            EnvManager.Instance.BassVisibleDepth=-1.0f;
        }else{
            if(PointParameters.Instance.depth<-7.0f){
                EnvManager.Instance.waveParams.waveType_color=WAVETYPE_COLOR.BLUE;
            }
            EnvManager.Instance.BassVisibleDepth=(0.4f*(EnvManager.Instance.waveParams.waveClearness*10.0f))-4.0f;
        }
		 */
		WaterController.Instance.SetWater(waveParams.waveClearness,waveParams.waveType_color, waveParams.waveSpeed, waveParams.waveType);
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
