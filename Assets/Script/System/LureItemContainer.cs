using UnityEngine;
using System.Collections;
using PathologicalGames;
public class LureItemContainer : MonoBehaviour {

    public RureItem[] items;

    public RureItem GetItem(int i){
        return items[i];
    }
    public bool isUsed=false;
    public void SetActive(int i,bool isActive){
        if(items[i].gameObject.activeSelf!=isActive)NGUITools.SetActive(items[i].gameObject,isActive);
    }

    public void SetItems(int id,string tittle,int num,int maxhas,string itemSp,int isOne,bool isAvaillable,string markSp,Callback_OnTappedItem tapEvent,UIAtlas at){
        this.OnTappedItem=null;
        isUsed=true;
        this.OnTappedItem+=tapEvent;
        items[isOne].SetItems(id,tittle,num,maxhas,itemSp,this,isAvaillable,markSp,at);
    }
    public void SetItems(int id,string tittle,int num,int has,int equip,int maxhas,string itemSp,int isOne,bool isAvaillable,string markSp,Callback_OnTappedItem tapEvent,UIAtlas at){
        this.OnTappedItem=null;
        isUsed=true;
        this.OnTappedItem+=tapEvent;
        items[isOne].SetItems(id,tittle,num,has,equip,maxhas,itemSp,this,isAvaillable,markSp,at);
    }

    public void SetItems(int id,string tittle,int num,int maxhas,string itemSp,int isOne,bool isAvaillable,string markSp,string extra,Callback_OnTappedItem tapEvent,UIAtlas at){
        this.OnTappedItem=null;
        isUsed=true;
        this.OnTappedItem+=tapEvent;
        items[isOne].SetItems(id,tittle,num,maxhas,itemSp,this,isAvaillable,markSp,extra,at);
    }

    public void SetItems(int id,string tittle,int num,int has,int equip,int maxhas,string itemSp,int isOne,bool isAvaillable,string markSp,string extra,Callback_OnTappedItem tapEvent,UIAtlas at){
        this.OnTappedItem=null;
        isUsed=true;
        this.OnTappedItem+=tapEvent;
        items[isOne].SetItems(id,tittle,num,has,equip,maxhas,itemSp,this,isAvaillable,markSp,extra,at);
    }


    public delegate void Callback_OnTappedItem(int num,bool isInit);
    public  event Callback_OnTappedItem OnTappedItem;

    public void OnTapped(int num){
        if(OnTappedItem!=null)OnTappedItem(num,false);
    }
}
