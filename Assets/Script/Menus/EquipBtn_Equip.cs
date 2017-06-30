using UnityEngine;
using System.Collections;

public class EquipBtn_Equip : MonoBehaviour {



    public void SetDelegate(Callback_OnEquip onEquip){
        this.onEquip=onEquip;
    }
    public LureSpriteContainer current;
    public LureSpriteContainer kouho;
    public void SetCurrent(string tittle,string spName,UIAtlas at){

        current.SetLureParams(tittle, spName, at);
    }

    public void SetCurrent(string tittle,string spName,UIAtlas at,string addOnSp,bool shouldShowBack,Vector3 AdonLocalTrans,Vector3 AdonLocalRot){
        current.SetLureParams(tittle, spName, at);
        current.SetRig(addOnSp,shouldShowBack,AdonLocalTrans,AdonLocalRot);
    }

    public void SetKouho(string tittle,string spName,UIAtlas at,bool isAvaillable){
        kouho.SetLureParams(tittle, spName, at);
        SetEquipBtn(isAvaillable);
    }

    public void SetKouho(string tittle,string spName,UIAtlas at,string addOnSp,bool shouldShowBack,Vector3 AdonLocalTrans,Vector3 AdonLocalRot,bool isAvaillable){
        kouho.SetLureParams(tittle, spName, at);
        kouho.SetRig(addOnSp,shouldShowBack,AdonLocalTrans,AdonLocalRot);
        SetEquipBtn(isAvaillable);
    }

    public UISprite eqpBtn;

    void SetEquipBtn(bool isAvaillable){
        //Debug.Log("Equip btn to"+isAvaillable);
        this.isAvaillable=isAvaillable;
        eqpBtn.alpha=isAvaillable?1.0f:0.3f;

    }
    public bool isAvaillable=false;

    public RaderGraph rader;

    public delegate void Callback_OnEquip();
    public  event Callback_OnEquip onEquip;


    public void OnEquip(){
        if(isAvaillable)if(onEquip!=null)onEquip();
    }
    public void OnClose(){
        rods.OnCloseEquip();
    }

    public EquipMenu_Rod rods;


    public UILabel aishouTotal;
    public UILabel aishouCurrent;
    public UILabel aishouChange;
    public void SetAishouTotal(float num1,float num2){

        Debug.Log("SetAishouTotal"+num1.ToString("F2")+" "+num2.ToString("F2"));
        if(num1<-10.0f && num2<-10.0f){
            SetAishouLbl(aishouTotal, -100.0f);
        }else{
            if(num1<-10.0f){
                SetAishouLbl(aishouTotal, num1);
            }else if(num2<-10.0f){
                SetAishouLbl(aishouTotal, num2);
            }else{
                SetAishouLbl(aishouTotal, num1+num2);
            }

        }

    }
    public void SetAishouCurrent(float num){
        //Debug.Log("SetAishouCurrent"+num);
        SetAishouLbl(aishouCurrent, num);
    }
    public void SetAishouChanged(float num){

        SetAishouLbl(aishouChange, num);
    }


    public void SetAishouLbl(UILabel lbl, float aishou){

        if(aishou<-10.0f){
            lbl.text="--";
            lbl.color=Color.white;
        }else{
            if(aishou==0.0f){
                lbl.text=Localization.Get("MatchHeavy");
                lbl.color=GUIColors.Instance.MatchColor_Plus;
            }else if(aishou<0.0f){
                lbl.color=GUIColors.Instance.MatchColor_Minus;
                lbl.text=PSGameUtils.GetPercent(aishou);
            }else{
                lbl.color=GUIColors.Instance.MatchColor_Plus;
                lbl.text=PSGameUtils.GetPercent(aishou);
            }

        }

    }
}
