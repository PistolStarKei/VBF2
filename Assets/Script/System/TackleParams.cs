using UnityEngine;
using System.Collections;

[System.Serializable]
public class CurrentTackle{
    
    //1-4
    public string name="";
    public bool isMainTackle=false;
    public int lineNum=-1;
    public bool isSoft=false;
    public int lureNum=-1;

    public bool SetStringData(string str){
        string[] strs=PSGameUtils.SplitStringData(str,new char[]{';'});
        if(strs.Length!=5){
            Debug.LogError("not match length"+str);
            return false;
        }

        name=strs[0];
        bool boo=false;
        if(bool.TryParse(strs[1],out boo)){
            isMainTackle=boo;
        }else{
            return false;
        }

        int num=-1;
        if(int.TryParse(strs[2],out num)){
            lineNum=num;
        }else{
            return false;
        }
        if(bool.TryParse(strs[3],out boo)){
            isSoft=boo;
        }else{
            return false;
        }
        if(int.TryParse(strs[4],out num)){
            lureNum=num;
        }else{
            return false;
        }


        return true;
    }
    public string ToStringData(){
        string str="";
        str+=name+";";
        str+=isMainTackle.ToString()+";";
        str+=lineNum.ToString()+";";
        str+=isSoft.ToString()+";";
        str+=lureNum.ToString();
        return str;
    }

}
[System.Serializable]
public class Tackle_Params{
    //ほぼ定数値いじらない
    public float Params_fukeSpeed=0.006f;
    public float ReelSpeed = 0.1f;

    //タックル総合値　　//飛距離;感度;適合サイズ;フッキング;強度

    //メートル単位での飛距離  min 3m max 60m  ルアーの飛距離　ロッド　リール性能　ライン
    public float Cast_Range=10.0f;
    //1-10 どれだけ早く見えるか？
    public float Params_RodKando = 1.0f;
    //最適なサイズ　ポイント獲得に影響
    public float SizeFitWith=30.0f;
    //1-10 どれだけ早く見えるか？
    public float Params_FoockingPower = 1.1f;
    //強度　引く速さ
    public float RodStrength=1.0f;
    //風の影響率　min 1 max 10
    public int Cast_WindFactor=1;

    //ルアー値
    public int LureApealPower=1;
    //コンタクト;根掛かり回避;ラトル;スプラッシュ;集魚剤  0=false 1=true 
    public bool[] avility=new bool[6];
    //天候と水質との相性 ルアー変更時にこれだけ変える
    public float appealFactor=0.0f;
    public int inwaterBrightNess=0;
    //ルアーの色との相性 50 基準　　100が最も悪い　
    //影響ある場合のみ中間からぶれる
    public int colorVividNess=0;
    //定数で良いかも　ルアーの最大値を決める
    public float weightOfRure=0.9f;

    //CMで　サイズ適合性 大物専用
    public float sizeMatchIn=2.0f;

    //100段階で
    public float negakariKaihi=0.0f;


    //ライン値
    //1.0-0.5　耐久性
    public float Params_lineLife = 1.0f;
    //キャストコントロール性能
    public bool Params_RodTypeIsSpining = false;



    //Line
    public float Params_lineDamage = 0.0f;

    public void ResetTempValue(){
        Params_lineDamage = 0.0f;
    }


}
public class TackleParams : PS_SingletonBehaviour<TackleParams> {
    public Tackle_Params tParams;

    public Color[] lineColors=new Color[4];

    public CurrentTackle currentTackle;

   
    public void UpdateCurrentTackle(int ovveride){
        Debug.Log("UpdateCurrentTackle"+ovveride);
        if(ovveride==-1){
            bool found=false;
            for(int i=0;i<DataManger.Instance.GAMEDATA.tackleSlots.Count;i++){
                if(DataManger.Instance.IsMainRods(i)){
                    found=true;
                    currentTackle.name=DataManger.Instance.GAMEDATA.tackleSlots[i].name;
                    currentTackle.lureNum=DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum;
                    currentTackle.lineNum=DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum;
                    currentTackle.isSoft=DataManger.Instance.GAMEDATA.tackleSlots[i].isSoft;
                    currentTackle.isMainTackle=DataManger.Instance.GAMEDATA.tackleSlots[i].isMainTackle;
                }
            }

            if(!found){
                Debug.LogError("no main rods");
            }
        }else{
                bool found=true;
                currentTackle.name=DataManger.Instance.GAMEDATA.tackleSlots[ovveride].name;
                currentTackle.lureNum=DataManger.Instance.GAMEDATA.tackleSlots[ovveride].lureNum;
                currentTackle.lineNum=DataManger.Instance.GAMEDATA.tackleSlots[ovveride].lineNum;
                currentTackle.isSoft=DataManger.Instance.GAMEDATA.tackleSlots[ovveride].isSoft;
                currentTackle.isMainTackle=DataManger.Instance.GAMEDATA.tackleSlots[ovveride].isMainTackle;
                
            if(currentTackle.name=="" || currentTackle.lineNum==-1 || currentTackle.lureNum==-1){
                found=false;
            }

            if(!found){
                Debug.LogError("line lure is null");
            }
        }


    }

