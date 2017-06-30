using UnityEngine;
using System.Collections;

public class ShopContentHolder : MonoBehaviour {

    public UILabel tittle;
    public UILabel desc;
    public UILabel btnLb;
    public UILabel offer;
    public void SetItem(string tittle,string desc,string btnText,bool isBtnEnable,string offerText,Menu_Shop menu){
       
        offer.text=offerText;
        this.tittle.text=tittle;
        this.desc.text=desc;
        btnLb.text=btnText;
        SetBtn(isBtnEnable);
        this.menu=menu;
    }
    public Menu_Shop menu;
    public int id=0;
    public void OnTap(){
        if(btn.gameObject.GetComponent<UIButtonOffset>().enabled){
            menu.OnTapItems(id);
            SetBtn(false);
        }
    }
    public UISprite btn;
    public void SetBtn(bool isEnable){
        if(isEnable){
            btn.alpha=1.0f;
            btn.gameObject.GetComponent<UIButtonOffset>().enabled=true;
        }else{
            btn.alpha=100.0f/255.0f;
            btn.gameObject.GetComponent<UIButtonOffset>().enabled=false;
        }
    }

}
