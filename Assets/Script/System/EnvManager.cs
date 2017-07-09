using UnityEngine;
using System.Collections;

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

public class EnvManager : PS_SingletonBehaviour<EnvManager> {

    //現在の深さ
    public float BottomDepth = 5.0f;
    public float BassVisibleDepth = -3.0f;
    public WaveParames waveParams;
   
    public void UpdateWater(){
        WaterPlane.Instance.SetWater(waveParams.waveClearness,waveParams.waveType_color, waveParams.waveSpeed, waveParams.waveType);
    }

    public WeatherType currentWeather=WeatherType.Sunny;
        

    //1-10
    public int GetEnvFactor(){


        //天候とルアーとの一致
		if(GameController.Instance.skyParams.isRainy){
            //雨の日
        }
        switch(waveParams.waveType){
            case WAVETYPE.NONE:
                break;
            case WAVETYPE.NORMAL:
                break;
            case WAVETYPE.STILL:
                break;
            case WAVETYPE.TALL:
                break;
        }

        if(waveParams.waveClearness<=0.2f){

        }
        //LineScript.Instance.SetLineWidth();
       
        //ラインの太さがどう影響するか？

        return 0;
    }



    //0-100 50は中間値
   public int GetInWaterBrightness(){
        //水の色　1０％
        int num=50;

        //時間と天候　20％
        int num3=0;
		switch(GameController.Instance.skyParams.time){
        case TimeOfDay.MORNIG:
            //天気　10％ よければー　悪ければ＋に
            if(currentWeather==WeatherType.Sunny){
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
            if(currentWeather==WeatherType.Sunny){
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
        if(waveParams.waveType_color==WAVETYPE_COLOR.SAND){
            num+=((int)((val-waveParams.waveClearness)*100.0f));
        }else if(waveParams.waveType_color==WAVETYPE_COLOR.GREEN){
            num+=((int)((val-waveParams.waveClearness)*100.0f));
        }else if(waveParams.waveType_color==WAVETYPE_COLOR.BLUE){
            num+=((int)((val-waveParams.waveClearness)*100.0f));
        }else if(waveParams.waveType_color==WAVETYPE_COLOR.CLEAR){
            num+=((int)((val-waveParams.waveClearness)*100.0f));
        }

        num=PSGameUtils.ClampInte(num,0,100);

        return num;
    }

}
