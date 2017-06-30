using UnityEngine;
using System.Collections;

public class BotomSetter : MonoBehaviour {

    void ShowNA(){
        PSGameUtils.ActiveNGUIObject(bottomBuyBtns,true);
    }
    void HideNA(){
        PSGameUtils.ActiveNGUIObject(bottomBuyBtns,false);
    }

    public void OnNA(){
        ShowNA();
        SetMiddle(true,false);
        PSGameUtils.ActiveNGUIObject(bottomEquipped,false);
        PSGameUtils.ActiveNGUIObject(bottomEquipBtn,false);
        PSGameUtils.ActiveNGUIObject(bottomAddons,false);
    }

    public void OnRodSoubi(){
        SetMiddle(true,false);
        PSGameUtils.ActiveNGUIObject(bottomAddons,false);
        PSGameUtils.ActiveNGUIObject(bottomEquipped,true);
        PSGameUtils.ActiveNGUIObject(bottomEquipBtn,false);
        HideNA();
    }

    public void OnLureChange(bool isHard){
        SetMiddle(false,isHard?false:true);
        HideNA();
        PSGameUtils.ActiveNGUIObject(bottomEquipped,false);

        if(isHard){
            PSGameUtils.ActiveNGUIObject(bottomAddons,false);
            PSGameUtils.ActiveNGUIObject(bottomEquipBtn,true);
        }else{
            PSGameUtils.ActiveNGUIObject(bottomAddons,true);
            PSGameUtils.ActiveNGUIObject(bottomEquipBtn,true);
        }
    }

    public GameObject bottomEquipBtn;
    public GameObject bottomBuyBtns;
    public GameObject bottomAddons;
    public GameObject bottomEquipped;

    public GameObject middleObj;
    public Transform topObj;
    public Transform equipObj;
    public Transform adonObj;
    public void SetMiddle(bool isShow,bool isSoft){
        if(isShow){
            PSGameUtils.ActiveNGUIObject(middleObj,true);
            topObj.transform.localPosition=new Vector3(-40.2f,128.34f,0.0f);
        }else{
            PSGameUtils.ActiveNGUIObject(middleObj,false);

            if(isSoft){
                topObj.transform.localPosition=new Vector3(-40.2f,132.7f,0.0f);
                equipObj.transform.localPosition=new Vector3(-800f,-559.2f,0.0f);
                adonObj.transform.localPosition=new Vector3(-795.6f,-264.3f,0.0f);
            }else{
                topObj.transform.localPosition=new Vector3(-40.2f,-8.28f,0.0f);
                equipObj.transform.localPosition=new Vector3(-800f,-396.56f,0.0f);
                adonObj.transform.localPosition=new Vector3(-40.2f,-396.56f,0.0f);
            }


        }
    }
   
}