    public void OnEquiMenuClosed(){
        StartCoroutine(OnEquiMenuClosedInvoke());
    }


   public IEnumerator OnEquiMenuClosedInvoke(){
        yield return null;
        if(currentTackle.name==""){
            Debug.LogError("no rods in current");
        }else{

            int current=0;
            bool found=false;
            for(int i=0;i<DataManger.Instance.GAMEDATA.tackleSlots.Count;i++){
                if(currentTackle.name==DataManger.Instance.GAMEDATA.tackleSlots[i].name){
                    found=true;
                    Debug.Log("現在のタックルの装備を更新します");
                    currentTackle.lureNum=DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum;
                    currentTackle.lineNum=DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum;
                    currentTackle.isSoft=DataManger.Instance.GAMEDATA.tackleSlots[i].isSoft;
                    currentTackle.isMainTackle=DataManger.Instance.GAMEDATA.tackleSlots[i].isMainTackle;
                }
            }

            if(!found){
                Debug.LogError("no rods found for name"+currentTackle.name);
            }else{
                Debug.LogWarning("ロッドは正常！　ここでロッドを装備する");

                WaitAndCover.Instance.ShowWait();
                WaitAndCover.Instance.CoverAll(false);
                Coroutine col= StartCoroutine(AffectCurrrentTackle(false));
                yield return col;
                WaitAndCover.Instance.StopWait();
                WaitAndCover.Instance.UnCoverAll();
            }

        }
    }


    public IEnumerator AffectCurrrentTackle(bool isInit){
        if(currentTackle.name==""){
            Debug.LogError("タックル装備なし");
            yield break; 
        }


        Debug.LogWarning("AffectCurrrentTackle ここでロッドを先にスポーンしろ");

        if(currentTackle.lureNum==-1){
            Debug.LogError("ルアー装備なし");
        }else{
            
            if(currentTackle.isSoft){
                int[] adons=PSGameUtils.StringToIntArray(Constants.SoftLureDatas.AvaillableRig[currentTackle.lureNum],new char[]{';'});
                if(adons.Length<=0){
                    Debug.LogError("無効なデータ");
                }else{
                    int outInt=0;

                    if(DataManger.Instance.GAMEDATA.SettedRig.TryGetValue(Constants.SoftLureDatas.itemTittles[currentTackle.lureNum],out outInt)){
                        if(outInt>=adons.Length){
                            Debug.LogError("無効なデータ");
                            LureSpawner.Instance.SpawnLure(Constants.SoftLureDatas.itemTittles[currentTackle.lureNum],
                                0);
                        }else{
                            LureSpawner.Instance.SpawnLure(Constants.SoftLureDatas.itemTittles[currentTackle.lureNum],
                                outInt);
                        }
                    }else{
                        Debug.LogError("無効なデータ");
                    }


                }



            }else{
              
                LureSpawner.Instance.SpawnLure(Constants.LureDatas.itemTittles[currentTackle.lureNum]);
            }
                
            UpdateLure();
        }
       
   
        if(currentTackle.lineNum==-1){
            Debug.LogError("ライン装備なし");
        }else{
            Debug.LogError("ライン"+Constants.LineDatas.lineColor[currentTackle.lineNum]+" "+Constants.LineDatas.lineWidth[currentTackle.lineNum]);

            if(isInit){
                LineScript.Instance.CreateLine(lineColors[Constants.LineDatas.lineColor[currentTackle.lineNum]],Constants.LineDatas.lineWidth[currentTackle.lineNum]);
            }
            //ラインをエフェクトする
            UpdateLine();

        }

        //tParamsに反映
        yield return new WaitForSeconds(0.5f);

        if(currentTackle.lureNum==-1){

            Debug.LogWarning("ルアー装備なし　適切に対処する");
        }else{
            UpdateScaleFactor();
        }
       


    }
   //パラメータを更新する
    public void UpdateLure(){

        string str="";

        if(!currentTackle.isSoft){
            str= Constants.LureDatas.avility[currentTackle.lureNum];
            tParams.avility=PSGameUtils.StringToBoolArray(str);
        }else{

            int currentRig=GetEquippedRigID(currentTackle.lureNum);
            tParams.avility=PSGameUtils.MergeAvility(Constants.RigDatas.avilitys[currentRig],Constants.SoftLureDatas.avilitys[currentTackle.lureNum]);
        }
            
       
    }

    public int GetEquippedRigID(int current){

        int[] adons=GetAvaillableAdons(current);
        if(GetEquippedRig(current)>=adons.Length){
            Debug.LogError("GetEquippedRig Error length over"+current+" "+adons.Length+" "+GetEquippedRig(current));
            return 0;
        }

        return adons[GetEquippedRig(current)];


    }

