using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System; 

[System.Serializable]
public class VBFData{

    public Dictionary<string,int> lureHas_Hard_Temp=new Dictionary<string,int> ();
    public Dictionary<string,int> lureHas_Soft_Temp=new Dictionary<string,int> ();

    public List<string> lureHasDebug_Temp=new List<string>();
    public List<string> lureSoftHasDebug_Temp=new List<string>();

    public Dictionary<string,int> lureHas_Hard=new Dictionary<string,int> ();
    public Dictionary<string,int> lureHas_Soft=new Dictionary<string,int> ();
    public Dictionary<string,int> lureHas_Line=new Dictionary<string,int> ();
    public Dictionary<string,int> lureHas_Rods=new Dictionary<string,int> ();

    public Dictionary<string,int>  SettedRig=new Dictionary<string,int> ();

    public Dictionary<string,bool> RigHas=new Dictionary<string,bool> ();

    public List<string>  RigHasDebug=new List<string>();
    public List<string> lureHasDebug=new List<string>();
    public List<string> lureSoftHasDebug=new List<string>();
    public List<string> lureLineHasDebug=new List<string>();
    public List<string> lureRodsHasDebug=new List<string>();



    public long Doller=0L;
    public long Gold=0L;
    public int playerLevel=0;
    public int expOnLevel=0;
    public int totalWins=0;
    public bool earnedLoginBonus=false;
    public bool isAdFree=false;
    public string lastLoginTime="";
    public int DayFromInstall=0;
    public int RenzokuLogin=0;
    public string Countly="";
    public float SPOfferLeft=0.0f;

    public bool Liked=false;
    public EntryData currentEntry;
    public EntryData prevEntry;

    public List<CurrentTackle> tackleSlots=new List<CurrentTackle>();

}
[System.Serializable]
public class EntryData{

    public int month=-1;
    public int year=1999;
    public int category=0;
    public int buttleNum=0;
    public bool isPushed=false;
    public long score=0;

    public bool SetFromStringData(string dat){
        string[] str=PSGameUtils.SplitStringData(dat,new char[]{';'});
        if(str.Length!=6)return false;

        int num=0;
        if(int.TryParse(str[0] ,out num)){
            month=num;
        }else{
            return false;
        }
        if(int.TryParse(str[1] ,out num)){
            year=num;
        }else{
            return false;
        }
        if(int.TryParse(str[2] ,out num)){
            category=num;
        }else{
            return false;
        }
        if(int.TryParse(str[3] ,out num)){
            buttleNum=num;
        }else{
            return false;
        }
        bool nl=false;
        if(bool.TryParse(str[4] ,out nl)){
            isPushed=nl;
        }else{
            return false;
        }
        long num2=0L;
        if(long.TryParse(str[4] ,out num2)){
            score=num2;
        }else{
            return false;
        }
        return true;
    }

    public string ToStringData(){
        string str="";
        if(month==-1){
            return str;
        }
        str+=month.ToString()+";";
        str+=year.ToString()+";";
        str+=category.ToString()+";";
        str+=buttleNum.ToString()+";";
        str+=isPushed.ToString()+";";
        str+=score.ToString();
        return str;
    }

}
public class DataManger : PS_SingletonBehaviour<DataManger> {

    public int HowManyEquipped_Lure(int num,bool isSoft){
        int i=0;
        foreach(CurrentTackle tcle in GAMEDATA.tackleSlots){
            if(tcle.lureNum==num && tcle.isSoft==isSoft){
                i++;
            }
        }
        return i;
    }

    public int IsEquipped_Lure(int num,bool isSoft){
        foreach(CurrentTackle tcle in GAMEDATA.tackleSlots){
                if(tcle.lureNum==num && tcle.isSoft==isSoft){
                    return GAMEDATA.tackleSlots.IndexOf(tcle);
                }
        }
        return -1;
    }
    public int IsEquipped_Line(int num){
        foreach(CurrentTackle tcle in GAMEDATA.tackleSlots){
                if(tcle.lineNum==num){
                    return GAMEDATA.tackleSlots.IndexOf(tcle);
                }
        }
        return -1;
    }
    public bool IsMainRods(int num){
        if(GAMEDATA.tackleSlots.Count>num)return GAMEDATA.tackleSlots[num].isMainTackle;
        return false;
    }


