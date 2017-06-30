using UnityEngine;
using System.Collections;

public class AddOnBtnEquip : MonoBehaviour {

    public UILabel tittle;
    public UILabel price;
    public UILabel desc;
    public UISprite selected;
    public UISprite bg;
    public UISprite item;
    public SoftAddOns_Equip adons;

    public void SetItems(string tittle,string desc,string price,string itemSp,bool isBuyed,bool isEquipped){
        this.tittle.text=tittle;
        this.desc.text=desc;
        item.spriteName=itemSp;

        SetState(isBuyed,isEquipped);

    }
    public void SetItems(){
        SetState();
    }
    public void SetEquipped(bool isOn){

        if(isOn){
            bg.color=bg_selectable;
            selected.color=equippedColor;
            this.price.color=equippedColor;
            this.price.text=Localization.Get("Equipped");
        }else{
            bg.color=bg_selectable;
            selected.color=Color.black;
            this.price.color=Color.white;
            this.price.text=Localization.Get("Equippable");
        }

    }
    public UISprite logo;
    public bool isEquipped=false;
    public void SetState(bool isBuyed,bool isEquipped){
        logo.enabled=false;
        gameObject.GetComponent<UIWidget>().alpha=1.0f;
        if(isBuyed){
            gameObject.GetComponent<UIWidget>().alpha=1.0f;
            this.desc.color=bg_unselectable;
            if(isEquipped){
                this.isEquipped=true;
                //購入済み　で装着中
                SetEquipped(isEquipped);
            }else{
                this.isEquipped=false;
                //購入済み　で装着可能
                SetEquipped(isEquipped);
            }
        }else{
            gameObject.GetComponent<UIWidget>().alpha=0.3f;
            this.isEquipped=false;
            //未購入　　
            bg.color=bg_unselectable;
            this.desc.color=bg_selectable;
            selected.color=Color.black;
            this.price.text=Localization.Get("NotAvaillable");
            this.price.color=Color.white;
        }
    }
    public void SetState(){
        gameObject.GetComponent<UIWidget>().alpha=0.3f;
        //購入不可能　非対応
        logo.enabled=true;
        bg.color=bg_unselectable;
        this.desc.text="";
        this.tittle.text="";
        this.price.text="";
        item.spriteName="";
        selected.color=Color.black;
        gameObject.GetComponent<UIWidget>().alpha=0.23f;
    }






    public Color bg_selectable;
    public Color bg_unselectable; 
    public Color equippedColor;

    public int num=0;
    public void OnTapped(){
        Debug.Log("OnTapped"+num);
        adons.OnClicked(num,isEquipped);
    }


}
