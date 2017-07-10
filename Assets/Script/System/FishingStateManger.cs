using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public enum GameMode{Menu,Move,Cast,Throwing,ReelingOnLand,Reeling,Fight,Result};
//Menu(lodge)<- ->Test
//Menu(lodge)<- ->Boat
//Boat(Single)<- ->Cast
//Cast<- (miss cast on terrain)
//Cast<- ->Reeling
//Reeling<- ->Fight
//Fight->Result
//Result->Cast
public class FishingStateManger : SingletonStatefulObjectBase<FishingStateManger, GameMode> {

    private static FishingStateManger instance;
    public static FishingStateManger Instance {
        get {
            if (instance == null) {
                instance = (FishingStateManger)FindObjectOfType(typeof(FishingStateManger));

                if (instance == null) {
                    //Debug.Log(typeof(T) + "is null");
                }
            }
            return instance;
        }
    }

    public CastMoveBtn castMoveBtn;
    public void OnCast(bool isCast){
        if(isCast){
            ChangeStateTo(GameMode.Cast);
        }else{
            ChangeStateTo(GameMode.Move);
        }
    }

    IEnumerator UpdateTimeAndEnvironment(){
        WaitAndCover.Instance.ShowWait();
        WaitAndCover.Instance.CoverAll(true);
        LakeEnvironmentalParamas.Instance.UpdateFieldTime();
        yield return null;
       
        yield return null;
    }
    IEnumerator CreateEnvironment(){


        Coroutine tine=StartCoroutine(UpdateTimeAndEnvironment());
        yield return tine;

       
        tine=StartCoroutine(TackleParams.Instance.AffectCurrrentTackle(true));
        yield return tine;

        WaitAndCover.Instance.StopWait();
        WaitAndCover.Instance.UnCoverAll();

        Debug.Log("終わり");
        Player.Instance.SetPlayerState(true);
        ChangeStateTo(GameMode.Move);

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
    public string debugLure="";
    public int debugRig=1;
    public bool debugIsSoft=false;
    void Start(){
        Debug.LogWarning("これは本番では消す");
        if(isDebugMode){
            
            DataManger.Instance.LoadData();
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

        Debug.Log("ポイントに入ったとき");
        stateList.Add(new modeMenu(this));
        stateList.Add(new modeMove(this));
        stateList.Add(new modeCast(this));
        stateList.Add(new modeThrowing(this));
        stateList.Add(new modeReelingOnLand(this));
        stateList.Add(new modeReeling(this));
        stateList.Add(new modeFight(this));
        stateList.Add(new modeResult(this));

        stateMachine = new StateMachineMode<FishingStateManger>();
        StartCoroutine( CreateEnvironment());
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
    private class modeMenu : Mode<FishingStateManger>
    {
        //Move Cast のみ
        public modeMenu(FishingStateManger owner) : base(owner) {}

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




    private class modeMove : Mode<FishingStateManger>
    {
        //Menu Cast のみ
        public modeMove(FishingStateManger owner) : base(owner) {}

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

   


    private class modeCast : Mode<FishingStateManger>
    {
        //Menu,Move,Throwing
        public modeCast(FishingStateManger owner) : base(owner) {}

        public override void Enter() {
            Debug.Log("Enter modeCast");


            if(owner.currentMode==GameMode.Move)Player.Instance.SetPlayerState(false);
            LineScript.Instance.HideLine();
            RodController.Instance.InitRod();
            LureController.Instance.SetToDefaultPosition(Player.Instance.lureDefaultPos);
            LineScript.Instance.Length=2.5f;
            FishingStateManger.Instance.RemoveChasingBass();
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
    private class modeThrowing : Mode<FishingStateManger>
    {
        //Menu,Move,Cast
        public modeThrowing(FishingStateManger owner) : base(owner) {}

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
    private class modeReelingOnLand : Mode<FishingStateManger>
    {
        //Throwing,ReelingOnLand,Reeling,Fight
        public modeReelingOnLand(FishingStateManger owner) : base(owner) {}

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
    private class modeReeling : Mode<FishingStateManger>
    {
        //Throwing,ReelingOnLand,Fight
        public modeReeling(FishingStateManger owner) : base(owner) {}

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
    private class modeFight : Mode<FishingStateManger>
    {
        //Throwing,ReelingOnLand,Reeling
        public modeFight(FishingStateManger owner) : base(owner) {}

        public override void Enter() {
            Debug.Log("Enter modeFight");
            owner.isShowedFightToUser=false;
            if(owner.currentMode==GameMode.Reeling){
                TackleParams.Instance.tParams.Params_lineDamage=0.0f;

                FishingStateManger.instance.OnBassBite();
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
    private class modeResult : Mode<FishingStateManger>
    {
        //Fightのみ
        public modeResult(FishingStateManger owner) : base(owner) {}

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