    public bool canBuyDolller(int price){
        if(GAMEDATA.Doller>=price){
            return true;
        }
        return false;
    }
    public bool canBuyGold(int price){
        if(GAMEDATA.Gold>=price){
            return true;
        }
        return false;
    }
    public bool canBuy(int priceDoller,int priceGold){
        if(!canBuyDolller(priceDoller)){
            return false;
        }
        if(!canBuyGold(priceGold)){
            return false;
        }
        return true;
    }

    public static string DataFilename = "VBF2DAT.txt";

    public void AddExp(int exp){
        if( GAMEDATA.playerLevel>=Constants.Params.maxLevel){
            return;
        }
        ES2.Save(ES2.Load<int>(DataFilename+"?tag=pendingExp")+exp,DataFilename+"?tag=pendingExp");
    }
    public int GetExp(){
        return ES2.Load<int>(DataFilename+"?tag=pendingExp");
    }
    public void ClearExp(){
        ES2.Save(0,DataFilename+"?tag=pendingExp");
    }
    public void CreateNewData(){
        Debug.Log("データの新規作成");


        ES2.Save(0,DataFilename+"?tag=gettedGolds");
        ES2.Save(false,DataFilename+"?tag=isAdFree");
        ES2.Save(0,DataFilename+"?tag=totalWins");
        ES2.Save(0,DataFilename+"?tag=expOnLevel");
        ES2.Save(0,DataFilename+"?tag=pendingExp");
        ES2.Save(false,DataFilename+"?tag=isLiked");
        ES2.Save(false,DataFilename+"?tag=earnedLoginBonus");
        ES2.Save( PSGameUtils.DateTimeToString(DateTime.Now),DataFilename+"?tag=lastLoginTime");
        ES2.Save(0.0f,DataFilename+"?tag=SPOfferLeft");
        ES2.Save(0,DataFilename+"?tag=RenzokuLogin");
        ES2.Save(0,DataFilename+"?tag=DayFromInstall");
        ES2.Save(0L,DataFilename+"?tag=Currency_Doller");
        ES2.Save(0L,DataFilename+"?tag=Currency_Gold");
        ES2.Save("",DataFilename+"?tag=Countly");
        ES2.Save(1,DataFilename+"?tag=playerLevel");

        Debug.Log("作成　1");
        //LakeEncironmentでセーブしてる
        ES2.Save(new int[3]{-1,-1,-1},DataFilename+"?tag=Field_weatherRireki");
        ES2.Save(0,DataFilename+"?tag=Field_startKion");
        ES2.Save(Vector3.zero,DataFilename+"?tag=Field_windDirection");
        Debug.Log("作成　1-1");
        Dictionary<string,int> lureHas_Hard=new Dictionary<string,int> ();
        for(int i=0;i<Constants.LureDatas.itemTittles.Length;i++){
            lureHas_Hard.Add(Constants.LureDatas.itemTittles[i],0);
        }
        Debug.Log("作成　1-2");
        ES2.Save( lureHas_Hard,DataFilename+"?tag=lureHas_Hard");
        Debug.Log("作成　1-3");

        Dictionary<string,bool> RigHas=new Dictionary<string,bool> ();
        for(int i=0;i<Constants.RigDatas.itemTittles.Length;i++){
            RigHas.Add(Constants.RigDatas.itemTittles[i],i==0?true:false);
        }


        Debug.Log("作成　1-4");
        Dictionary<string,int> SettedRig=new Dictionary<string,int> ();
        Dictionary<string,int> lureHas_Soft=new Dictionary<string,int> ();
        for(int i=0;i<Constants.SoftLureDatas.itemTittles.Length;i++){
            lureHas_Soft.Add(Constants.SoftLureDatas.itemTittles[i],0);
            SettedRig.Add(Constants.SoftLureDatas.itemTittles[i],0);
        }

        Debug.Log("作成　1-5");
        ES2.Save(SettedRig,DataFilename+"?tag=settedRig");


        ES2.Save(RigHas,DataFilename+"?tag=rigHas");

        ES2.Save( lureHas_Soft,DataFilename+"?tag=lureHas_Soft");
        Debug.Log("作成　2");
        Dictionary<string,int> lureHas_Line=new Dictionary<string,int> ();
        for(int i=0;i<Constants.LineDatas.itemTittles.Length;i++){
            lureHas_Line.Add(Constants.LineDatas.itemTittles[i],0);
        }
        ES2.Save( lureHas_Line,DataFilename+"?tag=lureHas_Line");
        Debug.Log("作成　3");
        Dictionary<string,int> lureHas_Rods=new Dictionary<string,int> ();
        for(int i=0;i<Constants.RodsDatas.itemTittles.Length;i++){
            lureHas_Rods.Add(Constants.RodsDatas.itemTittles[i],0);
        }
        ES2.Save( lureHas_Rods,DataFilename+"?tag=lureHas_Rods");
        Debug.Log("作成　4");
        ES2.Save("",DataFilename+"?tag=prevEntry");
        ES2.Save("",DataFilename+"?tag=culEntry");

        Debug.Log("作成 5");
        string[] tackles=new string[Constants.RodsDatas.itemTittles.Length];

        for(int i=0;i<Constants.RodsDatas.itemTittles.Length;i++){
            CurrentTackle tcke=new CurrentTackle();
            tcke.name=Constants.RodsDatas.itemTittles[i];
            tcke.isMainTackle=false;
            tcke.lineNum=-1;
            tcke.isSoft=false;
            tcke.lureNum=-1;
            tackles[i]=tcke.ToStringData();
        }

        ES2.Save( tackles,DataFilename+"?tag=tackleSlots");
        Debug.Log("作成完了");


       



    }

