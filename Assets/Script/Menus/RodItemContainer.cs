using UnityEngine;
using System.Collections;
using PathologicalGames;
public class RodItemContainer : MonoBehaviour {

    public bool isUsed=false;
    public void SetActive(bool isActive){
        PSGameUtils.ActiveNGUIObject(gameObject,isActive);
    }
        
    public delegate void Callback_OnTappedItem(int num,bool isInit);
    public  event Callback_OnTappedItem OnTappedItem;

    public void OnClick(){
        if(OnTappedItem!=null)OnTappedItem(itemNum,false);
    }


    public int itemNum=0;
    public bool isSelected=false;
    public void SetItems(int id,string tittle,int num,int maxhas,string itemSp,bool isAvaillable,string markSp,UIAtlas at,Callback_OnTappedItem tapEvent){
        Debug.Log(""+isAvaillable);
        this.OnTappedItem+=tapEvent;
        itemNum=id;
        this.isAvaillable=isAvaillable;
        this.tittle.text=tittle;
        item.atlas=at;
        item.material=at.spriteMaterial;
        item.spriteName=itemSp;
        marksp.text=markSp;
        SetExtraDesc("");
        SetSelect(false);
        SetNums(num,maxhas);
        isUsed=true;
    }
    public UILabel lbls;
    public void SetItems(int id,string tittle,int num,int maxhas,string itemSp,bool isAvaillable,string markSp,string extra,UIAtlas at,Callback_OnTappedItem tapEvent){

        Debug.Log(""+isAvaillable);
        this.OnTappedItem+=tapEvent;
        itemNum=id;
        this.isAvaillable=isAvaillable;
        this.tittle.text=tittle;
        item.atlas=at;
        item.spriteName=itemSp;
        item.material=at.spriteMaterial;
        marksp.text=markSp;
        SetExtraDesc(extra);
        SetSelect(false);
        SetNums(num,maxhas);

        isUsed=true;

    }

    public UISprite extraBg;
    public void SetExtraDesc(string str){

        Debug.Log("SetExtraDesc"+str);
        if(str==""){
            if(lbls.gameObject.activeSelf!=false)
                NGUITools.SetActive(lbls.gameObject,false);
        }else{
            if(str=="Main"){
                if(lbls.gameObject.activeSelf!=true)NGUITools.SetActive(lbls.gameObject,true);
                lbls.text="DEF";
            }else{
                if(lbls.gameObject.activeSelf!=true)NGUITools.SetActive(lbls.gameObject,true);
                lbls.text="CUR";
            }

        }
    }

    public UILabel marksp;
    public bool isAvaillable=false;
    public UILabel tittle;
    public UISprite item;
    public UISprite bg;

    public Color extraDef;
    public Color extraCul;
    public void SetSelect(bool isSelected){
        this.isSelected=isSelected;
        if(isSelected){
            if(lbls.text!=""){
                if(lbls.text=="DEF"){
                    lbls.color=Color.white;
                    extraBg.color=extraDef;
                }else{
                    lbls.color=Color.white;
                    extraBg.color= extraCul;
                }

            }
            bg.color=GUIColors.Instance.GUI_List_Selected;
            marksp.color=GUIColors.Instance.GUI_List_UnAvaillable;
        }else{
            if(isAvaillable){
                bg.color=GUIColors.Instance.GUI_List_Availlable;
                marksp.color=GUIColors.Instance.GUI_List_UnAvaillable;

                if(lbls.text!=""){
                    if(lbls.text=="DEF"){
                        lbls.color=Color.white;
                        extraBg.color=extraDef;
                    }else{
                        lbls.color=Color.white;
                        extraBg.color= extraCul;
                    }

                }

            }else{
                bg.color=GUIColors.Instance.GUI_List_UnAvaillable;
                marksp.color=Color.white;

                if(lbls.text!=""){
                    if(lbls.text=="Main"){
                        lbls.color=Color.white;
                        extraBg.color=extraDef;
                    }else{
                        lbls.color=Color.white;
                        extraBg.color= extraCul;
                    }

                }
            }
        }

    }
    public SetNumsGUI setNum;
    public void SetNums(int num,int max){
        setNum.SetNums(num,max);
    }

}
