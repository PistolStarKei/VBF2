using UnityEngine;
using System.Collections;

public class RureItem : MonoBehaviour {

    public LureItemContainer container;

    public int itemNum=0;
    public bool isSelected=false;
    public void SetItems(int id,string tittle,int num,int maxhas,string itemSp,LureItemContainer container,bool isAvaillable,string markSp,UIAtlas at){
        itemNum=id;
        this.isAvaillable=isAvaillable;
        this.container=container;
        this.tittle.text=tittle;
        item.atlas=at;
        item.material=at.spriteMaterial;
        item.spriteName=itemSp;
        heavyLabel.text=markSp;
        SetSelect(false);
        SetNums(num,maxhas);
        SetExtraDesc("");
    }

    public void SetItems(int id,string tittle,int num,int has,int equip,int maxhas,string itemSp,LureItemContainer container,bool isAvaillable,string markSp,UIAtlas at){
        itemNum=id;
        if(has<=0){
            this.isAvaillable=false;
        }else{
            this.isAvaillable=isAvaillable;
        }
        this.container=container;
        this.tittle.text=tittle;
        item.atlas=at;
        item.material=at.spriteMaterial;
        item.spriteName=itemSp;
        heavyLabel.text=markSp;
        SetSelect(false);
        SetNums(num,has,equip,maxhas);
        SetExtraDesc("");
    }



    public UILabel lbls;
    public void SetItems(int id,string tittle,int num,int maxhas,string itemSp,LureItemContainer container,bool isAvaillable,string markSp,string extra,UIAtlas at){
        itemNum=id;

        this.isAvaillable=isAvaillable;
        this.container=container;
        this.tittle.text=tittle;
        item.atlas=at;
        item.spriteName=itemSp;
        item.material=at.spriteMaterial;

        heavyLabel.text=markSp;
        SetSelect(false);
        SetNums(num,maxhas);
        SetExtraDesc(extra);
       
    }

    public void SetItems(int id,string tittle,int num,int has,int equip,int maxhas,string itemSp,LureItemContainer container,bool isAvaillable,string markSp,string extra,UIAtlas at){
        itemNum=id;

        if(has<=0){
            this.isAvaillable=false;
        }else{
            this.isAvaillable=isAvaillable;
        }
       
        this.container=container;
        this.tittle.text=tittle;
        item.atlas=at;
        item.spriteName=itemSp;
        item.material=at.spriteMaterial;

        heavyLabel.text=markSp;
        SetSelect(false);
        SetNums(num,has,equip,maxhas);
        SetExtraDesc(extra);

    }
    public void SetExtraDesc(string str){
        if(str==""){
            if(lbls.gameObject.activeSelf!=false)
            NGUITools.SetActive(lbls.gameObject,false);
        }else{
            if(lbls.gameObject.activeSelf!=true)NGUITools.SetActive(lbls.gameObject,true);
            lbls.text=str;
        }
    }


    public UILabel heavyLabel;
    void OnClick () {
        container.OnTapped(itemNum);
    }
    public bool isAvaillable=false;
    public UILabel tittle;
    public UISprite item;
    public UISprite bg;
    public void SetSelect(bool isSelected){
        this.isSelected=isSelected;
        if(isSelected){
            bg.color=GUIColors.Instance.GUI_List_Selected;
            heavyLabel.color=GUIColors.Instance.GUI_List_UnAvaillable;
        }else{
            if(isAvaillable){
                bg.color=GUIColors.Instance.GUI_List_Availlable;
                heavyLabel.color=GUIColors.Instance.GUI_List_UnAvaillable;
            }else{
                bg.color=GUIColors.Instance.GUI_List_UnAvaillable;
                heavyLabel.color=Color.white;
            }
        }
    }

    public SetNumsGUI setNum;
    public void SetNums(int num,int max){
        setNum.SetNums(num,max);
    }

    public void SetNums(int num,int has,int equip,int max){
        Debug.Log("setNums num="+num+" has="+has+" eq="+equip+" max="+max);
        setNum.SetNums(num,has,equip,max);
    }

}
