using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LureShop_Menu_Hard : TackeMenuParent {

    //Public Events
    public override void OnTapHatena(){Debug.Log(GetMethodName()+this.name);}

    public override void  OnBtnBuy(){
        Debug.Log(GetMethodName()+this.name);
      
            //購入

            if(DataManger.Instance.canBuyDolller(GetPrice(currentSelect))){
                //個数のアフェクト
                DataManger.Instance.GAMEDATA.Doller-=(long)GetPrice(currentSelect);
                DataManger.Instance.GAMEDATA.lureHas_Hard[GetTittle(currentSelect)]++;
                CheckEarlyAccess();
                SetContents(currentSelect);
                AudioManager.Instance.BuyAndEquip();
            itemList.items[Mathf.FloorToInt(currentSelect/2.0f)].GetItem(currentSelect%2).SetNums(GetHasNum(currentSelect),GetHasNum(currentSelect),DataManger.Instance.HowManyEquipped_Lure(currentSelect,false)
                    ,GetMaxSlots(currentSelect));
                itemList.items[Mathf.FloorToInt(currentSelect/2.0f)].GetItem(currentSelect%2).isAvaillable=true;
                WaitAndCover.Instance.ShowInfoList(Localization.Get("Info_Buy")+GetTittle(currentSelect));
            }

    }
    public override void  OnBtnBuySP(){
        Debug.Log(GetMethodName()+this.name);
       
            //購入
            if(DataManger.Instance.canBuyGold(GetPrice(currentSelect)/Constants.Params.DoltoGold)){
                if(MenuManager.Instance!=null)MenuManager.Instance.backToHome.Hide();
                WaitAndCover.Instance.ShowYesNoPopup(Localization.Get("TittleBuy"),Localization.Get("DescBuy")+"\n"+GetTittle(currentSelect)
                    +" : "+(GetPrice(currentSelect)/Constants.Params.DoltoGold).ToString()+" G",Localization.Get("Purchase"),Localization.Get("Cancel"),OnBuySPComplete);
                
            }
    }

    public void OnBuySPComplete(bool isYesed){
        Debug.Log("OnBuySPComplete"+isYesed);
        if(isYesed){
           
                //個数のアフェクト
                DataManger.Instance.GAMEDATA.Doller-=(long)GetPrice(currentSelect)/Constants.Params.DoltoGold;
                DataManger.Instance.GAMEDATA.lureHas_Hard[GetTittle(currentSelect)]++;
            Debug.Log("OnBuySPComplete"+DataManger.Instance.GAMEDATA.lureHas_Hard[GetTittle(currentSelect)]);
                CheckEarlyAccess();
                SetContents(currentSelect);
            AudioManager.Instance.BuyAndEquip();
            itemList.items[Mathf.FloorToInt(currentSelect/2.0f)].GetItem(currentSelect%2).SetNums(GetHasNum(currentSelect),GetHasNum(currentSelect),DataManger.Instance.HowManyEquipped_Lure(currentSelect,false)
                ,GetMaxSlots(currentSelect));
            itemList.items[Mathf.FloorToInt(currentSelect/2.0f)].GetItem(currentSelect%2).isAvaillable=true;
            WaitAndCover.Instance.ShowInfoList(Localization.Get("Info_Buy")+GetTittle(currentSelect));
        }else{
            
        }
        if(MenuManager.Instance!=null)MenuManager.Instance.backToHome.Show();
    }

    void OnEquip(int rod){
            //セレクト　装備
            DataManger.Instance.GAMEDATA.tackleSlots[rod].lureNum=currentSelect;
            DataManger.Instance.GAMEDATA.tackleSlots[rod].isSoft=false;

    }




    public override void OnTappedItem(int num,bool isInit){
        Debug.Log(GetMethodName()+this.name);
        if(!isInit && num==currentSelect)return;
        if(!isInit)AudioManager.Instance.SelectItemList();
        int row=Mathf.FloorToInt(currentSelect/2.0f);

        if( GetItemSpriteLength()>=currentSelect)itemList.items[row].GetItem(currentSelect%2).SetSelect(false);
        currentSelect= num;
        row=Mathf.FloorToInt(currentSelect/2.0f);
        if( GetItemSpriteLength()>=currentSelect)itemList.items[row].GetItem(num%2).SetSelect(true);

        SetContents(num);
        //0 1 0
        //2 3 1
    }



    //Public Methods
    public override void Show(int currentLure){

        Debug.LogError("Show"+gameObject.name);

        WaitAndCover.Instance.ShowWait();
        WaitAndCover.Instance.CoverAll(true);
        StartCoroutine(Init(currentLure));
    }
    public override IEnumerator Init(int currentLure){
        Debug.LogError("Init "+gameObject.name);
        Coroutine cor;
        ShowItemList(false);
        cor=StartCoroutine(SetItemLists());
        Debug.LogError("Init 1"+gameObject.name);
        yield return cor;
        Debug.LogError("Init 1-1"+gameObject.name);
        CheckEarlyAccess();
        Debug.LogError("Init 1-2"+gameObject.name);
        currenEquippedLure=currentLure;
        affecter.topBtn.SetBtnCallback(OnBtnBuy);
        affecter.btmBtn.SetBtnCallback(OnBtnBuySP);

        rader.InitRader( PSGameUtils.SplitStringData(Localization.Get("LureParams_Tittles"),new char[]{';'}),transform);
        Debug.LogError("Init 1-3"+gameObject.name);
       
            currentSelect=0;
            OnTappedItem(currentSelect,true);
            InitAvility(PSGameUtils.SplitStringData(Localization.Get("LureParams_SPAvils"),new char[]{';'}));
        Debug.LogError("Init 2"+gameObject.name);
        yield return new WaitForSeconds(0.5f);
        WaitAndCover.Instance.StopWait();
        WaitAndCover.Instance.UnCoverAll();

    }
    public UIAtlas itemAtlas;
    public  override IEnumerator SetItemLists(){

        yield return null;
        itemList.view.ResetPosition();
        foreach(LureItemContainer it in  itemList.items){
            it.isUsed=false;
        }
        Debug.LogError("SetItemLists 1"+gameObject.name);
        int num=0;
        for(int i=0;i<GetItemSpriteLength()/2;i++){

            if(num< GetItemSpriteLength()){
                if(itemList.items==null || itemList.items.Count<i+1){
                    //add rows
                    //Debug.Log("add rows ");
                    itemList.SpawnRow();
                }
                //Debug.Log("Start1 ");
                //Debug.Log("Start "+itemList.items.Count);

                if(itemList.items[i].gameObject.activeSelf!=true)NGUITools.SetActive(itemList.items[i].gameObject,true);
                itemList.items[i].SetActive(0,true);

                Debug.LogError("SetItemLists 2"+gameObject.name);
                int eqp=-1;
                Debug.LogError("SetItemLists 2-1"+gameObject.name);
                eqp=DataManger.Instance.IsEquipped_Lure(num,false);
                eqp++;
                if(eqp>0){
                    //装備中
                    //int id,string tittle,int num,int maxhas,string itemSp,int isOne,bool isAvaillable,string markSp,string extra,Callback_OnTappedItem tapEvent,UIAtlas at,Material mat
                    Debug.LogError("装備中"+GetTittle(num));
                    itemList.items[i].SetItems(num,  GetTittle(num), GetHasNum(num), GetHasNum(num),DataManger.Instance.HowManyEquipped_Lure(num,false),
                        GetMaxSlots(num), GetItemSprite(num),0,isHasOrAvaillable(num),  GetItemMarkSprite(num),"E:"+eqp,OnTappedItem,itemAtlas);
                    
                }else{
                    //装備していない
                    itemList.items[i].SetItems(num,  GetTittle(num), GetHasNum(num), GetHasNum(num),DataManger.Instance.HowManyEquipped_Lure(num,false),
                        GetMaxSlots(num), GetItemSprite(num),0,isHasOrAvaillable(num),  GetItemMarkSprite(num),OnTappedItem,itemAtlas);
                    
                }
                Debug.LogError("SetItemLists 3"+gameObject.name);
                num++;
                if(num< GetItemSpriteLength()){
                    if(itemList.items[i].gameObject.activeSelf!=true)NGUITools.SetActive(itemList.items[i].gameObject,true);
                    itemList.items[i].SetActive(1,true);
                    eqp=DataManger.Instance.IsEquipped_Lure(num,false);
                    eqp++;
                    if(eqp>0){
                        //装備中

                        itemList.items[i].SetItems(num,  GetTittle(num), GetHasNum(num), GetHasNum(num),DataManger.Instance.HowManyEquipped_Lure(num,false),
                            GetMaxSlots(num), GetItemSprite(num),1,isHasOrAvaillable(num),  GetItemMarkSprite(num),"E:"+eqp,OnTappedItem,itemAtlas);

                    }else{
                        //装備していない
                        itemList.items[i].SetItems(num,  GetTittle(num), GetHasNum(num), GetHasNum(num),DataManger.Instance.HowManyEquipped_Lure(num,false),
                            GetMaxSlots(num), GetItemSprite(num),1,isHasOrAvaillable(num),  GetItemMarkSprite(num),OnTappedItem,itemAtlas);

                    }
                }else{
                    itemList.items[i].SetActive(1,false);
                }
                num++;
            }else{
                Debug.LogError("SetItemLists 4"+gameObject.name);
                if(itemList.items[i].gameObject.activeSelf!=false)NGUITools.SetActive(itemList.items[i].gameObject,false);
            }

        }
        Debug.Log("item list "+num);
        itemList.itemCount=num;
        foreach(LureItemContainer it in  itemList.items){
            if(!it.isUsed)NGUITools.SetActive(it.gameObject,false);
        }
        itemList.AffectGrid();
        itemList.view.ResetPosition();
    }

    public  override void SetContents(int i){
        //タイトル　画像　マーク
        affecter.SetLureParams(  GetTittle(i), GetItemSprite(i),  GetItemMarkSprite(i),itemAtlas);

        //個数　レンジ　特殊能力　ルアーのみ int has,int reallyhas,int equip,int maxhas
       
        affecter.SetNum(GetHasNum(i),GetHasNum(i), DataManger.Instance.HowManyEquipped_Lure(i,false), GetMaxSlots(i));
        affecter.SetRangeValue(GetRangePivot(i), GetRangeSize(i));
        affecter.mainRodToggle.SetActive(false,false);
        SetAvilities(GetSPAvility(i));
        //レーダー
      
        if(GetHasNum(i)>0){
            affecter.SetBuyBtns(false
                , GetPrice(i),canEarlyAccess(i),GetPrice(i)/Constants.Params.DoltoGold,  GetItemUnloclRank(i),
                isMax( GetHasNum(i),GetMaxSlots(i)));
        }else{
            affecter.SetBuyBtns(DataManger.Instance.GAMEDATA.playerLevel< GetItemUnloclRank(i)?true:false
                , GetPrice(i),canEarlyAccess(i),GetPrice(i)/Constants.Params.DoltoGold,  GetItemUnloclRank(i),
                isMax( GetHasNum(i),GetMaxSlots(i)));
        }
            



            rader.SetRaderData(new float[5]{ GetGraph(i,1), GetGraph(i,2), GetGraph(i,3), GetGraph(i,4), GetGraph(i,5)});



    }

   

    public override void CheckEarlyAccess(){
        int i=0;
        earlyAccessAvaillable.Clear();
        foreach(LureItemContainer con in itemList.items){
            if(i< GetItemSpriteLength()){
                if( earlyAccessAvaillable.Count<Constants.Params.lureEarlyAccess&&!isHasOrAvaillable(i)){

                    earlyAccessAvaillable.Add(i);
                }
            }
            i++;
            if(i< GetItemSpriteLength()){
                if( earlyAccessAvaillable.Count<Constants.Params.lureEarlyAccess&&!isHasOrAvaillable(i)){

                    earlyAccessAvaillable.Add(i);
                }
            }
            i++;
        }
    }
    public override bool isHasOrAvaillable(int num){
      
        Debug.Log("isHasorAvaillable"+GetHasNum(num));
            if(DataManger.Instance.GAMEDATA.playerLevel>=  GetItemUnloclRank(num)){
                //アンロックされている
                return true;
            }else{
                if( GetHasNum(num)>0){
                    //アンロックされていないが持っている
                    return true;
                }
            }

        return false;
    }

    public int GetHasNum(int current){
        return DataManger.Instance.GAMEDATA.lureHas_Hard[ GetTittle(current)];
    }

    private float GetGraph(int current,int num){
        switch(num){
        case 1:
            return Constants.LureDatas.graph_1[current];
            break;
        case 2:
            return  Constants.LureDatas.graph_2[current];
            break;
        case 3:
            return  Constants.LureDatas.graph_3[current];
            break;
        case 4:
            return Constants.LureDatas.graph_4[current];
            break;
        case 5:
            return  Constants.LureDatas.graph_5[current];
            break;
        }
        return 0.0f;
    }
    private int GetPrice(int current){
        return Constants.LureDatas.itemPrices[current];
    }
    private int GetMaxSlots(int current){
        return Constants.LureDatas.itemNumsMax[current];

    }
    private int GetItemUnloclRank(int current){
        return Constants.LureDatas.itemUnlockAt[current];

    }
    private string GetTittle(int current){
        return Constants.LureDatas.itemTittles[current];
    }
    private string GetItemSprite(int current){
        return Constants.LureDatas.itemSprites[current];
    }
    private int GetItemSpriteLength(){
        return Constants.LureDatas.itemSprites.Length;
    }
    private string GetItemMarkSprite(int current){
        return  Constants.Params.HeavyString[(int)Constants.LureDatas.heavyCategory[current]];
    }
    private LureRangePivot GetRangePivot(int current){
        return (LureRangePivot) Constants.LureDatas.rangePivot[current];
    }
    private float GetRangeSize(int current){
        return Constants.LureDatas.rangeSize[current];
    }
    private bool[] GetSPAvility(int current){
        return PSGameUtils.StringToBoolArray(Constants.LureDatas.avility[current]);
    }

}
