using UnityEngine;
using System.Collections;

public class SPAvility : MonoBehaviour {

    public UILabel lb;

    public void SetActive(bool isOn){
        if(gameObject.activeSelf!=isOn)NGUITools.SetActive(gameObject,isOn);
    }
    public void SetText(string str){
        lb.text=str;
    }
    public void SetEnabled(bool isOn){
        if(isOn){
            lb.color=Color.white;
        }else{
            lb.color=GUIColors.Instance.GUI_SPAvility_None;
        }
    }
}
