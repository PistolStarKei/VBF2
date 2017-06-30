using UnityEngine;
using System.Collections;
using System;


public class EquipLure :MenuContents {

    public override void Show(){
        Debug.Log("Show"+gameObject.name);
        if(!isShow){
            current= FishingStateManger.Instance.currentMode;
            Debug.Log("Show"+FishingStateManger.Instance.currentMode.ToString());
            FishingStateManger.Instance.ChangeStateTo(GameMode.Menu);
            OnShowStart();
            isShow=true;
            MenuTPs.duration=tpsDuration[0];
            MenuTAs.duration=tasDuration[0];
            MenuTPs.PlayForward();
            MenuTAs.PlayForward();
            MenuTPs.onFinished.Clear();
            EventDelegate.Add(MenuTPs.onFinished,OnShowComplete);
            if(!gameObject.activeSelf)NGUITools.SetActive(gameObject,true);
            EventDelegate.Execute(onShow);
        }
    }
    public override void Hide(){
        if(isShow){
            
            OnHideStart();
            isShow=false;
            MenuTPs.duration=tpsDuration[1];
            MenuTAs.duration=tasDuration[1];
            MenuTPs.PlayReverse();
            MenuTAs.PlayReverse();
            HideLeft();
            MenuTPs.onFinished.Clear();
            EventDelegate.Add(MenuTPs.onFinished,OnHideComplete);
            EventDelegate.Execute(onHide);
        }
    }

    public void OnShowStart(){
        ShowLeft();
    }
    public void OnHideStart(){
        Debug.LogWarning("ロッドをアフェクトする");


    }
    GameMode current;
    public override void OnHideComplete(){
        TackleParams.Instance.OnEquiMenuClosed();
        FishingStateManger.Instance.ChangeStateTo(current);
        NGUITools.SetActive(lureMenu.lurelistObj.gameObject,false);
        NGUITools.SetActive(lureMenu.rodlistObj.gameObject,false);

        base.OnHideComplete();
    }
    public override void OnShowComplete(){
        Debug.Log("OnShowComplete"+FishingStateManger.Instance.currentMode.ToString());
        ShowContents();
    }

    public TackeMenuParent lureMenu;
    public void ShowContents(){
        lureMenu.Show(0);

    }

    public TweenPosition MenuTPsB;
    public TweenAlpha MenuTAsB;

    void HideLeft(){
        MenuTPsB.duration=tpsDuration[1];
        MenuTAsB.duration=tasDuration[1];
        MenuTPsB.PlayReverse();
        MenuTAsB.PlayReverse();
    }
    void ShowLeft(){
        MenuTPsB.duration=tpsDuration[0];
        MenuTAsB.duration=tasDuration[0];
        MenuTPsB.PlayForward();
        MenuTAsB.PlayForward();
    }

}
