using UnityEngine;
using System.Collections;

public class Menu_Shop : MenuContents  {

    public override void Show(){
        Debug.Log("Show"+gameObject.name);
        if(!isShow){

            OnShowStart();
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
    public override void Hide(){
        if(isShow){

            OnHideStart();
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

    public void OnShowStart(){
        SetContents();
        if(!MenuManager.Instance.tab.gameObject.activeSelf)NGUITools.SetActive(MenuManager.Instance.tab.gameObject,true);
        if(Localization.language!="Japanese"){
            NGUITools.SetActive(shoutorihiki,false);
        }
        UserName.Instance.SetPosition(true);
    }
    public void OnHideStart(){

    }
    public override void OnHideComplete(){
        base.OnHideComplete();
    }
    public override void OnShowComplete(){
        if(MenuManager.Instance==null)Debug.LogError("MenuManager==null");
        MenuManager.Instance.backToHome.Show();
        base.OnShowComplete();
    }
    public GameObject shoutorihiki;
    void SetContents(){
        //LoginBonus
        freeItems[0].SetItem(Localization.Get("TittleLoginBonus"),Localization.Get("DescLoginBonus"),Localization.Get("BtnLoginBonus")
            ,DataManger.Instance.GAMEDATA.earnedLoginBonus?false:true,DataManger.Instance.GAMEDATA.RenzokuLogin.ToString()+Localization.Get("OfferLoginBonus"),this);
        
        //facebook
        freeItems[1].SetItem(Localization.Get("TittleFB"),Localization.Get("DescFB"),Localization.Get("BtnFB")
            ,DataManger.Instance.GAMEDATA.Liked?false:true,Constants.Params.LikeBonus.ToString()+"\n"+"GOLD",this);
        
        //PV
        freeItems[2].SetItem(Localization.Get("TittlePV"),Localization.Get("DescPV"),Localization.Get("BtnPV")
            ,PS_Plugin.Instance.isConnected_TJ?true:false,Constants.Params.PVBonus.ToString()+"\n"+"GOLD",this);

        //TJ
        freeItems[3].SetItem(Localization.Get("TittleTJ"),Localization.Get("DescTJ"),Localization.Get("BtnTJ")
            , PS_Plugin.Instance.isConnected_TJ?true:false,Constants.Params.PVBonus.ToString()+"\n"+"GOLD",this);

        if(StoreListener.Instance.IsStoreAvaillable()){
            paidItems[0].SetItem(Localization.Get("TittleItem1"),Localization.Get("DescItem1"), StoreListener.Instance.items.prices[0]
                ,true,Localization.Get("OfferItem1"),this);
            paidItems[1].SetItem(Localization.Get("TittleItem2"),Localization.Get("DescItem2"), StoreListener.Instance.items.prices[1]
                ,true,Localization.Get("OfferItem2"),this);
            paidItems[2].SetItem(Localization.Get("TittleItem3"),Localization.Get("DescItem3"), StoreListener.Instance.items.prices[2]
                ,true,Localization.Get("OfferItem3"),this);
            paidItems[3].SetItem(Localization.Get("TittleItem4"),Localization.Get("DescItem4"), StoreListener.Instance.items.prices[3]
                ,true,Localization.Get("OfferItem4"),this);
        }else{
            //課金
            paidItems[0].SetItem(Localization.Get("TittleItem1"),Localization.Get("DescItemNC"),"--"
                ,false,"",this);
            paidItems[1].SetItem(Localization.Get("TittleItem2"),Localization.Get("DescItemNC"),"--"
                ,false,"",this);
            paidItems[2].SetItem(Localization.Get("TittleItem3"),Localization.Get("DescItemNC"),"--"
                ,false,"",this);
            paidItems[3].SetItem(Localization.Get("TittleItem4"),Localization.Get("DescItemNC"),"--"
                ,false,"",this);
            
        }

    }

    public void OnTapTokushou(){
        Application.OpenURL(Constants.Params.tokushouPageURL);
    }
   

    public void OnFacebook(bool isYesed){
        if(isYesed){
            Application.OpenURL(Constants.Params.fbPageURL);
            freeItems[1].SetItem(Localization.Get("TittleFB"),Localization.Get("DescFB"),Localization.Get("BtnFB")
                ,false,Constants.Params.LikeBonus.ToString()+"\n"+"GOLD",this);
           
            DataManger.Instance.GAMEDATA.Liked=true;
            ES2.Save(ES2.Load<int>( DataManger.DataFilename+"?tag=gettedGolds")+Constants.Params.LikeBonus, DataManger.DataFilename+"?tag=gettedGolds");
            DataManger.Instance.GAMEDATA.Gold+=ES2.Load<int>( DataManger.DataFilename+"?tag=gettedGolds");
            ES2.Save(0,DataManger.DataFilename+"?tag=gettedGolds");
            if(UserName.Instance!=null){
                UserName.Instance.UpdateCurrencyNums();
            }
            DataManger.Instance.SaveData();
        }
    }
    public void OnTapItems(int i){
        switch(i){
            case  0:
            //LoginBonus
                break;
            case  1:
            //facebook
                WaitAndCover.Instance.ShowYesNoPopup(Localization.Get("TittleLike"),Localization.Get("DescLike"),Localization.Get("Open"),Localization.Get("Cancel"),OnFacebook);
                break;
            case  2:
            //PV
                PS_Plugin.Instance.tjListener.Show_Video();
                break;
            case  3:
            //TJ
                PS_Plugin.Instance.tjListener.Show_Offer_Wall();
                break;
            case  4:
                OnPurchse(Constants.Params.skus[0]);
                break;
            case  5:
                OnPurchse(Constants.Params.skus[1]);
                break;
            case  6:
                OnPurchse(Constants.Params.skus[2]);
                break;
            case  7:
                OnPurchse(Constants.Params.skus[3]);
                break;
        }
    }
    void OnPurchse(string id){
        WaitAndCover.Instance.ShowWait();
        WaitAndCover.Instance.CoverAll(true);
        StoreListener.Instance.purchasFailledEvent+=OnParchaseFailled;
        StoreListener.Instance.purchasedEvent+= OnParchaseSuccessed;
        StoreListener.Instance.PurchaseProduct(id);
    }
    public ShopContentHolder[] freeItems;
    public ShopContentHolder[] paidItems;

    public void OnParchaseFailled(string str){
        WaitAndCover.Instance.ShowInfoPopup(Localization.Get("PurchaseFailled"),Info_IC.Warning,OnPurchaseInfoTapped);
    }
    public void OnParchaseSuccessed(string str){
        DataManger.Instance.GAMEDATA.Gold=ES2.Load<long>(DataManger.DataFilename+"?tag=Currency_Gold");
        DataManger.Instance.GAMEDATA.isAdFree=ES2.Load<bool>(DataManger.DataFilename+"?tag=isAdFree");
        //ポップアップでその旨を告知する。その後カバーを取る。

        string addedG="[FF9933]";
        if(str==Constants.Params.skus[0]){
            addedG+=Constants.Params.purchaseGold[0].ToString()+"[-]G";
        }else if(str==Constants.Params.skus[1]){
            addedG+=Constants.Params.purchaseGold[1].ToString()+"[-]G";
        }else if(str==Constants.Params.skus[2]){
            addedG+=Constants.Params.purchaseGold[2].ToString()+"[-]G";
        }else if(str==Constants.Params.skus[3]){
            addedG+=Constants.Params.purchaseGold[3].ToString()+"[-]G";
        }
        WaitAndCover.Instance.ShowInfoPopup(Localization.Get("PurchaseSucceed")+addedG+Localization.Get("PurchaseSucceed2"),Info_IC.Normal,OnPurchaseInfoTapped);


    }
    public void OnPurchaseInfoTapped(){
        UserName.Instance.UpdateCurrencyNums();
        StoreListener.Instance.purchasedEvent-= OnParchaseSuccessed;
        StoreListener.Instance.purchasFailledEvent-=OnParchaseFailled;
        WaitAndCover.Instance.StopWait();
        WaitAndCover.Instance.UnCoverAll();
    }


}
