using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PS_Toggle : MonoBehaviour {

    public void SetActive(bool isOn,bool isActive){
        //Debug.Log("PS_Toggle SetActive"+isOn+" "+isActive);
        PSGameUtils.ActiveNGUIObject(gameObject,isOn);
        if(isOn){
            if(isActive){
                gameObject.GetComponent<UISprite>().alpha=1.0f;
            }else{
                gameObject.GetComponent<UISprite>().alpha=0.3f;
            }
        }
    }

    void Start(){
        toggleLB.text=Localization.Get("MainRod");
    }
    public Color toggleBG_OnT;
    public Color toggleBG_OnB;
    public Color toggleBG_OffT;
    public Color toggleBG_OffB;


    public UISprite mainToggle;
    public UILabel mainToggleLB;
    public UILabel toggleLB;
    public bool isOn=false;
    public void SetToggle(bool isMain){
        
        if(isMain){
            mainToggleLB.text="YES";
            mainToggle.transform.localPosition=new Vector3(-134.72f,mainToggle.transform.localPosition.y,mainToggle.transform.localPosition.z);
            mainToggleLB.transform.localPosition=new Vector3(47.15f,mainToggleLB.transform.localPosition.y,mainToggleLB.transform.localPosition.z);
            gameObject.GetComponent<UISprite>().gradientTop=toggleBG_OnT;
            gameObject.GetComponent<UISprite>().gradientBottom=toggleBG_OnB;
        }else{
            mainToggleLB.text="NO";
            mainToggle.transform.localPosition=new Vector3(-18.39f,mainToggle.transform.localPosition.y,mainToggle.transform.localPosition.z);
            mainToggleLB.transform.localPosition=new Vector3(-41.2f,mainToggleLB.transform.localPosition.y,mainToggleLB.transform.localPosition.z);
            gameObject.GetComponent<UISprite>().gradientTop=toggleBG_OffT;
            gameObject.GetComponent<UISprite>().gradientBottom=toggleBG_OffB;
        }
        this.isOn=isMain;
    }
    public void OnTapped(){
        if(isOn){
            Debug.LogError("すでにメインなので変更はできない");
            //SetToggle(this.isOn?false:true);
        }else{
            SetToggle(this.isOn?false:true);
            if(SetRodToMain!=null)EventDelegate.Execute(SetRodToMain);
        }
    }

    public List<EventDelegate> SetRodToMain = new List<EventDelegate>();

}
