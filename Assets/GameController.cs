using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

public enum GameMode{Menu,Boat,Move,Cast,Throwing,ReelingOnLand,Reeling,Fight,Result};

public class GameController : SingletonStatefulObjectBase<GameController, GameMode> {



	void Awake () {
		//Startの前に呼ばれる
		Debug.Log("Awake ");


	}
	public string debugLure="";
	public int debugRig=1;
	public bool debugIsSoft=false;


	void Start () {
		Debug.Log("Start ");
		if(isDebugMode){

			//データのロードをする
			DataManger.Instance.LoadData();

			//ルアーの数と、定義データの数が違う時にエラーを吐く
			if(DataManger.Instance.GAMEDATA.lureHas_Hard.Count!=Constants.LureDatas.itemTittles.Length){
				Debug.LogError("ポイントに入ったとき "+DataManger.Instance.GAMEDATA.lureHas_Hard.Count);
				Debug.LogError("ポイントに入ったとき "+Constants.LureDatas.itemTittles.Length);
				Debug.LogError("ポイントに入ったとき ルアーおかしい");
			}

			for(int i=0;i<DataManger.Instance.GAMEDATA.lureHas_Hard.Count;i++){
				DataManger.Instance.GAMEDATA.lureHas_Hard[Constants.LureDatas.itemTittles[i]]=2;
			}

			if(DataManger.Instance.GAMEDATA.lureHas_Soft.Count!=Constants.SoftLureDatas.itemTittles.Length)Debug.LogError("ポイントに入ったとき ルアーおかしい");

			for(int i=0;i<DataManger.Instance.GAMEDATA.lureHas_Soft.Count;i++){
				DataManger.Instance.GAMEDATA.lureHas_Soft[Constants.SoftLureDatas.itemTittles[i]]=2;
			}

			//リグは全て持っている
			DataManger.Instance.GAMEDATA.RigHas[Constants.RigDatas.itemTittles[0]]=true;
			DataManger.Instance.GAMEDATA.RigHas[Constants.RigDatas.itemTittles[1]]=true;
			DataManger.Instance.GAMEDATA.RigHas[Constants.RigDatas.itemTittles[2]]=true;
			DataManger.Instance.GAMEDATA.RigHas[Constants.RigDatas.itemTittles[3]]=true;
			DataManger.Instance.GAMEDATA.RigHas[Constants.RigDatas.itemTittles[4]]=true;
			DataManger.Instance.GAMEDATA.RigHas[Constants.RigDatas.itemTittles[5]]=true;

			/*DataManger.Instance.GAMEDATA.lureHas_Hard[Constants.LureDatas.itemTittles[0]]=2;
            DataManger.Instance.GAMEDATA.lureHas_Hard[Constants.LureDatas.itemTittles[1]]=1;
            DataManger.Instance.GAMEDATA.lureHas_Hard[Constants.LureDatas.itemTittles[2]]=5;
            DataManger.Instance.GAMEDATA.lureHas_Hard[Constants.LureDatas.itemTittles[10]]=2;*/

			/*DataManger.Instance.GAMEDATA.lureHas_Soft[Constants.LureDatas.itemTittles[1]]=5;
			DataManger.Instance.GAMEDATA.lureHas_Soft[Constants.LureDatas.itemTittles[1]]=5;
			DataManger.Instance.GAMEDATA.lureHas_Soft[Constants.LureDatas.itemTittles[1]]=5;
			DataManger.Instance.GAMEDATA.lureHas_Soft[Constants.LureDatas.itemTittles[1]]=5;
			DataManger.Instance.GAMEDATA.lureHas_Soft[Constants.LureDatas.itemTittles[1]]=5;
			DataManger.Instance.GAMEDATA.lureHas_Soft[Constants.LureDatas.itemTittles[1]]=5;*/

			DataManger.Instance.GAMEDATA.lureHas_Rods[Constants.RodsDatas.itemTittles[0]]=1; 
			DataManger.Instance.GAMEDATA.lureHas_Rods[Constants.RodsDatas.itemTittles[1]]=1;
			DataManger.Instance.GAMEDATA.lureHas_Line[Constants.LineDatas.itemTittles[0]]=1;
			DataManger.Instance.GAMEDATA.lureHas_Line[Constants.LineDatas.itemTittles[3]]=1;


			//釣り場用の個数データ
			DataManger.Instance.AffectLureHasTempData();
			DataManger.Instance.GAMEDATA.lureHas_Hard_Temp[Constants.LureDatas.itemTittles[0]]=1;

			DataManger.Instance.GAMEDATA.tackleSlots[0].lineNum=0;


			TackleParams.Instance.currentTackle.name="Rods1";
			if(debugIsSoft){
				TackleParams.Instance.currentTackle.lureNum=Array.IndexOf(Constants.SoftLureDatas.itemTittles,debugLure);
			}else{
				TackleParams.Instance.currentTackle.lureNum=Array.IndexOf(Constants.LureDatas.itemTittles,debugLure);
			}

			DataManger.Instance.GAMEDATA.tackleSlots[0].lureNum=TackleParams.Instance.currentTackle.lureNum;
			DataManger.Instance.GAMEDATA.tackleSlots[0].isSoft=debugIsSoft;

			Debug.LogError("ポイントに入ったとき"+TackleParams.Instance.currentTackle.lureNum);

			DataManger.Instance.GAMEDATA.SettedRig[debugLure]=debugRig;

			DataManger.Instance.GAMEDATA.lureHas_Soft_Temp[Constants.SoftLureDatas.itemTittles[TackleParams.Instance.currentTackle.lureNum]]--; 
			DataManger.Instance.UpdateHasTempData();


			TackleParams.Instance.currentTackle.lineNum=0;
			TackleParams.Instance.currentTackle.isSoft=debugIsSoft;
			DataManger.Instance.UpdateDebugData();
			DataManger.Instance.SaveData();
		}


		//ステートマシンの設定
		stateList.Add(new modeMenu(this));
		stateList.Add(new modeBoat(this));
		stateList.Add(new modeMove(this));
		stateList.Add(new modeCast(this));
		stateList.Add(new modeThrowing(this));
		stateList.Add(new modeReelingOnLand(this));
		stateList.Add(new modeReeling(this));
		stateList.Add(new modeFight(this));
		stateList.Add(new modeResult(this));

		stateMachine = new StateMachineMode<GameController>();

		//ここで環境の設定をしている
		StartCoroutine( CreateEnvironment());

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
	public CastMoveBtn castMoveBtn;
	public void OnCast(bool isCast){
		if(isCast){
			ChangeStateTo(GameMode.Cast);
		}else{
			ChangeStateTo(GameMode.Move);
		}
	}


	IEnumerator UpdateTimeAndEnvironment(){
		
		LakeEnvironmentalParamas.Instance.UpdateFieldTime();
		yield return null;
	}

	IEnumerator CreateEnvironment(){
		WaitAndCover.Instance.ShowWait();
		WaitAndCover.Instance.CoverAll(true);

		//時間と天候を進める
		Coroutine tine=StartCoroutine(UpdateTimeAndEnvironment());
		yield return tine;

		//現在のタックルを反映させる
		tine=StartCoroutine(TackleParams.Instance.AffectCurrrentTackle(true));
		yield return tine;



		Debug.Log("終わり");
		//プレイヤの腕にロッドを設置する
		Player.Instance.SetPlayerState(true);

		//Moveステートへ移行する
		ChangeStateTo(GameMode.Move);
		WaitAndCover.Instance.StopWait();
		WaitAndCover.Instance.UnCoverAll();
	}

	WAVETYPE getWaveType(int wind){

		if(wind<=2){
			return WAVETYPE.STILL;
		}else if(wind<=5){
			return WAVETYPE.NORMAL;
		}
		return WAVETYPE.TALL;

	}


	float GetWaterCleaness(){
		//過去4日の天気で雨続き+Mudだと最大に　　1.0f
		float rainy=0.0f;
		List<WeatherType> wt= LakeEnvironmentalParamas.Instance.weather.weatherRireki;
		int num=0;
		//過去4日の日照りを
		float waterHeat=0.0f;
		foreach(WeatherType wtp in wt){
			if(num>3)break;
			if(wtp==WeatherType.HeavyRain){
				rainy+=0.08f;
				waterHeat-=0.1f;
			}else if(wtp==WeatherType.Rain){
				rainy+=0.02f;
				waterHeat-=0.1f;
			}else if(wtp==WeatherType.Sunny){
				rainy-=0.015f;
				waterHeat+=0.25f;
			}else{
				waterHeat+=0.0f;
				rainy-=0.015f;
			}
			num++;
		}

		//0-0.33
		if(rainy<0.0f)rainy=0.0f;
		//大きいほど強く濁る　0 1 2 3 アオコの影響を受けない
		rainy=rainy*(int)PointParameters.Instance.waterFlow;
		//0-1,0

		//ポイントにベースの濁り
		float val=PointParameters.Instance.baseWaterCleaness;

		//基本はクリア
		GameController.Instance.waveParams.waveType_color=WAVETYPE_COLOR.CLEAR;

		if(rainy>0.0f){
			val+=rainy/((int)PointParameters.Instance.Bottom+1);
		}

		if(PointParameters.Instance.Bottom==BottomType.Mud || PointParameters.Instance.Bottom==BottomType.Sand){

			if(val>0.6f)GameController.Instance.waveParams.waveType_color=WAVETYPE_COLOR.SAND;
		}

		//青子発生
		if(LakeEnvironmentalParamas.Instance.weather.Kion>28 && waterHeat>0.4f){

			if(val>(0.6f-((int)PointParameters.Instance.waterFlow*0.1f))){
				GameController.Instance.waveParams.waveType_color=WAVETYPE_COLOR.GREEN;
			}
		}


		return val;
	}

	public bool isDebugMode=true;

	public Material bassMatInWater;
	public Material bassMatFight;

	public UISlider rodBar;
	public void SetLineDamageOnMax(float angle,float max){
		//more angle more damage
		float num=0.0f;
		if(angle>120.0f)angle=120.0f;

		if(Button_Float.Instance.isDragging){
			rodBar.value=1.0f;
			//LineScript.Instance.SetLineDamage((1.0f/100.0f)*currentFightingBass.parameters.sizeNanido);
			LineScript.Instance.SetLineDamage((1.0f/100.0f));
			return;

		}
		if(currentFightingBass==null){

		}else{
			if(currentFightingBass.fightState==FightState.Tukkomi || currentFightingBass.fightState==FightState.Monkey){

				if(angle>max){
					num= RodController.Instance.bendingPower/1.5f;

					num=num+(((angle-max)/angle)*0.5f);

				}else{
					num= RodController.Instance.bendingPower/1.5f;

				}

			}else{
				num= RodController.Instance.bendingPower/1.5f;
			}
		}
		if(num>0.35f){
			//LineScript.Instance.SetLineDamage((num/100.0f)*currentFightingBass.parameters.sizeNanido);
			LineScript.Instance.SetLineDamage((num/100.0f));
		}
		rodBar.value=num;

	}


	public void BassIsChasing(bool isChased,Transform trans){
		if(isChased){
			currentFightingBass=trans.gameObject.GetComponent<Bass>();
			currentChasingBass=trans;
		}else{
			currentChasingBass=null;
			currentFightingBass=null;
		}

	}
	public void SetCurrentBassJump(){
		if(currentFightingBass!=null){
			if(currentFightingBass.isJumping){
				currentFightingBass.isJumpKaihi=true;
			}
		}
	}
	public Transform currentChasingBass;
	public Bass currentFightingBass;
	public void RemoveChasingBass(){
		if(currentFightingBass!=null){
			currentFightingBass.OnLureKaihsu();
			currentFightingBass=null;
		}
		if(currentChasingBass!=null)currentChasingBass=null;
	}



	public bool isRodEnabled(){
		if(LureController.Instance==null)return false;
		if(currentMode==GameMode.Reeling||currentMode==GameMode.ReelingOnLand ||currentMode==GameMode.Fight){
			return true;
		}else{
			return false;
		}
	}
	public  bool isStateWithin(GameMode mode){
		if((int)currentMode<=(int)mode){
			return true;
		}else{
			return false;
		}
	}
	public bool IsMovingState(){
		if(currentMode==GameMode.Cast  || currentMode==GameMode.Move){
			return true;
		}
		return false;
	}
	public GameMode currentMode=GameMode.Menu;
	public void ChangeStateTo(GameMode to){

		if(currentMode==to){
			Debug.LogError("Same Mode Change");
			return;
		}
		Debug.Log("ChangeState "+currentMode.ToString()+">"+to.ToString());
		//ステートチェンジを判定
		bool canChange=true;
		//set something　{Menu,Move,Cast,Throwing,ReelingOnLand,Reeling,Fight,Result};
		switch(to){
		case GameMode.Menu:
			if(!isStateWithin(GameMode.Cast))canChange=false;
			break;
		case GameMode.Boat:
			if(!isStateWithin(GameMode.Cast))canChange=false;
			break;
		case GameMode.Move:
			if(!isStateWithin(GameMode.Cast))canChange=false;
			break;
		case GameMode.Cast:
			//if(currentMode==GameMode.Throwing)canChange=false;
			break;
		case GameMode.Throwing:
			if(currentMode!=GameMode.Cast)canChange=false;
			break;
		case GameMode.ReelingOnLand:
			if(currentMode==GameMode.Result)canChange=false;
			if(isStateWithin(GameMode.Cast))canChange=false;
			break;
		case GameMode.Reeling:
			if(currentMode==GameMode.Result)canChange=false;
			if(isStateWithin(GameMode.Cast))canChange=false;
			break;
		case GameMode.Fight:
			if(currentMode==GameMode.Result)canChange=false;
			if(isStateWithin(GameMode.Cast))canChange=false;
			break;
		case GameMode.Result:
			if(currentMode!=GameMode.Fight)canChange=false;
			break;
		}

		if(!canChange){
			Debug.LogError("Same Mode Change");
			return;
		}
		ChangeState(to);

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



	private static GameController instance;
	public static GameController Instance {
		get {
			if (instance == null) {
				instance = (GameController)FindObjectOfType(typeof(GameController));

				if (instance == null) {
					//Debug.Log(typeof(T) + "is null");
				}
			}
			return instance;
		}
	}
	public void SetControllers(bool joystick,bool reel,bool cast,bool castmoveMode,bool isMoveMode){
		CastBtn.Instance.ShowCastBtn(cast);
		JoystickFloat.Instance.Show(joystick);
		Button_Float.Instance.Show(reel);
		if(castmoveMode){
			castMoveBtn.SetState(isMoveMode);
		}else{
			castMoveBtn.Hide();
		}



	}
	public TackleBtn tackleBtn;
	public EquipLure tackleMenu;
	private class modeMenu : Mode<GameController>
	{
		//Move Cast のみ
		public modeMenu(GameController owner) : base(owner) {}

		public override void Enter() {
			Debug.Log("Enter modeMenu");
			switch(owner.currentMode){
			case GameMode.Move:
				break;
			case GameMode.Cast:
				break;
			}
			HUD_LureParams.Instance.Hide();
			owner.SetControllers(false,false,false,false,false);
			owner.tackleBtn.SetState(true);

			owner.currentMode=GameMode.Menu;
		}
		public override void Exit() {
			Debug.Log("Exit modeMenu");
		}
	}

	private class modeBoat : Mode<GameController>
	{
		//Move Cast のみ
		public modeBoat(GameController owner) : base(owner) {}

		public override void Enter() {
			Debug.Log("Enter modeBoat");
			switch(owner.currentMode){
				case GameMode.Move:
					break;
				case GameMode.Cast:
					break;
			}
			HUD_LureParams.Instance.Hide();
			owner.SetControllers(false,false,false,false,false);
			owner.tackleBtn.SetState(true);

			owner.currentMode=GameMode.Boat;
		}
		public override void Exit() {
			Debug.Log("Exit modeMenu");
		}
	}


	private class modeMove : Mode<GameController>
	{
		//Menu Cast のみ
		public modeMove(GameController owner) : base(owner) {}

		public override void Enter() {

			if(owner.currentMode==GameMode.Cast)Player.Instance.SetPlayerState(true);
			Debug.Log("Enter modeMove");
			//ロッドとルアー　ラインを非表示
			LineScript.Instance.HideLine();
			HUD_LureParams.Instance.Hide();
			Player.Instance.ActiveBassEnabler(true);
			//bool joystick,bool reel,bool cast,bool castmoveMode,bool isMoveMode
			owner.SetControllers(true,false,false,true,false);
			owner.tackleBtn.SetState(false);
			owner.currentMode=GameMode.Move;
		}
		public override void Exit() {
			Debug.Log("Exit modeMove");
		}
	}




	private class modeCast : Mode<GameController>
	{
		//Menu,Move,Throwing
		public modeCast(GameController owner) : base(owner) {}

		public override void Enter() {
			Debug.Log("Enter modeCast");


			if(owner.currentMode==GameMode.Move)Player.Instance.SetPlayerState(false);
			LineScript.Instance.HideLine();
			RodController.Instance.InitRod();
			LureController.Instance.SetToDefaultPosition(Player.Instance.lureDefaultPos);
			LineScript.Instance.Length=2.5f;
			GameController.Instance.RemoveChasingBass();
			JoystickFloat.Instance.guiMode=GUIMODE.NONE;
			HUD_LureParams.Instance.Hide();
			RodController.Instance.ShowRod(true);
			LureController.Instance.appeal.SetToDefaultState(false);
			Button_Float.Instance.StopReelAudio();
			if((owner.GetisNegakariOrFoockingState()))owner.isNegakariORFoockingState=false;
			if(LureController.Instance.lureParams!=null && !LureController.Instance.isLureActive()){
				Debug.LogError("ルアーを削除");
				if(TackleParams.Instance.currentTackle.lureNum!=-1){
					//装備で減っているのでいらない
					/*if(TackleParams.Instance.currentTackle.isSoft){
                        DataManger.Instance.GAMEDATA.lureHas_Soft_Temp[Constants.SoftLureDatas.itemTittles[TackleParams.Instance.currentTackle.lureNum]]--;
                    }else{
                        DataManger.Instance.GAMEDATA.lureHas_Hard_Temp[Constants.LureDatas.itemTittles[TackleParams.Instance.currentTackle.lureNum]]--;
                    }*/
					if(TackleParams.Instance.currentTackle.name!="") DataManger.Instance.GAMEDATA.tackleSlots[Array.IndexOf(Constants.RodsDatas.itemTittles,TackleParams.Instance.currentTackle.name)].lureNum=-1;

					TackleParams.Instance.currentTackle.lureNum=-1;
				}

				//DataManger.Instance.GAMEDATA.lureHas_Soft_Temp[];
				Destroy(LureController.Instance.lureParams.gameObject);
			}
			switch(owner.currentMode){

			case GameMode.Menu:
				break;
			case GameMode.Throwing:
				//キャストミス
				Debug.LogError("キャストミス ここでペナルティを");
				break;
			case GameMode.ReelingOnLand:
				break;
			case GameMode.Reeling:
				//ルアー回収
				Debug.LogError("ルアー回収");

				break;
			case GameMode.Result:
				//catched
				break;
			case GameMode.Move:
				//catched


				break;
			}


			//bool joystick,bool reel,bool cast,bool castmoveMode,bool isMoveMode
			owner.SetControllers(false,false,true,true,true);
			owner.tackleBtn.SetState(false);
			if(owner.currentMode==GameMode.Throwing ){
				ZoomCamera.Instance.BackCamera();
			}

			//バスを有効にする
			Player.Instance.bassEnable.AbleAllBass();
			//バストリガーを無効に
			Player.Instance.ActiveBassEnabler(false);
			owner.currentMode=GameMode.Cast;

		}
		public override void Exit() {
			Debug.Log("Exit modeCast");
		}
	}
	private class modeThrowing : Mode<GameController>
	{
		//Menu,Move,Cast
		public modeThrowing(GameController owner) : base(owner) {}

		public override void Enter() {
			Debug.Log("Enter modeThrowing");

			JoystickFloat.Instance.guiMode=GUIMODE.NONE;
			owner.isNegakariORFoockingState=false;

			owner.BassIsChasing(false,null);
			Button_Float.Instance.isCovered=false;
			LureController.Instance.SetToDefaultPosition(Player.Instance.lureDefaultPos);


			//bool joystick,bool reel,bool cast,bool castmoveMode,bool isMoveMode
			owner.SetControllers(false,false,false,false,false);
			owner.tackleBtn.Hide();
			HUD_LureParams.Instance.Show();
			owner.currentMode=GameMode.Throwing;
		}
		public override void Exit() {
			Debug.Log("Exit modeThrowing");
		}
	}
	private class modeReelingOnLand : Mode<GameController>
	{
		//Throwing,ReelingOnLand,Reeling,Fight
		public modeReelingOnLand(GameController owner) : base(owner) {}

		public override void Enter() {
			Debug.Log("Enter modeReelingOnLand");
			if(! Button_Float.Instance.isCovered) RodController.Instance.RotateRodToDefault(false);
			JoystickFloat.Instance.guiMode=GUIMODE.ROD;

			LureController.Instance.appeal.SetToDefaultState(true);
			LureController.Instance.isOnLand=true;
			LureController.Instance.OnWater(false);
			LineScript.Instance.InvokeFuke(true,true,false);
			Player.Instance.bassEnable.AbleAllBass();

			if(owner.currentMode==GameMode.Throwing){

				Debug.Log("投げたらそのまま桟橋に");
				//投げたらそのまま桟橋に
				ZoomCamera.Instance.BackCamera();
				//bool joystick,bool reel,bool cast,bool castmoveMode,bool isMoveMode
				owner.SetControllers(true,true,false,false,false);
				owner.tackleBtn.Hide();
			}

			owner.currentMode=GameMode.ReelingOnLand;
		}
		public override void Exit() {
			Debug.Log("Exit modeReelingOnLand");
		}
	}
	private class modeReeling : Mode<GameController>
	{
		//Throwing,ReelingOnLand,Fight
		public modeReeling(GameController owner) : base(owner) {}

		public override void Enter() {
			Debug.Log("Enter modeReeling");
			switch(owner.currentMode){
			case GameMode.Throwing:
				//On Land
				if(! Button_Float.Instance.isCovered) RodController.Instance.RotateRodToDefault(false);
				JoystickFloat.Instance.guiMode=GUIMODE.ROD;

				LureController.Instance.OnWater(true);
				LureController.Instance.appeal.SetToDefaultState(true);

				if(LureController.Instance.mover.SinkingObj){
					LineScript.Instance.InvokeFuke(true,true,true);
				}else{
					LineScript.Instance.InvokeFuke(true,true,false);
				}

				break;
			case GameMode.ReelingOnLand:
				//再び水中に
				if(! Button_Float.Instance.isCovered) RodController.Instance.RotateRodToDefault(false);
				LureController.Instance.OnWater(true);
				break;
			case GameMode.Fight:
				//barashi
				//hide gui
				//visible bass
				break;
			}

			if(owner.currentMode==GameMode.Throwing){
				//bool joystick,bool reel,bool cast,bool castmoveMode,bool isMoveMode
				owner.SetControllers(true,true,false,false,false);
				owner.tackleBtn.Hide();
				ZoomCamera.Instance.BackCamera();
			}


			Player.Instance.bassEnable.AbleAllBass();
			owner.currentMode=GameMode.Reeling;
		}
		public override void Exit() {
			Debug.Log("Exit modeReeling");
		}
	}
	public bool isShowedFightToUser=false;
	private class modeFight : Mode<GameController>
	{
		//Throwing,ReelingOnLand,Reeling
		public modeFight(GameController owner) : base(owner) {}

		public override void Enter() {
			Debug.Log("Enter modeFight");
			owner.isShowedFightToUser=false;
			if(owner.currentMode==GameMode.Reeling){
				TackleParams.Instance.tParams.Params_lineDamage=0.0f;

				GameController.instance.OnBassBite();
			}else{
				Debug.Log("to fight mode from not reelinf");
			}
			Player.Instance.bassEnable.DisableAllBass();

			if(owner.currentMode==GameMode.Throwing){
				//bool joystick,bool reel,bool cast,bool castmoveMode,bool isMoveMode
				owner.SetControllers(true,true,false,false,false);
				owner.tackleBtn.Hide();
				ZoomCamera.Instance.BackCamera();
			}

			owner.currentMode=GameMode.Fight;
		}
		public override void Exit() {
			Debug.Log("Exit modeFight");
		}
	}
	private class modeResult : Mode<GameController>
	{
		//Fightのみ
		public modeResult(GameController owner) : base(owner) {}

		public override void Enter() {
			Debug.Log("Enter modeFight");
			Player.Instance.bassEnable.AbleAllBass();
			owner.currentMode=GameMode.Result;
			HUD_LureParams.Instance.Hide();

			//bool joystick,bool reel,bool cast,bool castmoveMode,bool isMoveMode
			owner.SetControllers(false,false,false,false,false);
			owner.tackleBtn.Hide();
		}
		public override void Exit() {
			Debug.Log("Exit modeFight");
		}
	}



	public void OnNegakari(){
		isNegakariORFoockingState=true;

		JoystickFloat.Instance.OnNegakari();
	}
	public void OnNegakariKaihi(){
		isNegakariORFoockingState=false;

	}
	public void OnNegakariKaihiTimeOver(){
		isNegakariORFoockingState=false;

		WaitAndCover.Instance.ShowInfoPopup(Localization.Get("WarningLostLure"),Info_IC.Warning,OnLostYes);

	}
	public void OnLostYes(){
		if(LureController.Instance.lureParams!=null){
			LureController.Instance.HideLure();
		}
		ChangeStateTo(GameMode.Cast);
	}

	public void OnBassBite(){
		isNegakariORFoockingState=true;
		JoystickFloat.Instance.OnBite();
	}

	public void OnFoocked(){
		isNegakariORFoockingState=false;
	}

	bool isNegakariORFoockingState=false;
	public bool GetisNegakariOrFoockingState(){
		return isNegakariORFoockingState;
	}

	public void OnTerrain(){
		Debug.LogWarning("キャストミス　アニメさせる");
		ChangeStateTo(GameMode.Cast);
	}


}
