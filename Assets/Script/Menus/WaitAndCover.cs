using UnityEngine;
using System.Collections;

public class WaitAndCover : PS_SingletonBehaviour<WaitAndCover>  {

    public void ShowYesNoBtns(string yes,string no,Callback_OnUserYeaNo callback){
        userYesNoEvent+=callback;
        btns.ShowYesNo(yes,no);
    }
    public YesNoBtn btns;
    //はい　いいえのポップアップ
    public YesNoPopup yesno;
    public void ShowYesNoPopup(string tittle,string desc,string yes,string no,Callback_OnUserYeaNo callback){
        faderTween.gameObject.GetComponent<UIEventTrigger>().onClick.Clear();
        userYesNoEvent=null;
        userYesNoEvent+=callback;
        yesno.ShowYesNo(tittle,desc,yes,no);
    }
    public delegate void Callback_OnUserYeaNo(bool flag);
    public  event Callback_OnUserYeaNo userYesNoEvent;
    public void OnYes(){
        AudioManager.Instance.ButtonYes();
        if(yesno.gameObject.activeSelf)yesno.HideYesNo();
        if(userYesNoEvent!=null)userYesNoEvent(true);
    }
    public void OnNo(){
        AudioManager.Instance.Cancel();
        if(yesno.gameObject.activeSelf)yesno.HideYesNo();
        if(userYesNoEvent!=null)userYesNoEvent(false);
    }


    public delegate void Callback_OnUserTappedInfo();
    public  event Callback_OnUserTappedInfo infoTappedEvent;
    //タップさせるポップアップ
    public FaderInfo faderInfo;
    public void ShowInfoPopup(string info,Info_IC icon,Callback_OnUserTappedInfo infoTappedEvent){
        if(faderInfo.gameObject.activeSelf)return;
        this.infoTappedEvent=infoTappedEvent;
        EventDelegate.Add( faderTween.gameObject.GetComponent<UIEventTrigger>().onClick,OnTapCover);
        string iconName="";
        switch(icon){
        case Info_IC.Normal:
            iconName="ic_infos";
            break;
        case Info_IC.Warning:
            iconName="ic_warning";
            break;
        }
        faderInfo.Show(info,iconName,this);
        CoverAll(true);
    }
    public void ShowInfoPopup(string info,Info_IC icon){
        if(faderInfo.gameObject.activeSelf)return;
        infoTappedEvent=null;
        EventDelegate.Add( faderTween.gameObject.GetComponent<UIEventTrigger>().onClick,OnTapCover);
        string iconName="";
        switch(icon){
        case Info_IC.Normal:
            iconName="ic_infos";
            break;
        case Info_IC.Warning:
            iconName="ic_warning";
            break;
        }
        faderInfo.Show(info,iconName,this);
        CoverAll(true);
    }
    public void OnHideTapInfo(){
        if(infoTappedEvent!=null)infoTappedEvent();
    }
    public void OnTapCover(){
        if(faderInfo.gameObject.activeSelf && faderInfo.isTappable){
            faderTween.gameObject.GetComponent<UIEventTrigger>().onClick.Clear();
            faderInfo.OnTapped();
        }
    }

   

    public void Test(){
        ShowInfoList("購入完了 300G");
    }
    public  InfoLauncher info;
    public void ShowInfoList(string info){
       this.info.AddInfo(info);
    }
   

   


    void OnEnable(){
        if(PS_Plugin.Instance!=null)PS_Plugin.Instance.tjListener.earnedEvent+=OnEarnedTJPoints;
    }
    void OnDisabled(){
        if(PS_Plugin.Instance!=null)PS_Plugin.Instance.tjListener.earnedEvent-=OnEarnedTJPoints;
    }
    public void OnEarnedTJPoints(int amount){
        DataManger.Instance.GAMEDATA.Gold+=ES2.Load<int>( DataManger.DataFilename+"?tag=gettedGolds");
        ES2.Save(0,DataManger.DataFilename+"?tag=gettedGolds");
        if(UserName.Instance!=null){
            UserName.Instance.UpdateCurrencyNums();
        }
        DataManger.Instance.SaveData();
    }



    public GameObject shutterObj;
    void ShowShutter(bool isOn){
        if(shutterObj.gameObject.activeSelf!=isOn)NGUITools.SetActive(shutterObj.gameObject,isOn);
    }
    public void Shutter(){
        AudioController.Play("shutter");
        ShowShutter(false);
        CoverAll(false);
        //Call Listener and get callback to OnShared

    }
    public void OnShared(bool isSuccess){
        if(isSuccess){
            
        }else{
            
        }
        UnCoverAll();
        ShowShutter(true);
    }


    bool isWaitInvoking=false;
    public void ShowWait(){
        if(col==null)col=StartCoroutine(WaitInvoke());
    }
    public UISprite waitSprite;
    void ShowWaitSP(bool isOn){
        if(waitSprite.gameObject.activeSelf!=isOn)NGUITools.SetActive(waitSprite.gameObject,isOn);
    }
    Coroutine col;
    IEnumerator WaitInvoke(){
        if(isWaitInvoking)yield break;
           isWaitInvoking=true;
            ShowWaitSP(true);
        while(isWaitInvoking){
            waitSprite.transform.Rotate(Vector3.forward * Time.deltaTime*-600.0f);
            yield return null;
        }
        isWaitInvoking=false;
        ShowWaitSP(false);
        col=null;
    }
    public void StopWait(){
        if(col!=null)StopCoroutine(col);
        ShowWaitSP(false);
        col=null;
        isWaitInvoking=false;
    }
    public void CoverAll(bool isTween){
        if(isCovering)return;
        isCovering=true;
        if(isTween){
            ShowTweenCover(true);
            faderTween.ResetToBeginning();
            faderTween.PlayForward();
        }else{
            ShowCoverNoneTween(true);
        }
    }
    public void UnCoverAll(){
        if(!isCovering)return;
        //Debug.Log("UnCoverAll");
        if(faderOBJ.gameObject.activeSelf) ShowCoverNoneTween(false);
        if(faderTween.gameObject.activeSelf)ShowTweenCover(false);
    }

    public GameObject faderOBJ;
    public void ShowCoverNoneTween(bool isOn){
        isCovering=isOn;

        if(faderOBJ.gameObject.activeSelf!=isOn)NGUITools.SetActive(faderOBJ.gameObject,isOn);
    }

    public TweenAlpha faderTween;
    void CoverAllTween(){
        ShowTweenCover(true);
        faderTween.ResetToBeginning();
        faderTween.PlayForward();
    }
    public void UnCoverAllTween(){
        ShowTweenCover(false);
    }
    public void ShowTweenCover(bool isOn){
        isCovering=isOn;
        if(faderTween.gameObject.activeSelf!=isOn)NGUITools.SetActive(faderTween.gameObject,isOn);
    }
  
    public bool isCovering=false;

    public void OnFaded(){
        if(faderTween.direction==AnimationOrTween.Direction.Forward){

        }else{
            ShowTweenCover(false);
        }
    }

   
}
