using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum WeatherType{Sunny,Cloudy,Rain,HeavyRain}
public enum FieldTime{Night,EMorning,Morning,Noon,AfterNoon,Evening}
public enum FieldConditions{TooBad,Bad,Normal,Good,TooGood}

public enum WeatherTokusei{Summer,Spring,Winter,Autumn}
[System.Serializable]
public class FieldWeather{
    public List<WeatherType> weatherRireki =new List<WeatherType>();
    public int[] RirekiToIntArray(){
        int[] array=new int[weatherRireki.Count];
        for(int i=0;i<weatherRireki.Count;i++){
            array[i]=(int)weatherRireki[i];
        }
        return  array;
    }
    public WeatherType GetTodayWeather(){
        if(weatherRireki.Count>=3){
            return weatherRireki[3];
        }else{
            return WeatherType.Sunny;
        }

    }
    //変動しない　初期化した時に決定
    public int startKionAt=20;
    //4=巨風 1.0
    public Vector3 WindDirection=Vector3.zero;

    //変動する
    public FieldTime fieldTime;

    public FieldConditions conditions;
    public int Kion=20;
}
[System.Serializable]
public class PointWeather{
    public int pointNum=0;
    public int Suion=20;
    public FieldConditions conditions;

}
public class LakeEnvironmentalParamas : PS_SingletonBehaviour<LakeEnvironmentalParamas> {

    public TimeOfDay GetTimeOfDayForSky(){
        TimeOfDay dat=TimeOfDay.NIGHT;;
        switch(weather.fieldTime){
        case FieldTime.Night:
            dat=TimeOfDay.NIGHT;
            break;
        case FieldTime.EMorning:
            dat=TimeOfDay.MORNIG;
            break;
        case FieldTime.Morning:
            dat=TimeOfDay.MORNIG;
            break;
        case FieldTime.Noon:
            dat=TimeOfDay.DAY;
            break;
        case FieldTime.AfterNoon:
            dat=TimeOfDay.DAY;
            break;
        case FieldTime.Evening:
            dat=TimeOfDay.YU;
            break;
        }
        return dat;
    }

    public FieldWeather weather =new FieldWeather();


    //現在のタックル情報
    public PointWeather currentPointWeather;



    public void InitWeather(bool isNewUser,bool isNewDay){
        weather =new FieldWeather();
        int[] rireki= ES2.LoadArray<int>(DataManger.DataFilename+"?tag=Field_weatherRireki");
        if(rireki.Length!=7 && !isNewUser){
            Debug.LogError("newUser==falseなのに天気がおかしい！");
            isNewUser=true;
        }
        if(isNewUser){
            Debug.Log("全体の天候　新規作成");
            List<WeatherType> rireki2 =new List<WeatherType>();
            rireki2.Add(GetWeather());
            rireki2.Add(GetWeather());
            rireki2.Add(GetWeather());
            rireki2.Add(GetWeather());
            rireki2.Add(GetWeather());
            rireki2.Add(GetWeather());
            rireki2.Add(GetWeather());
            weather.weatherRireki=rireki2;
            weather.startKionAt=GetStartKion();
            weather.WindDirection=GetStartWind();
            //save Data
            ES2.Save(weather.RirekiToIntArray(),DataManger.DataFilename+"?tag=Field_weatherRireki");
            ES2.Save(weather.startKionAt,DataManger.DataFilename+"?tag=Field_startKion");
            ES2.Save(weather.WindDirection,DataManger.DataFilename+"?tag=Field_windDirection");
        }else{
            Debug.Log("全体の天候　ロード");
            List<WeatherType> weatherRireki =new List<WeatherType>();
            for(int i=0;i<rireki.Length;i++){
                weatherRireki.Add((WeatherType)rireki[i]);
            }
            this.weather.weatherRireki=weatherRireki;

            if(isNewDay){
                Debug.LogError("全体の天候　次の日に");
                //天気を追加する
                weather.weatherRireki.RemoveAt(0);
                weather.weatherRireki.Add(GetWeather());
                weather.startKionAt=GetStartKion();
                weather.WindDirection=GetStartWind();
                //save Data
                ES2.Save(weather.RirekiToIntArray(),DataManger.DataFilename+"?tag=Field_weatherRireki");
                ES2.Save(weather.startKionAt,DataManger.DataFilename+"?tag=Field_startKion");
                ES2.Save(weather.WindDirection,DataManger.DataFilename+"?tag=Field_windDirection");
            }else{
                weather.startKionAt=ES2.Load<int>(DataManger.DataFilename+"?tag=Field_startKion");;
                weather.WindDirection=ES2.Load<Vector3>(DataManger.DataFilename+"?tag=Field_windDirection");;

            }
        }
        UpdateFieldTime();


    }

  
    //マップ画面に入るたびにも呼ぶこと
    public void UpdateFieldTime(){
        TimeManager.Instance.UpdateTime();

        Debug.Log("UpdateFieldTime"+TimeManager.Instance.time);
        weather.fieldTime=GetFieldTime(TimeManager.Instance.time);

        //気温を設定する
        weather.Kion=GetKion();
        //基づいてコンディションが決定する
        weather.conditions=GetCondition(weather.Kion-2);
    }



