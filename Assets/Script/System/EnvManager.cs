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
[System.Serializable]
public class PARAMS_SKY{
    public TimeOfDay time=TimeOfDay.DAY;

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
    public bool isCloudy=false;
    public bool isRainy=false;
    public float windSpeed=1.0f;
    public FogDensity fogDensity=FogDensity.NONE;
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
        
    public PARAMS_SKY skyParams;
    public void UpdateSky(){
        switch(currentWeather){
        case WeatherType.Sunny:
            break;
        case WeatherType.Cloudy:
            skyParams.isCloudy=true;
            break;
        case WeatherType.Rain:
            skyParams.isRainy=true;
            break;
        case WeatherType.HeavyRain:
            skyParams.isRainy=true;
            break;
        }
        skyParams.windSpeed=0.1f;
        SkyController.Instance.UpdateSky();
    }

    //1-10
    public int GetEnvFactor(){

        //天候とルアーとの一致
        if(skyParams.isRainy){
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
        switch(skyParams.time){
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