    public void UpdateHasTempData(){
        GAMEDATA.lureSoftHasDebug_Temp.Clear();
        GAMEDATA.lureHasDebug_Temp.Clear();

        foreach(KeyValuePair<string, int> p in GAMEDATA.lureHas_Soft_Temp) {
            GAMEDATA.lureSoftHasDebug_Temp.Add(""+p.Key+" / "+p.Value);
        }

        foreach(KeyValuePair<string, int> p in GAMEDATA.lureHas_Hard_Temp) {
            GAMEDATA.lureHasDebug_Temp.Add(""+p.Key+" / "+p.Value);
        }




    }
    public void AffectLureHasTempData(){
        GAMEDATA.lureHas_Hard_Temp.Clear();
        GAMEDATA.lureHas_Soft_Temp.Clear();

        foreach(KeyValuePair<string, int> p in GAMEDATA.lureHas_Soft) {
            GAMEDATA.lureHas_Soft_Temp.Add(p.Key,p.Value);
            GAMEDATA.lureSoftHasDebug_Temp.Add(""+p.Key+" / "+p.Value);
        }

        foreach(KeyValuePair<string, int> p in GAMEDATA.lureHas_Hard) {
            GAMEDATA.lureHas_Hard_Temp.Add(p.Key,p.Value);
            GAMEDATA.lureHasDebug_Temp.Add(""+p.Key+" / "+p.Value);
        }




    }
    public void SaveData(){
        if(!isDataLoaded)return;
        Debug.Log("データのセーブ");
        if(GAMEDATA.currentEntry!=null) ES2.Save(GAMEDATA.currentEntry.ToStringData(),DataFilename+"?tag=culEntry");
        if(GAMEDATA.prevEntry!=null) ES2.Save(GAMEDATA.prevEntry.ToStringData(),DataFilename+"?tag=prevEntry");
        ES2.Save(GAMEDATA.expOnLevel,DataFilename+"?tag=expOnLevel");
        ES2.Save( GAMEDATA.totalWins,DataFilename+"?tag=totalWins");


        ES2.Save(GAMEDATA.playerLevel,DataFilename+"?tag=playerLevel");
        ES2.Save(GAMEDATA.lastLoginTime,DataFilename+"?tag=lastLoginTime");
        ES2.Save(GAMEDATA.SPOfferLeft,DataFilename+"?tag=SPOfferLeft");
        ES2.Save(GAMEDATA.RenzokuLogin,DataFilename+"?tag=RenzokuLogin");
        ES2.Save(GAMEDATA.DayFromInstall,DataFilename+"?tag=DayFromInstall");
        ES2.Save(GAMEDATA.Countly,DataFilename+"?tag=Countly");
        ES2.Save(GAMEDATA.Gold,DataFilename+"?tag=Currency_Gold");
        ES2.Save(GAMEDATA.Doller,DataFilename+"?tag=Currency_Doller");
        ES2.Save( GAMEDATA.lureHas_Hard,DataFilename+"?tag=lureHas_Hard");
        ES2.Save( GAMEDATA.lureHas_Soft,DataFilename+"?tag=lureHas_Soft");
        ES2.Save( GAMEDATA.lureHas_Line,DataFilename+"?tag=lureHas_Line");
        ES2.Save( GAMEDATA.lureHas_Rods,DataFilename+"?tag=lureHas_Rods");
        ES2.Save(GAMEDATA.Liked,DataFilename+"?tag=isLiked");
        ES2.Save(GAMEDATA.earnedLoginBonus,DataFilename+"?tag=earnedLoginBonus");
        ES2.Save(GAMEDATA.isAdFree,DataFilename+"?tag=isAdFree");

        string[] tackles=new string[GAMEDATA.tackleSlots.Count];

        for(int i=0;i<GAMEDATA.tackleSlots.Count;i++){
            tackles[i]=GAMEDATA.tackleSlots[i].ToStringData();
        }

        ES2.Save( tackles,DataFilename+"?tag=tackleSlots");
        ES2.Save(GAMEDATA.RigHas,DataFilename+"?tag=rigHas");
        ES2.Save(GAMEDATA.SettedRig,DataFilename+"?tag=settedRig");
       


    }
	public void LoadData(){
        Debug.Log("データのロード");

        GAMEDATA=new VBFData();
        GAMEDATA.Liked=ES2.Load<bool>(DataFilename+"?tag=isLiked");
        GAMEDATA.expOnLevel=ES2.Load<int>(DataFilename+"?tag=expOnLevel");
        GAMEDATA.totalWins=ES2.Load<int>(DataFilename+"?tag=totalWins");
        GAMEDATA.earnedLoginBonus=ES2.Load<bool>(DataFilename+"?tag=earnedLoginBonus");
        GAMEDATA.playerLevel=ES2.Load<int>(DataFilename+"?tag=playerLevel");
        GAMEDATA.lastLoginTime=ES2.Load<string>(DataFilename+"?tag=lastLoginTime");
        GAMEDATA.SPOfferLeft=ES2.Load<float>(DataFilename+"?tag=SPOfferLeft");
        GAMEDATA.RenzokuLogin=ES2.Load<int>(DataFilename+"?tag=RenzokuLogin");
        GAMEDATA.DayFromInstall=ES2.Load<int>(DataFilename+"?tag=DayFromInstall");
        GAMEDATA.Countly=ES2.Load<string>(DataFilename+"?tag=Countly");
        GAMEDATA.Gold=ES2.Load<long>(DataFilename+"?tag=Currency_Gold");
        GAMEDATA.Doller=ES2.Load<long>(DataFilename+"?tag=Currency_Doller");
        GAMEDATA.isAdFree=ES2.Load<bool>(DataFilename+"?tag=isAdFree");
        GAMEDATA.lureHas_Hard=ES2.LoadDictionary<string,int>(DataFilename+"?tag=lureHas_Hard");
        for(int i=0;i<Constants.LureDatas.itemTittles.Length;i++){
            if(!GAMEDATA.lureHas_Hard.ContainsKey(Constants.LureDatas.itemTittles[i])){
                Debug.LogError("追加のルアーあり "+Constants.LureDatas.itemTittles[i]);
                GAMEDATA.lureHas_Hard.Add(Constants.LureDatas.itemTittles[i],0);
            }

        }

        foreach (string value in GAMEDATA.lureHas_Hard.Keys) {
            GAMEDATA.lureHasDebug.Add(value+"/"+GAMEDATA.lureHas_Hard[value]);
        }


        GAMEDATA.lureHas_Soft=ES2.LoadDictionary<string,int>(DataFilename+"?tag=lureHas_Soft");
        GAMEDATA.RigHas=ES2.LoadDictionary<string,bool>(DataFilename+"?tag=rigHas");

       

        GAMEDATA.SettedRig=ES2.LoadDictionary<string,int>(DataFilename+"?tag=settedRig");

        for(int i=0;i<Constants.SoftLureDatas.itemTittles.Length;i++){
            if(!GAMEDATA.lureHas_Soft.ContainsKey(Constants.SoftLureDatas.itemTittles[i])){
                Debug.LogError("追加のSoftあり "+Constants.SoftLureDatas.itemTittles[i]);
                GAMEDATA.lureHas_Soft.Add(Constants.SoftLureDatas.itemTittles[i],0);
                GAMEDATA.SettedRig.Add(Constants.SoftLureDatas.itemTittles[i],0);
            }
        }

        for(int i=0;i<Constants.RigDatas.itemTittles.Length;i++){
            if(!GAMEDATA.RigHas.ContainsKey(Constants.RigDatas.itemTittles[i])){
                Debug.LogError("追加のRigあり "+Constants.RigDatas.itemTittles[i]);
                GAMEDATA.RigHas.Add(Constants.RigDatas.itemTittles[i],false);
            }
           
        }
        foreach (string value in GAMEDATA.lureHas_Soft.Keys) {
            
            GAMEDATA.lureSoftHasDebug.Add(value+"/"+GAMEDATA.lureHas_Soft[value]);
        }
        foreach (string value in GAMEDATA.RigHas.Keys) {
            GAMEDATA.RigHasDebug.Add(value+"/"+GAMEDATA.RigHas[value]);
        }

        GAMEDATA.lureHas_Line=ES2.LoadDictionary<string,int>(DataFilename+"?tag=lureHas_Line");
        for(int i=0;i<Constants.LineDatas.itemTittles.Length;i++){
            if(!GAMEDATA.lureHas_Line.ContainsKey(Constants.LineDatas.itemTittles[i])){
                Debug.LogError("追加のLineあり "+Constants.LineDatas.itemTittles[i]);
                GAMEDATA.lureHas_Line.Add(Constants.LineDatas.itemTittles[i],0);
            }
        }
        foreach (string value in GAMEDATA.lureHas_Line.Keys) {
            GAMEDATA.lureLineHasDebug.Add(value+"/"+GAMEDATA.lureHas_Line[value]);
        }

       

        GAMEDATA.lureHas_Rods=ES2.LoadDictionary<string,int>(DataFilename+"?tag=lureHas_Rods");
        for(int i=0;i<Constants.RodsDatas.itemTittles.Length;i++){
            if(!GAMEDATA.lureHas_Rods.ContainsKey(Constants.RodsDatas.itemTittles[i])){
                Debug.LogError("追加のRodsあり "+Constants.RodsDatas.itemTittles[i]);
                GAMEDATA.lureHas_Rods.Add(Constants.RodsDatas.itemTittles[i],0);

            }
        }

        GAMEDATA.tackleSlots=new List<CurrentTackle>();

        string[] tackles=ES2.LoadArray<string>(DataFilename+"?tag=tackleSlots");
        for(int i=0;i<Constants.RodsDatas.itemTittles.Length;i++){
            CurrentTackle tcke=new CurrentTackle();

            if(i>=tackles.Length){
                Debug.LogError("追加のRodsあり "+Constants.RodsDatas.itemTittles[i]);
                tcke.name=Constants.RodsDatas.itemTittles[i];
                GAMEDATA.tackleSlots.Add(tcke);
            }else{
                if(tcke.SetStringData(tackles[i])){

                    GAMEDATA.tackleSlots.Add(tcke);
                }else{


                    Debug.LogError("Error:NO Data FAILURE");
                }
            }
           

        }


        foreach (string value in GAMEDATA.lureHas_Rods.Keys) {
            GAMEDATA.lureRodsHasDebug.Add(value+"/"+GAMEDATA.lureHas_Rods[value]);
        }

        currentCategory=0;
        previousCategory=0;
        if(ES2.Load<string>(DataFilename+"?tag=culEntry")==""){
            Debug.Log("Entry ==null");
            GAMEDATA.currentEntry=null;
        }else{
            GAMEDATA.currentEntry=null;
            GAMEDATA.currentEntry=new EntryData();
            if(GAMEDATA.currentEntry.SetFromStringData(ES2.Load<string>(DataFilename+"?tag=culEntry"))){
                currentCategory=GAMEDATA.currentEntry.category;
            }else{
                Debug.Log("Error:NO ENTRY FAILURE");
            }
           

        }
        if(ES2.Load<string>(DataFilename+"?tag=prevEntry")==""){
            Debug.Log("Entry2 ==null");
            GAMEDATA.prevEntry=null;
        }else{
            GAMEDATA.prevEntry=null;
            GAMEDATA.prevEntry=new EntryData();
            if(GAMEDATA.prevEntry.SetFromStringData(ES2.Load<string>(DataFilename+"?tag=prevEntry"))){
                previousCategory=GAMEDATA.prevEntry.category;
            }else{
                Debug.Log("Error:NO ENTRY FAILURE 2");
            }
        }
        isDataLoaded=true;
	}

