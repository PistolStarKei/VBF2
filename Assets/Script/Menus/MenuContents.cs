using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuContents : MonoBehaviour {


  
    public bool isShow=false;
    public float[] tpsDuration=new float[2]{0.5f,0.1f};
    public float[] tasDuration=new float[2]{0.4f,0.3f};
    public TweenPosition MenuTPs;
    public TweenAlpha MenuTAs;

    public virtual void Show(){
        Debug.Log("MenuContents Show");
        if(!isShow){
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
    public virtual void Hide(){
        if(isShow){
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

    public virtual void OnHideComplete(){
        if(!isShow){
            EventDelegate.Execute(onHideComplete);
            if(MenuManager.Instance!=null)MenuManager.Instance.OnHideCurrent(this);
            if(gameObject.activeSelf)NGUITools.SetActive(gameObject,false);
        }
    }
    public virtual void OnShowComplete(){
        if(isShow){
            EventDelegate.Execute(onShowComplete);
            if(MenuManager.Instance!=null)MenuManager.Instance.OnShowedContents(this);
        }
    }
    public List<EventDelegate> onHide = new List<EventDelegate>();
    public List<EventDelegate> onHideComplete = new List<EventDelegate>();
    public List<EventDelegate> onShow = new List<EventDelegate>();
    public List<EventDelegate> onShowComplete = new List<EventDelegate>();
}
