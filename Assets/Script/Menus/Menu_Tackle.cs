using UnityEngine;
using System.Collections;
using System;
public class Menu_Tackle :MenuContents {

    public override void Show(){
        Debug.Log("Show"+gameObject.name);
        if(!isShow){
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
        
        if(MenuManager.Instance.tab.gameObject.activeSelf)NGUITools.SetActive(MenuManager.Instance.tab.gameObject,false);
        ShowLeft();
        tackleTabs.onTabChanged+=OnTabChanged;
        tackleTabs.currentTabs=tackleTabs.tabs[Array.IndexOf(Constants.Params.Tab_SHOP,"HARD")];
        tackleTabs.currentTabs.SetState(true);
        UserName.Instance.SetPosition(false);
    }
    public void OnHideStart(){
        if(MenuManager.Instance.tacleTab.gameObject.activeSelf)NGUITools.SetActive(MenuManager.Instance.tacleTab.gameObject,false);
        tackleTabs.ClearState();
        NGUITools.SetActive(tackleTabs.gameObject,false);
    }
    public override void OnHideComplete(){
        base.OnHideComplete();
    }
    public override void OnShowComplete(){
        MenuManager.Instance.backToHome.Show();
        Debug.Log(LureShop_Menu_Hard.GetMethodName()+this.name+"1");
        if(!MenuManager.Instance.tacleTab.gameObject.activeSelf)NGUITools.SetActive(MenuManager.Instance.tacleTab.gameObject,true);
       
        NGUITools.SetActive(tackleTabs.gameObject,true);
       
        tackleTabs.SetTabState("HARD");
        Debug.Log(LureShop_Menu_Hard.GetMethodName()+this.name+"2");
        ShowContents("HARD");
       
        Debug.Log(LureShop_Menu_Hard.GetMethodName()+this.name+"3");
    }

    public Menu_Tab tackleTabs;
    public void OnTabChanged(string menu){
        Debug.Log("OnTabChanged");
        ShowContents(menu);
    }
    public TackeMenuParent[] lureMenu;
    public void ShowContents(string menu){
        if(menu=="ROD"){
            lureMenu[3].Show(0);
        }else if(menu=="LINE"){
            lureMenu[2].Show(0);
        }else if(menu=="SOFT"){
            lureMenu[1].Show(0);
        }else if(menu=="HARD"){
            lureMenu[0].Show(0);
        }

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