    public int currentCategory=0;
    public int previousCategory=0;

    bool isDataLoaded=false;
    public bool needToSingleton=false;
    void DestroyAll(){
        foreach (Transform childTransform in gameObject.transform) Destroy(childTransform.gameObject);
        Destroy(gameObject);
    }
    bool isInitted=false;
	void Awake()
	{
        if(needToSingleton){
            if(this != Instance)
            {
                Debug.Log("DeleteAll");
                DestroyAll();
                return;
            }
        }
        if(isInitted)return;
        isInitted=true;
        DontDestroyOnLoad(this.gameObject);

        if(PS_Plugin.Instance!=null)PS_Plugin.Instance.InitAll();
	}   


    IEnumerator LoadDatas(){
        Debug.Log("****Dataの初期化");
        bool isNewUser=false;
        if(Application.isEditor && isDeleteData){
            Debug.Log("データを新規作成");
            ES2.DeleteDefaultFolder();
            CreateNewData();
            isNewUser=true;
            LoadData();
        }else{
            if(!ES2.Exists(DataFilename+"?tag=SPOfferLeft")){
                Debug.Log("データを新規作成");
                CreateNewData();
                LoadData();
                isNewUser=true;
                LoadingMenuManager.Instance.SetProgress(1,"creating datas");
                yield return new WaitForSeconds(1.0f);
            }else{
                Debug.Log("データをロード");
                LoadData();
                LoadingMenuManager.Instance.SetProgress(1,"loading datas");
                yield return new WaitForSeconds(1.0f);
            }
        }

        Debug.Log("****TimeManagerの初期化");
        LoadingMenuManager.Instance.SetProgress(1,"updating time");
        if(TimeManager.Instance.CheckSameDayLogin(GAMEDATA.lastLoginTime,GAMEDATA.RenzokuLogin)){
            //同じ日
            Debug.Log("同じ日");
            LakeEnvironmentalParamas.Instance.InitWeather(isNewUser,false);
        }else{
            //違う日
            Debug.Log("違う日");
            LoadingMenuManager.Instance.SetProgress(1,"set environment");
            LakeEnvironmentalParamas.Instance.InitWeather(isNewUser,true);

            if(ES2.Load<int>(DataFilename+"?tag=DayFromInstall")<=7){
                ES2.Save(ES2.Load<int>(DataFilename+"?tag=DayFromInstall")+1,DataFilename+"?tag=DayFromInstall");
                Debug.Log(""+ES2.Load<int>(DataFilename+"?tag=DayFromInstall"));
            }
            //月が同じかをチェックする。

            if(GAMEDATA.currentEntry!=null){
                Debug.Log("エントリしている");
                if(GAMEDATA.currentEntry.year!=TimeManager.Instance.currentYear){
                    Debug.Log("年変わり");
                    OnNextMonth();
                }else{
                    if(GAMEDATA.currentEntry.month!=TimeManager.Instance.currentMonth){
                        Debug.Log("月変わり");
                        OnNextMonth();
                    }else{
                        Debug.Log("同じ月");
                    }
                }

            }else{
                Debug.Log("エントリなし");
            }

            //GPGSListener.Instance.GetCurrentPushID();
        }



        yield return new WaitForSeconds(2.5f);
        if(Application.isEditor && isLoadMain)PS_Plugin.Instance.LoadScene("MainScene");
    }


