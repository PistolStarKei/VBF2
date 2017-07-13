using UnityEngine;
using System.Collections;
using System;

public class NegakariMeter : PS_SingletonBehaviour<NegakariMeter> {

    public void Test(){
        Show(3.0f);
    }
    public void Show(float showTime){
        if(isInvoking){
            Debug.LogError("Show 実行中");
            return;
        }
        NGUITools.SetActive(guis,false);
        col=StartCoroutine(ShowGUI(showTime));

    }

    Coroutine col;
    public GameObject guis;
    bool isInvoking=false;
     IEnumerator ShowGUI(float showTime){
        if(isInvoking)yield break;
        isInvoking=true;
        float wait=0.0f;
        while(true){
            wait+=Time.deltaTime;
            if(LineScript.Instance.lineTention>=1.9f && wait>showTime){
                break;
            }
            yield return null;

        }
        NGUITools.SetActive(guis,true);
        timeLimit=5.0f;
        JoystickFloat.Instance.foockedPower=0.0f;
        rodMovedTime=0.0f;
        rodMovedLength=0.0f;
        isTimerOn=true;
        AudioManager.Instance.Alert(true);

    }


    public UILabel timeLabel;
    TimeSpan timeSpan;
    public UISprite timeSP;
    void SetTime(){
        if(timeLimit<=0.0f){
            timeLabel.text = "0:00";
            OnTimeUp();
            return;
        }
        timeSpan = TimeSpan.FromSeconds(timeLimit);
        timeSP.fillAmount=1.0f-(timeLimit/5.0f);
        timeLabel.text = string.Format("{0:D1}:{1:D2}", timeSpan.Seconds,Mathf.FloorToInt(timeSpan.Milliseconds/10.0f));
    }
    float timeLimit=5.0f;
    bool isTimerOn=false;
	// Update is called once per frame
	void Update () {
        if(isTimerOn){
            timeLimit-=Time.deltaTime;
            SetTime();
        }

	}
    float rodMovedTime=0.0f;
    float rodMovedLength=0.0f;

    public void OnRodMove(Vector2 delta,Vector2 preVDelata){
        if(!isTimerOn ) return;
        Debug.Log("回避抽選"+delta+"prevDelata"+preVDelata);
        if(LineScript.Instance.isLineHasTention()){
            rodMovedTime+=Time.deltaTime;
            rodMovedLength+=Vector2.Distance(preVDelata,delta);
            if(rodMovedLength>=0.4f){
                Debug.Log("回避抽選");
                OnFoocked((rodMovedLength/rodMovedTime));

                rodMovedTime=0.0f;
                rodMovedLength=0.0f;
            } 
        }else{
            Debug.Log("テンションなし");
            rodMovedTime=0.0f;
            rodMovedLength=0.0f;
        }

       
    }

    public  void OnFoocked(float power){

            if(power>10.0f)power=10.0f;

            if(PSGameUtils.Chusen(0.05f)){
                FoockOff();
            }
    }

    void FoockOff(){
        isTimerOn=false;
        AudioManager.Instance.LevelUp2();
        GameController.Instance.OnNegakariKaihi();
        Hide();
    }

    public void Hide(){
        if(col!=null && isInvoking)StopCoroutine(col);
        isInvoking=false;
        NGUITools.SetActive(guis,false);

        AudioManager.Instance.Alert(false);
    }
    void OnTimeUp(){
        isTimerOn=false;
       
        GameController.Instance.OnNegakariKaihiTimeOver();
        Hide();

    }
}
