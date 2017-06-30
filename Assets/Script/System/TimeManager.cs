using UnityEngine;
using System.Collections;
using System;

public class TimeManager : PS_SingletonBehaviour<TimeManager>  {

    public bool isSameDayLogin=false;
    public int currentYear=0;
    public int currentMonth=0;
    public int previousMonth=0;
    public int time=0;
    public void UpdateTime(){
        time=DateTime.Now.Hour;
    }

    void SetMonths(){
        currentMonth=DateTime.Now.Month;
        previousMonth=DateTime.Now.AddMonths(-1).Month;
        currentYear=DateTime.Now.Year;
    }
    public bool CheckSameDayLogin(string last,int  RenzokuLogin){
        
        DateTime lastTime=  PSGameUtils.StringToDateTime(last);

        SetMonths();
       
        if(PSGameUtils.IsSameDayLogin(lastTime)){
            Debug.Log("TimeManager 同じ日");
            isSameDayLogin=true;
        }else{

            DataManger.Instance.GAMEDATA.earnedLoginBonus=false;
            isSameDayLogin=false;
            if(PSGameUtils.IsRenzoku(lastTime)){
               
                RenzokuLogin++;
                DataManger.Instance.GAMEDATA.RenzokuLogin=RenzokuLogin;
                Debug.Log("TimeManager 違う日　連続"+DataManger.Instance.GAMEDATA.RenzokuLogin);
                ES2.Save(DataManger.Instance.GAMEDATA.RenzokuLogin,  DataManger.DataFilename+"?tag=RenzokuLogin");
            }else{
                Debug.Log("TimeManager 違う日　連続ブレイク");
                RenzokuLogin=0;
                DataManger.Instance.GAMEDATA.RenzokuLogin=RenzokuLogin;
            }
            DataManger.Instance.GAMEDATA.earnedLoginBonus=false;
            ES2.Save(DataManger.Instance.GAMEDATA.earnedLoginBonus,DataManger.DataFilename+"?tag=earnedLoginBonus");
            DataManger.Instance.SaveCurrentTime("?tag=lastLoginTime");
        }
        UpdateTime();
        return isSameDayLogin;
    }


   

}