    public void UpdateDebugData(){
        GAMEDATA.lureSoftHasDebug.Clear();
        GAMEDATA.lureHasDebug.Clear();
        GAMEDATA.RigHasDebug.Clear();
        GAMEDATA.lureLineHasDebug.Clear();
        GAMEDATA.lureRodsHasDebug.Clear();
        foreach (string value in GAMEDATA.lureHas_Soft.Keys) {
            GAMEDATA.lureSoftHasDebug.Add(value+"/"+GAMEDATA.lureHas_Soft[value]);
        }

        foreach (string value in GAMEDATA.lureHas_Hard.Keys) {
            GAMEDATA.lureHasDebug.Add(value+"/"+GAMEDATA.lureHas_Hard[value]);
        }



        foreach (string value in GAMEDATA.RigHas.Keys) {
            GAMEDATA.RigHasDebug.Add(value+"/"+GAMEDATA.RigHas[value]);
        }


        foreach (string value in GAMEDATA.lureHas_Line.Keys) {
            GAMEDATA.lureLineHasDebug.Add(value+"/"+GAMEDATA.lureHas_Line[value]);
        }

        foreach (string value in GAMEDATA.lureHas_Rods.Keys) {
            
            GAMEDATA.lureRodsHasDebug.Add(value+"/"+GAMEDATA.lureHas_Rods[value]);
        }

    }

