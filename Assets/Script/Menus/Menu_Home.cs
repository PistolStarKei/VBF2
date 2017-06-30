using UnityEngine;
using System.Collections;

public class Menu_Home : MenuContents {

    public ReaderBoadAffecter boadDataLoader;
    public ReaderBoadAffecterFriend boadDataLoaderFriend;
    public float[] tpsDurationB=new float[2]{0.5f,0.1f};
    public float[] tasDurationB=new float[2]{0.4f,0.3f};
    public TweenPosition MenuTPsB;
    public TweenAlpha MenuTAsB;


    public override void Show(){
        if(!isShow){
            if(!gameObject.activeSelf)NGUITools.SetActive(gameObject,true);
            OnShowStart();
            isShow=true;
           
            ShowTop();
            ShowBtm();
            EventDelegate.Execute(onShow);
        }
    }
    public override void Hide(){
        if(isShow){
            
            OnHideStart();
            isShow=false;
            HideTop();
            HideBtm();
            EventDelegate.Execute(onHide);
        }
    }
    void HideTop(){
        MenuTPs.duration=tpsDuration[1];
        MenuTAs.duration=tasDuration[1];
        MenuTPs.PlayReverse();
        MenuTAs.PlayReverse();
        MenuTPs.onFinished.Clear();
        EventDelegate.Add(MenuTPs.onFinished,OnHideComplete);
    }
    void HideBtm(){
        MenuTPsB.duration=tpsDuration[1];
        MenuTAsB.duration=tasDuration[1];
        MenuTPsB.PlayReverse();
        MenuTAsB.PlayReverse();
    }

    void ShowTop(){
        MenuTPs.duration=tpsDuration[0];
        MenuTAs.duration=tasDuration[0];
        MenuTPs.PlayForward();
        MenuTAs.PlayForward();
        MenuTPs.onFinished.Clear();
        EventDelegate.Add(MenuTPs.onFinished,OnShowComplete);
    }
    void ShowBtm(){
        MenuTPsB.duration=tpsDuration[0];
        MenuTAsB.duration=tasDuration[0];
        MenuTPsB.PlayForward();
        MenuTAsB.PlayForward();
    }

    public SetPlayerStats stats;
    public GameObject inFight;
    public UILabel inFightLb;
    public UILabel shoukinLb;
    public UISprite[] catSPs;
    public Transform category;
    public LoginBonusBtn loginBonus;
    void SetCategory(int cat){
        loginBonus.Show(DataManger.Instance.GAMEDATA.earnedLoginBonus?false:true);
        if(DataManger.Instance.GAMEDATA.currentEntry==null){
            NGUITools.SetActive(inFight,false);
        }else{
            NGUITools.SetActive(inFight,true);
            inFightLb.text=Localization.Get("Entry");
        }
        shoukinLb.text=Localization.Get("Shoikin")+" "+Constants.Params.shoukin[cat].ToString()+"$";

        foreach(UISprite sp in catSPs){
            sp.color=new Color(45.0f/255.0f,45.0f/255.0f,45.0f/255.0f);
        }
        catSPs[cat].color=Color.white;


    }

    public void OnShowStart(){
        UserName.Instance.SetPosition(true);
        if(!MenuManager.Instance.tab.gameObject.activeSelf)NGUITools.SetActive(MenuManager.Instance.tab.gameObject,true);

        SetCategory(DataManger.Instance.currentCategory);
        if(DataManger.Instance!=null){
            stats.SetDatas(DataManger.Instance.GAMEDATA.playerLevel
                ,DataManger.Instance.GAMEDATA.expOnLevel,Constants.Params.expToNext(DataManger.Instance.GAMEDATA.playerLevel),DataManger.Instance.GAMEDATA.totalWins);
            //TODO 前にトーナメント情報をセットする。boadDataLoader.ID
            //カテゴリ　参加中　
            boadDataLoader.UpdateScores(GPGSListener.Instance.GetCurrentPushID());
        }else{
            boadDataLoaderFriend.StartWait();
            boadDataLoader.StartWait();
        }
    }
    public void OnHideStart(){
        if(DataManger.Instance!=null){
            boadDataLoader.StopWait();
        }else{
            boadDataLoader.StopWait();
        }
    }
    public virtual void OnHideComplete(){
        base.OnHideComplete();
    }
    public virtual void OnShowComplete(){
        Debug.Log("OnShowComplete");
       
        if(DataManger.Instance!=null && DataManger.Instance.GetExp()>0){
                //獲得したEXPあり、アニメーションさせる。
            WaitAndCover.Instance.ShowTweenCover(false);
            WaitAndCover.Instance.ShowCoverNoneTween(true);
                stats.StartProgressAnimations(this);
        }else{
            Debug.Log("OnShowComplete 1");
            base.OnShowComplete();
        }
        

    }

    public void OnScoreLoaded(){
        Debug.Log("OnScoreLoaded");
        boadDataLoaderFriend.UpdateScores(GPGSListener.Instance.GetCurrentPushID());
    }
    public void OnHatena(){
        AudioController.Play("btn");
    }

    public void Invite(){
        AudioController.Play("btn");
    }
}
