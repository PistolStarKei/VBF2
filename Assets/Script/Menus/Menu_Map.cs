using UnityEngine;
using System.Collections;

public class Menu_Map :MenuContents {

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
            MenuTPs.onFinished.Clear();
            EventDelegate.Add(MenuTPs.onFinished,OnHideComplete);
            EventDelegate.Execute(onHide);
        }
    }
    public void OnShowStart(){
        UserName.Instance.SetPosition(false);
        if(!MenuManager.Instance.tab.gameObject.activeSelf)NGUITools.SetActive(MenuManager.Instance.tab.gameObject,true);
        DataManger.Instance.UpdateTimedata(true);
    }
    public void OnHideStart(){

    }
    public override void OnHideComplete(){
        base.OnHideComplete();
    }
    public override void OnShowComplete(){
        MenuManager.Instance.backToHome.Show();
        base.OnShowComplete();
    }




    void ShowIfNot(GameObject go,bool isShow){
        if(go.activeSelf!=isShow)NGUITools.SetActive(go,isShow);
    }

    public UIScrollView view;
    public GameObject topIndecater;
    public GameObject btmIndecater;
    public GameObject rightIndecater;
    public GameObject leftIndecater;
    void FixedUpdate(){
        //0 top 1 btm
        if(view.gameObject.activeSelf){
            if( view.verticalScrollBar.value<=0.1f){
                //top 下がまだある
                ShowIfNot(topIndecater,false);
            }else{
                ShowIfNot(topIndecater,true);
                if(view.verticalScrollBar.value>=0.9f){
                    //btm
                    ShowIfNot(btmIndecater,false);
                }else{
                    ShowIfNot(btmIndecater,true);
                }
            }
            if( view.horizontalScrollBar.value<=0.1f){
                //top 下がまだある
                ShowIfNot(rightIndecater,false);
            }else{
                ShowIfNot(rightIndecater,true);
                if(view.horizontalScrollBar.value>=0.9f){
                    //btm
                    ShowIfNot(leftIndecater,false);
                }else{
                    ShowIfNot(leftIndecater,true);
                }
            }
        }else{
            ShowIfNot(btmIndecater,false);
            ShowIfNot(topIndecater,false);
            ShowIfNot(leftIndecater,false);
            ShowIfNot(rightIndecater,false);
        }

    }
}