    public void UpdateTimedata(bool isMapMenu){
        if(TimeManager.Instance.CheckSameDayLogin(GAMEDATA.lastLoginTime,GAMEDATA.RenzokuLogin)){
            Debug.Log("同じ日");
            //マップ画面に入るたびに呼ぶ
            if(isMapMenu)LakeEnvironmentalParamas.Instance.UpdateFieldTime();

        }else{
            Debug.Log("違う日");
            //天候を次の日に
            LakeEnvironmentalParamas.Instance.InitWeather(false,true);

            if(GAMEDATA.currentEntry!=null){
                Debug.Log("エントリしている");
                if(GAMEDATA.currentEntry.year!=TimeManager.Instance.currentYear){
                    Debug.Log("年変わり");
                   OnNextMonth();
                }else{
                    if(GAMEDATA.currentEntry.month!=TimeManager.Instance.currentMonth){
                        Debug.Log("月変わり");
                        OnNextMonth();
                    }else{
                        Debug.Log("同じ月");
                    }
                }

            }
        }
        SaveData();
    }
    public void OnNextMonth(){
        GAMEDATA.prevEntry=GAMEDATA.currentEntry;

        GAMEDATA.currentEntry=new EntryData();
        GAMEDATA.currentEntry.month=TimeManager.Instance.currentMonth;
        GAMEDATA.currentEntry.year=TimeManager.Instance.currentYear;

        SaveData();
    }
    public bool isLoadMain=false;

