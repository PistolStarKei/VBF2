using UnityEngine;
using System.Collections;

public class LureShop_Menu_Soft : TackeMenuParent {

    public void Test(){
        //セレクトで２つを表示するとき
        rader.SetRaderData(new float[5]{ GetGraph(0,1), GetGraph(0,2), GetGraph(0,3), GetGraph(0,4), GetGraph(0,5)}

    ,new float[5]{ GetGraph(2,1),GetGraph(2,2),GetGraph(2,3),
        GetGraph(2,4),GetGraph(2,5)});
    }



    //Public Events
    public override void OnTapHatena(){Debug.Log(GetMethodName()+this.name);}
    public override void  OnBtnBuy(){
        Debug.Log(GetMethodName()+this.name);

            //購入
           
            if( DataManger.Instance.canBuyDolller(GetPrice(currentSelect))){
                //個数のアフェクト
                DataManger.Instance.GAMEDATA.Doller-=(long)GetPrice(currentSelect);
            DataManger.Instance.GAMEDATA.lureHas_Soft[GetTittle(currentSelect)]=GetMaxSlots(currentSelect);
                AudioManager.Instance.BuyAndEquip();
                CheckEarlyAccess();
                SetContents(currentSelect);
            itemList.items[Mathf.FloorToInt(currentSelect/2.0f)].GetItem(currentSelect%2).SetNums(GetHasNum(currentSelect),GetHasNum(currentSelect), DataManger.Instance.HowManyEquipped_Lure(currentSelect,true)
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

        if(isYesed){

            //個数のアフェクト
            DataManger.Instance.GAMEDATA.Doller-=(long)GetPrice(currentSelect)/Constants.Params.DoltoGold;
            DataManger.Instance.GAMEDATA.lureHas_Soft[GetTittle(currentSelect)]=GetMaxSlots(currentSelect);
            AudioManager.Instance.BuyAndEquip();
            CheckEarlyAccess();
            SetContents(currentSelect);

            itemList.items[Mathf.FloorToInt(currentSelect/2.0f)].GetItem(currentSelect%2).SetNums(GetHasNum(currentSelect),GetHasNum(currentSelect), DataManger.Instance.HowManyEquipped_Lure(currentSelect,true)
                ,GetMaxSlots(currentSelect));
            itemList.items[Mathf.FloorToInt(currentSelect/2.0f)].GetItem(currentSelect%2).isAvaillable=true;
            WaitAndCover.Instance.ShowInfoList(Localization.Get("Info_Buy")+GetTittle(currentSelect));
        }else{
        }
        if(MenuManager.Instance!=null)MenuManager.Instance.backToHome.Show();
    }




    public void OnTappedAddonItem(int num){
        Debug.Log("OnTappedAddonItem"+num);
       
         OnBuyRig(num);
    }
    public void OnBuyRig(int rigNum){
          int[] adons=GetAvaillableAdons(currentSelect);
            if(!getHasRig(adons[rigNum])){
                
                this.rigNum=rigNum;
                Debug.Log("持っていないので購入できる "+this.rigNum);
                if(DataManger.Instance.canBuyDolller(GetRigPrice(adons[rigNum]))){
                    if(MenuManager.Instance!=null)MenuManager.Instance.backToHome.Hide();
                    WaitAndCover.Instance.ShowYesNoPopup(Localization.Get("TittleBuy"),Localization.Get("DescBuy")+"\n"+GetRigTittle(adons[rigNum])
                    +" : "+(GetRigPrice(adons[rigNum])).ToString()+" $",Localization.Get("Purchase"),Localization.Get("Cancel"),OnBuyRigComplete);
                }else{
                    Debug.Log("お金足りない "+this.rigNum);
                }
            }else{
                Debug.LogError("すでに持っている");
                //セレクト
                //装備のアフェクト
                
            if(isHasLure(currentSelect)){
                DataManger.Instance.GAMEDATA.SettedRig[GetTittle(currentSelect)]=rigNum;
                AudioManager.Instance.SelectItemList();
                AudioManager.Instance.Equip();
                SetContents(currentSelect);
            }
                
            }
       
    }
    int rigNum=0;
    public void OnBuyRigComplete(bool isYesed){
        Debug.Log("OnBuyRigComplete"+isYesed);
        int[] adons=GetAvaillableAdons(currentSelect);

        if(isYesed){

            //個数のアフェクト
            DataManger.Instance.GAMEDATA.Doller-=(long)GetRigPrice(adons[rigNum]);
            DataManger.Instance.GAMEDATA.RigHas[GetRigTittle(adons[rigNum])]=true;

            //装備のアフェクト
            DataManger.Instance.GAMEDATA.SettedRig[GetTittle(currentSelect)]=rigNum;
            AudioManager.Instance.BuyAndEquip();
            SetContents(currentSelect);

            WaitAndCover.Instance.ShowInfoList(Localization.Get("Info_Buy")+GetRigTittle(adons[rigNum]));
        }else{
            
        }
        if(MenuManager.Instance!=null)MenuManager.Instance.backToHome.Show();
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
        Debug.Log(GetMethodName()+this.name);
        WaitAndCover.Instance.ShowWait();
        WaitAndCover.Instance.CoverAll(true);
        StartCoroutine(Init(currentLure));
    }
    public override IEnumerator Init(int currentLure){
        Debug.Log(GetMethodName()+this.name);
        Coroutine cor;
        ShowItemList(false);
        cor=StartCoroutine(SetItemLists());
        yield return cor;

        CheckEarlyAccess();
        currenEquippedLure=currentLure;
        affecter.topBtn.SetBtnCallback(OnBtnBuy);
        affecter.btmBtn.SetBtnCallback(OnBtnBuySP);

        rader.InitRader( PSGameUtils.SplitStringData(Localization.Get("LureParams_Tittles"),new char[]{';'}),transform);

       
            currentSelect=0;
            OnTappedItem(currentSelect,true);
            InitAvility(PSGameUtils.SplitStringData(Localization.Get("LureParams_SPAvils"),new char[]{';'}));
        yield return new WaitForSeconds(0.5f);
        Debug.Log(GetMethodName()+this.name+"End");
        WaitAndCover.Instance.StopWait();
        WaitAndCover.Instance.UnCoverAll();

    }
    public UIAtlas itemAtlas;
    public  override IEnumerator SetItemLists(){

        Debug.Log(GetMethodName()+this.name);
        yield return null;
        //Viewの初期化
        itemList.view.ResetPosition();
        foreach(LureItemContainer it in  itemList.items){
            it.isUsed=false;
        }
        int num=0;
        for(int i=0;i<GetItemSpriteLength()/2;i++){
            Debug.Log(GetMethodName()+this.name+"Set"+i);
            //Debug.Log("Start "+itemList.items.Count);
            if(num< GetItemSpriteLength()){
                //Debug.Log("Start "+num);
                if(itemList.items==null || itemList.items.Count<i+1){
                    //add rows
                    //Debug.Log("add rows ");
                    itemList.SpawnRow();
                }
                //Debug.Log("Start1 ");
                //Debug.Log("Start "+itemList.items.Count);


                if(itemList.items[i].gameObject.activeSelf!=true)NGUITools.SetActive(itemList.items[i].gameObject,true);
                itemList.items[i].SetActive(0,true);

                int eqp=-1;
                eqp=DataManger.Instance.IsEquipped_Lure(num,true);
                eqp++;
                if(eqp>0){
                    //装備中
                    itemList.items[i].SetItems(num, GetTittle(num), GetHasNum(num), GetHasNum(num),DataManger.Instance.HowManyEquipped_Lure(num,true),
                        GetMaxSlots(num), GetItemSprite(num),0,isHasOrAvaillable(num), GetItemMarkSprite(num),"E:"+eqp,OnTappedItem,itemAtlas);
                }else{
                    //装備していない
                    itemList.items[i].SetItems(num, GetTittle(num), GetHasNum(num), GetHasNum(num),DataManger.Instance.HowManyEquipped_Lure(num,true),
                        GetMaxSlots(num), GetItemSprite(num),0,isHasOrAvaillable(num), GetItemMarkSprite(num),OnTappedItem,itemAtlas);

                }



                num++;
                //Debug.Log("Start 2");
                if(num< GetItemSpriteLength()){
                    //if(num==13)Debug.Log("Start- 1");
                    if(itemList.items[i].gameObject.activeSelf!=true)NGUITools.SetActive(itemList.items[i].gameObject,true);
                    itemList.items[i].SetActive(1,true);
                    eqp=DataManger.Instance.IsEquipped_Lure(num,true);
                    eqp++;
                    if(eqp>0){
                        //装備中
                        itemList.items[i].SetItems(num, GetTittle(num), GetHasNum(num), GetHasNum(num),DataManger.Instance.HowManyEquipped_Lure(num,true),
                            GetMaxSlots(num), GetItemSprite(num),1,isHasOrAvaillable(num), GetItemMarkSprite(num),"E:"+eqp,OnTappedItem,itemAtlas);
                    }else{
                        //装備していない
                        itemList.items[i].SetItems(num, GetTittle(num), GetHasNum(num), GetHasNum(num),DataManger.Instance.HowManyEquipped_Lure(num,true),
                            GetMaxSlots(num), GetItemSprite(num),1,isHasOrAvaillable(num), GetItemMarkSprite(num),OnTappedItem,itemAtlas);

                    }
                }else{
                    itemList.items[i].SetActive(1,false);
                }
                num++;
            }else{
                if(itemList.items[i].gameObject.activeSelf!=false)NGUITools.SetActive(itemList.items[i].gameObject,false);
            }

        }
        itemList.itemCount=num;
        foreach(LureItemContainer it in  itemList.items){
            if(!it.isUsed)NGUITools.SetActive(it.gameObject,false);
        }
        itemList.AffectGrid();
        itemList.view.ResetPosition();

        Debug.Log(GetMethodName()+this.name+"End");
    }



    public  override void SetContents(int i){
        //右側のコンテンツを構成する
        Debug.Log(GetMethodName()+this.name);

        //タイトル　画像　マーク
        affecter.SetLureParams( GetTittle(i), GetItemSprite(i), GetItemMarkSprite(i),itemAtlas);

        //個数　レンジ　特殊能力　ルアーのみ
        affecter.SetNum(GetHasNum(i),GetHasNum(i), DataManger.Instance.HowManyEquipped_Lure(currentSelect,true),GetMaxSlots(i));
        affecter.SetRangeValue(GetRangePivot(i),GetRangeSize(i));
        SetAvilities(GetSPAvility(i));

        affecter.mainRodToggle.SetActive(false,false);

        //レーダー
            if(isHasLure(i)){
                //持っているので、アドオンを表示する
                int[] adons=GetAvaillableAdons(i);
                Debug.Log("Adoons"+adons.Length);
                currentRig=GetEquippedRig(i);


                for(int e=0;e<5;e++){

                    if(e>=adons.Length){
                        affecter.SetAdonItemsToUA(e);
                    }else{
                        if(e==0){
                            affecter.SetAdonItems(e,Constants.RigDatas.itemTittles[adons[e]],"",
                                Constants.RigDatas.itemPrices[adons[e]].ToString()+"$",Constants.RigDatas.itemSprites[adons[e]],getHasRig(adons[e]),currentRig==e?true:false);
                            
                        }else{
                            affecter.SetAdonItems(e,Constants.RigDatas.itemTittles[adons[e]],GetRigDesc(adons[e]),
                                Constants.RigDatas.itemPrices[adons[e]].ToString()+"$",Constants.RigDatas.itemSprites[adons[e]],getHasRig(adons[e]),currentRig==e?true:false);
                            
                        }
                        //itemType

                       
                        
                    }
                }

                //rigNum adons[e]
                affecter.SetRig(Constants.RigDatas.itemSprites[adons[currentRig]]
                    ,Constants.RigPositionDefines.GetShouldShowBack(adons[currentRig],Constants.SoftLureDatas.itemType[i]),
                    Constants.RigPositionDefines.GetRigPos(adons[currentRig],Constants.SoftLureDatas.itemType[i]),
                    Constants.RigPositionDefines.GetRigRot(adons[currentRig],Constants.SoftLureDatas.itemType[i]));
                
                //アドオン込みのデータを表示する
                if(currentRig==0){
                    rader.SetRaderData(new float[5]{ GetGraph(i,1), GetGraph(i,2), GetGraph(i,3), GetGraph(i,4), GetGraph(i,5)});
                }else{
                    rader.SetRaderData(new float[5]{ GetGraph(i,1)+GetAddonGraph(currentRig,1), GetGraph(i,2)+GetAddonGraph(currentRig,2),
                        GetGraph(i,3)+GetAddonGraph(currentRig,3), GetGraph(i,4)+GetAddonGraph(currentRig,4), GetGraph(i,5)+GetAddonGraph(currentRig,5)});
                }
               
            }else{
                //持っていないので、購入ボタンを表示する
                affecter.SetBuyBtns(DataManger.Instance.GAMEDATA.playerLevel<GetItemUnloclRank(i)?true:false
                    , GetPrice(i),canEarlyAccess(i), GetPrice(i)/Constants.Params.DoltoGold, GetItemUnloclRank(i),
                    isMax( GetHasNum(i),GetMaxSlots(currentSelect)));
                
                rader.SetRaderData(new float[5]{ GetGraph(i,1), GetGraph(i,2), GetGraph(i,3), GetGraph(i,4), GetGraph(i,5)});
            }
           


    }



    public int GetEquippedRigID(int current){

        int[] adons=GetAvaillableAdons(current);
        if(current<=adons.Length)Debug.LogError("GetEquippedRig Error length over");

        return adons[GetEquippedRig(current)];


    }

    public int currentRig=0;

    public int GetEquippedRig(int num){
        int outInt=0;

        if(DataManger.Instance.GAMEDATA.SettedRig.TryGetValue(Constants.SoftLureDatas.itemTittles[num],out outInt)){
            return outInt;
        }
        return 0;
    }

    public  bool getHasRig(int num){

        Debug.LogError("getHasRig"+num);
        bool str=false;
        if(DataManger.Instance.GAMEDATA.RigHas.TryGetValue(Constants.RigDatas.itemTittles[num],out str)){
                return str;
        }else{
            Debug.LogError("Not match");
                return false;
        }
            

       
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
    public  bool isHasLure(int num){
        if( GetHasNum(num)>0){
            //持っている
            return true;
        }

        return false;
    }

    public override bool isHasOrAvaillable(int num){
       
            if(DataManger.Instance.GAMEDATA.playerLevel>= GetItemUnloclRank(num)){
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

    private int[] GetAvaillableAdons(int current){
        Debug.Log("GetAvaillableAdons"+current+" "+Constants.SoftLureDatas.AvaillableRig[current]);
        return PSGameUtils.StringToIntArray(Constants.SoftLureDatas.AvaillableRig[current],new char[]{';'});
    }

    private float GetAddonGraph(int current,int num){
        
        switch(num){
        case 1:
            return Constants.RigDatas.graph_1[current];
            break;
        case 2:
            return  Constants.RigDatas.graph_2[current];
            break;
        case 3:
            return  Constants.RigDatas.graph_3[current];
            break;
        case 4:
            return Constants.RigDatas.graph_4[current];
            break;
        case 5:
            return  Constants.RigDatas.graph_5[current];
            break;
        }
        return 0.0f;
    }
    string[] paramS=new string[2];
    private string GetRigDesc(int current){
        string str="";
        if(paramS.Length!=5)paramS=PSGameUtils.SplitStringData(Localization.Get("LureParams_Tittles_Short"),new char[]{';'});
      

        float val=Constants.RigDatas.graph_1[current];
        if(val>0.0f){
            str+=paramS[0]+"[00A0FFFF]+"+Mathf.FloorToInt(val)+"[-]";
            str+=" ";
        }else{
                if(val==0.0f){
                    
                }else{
                    str+=paramS[0]+"[FF0099FF]"+Mathf.FloorToInt(val)+"[-]";
                str+=" ";
                }
        }

        val=Constants.RigDatas.graph_2[current];
        if(val>0.0f){
            str+=paramS[1]+"[00A0FFFF]+"+Mathf.FloorToInt(val)+"[-]";
        }else{
            if(val==0.0f){

            }else{
                str+=paramS[1]+"[FF0099FF]"+Mathf.FloorToInt(val)+"[-]";
            }
        }

        str+="\n";
        val=Constants.RigDatas.graph_3[current];
        if(val>0.0f){
            str+=paramS[2]+"[00A0FFFF]+"+Mathf.FloorToInt(val)+"[-]";
            str+=" ";
        }else{
            if(val==0.0f){

            }else{
                str+=paramS[2]+"[FF0099FF]"+Mathf.FloorToInt(val)+"[-]";
                str+=" ";
            }
        }
        val=Constants.RigDatas.graph_4[current];
        if(val>0.0f){
            str+=paramS[3]+"[00A0FFFF]+"+Mathf.FloorToInt(val)+"[-]";
        }else{
            if(val==0.0f){

            }else{
                str+=paramS[3]+"[FF0099FF]"+Mathf.FloorToInt(val)+"[-]";
            }
        }
        str+="\n";
        val=Constants.RigDatas.graph_5[current];
        if(val>0.0f){
            str+=paramS[4]+"[00A0FFFF]+"+Mathf.FloorToInt(val)+"[-]";
        }else{
            if(val==0.0f){

            }else{
                str+=paramS[4]+"[FF0099FF]"+Mathf.FloorToInt(val)+"[-]";
            }
        }

        return str;
    }

    private float GetGraph(int current,int num){
        switch(num){
        case 1:
            return 0.0f;
            break;
        case 2:
            return  Constants.SoftLureDatas.graph_2[current];
            break;
        case 3:
            return  Constants.SoftLureDatas.graph_3[current];
            break;
        case 4:
            return 0.0f;
            break;
        case 5:
            return  Constants.SoftLureDatas.graph_5[current];
            break;
        }
        return 0.0f;
    }
    private int GetPrice(int current){
        return Constants.SoftLureDatas.itemPrices[current];
    }
    private int GetRigPrice(int current){
        return Constants.RigDatas.itemPrices[current];
    }
    private int GetMaxSlots(int current){
        return Constants.SoftLureDatas.itemNumsMax[current];

    }
    private int GetItemUnloclRank(int current){
        return Constants.SoftLureDatas.itemUnlockAt[current];

    }
    private string GetTittle(int current){
        return Constants.SoftLureDatas.itemTittles[current];
    }
    private string GetRigTittle(int current){
        return Constants.RigDatas.itemTittles[current];
    }
    private string GetItemSprite(int current){
        return Constants.SoftLureDatas.itemSprites[current];
    }
    private int GetItemSpriteLength(){
        return Constants.SoftLureDatas.itemSprites.Length;
    }
    private string GetItemMarkSprite(int current){
        int currentRig=GetEquippedRigID(current);
        return  Constants.Params.HeavyString[(int)Constants.RigDatas.heavyCategory[currentRig]];
    }
    public LureRangePivot GetRangePivot(int current){
        int currentRig=GetEquippedRigID(current);
        return (LureRangePivot) Constants.RigDatas.rangePivot[currentRig];
    }
    public float GetRangeSize(int current){
        int currentRig=GetEquippedRigID(current);
        return Constants.RigDatas.rangeSize[currentRig];
    }
    private bool[] GetSPAvility(int current){
        int currentRig=GetEquippedRigID(current);

        return PSGameUtils.MergeAvility(Constants.RigDatas.avilitys[currentRig],Constants.SoftLureDatas.avilitys[current]);
    }
    public int GetHasNum(int current){
        return DataManger.Instance.GAMEDATA.lureHas_Soft[ GetTittle(current)];
    }
}