    //以下　季節タイプにより行う
    FieldTime GetFieldTime(int time){
        Debug.Log("GetFieldTime"+time);
        //7-19;
        int[] timeSpan=Constants.Params.GetTimeSpan(DateTime.Now.Month);
        if(time<= timeSpan[0] && time>=timeSpan[5]){
            //夜
            return FieldTime.Night;
        }else if(time> timeSpan[0] && time<= timeSpan[1]){
            //早朝
            return FieldTime.EMorning;
        }else if(time>timeSpan[1] && time<=timeSpan[2]){
            //午前
            return FieldTime.Morning;
        }else if(time>timeSpan[2] && time<=timeSpan[3]){
            //昼
            return FieldTime.Noon;
        }else if(time>timeSpan[3] && time<=timeSpan[4]){
            // 午後
            return FieldTime.AfterNoon;
        }
        // 夕方
        return FieldTime.Evening;
    }


    //次の天気を取得するメソッド　Constants.Params.WeightsOfWeatherによる抽選を行う
    WeatherType GetWeather(){
        WeatherType[] strArray = new WeatherType[4] {WeatherType.Sunny, WeatherType.Cloudy,WeatherType.Rain,WeatherType.HeavyRain};
        return ExtRandom<WeatherType>.WeightedChoice(strArray, Constants.Params.WeightsOfWeather );
    }
        
    //深夜の天気を取得するメソッド　Constants.Paramsによる抽選を行う
    int GetStartKion(){
        //7-19;
        return UnityEngine.Random.Range(Constants.Params.MinStartKion[DateTime.Now.Month-1],Constants.Params.MaxStartKion[DateTime.Now.Month-1]);
    }
    //風の初期値
    Vector3 GetStartWind(){
        //7-19;
        Vector3 vec=Vector3.zero;

        float pow= ExtRandom<float>.WeightedChoice(Constants.Params.MaxWindPower,Constants.Params.WeightsOfMaxWindPower);
        //North
        if(UnityEngine.Random.value<=0.5f){
            vec+=Vector3.forward*pow;
        }else{
            vec+=Vector3.forward*-pow;
        }

        pow= ExtRandom<float>.WeightedChoice(Constants.Params.MaxWindPower,Constants.Params.WeightsOfMaxWindPower);

        if(UnityEngine.Random.value<=0.5f){
            vec+=Vector3.right*pow;
        }else{
            vec+=Vector3.right*-pow;
        }
        return vec;
    }
        



    //単なる目安　気温と時間による
    FieldConditions GetCondition(int suion){
        //7-19;
        if(suion<=10){
            return FieldConditions.TooBad;
        }else if(suion<=15){
            return FieldConditions.Bad;
        }else if(suion<=18){
            return FieldConditions.Normal;
        }else if(suion<=22){
            return FieldConditions.Good;
        }else if(suion<=25){
            return FieldConditions.TooGood;
        }else if(suion<=28){
            return FieldConditions.Good;
        }else if(suion<=32){
            return FieldConditions.Normal;
        }else if(suion<=35){
            return FieldConditions.Bad;
        }
           
        return FieldConditions.TooBad;
        
           

    }

  
    int GetKion(){
        //12-0 0へ行くほど
        int HourFactor =12-Mathf.Abs(12-DateTime.Now.Hour);
        int sa=Constants.Params.KionSa[DateTime.Now.Month-1];
        float hourFactor2=HourFactor*(1.0f/12.0f);
        //12 0.0f 0 1.0 5 
        float tenkoufactor=Constants.Params.KionFactor[(int)weather.GetTodayWeather()];

        return weather.startKionAt+(int)((sa*hourFactor2)*tenkoufactor);
    }




   

}