	public bool isDeleteData=false;

    public VBFData GAMEDATA;
	
	
    public void SaveCurrentTime(string tag){
        ES2.Save( PSGameUtils.DateTimeToString(DateTime.Now),DataFilename+tag);
    }

    public void OnPluginNotReady(){
        //GPGにログインできない　入れない　電波障害
        Debug.Log("OnPluginNotReady");
        WaitAndCover.Instance.ShowYesNoPopup(Localization.Get("NCError"),Localization.Get("DescNCError"),
            Localization.Get("Retry"),Localization.Get("Quit"),RetryOrQuit);

    }
    public void RetryOrQuit(bool isYes){
        if(isYes){
            LoadingMenuManager.Instance.SetProgressToDefault();
            if(PS_Plugin.Instance!=null)PS_Plugin.Instance.InitAll();
        }else{
            Application.Quit();
        }
    }
    public void OnUserYNNotThisYear(bool isYes){
        Debug.Log("OnUserYNNotThisYear"+isYes);
        if(isYes){
            Application.OpenURL(Constants.Params.StoreURL);
        }else{
            Application.Quit();
        }
    }
    public void ReportBugOrQuit(bool isYes){
        if(isYes){
            Application.OpenURL(Constants.Params.FeedBackURL);
        }else{
            Application.Quit();
        }
    }