    public int GetEquippedRig(int num){
        int outInt=0;

        if(DataManger.Instance.GAMEDATA.SettedRig.TryGetValue(Constants.SoftLureDatas.itemTittles[num],out outInt)){
            return outInt;
        }
        return 0;
    }

    private int[] GetAvaillableAdons(int current){
        return PSGameUtils.StringToIntArray(Constants.SoftLureDatas.AvaillableRig[current],new char[]{';'});
    }


    public void UpdateLine(){
        LineScript.Instance.SetLineWidth(Constants.LineDatas.lineWidth[currentTackle.lineNum],lineColors[Constants.LineDatas.lineColor[currentTackle.lineNum]]);
    }

    //0.3-1.0f
    public void UpdateScaleFactor(){


        int waterVisivility=0;//100ほど悪い
        tParams.inwaterBrightNess=EnvManager.Instance.GetInWaterBrightness();
        waterVisivility=tParams.inwaterBrightNess;

        int miness=0;
        if(waterVisivility>50){
            //ラトルあり
            if(isLureHasAvility(2))waterVisivility-=20;
            //匂いあり
            if(isLureHasAvility(4))waterVisivility-=20;
        }

        //波の高さとスプラッシュ系 
        if(EnvManager.Instance.waveParams.waveType==WAVETYPE.TALL){
            if(isLureHasAvility(3))waterVisivility+=5;
        }else if(EnvManager.Instance.waveParams.waveType==WAVETYPE.STILL){
            if(isLureHasAvility(3))waterVisivility-=5;
        }
        waterVisivility=PSGameUtils.ClampInte( waterVisivility,0,100);

        //以下

        //カラー相性 これで50 入ってくるので　0-100までになる
        int ColorFactor=ColorFitess(tParams.inwaterBrightNess);
        //100ほど悪い
        ColorFactor=ColorFactor-50;

        waterVisivility+=ColorFactor/2;
        waterVisivility=PSGameUtils.ClampInte( waterVisivility,0,100);


        float val=(50-waterVisivility)/100.0f;
        //-0.5 0.5 悪い　良い
        tParams.appealFactor=1.0f-val;

    }

    //ルアーの色との相性 50 基準　　100が最も悪い　
    int ColorFitess(int visibility){
        int val=50;
       
        int fac=(visibility-50)/2;
        //-25 25 良い　悪い

        if(fac>0){
            //視界悪いほど明るい　強く　黒い弱く
            val+=(int)(fac*GetDarkness());
            val-=(int)(fac*GetBrightness());
        }else{
            //視界良い ほどナチュラル強く  他は悪くなる
            val-=(int)(fac*GetDarkness());
            val-=(int)(fac*GetBrightness());
            val+=(int)(fac*GetNaturality());
        }

        //25-75

        //25% アースカラーほど強くなる
        if(EnvManager.Instance.waveParams.waveType_color==WAVETYPE_COLOR.SAND){
            val-=(int)(25*GetDarkness());
            val-=(int)(25*GetBrightness());
            val+=(int)(25*GetNaturality());
        }else if(EnvManager.Instance.waveParams.waveType_color==WAVETYPE_COLOR.GREEN){
            val-=(int)(25*GetDarkness());
            val-=(int)(25*GetBrightness());
            val+=(int)(25*GetNaturality());
        }else if(EnvManager.Instance.waveParams.waveType_color==WAVETYPE_COLOR.BLUE){
            val-=(int)(25*GetDarkness());
            val-=(int)(25*GetBrightness());
            val+=(int)(25*GetNaturality());
        }else if(EnvManager.Instance.waveParams.waveType_color==WAVETYPE_COLOR.CLEAR){
            val+=(int)(25*GetDarkness());
            val+=(int)(25*GetBrightness());
            val-=(int)(25*GetNaturality());
        }

        //0-100
        val=PSGameUtils.ClampInte(val,0,100);
        tParams.colorVividNess=val;
        return val;
    }

   
    //コンタクト;根掛かり回避;ラトル;スプラッシュ;集魚剤;反射板  0=false 1=true 
    bool isLureHasAvility(int data){
        return tParams.avility[data];
    }

    float GetLureColor(){
        
        return currentTackle.isSoft?Constants.SoftLureDatas.colorRange[currentTackle.lureNum]: Constants.LureDatas.colorRange[currentTackle.lureNum];;
    }
    float GetDarkness(){
        float col=GetLureColor();
        return col<0.0f?Mathf.Abs(col):0.0f;
    }
    float GetNaturality(){
        float col=GetLureColor();
        return 1.0f-Mathf.Abs(col);
    }
    float GetBrightness(){
        float col=GetLureColor();
        return col>0.0f?Mathf.Abs(col):0.0f;
    }
}