    public void OnPluginReady(string YearTittle){
        //""なら　年度が今年ではない　or　年度が合えば
        if(YearTittle==""){
            //""なら　年度が今年ではない アップデートを促す
            WaitAndCover.Instance.ShowYesNoPopup(Localization.Get("TittleNoRankingData"),Localization.Get("DescNoRankingData"),
                Localization.Get("Store"),Localization.Get("Quit"),OnUserYNNotThisYear);
        }else{
            string[] str= PSGameUtils.SplitStringData(YearTittle,new char[]{':'});
            if(str.Length!=3){
                //フォーマットエラー
                Debug.LogError("フォーマットエラー");
                WaitAndCover.Instance.ShowYesNoPopup("NTYF0"+Localization.Get("FatalError"),Localization.Get("DescFatalError"),
                    Localization.Get("Report"),Localization.Get("Quit"),ReportBugOrQuit);
            }else{
                int year=0;
                if(int.TryParse(str[0],out year)){
                    Debug.Log("年度は今年！！　"+year);

                    if(DateTime.Now.Year!=year){
                        //フォーマットエラー
                        Debug.LogError("フォーマットエラー");

                        WaitAndCover.Instance.ShowYesNoPopup("NTYF1"+Localization.Get("FatalError"),Localization.Get("DescFatalError"),
                            Localization.Get("Report"),Localization.Get("Quit"),ReportBugOrQuit);
                    }else{
                        //ゲームの開始可能 or データロード可能
                        //今年のapkで　
                        StartCoroutine(LoadDatas());
                        //PS_Plugin.Instance.LoadScene("VBFMain");
                    }
                }else{
                    //フォーマットエラー
                    Debug.LogError("フォーマットエラー");
                    WaitAndCover.Instance.ShowYesNoPopup("NTYF3"+Localization.Get("FatalError"),Localization.Get("DescFatalError"),
                        Localization.Get("Report"),Localization.Get("Quit"),ReportBugOrQuit);
                }
            }
        }

    }

    public string GetFormattedLong(long val){

        if(val>=1000){
            if(val>1000000){
                return (val/1000000.0f).ToString("0.00")+"M";
            }else{
                return (val/1000.0f).ToString("0.00")+"K";
            }
        }else{
            return val.ToString();
        }
    }
}
