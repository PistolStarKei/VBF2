using UnityEngine;
using System.Collections;

public class LureShop_Menu_Rods : TackeMenuParent {

    //Public Events
    public override void OnTapHatena(){Debug.Log(GetMethodName()+this.name);}
    public override void  OnBtnBuy(){
        Debug.Log(GetMethodName()+this.name);

            //購入
            if( DataManger.Instance.canBuyDolller( GetPrice(currentSelect))){
                //個数のアフェクト
                DataManger.Instance.GAMEDATA.Doller-=(long) GetPrice(currentSelect);
                DataManger.Instance.GAMEDATA.lureHas_Rods[GetTittle(currentSelect)]++;
                CheckEarlyAccess();
                AudioManager.Instance.BuyAndEquip();
                SetContents(currentSelect);
                roditemList.items[currentSelect].SetNums(GetHasNum(currentSelect)
                    ,GetMaxSlots(currentSelect));
            roditemList.items[currentSelect].isAvaillable=true;
                WaitAndCover.Instance.ShowInfoList(Localization.Get("Info_Buy")+GetTittle(currentSelect));
            }
    }

 

    public override void  OnBtnBuySP(){
        Debug.Log(GetMethodName()+this.name);
       
            //購入
            if(DataManger.Instance.canBuyGold( GetPrice(currentSelect)/Constants.Params.DoltoGold)){
                if(MenuManager.Instance!=null)MenuManager.Instance.backToHome.Hide();
                WaitAndCover.Instance.ShowYesNoPopup(Localization.Get("TittleBuy"),Localization.Get("DescBuy")+"\n"+GetTittle(currentSelect)
                    +" : "+( GetPrice(currentSelect)/Constants.Params.DoltoGold).ToString()+" G",Localization.Get("Purchase"),Localization.Get("Cancel"),OnBuySPComplete);

            }
    }

    public void OnBuySPComplete(bool isYesed){

        if(isYesed){

            //個数のアフェクト
            DataManger.Instance.GAMEDATA.Doller-=(long) GetPrice(currentSelect)/Constants.Params.DoltoGold;
            DataManger.Instance.GAMEDATA.lureHas_Rods[GetTittle(currentSelect)]++;
            CheckEarlyAccess();
            SetContents(currentSelect);
            AudioManager.Instance.BuyAndEquip();

            roditemList.items[currentSelect].SetNums(GetHasNum(currentSelect)
                ,GetMaxSlots(currentSelect));
            roditemList.items[currentSelect].isAvaillable=true;
            WaitAndCover.Instance.ShowInfoList(Localization.Get("Info_Buy")+GetTittle(currentSelect));

        }else{
            
        }
        if(MenuManager.Instance!=null)MenuManager.Instance.backToHome.Show();
    }
    public override void OnTappedItem(int num,bool isInit){
        
        if(!isInit && num==currentSelect)return;
        if(!isInit)AudioManager.Instance.SelectItemList();
        if( GetItemSpriteLength()>=currentSelect)roditemList.items[currentSelect].SetSelect(false);
        currentSelect= num;
        if( GetItemSpriteLength()>=currentSelect)roditemList.items[currentSelect].SetSelect(true);

        SetContents(num);
        //0 1 0
        //2 3 1
    }



    //Public Methods
    public override void Show(int currentLure){
        WaitAndCover.Instance.ShowWait();
        WaitAndCover.Instance.CoverAll(true);
        StartCoroutine(Init(currentLure));
    }
    public override IEnumerator Init(int currentLure){
        Coroutine cor;
        ShowItemList(true);
        cor=StartCoroutine(SetItemLists());
        yield return cor;

        CheckEarlyAccess();
        currenEquippedLure=currentLure;
        affecter.topBtn.SetBtnCallback(OnBtnBuy);
        affecter.btmBtn.SetBtnCallback(OnBtnBuySP);

        rader.InitRader( PSGameUtils.SplitStringData(Localization.Get("RodsParams_Tittles"),new char[]{';'}),transform);
        HideAvilities();
            //currentSelect=0;
            OnTappedItem(currentSelect,true);
        yield return new WaitForSeconds(0.5f);
        WaitAndCover.Instance.StopWait();
        WaitAndCover.Instance.UnCoverAll();

    }




    public RodItemListDynamic roditemList;

    public UIAtlas itemAtlas;
    public  override IEnumerator SetItemLists(){

        Debug.Log(GetMethodName()+this.name);
        yield return null;
        roditemList.view.ResetPosition();


        foreach(RodItemContainer it in  roditemList.items){
            it.isUsed=false;
        }


        int num=0;
        for(int i=0;i<GetItemSpriteLength();i++){
            //Debug.Log("Start "+roditemList.items.Count);
            if(num< GetItemSpriteLength()){
                //Debug.Log("Start "+num);
                if(roditemList.items==null || roditemList.items.Count<i+1){
                    //add rows
                    //Debug.Log("add rows ");
                    roditemList.SpawnRow();
                }
                //Debug.Log("Start1 ");
                //Debug.Log("Start "+roditemList.items.Count);
                if(roditemList.items[i].gameObject.activeSelf!=true)NGUITools.SetActive(roditemList.items[i].gameObject,true);
                roditemList.items[i].SetActive(true);

                if(DataManger.Instance.IsMainRods(num)){
                    //装備中
                   
                    roditemList.items[i].SetItems(num, GetTittle(num), GetHasNum(num), 
                        GetMaxSlots(num), GetItemSprite(num),isHasOrAvaillable(num), GetItemMarkSprite(num), "Main",itemAtlas,OnTappedItem);
                }else{
                    //装備していない
                    roditemList.items[i].SetItems(num, GetTittle(num), GetHasNum(num), 
                        GetMaxSlots(num), GetItemSprite(num),isHasOrAvaillable(num), GetItemMarkSprite(num),itemAtlas,OnTappedItem);

                }

                num++;

            }else{
                if(!roditemList.items[i].gameObject.activeSelf)NGUITools.SetActive(roditemList.items[i].gameObject,false);
            }

        }

        roditemList.itemCount=num;
        foreach(RodItemContainer it in  roditemList.items){
            if(!it.isUsed)NGUITools.SetActive(it.gameObject,false);
        }


        roditemList.AffectGrid();
        roditemList.view.ResetPosition();

    }


    public void SetRodToMain(){
        StartCoroutine(SetRodToMainInvoke());   
    }

    IEnumerator SetRodToMainInvoke(){
        WaitAndCover.Instance.ShowWait();
        WaitAndCover.Instance.CoverAll(true);
        yield return null;
        Debug.LogError("SetRodToMainInvoke 1");
        for(int i=0;i<DataManger.Instance.GAMEDATA.tackleSlots.Count;i++){
            if(DataManger.Instance.GAMEDATA.tackleSlots[i].isMainTackle){
                DataManger.Instance.GAMEDATA.tackleSlots[i].isMainTackle=false;
            }
        }
        Debug.LogError("SetRodToMainInvoke 2");
        DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isMainTackle=true;
        Coroutine col=StartCoroutine(SetItemLists());

        yield return col;
        WaitAndCover.Instance.StopWait();
        WaitAndCover.Instance.UnCoverAll();

    }



    public  override void SetContents(int i){
        Debug.Log(GetMethodName()+this.name);
        //タイトル　画像　マーク

        affecter.SetLureParams( GetTittle(i),GetItemSprite(i), GetItemMarkSprite(i),itemAtlas);

        //個数　レンジ　特殊能力　ルアーのみ
        affecter.SetNum(GetHasNum(i), GetMaxSlots(i));


        affecter.SetRangeValue();
        SetAvilities(GetSPAvility(i));
        //レーダー
        if(isHasRod(i)){
            //持っているので、装備を表示する

            affecter.mainRodToggle.SetActive(true,true);


            affecter.mainRodToggle.SetToggle( DataManger.Instance.GAMEDATA.tackleSlots[i].isMainTackle);
           
            if(DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum==-1){
                //何もない
                if(DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum==-1){
                    affecter.SetEquipped("","","","","","");
                    affecter. SetAishou(-100.0f,-100.0f,-100.0f);
                }else{
                    affecter.SetEquipped("","","","",Constants.LineDatas.itemSprites[DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum],Constants.LineDatas.itemTittles[DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum]);
                
                    affecter. SetAishou(-100.0f,-100.0f,GetAishouLine(i,DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum));
                }

            }else{
                if(DataManger.Instance.GAMEDATA.tackleSlots[i].isSoft){
                    
                    int currentRigID=equipSoft.GetEquippedRigID(DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum);
                    //string spriteName_Soft,string spriteName_RigSoft


                    if(DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum==-1){
                        affecter.SetEquipped("","",Constants.SoftLureDatas.itemSprites[DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum],
                            Constants.SoftLureDatas.itemTittles[DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum],"","");
                    }else{
                        affecter.SetEquipped("","",Constants.SoftLureDatas.itemSprites[DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum],
                            Constants.RigDatas.itemSprites[currentRigID],
                            Constants.RigPositionDefines.GetRigPos(currentRigID,Constants.SoftLureDatas.itemType[DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum]),
                            Constants.RigPositionDefines.GetRigRot(currentRigID,Constants.SoftLureDatas.itemType[DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum]),
                            Constants.SoftLureDatas.itemTittles[DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum],
                            Constants.RigPositionDefines.GetShouldShowBack(currentRigID,Constants.SoftLureDatas.itemType[DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum]),
                                Constants.LineDatas.itemSprites[DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum],
                                Constants.LineDatas.itemTittles[DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum]);
                    }
                    affecter. SetAishou(-100.0f,GetAishouLure(i,DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum,true),GetAishouLine(i,DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum));

                }else{
                    
                    if(DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum==-1){
                        affecter.SetEquipped(Constants.LureDatas.itemSprites[DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum],Constants.LureDatas.itemTittles[DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum],"","","","");
                    }else{
                        affecter.SetEquipped(Constants.LureDatas.itemSprites[DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum],Constants.LureDatas.itemTittles[DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum],"","",Constants.LineDatas.itemSprites[DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum],Constants.LineDatas.itemTittles[DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum]);
                    }
                    affecter. SetAishou(GetAishouLure(i,DataManger.Instance.GAMEDATA.tackleSlots[i].lureNum,false),-100.0f,GetAishouLine(i,DataManger.Instance.GAMEDATA.tackleSlots[i].lineNum));
                }

            }
            //affecter.SetEquipped(string spriteName_Hard,string tittle_Hard,string spriteName_Soft,string tittle_Soft,string spriteName_Line,string tittle_Line);

        }else{
            affecter.mainRodToggle.SetActive(true,false);
            affecter.mainRodToggle.SetToggle(false);
            affecter.SetBuyBtns(DataManger.Instance.GAMEDATA.playerLevel<GetItemUnloclRank(i)?true:false
                ,  GetPrice(i),canEarlyAccess(i),  GetPrice(i)/Constants.Params.DoltoGold, GetItemUnloclRank(i),
                isMax(  GetHasNum(currentSelect),GetMaxSlots(currentSelect)));
        }

        rader.SetRaderData(new float[5]{ GetGraph(i,1), GetGraph(i,2), GetGraph(i,3), GetGraph(i,4), GetGraph(i,5)});


    }

    public  bool isHasRod(int num){
        if( GetHasNum(num)>0){
            //持っている
            return true;
        }

        return false;
    }


    public override void CheckEarlyAccess(){
        int i=0;
        earlyAccessAvaillable.Clear();
        foreach(RodItemContainer con in roditemList.items){
            if(i< GetItemSpriteLength()){
                if( earlyAccessAvaillable.Count<Constants.Params.rodEarlyAccess&&!isHasOrAvaillable(i)){

                    earlyAccessAvaillable.Add(i);
                }
            }
            i++;
        }
    }
    public override bool isHasOrAvaillable(int num){
            if(DataManger.Instance.GAMEDATA.playerLevel>= GetItemUnloclRank(num)){
                //アンロックされている
                return true;
            }else{
               
                if(  GetHasNum(num)>0){
                    //アンロックされていないが持っている
                    return true;
                }
            }

        return false;
    }



    public float GetGraph(int current,int num){
        switch(num){
        case 1:
            return Constants.RodsDatas.graph_1[current];
            break;
        case 2:
            return  Constants.RodsDatas.graph_2[current];
            break;
        case 3:
            return  Constants.RodsDatas.graph_3[current];
            break;
        case 4:
            return Constants.RodsDatas.graph_4[current];
            break;
        case 5:
            return  Constants.RodsDatas.graph_5[current];
            break;
        }
        return 0.0f;
    }
    public int GetPrice(int current){
        return Constants.RodsDatas.itemPrices[current];
    }
    public int GetMaxSlots(int current){
        return Constants.RodsDatas.itemNumsMax[current];

    }
    public int GetItemUnloclRank(int current){
        return Constants.RodsDatas.itemUnlockAt[current];

    }
    public string GetTittle(int current){
        return Constants.RodsDatas.itemTittles[current];
    }
    public string GetItemSprite(int current){
        return Constants.RodsDatas.itemSprites[current];
    }
    public int GetItemSpriteLength(){
        return Constants.RodsDatas.itemSprites.Length;
    }
    public string GetItemMarkSprite(int current){
        return  Constants.Params.HeavyString[(int)Constants.RodsDatas.heavyCategory[current]];
    }
    public string GetSPAvility(int current){
        return Constants.RodsDatas.avility[current];
    }
    public int GetHasNum(int current){
        return DataManger.Instance.GAMEDATA.lureHas_Rods[ GetTittle(current)];
    }

    public  IEnumerator OnEquipHardInvoke(){
        WaitAndCover.Instance.ShowWait();
        WaitAndCover.Instance.CoverAll(false);
        equipBtn.SetDelegate(OnEquip_Hard);
        affecter.SetMode(BottomMenu.Btn);
        equipBtn.rader.InitRader( PSGameUtils.SplitStringData(Localization.Get("TotalLureParams_Tittles"),new char[]{';'}),transform);
        //現在のルアー
        int i=DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lureNum;
        Debug.Log("現在の装備 "+i+" "+DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft);
        currentChangeSelect=i;

        equipBtn.SetAishouTotal(GetAishouLure(currentSelect,currentChangeSelect,false),GetAishouLine(currentSelect, DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lineNum));
        equipBtn.SetAishouCurrent(GetAishouLure(currentSelect,currentChangeSelect,false));
        yield return new WaitForSeconds(0.5f);

        if(currentChangeSelect==-1){
            //何もない
            Debug.Log("現在の装備はハードではない");
            equipHard.Show(0);
            currentChangeSelect=0;
            SetCurrentLure(currentSelect,0);
        }else{
            currentChangeSelect=0;
            equipHard.Show(currentChangeSelect);
            SetCurrentLure(currentSelect,0);
        }
       


        OnSelect_Hard(currentChangeSelect);

        WaitAndCover.Instance.StopWait();
        WaitAndCover.Instance.UnCoverAll();

    }


    public void SetCurrentLure(int rodNum,int currentRigID){
        int lureNum=DataManger.Instance.GAMEDATA.tackleSlots[rodNum].lureNum;
        if(DataManger.Instance.GAMEDATA.tackleSlots[rodNum].isSoft){
            if(lureNum==-1){
                equipBtn.SetCurrent("","",equipSoft.itemAtlas);

               
            }else{
                equipBtn.SetCurrent(Constants.SoftLureDatas.itemTittles[lureNum],
                    Constants.SoftLureDatas.itemSprites[lureNum],equipSoft.itemAtlas

                    ,Constants.RigDatas.itemSprites[currentRigID],
                    Constants.RigPositionDefines.GetShouldShowBack(currentRigID,Constants.SoftLureDatas.itemType[lureNum]),
                    Constants.RigPositionDefines.GetRigPos(currentRigID,Constants.SoftLureDatas.itemType[lureNum]),
                    Constants.RigPositionDefines.GetRigRot(currentRigID,Constants.SoftLureDatas.itemType[lureNum])
                );
            }
        }else{
            if(DataManger.Instance.GAMEDATA.tackleSlots[rodNum].lureNum==-1){
                equipBtn.SetCurrent("","",equipHard.itemAtlas);
            }else{
                equipBtn.SetCurrent(Constants.LureDatas.itemTittles[lureNum],
                    Constants.LureDatas.itemSprites[lureNum],equipHard.itemAtlas);
            }
        }

    }
    public void OnEquipHard(){
        StartCoroutine(OnEquipHardInvoke());
    }

    public  IEnumerator OnEquipSoftInvoke(){
        WaitAndCover.Instance.ShowWait();
        WaitAndCover.Instance.CoverAll(false);
       
        equipBtn.SetDelegate(OnEquip_Soft);
        affecter.SetMode(BottomMenu.Btn);
        equipBtn.rader.InitRader( PSGameUtils.SplitStringData(Localization.Get("TotalLureParams_Tittles"),new char[]{';'}),transform);
        //現在のルアー
        int i=DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lureNum;
        Debug.Log("現在の装備 "+i+" "+DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft);
        currentChangeSelect=i;

       
        equipBtn.SetAishouTotal(GetAishouLure(currentSelect,currentChangeSelect,true)
            ,GetAishouLine(currentSelect, DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lineNum));
        equipBtn.SetAishouCurrent(GetAishouLure(currentSelect,currentChangeSelect,true));


        yield return new WaitForSeconds(0.5f);
        if(currentChangeSelect==-1){
            Debug.Log("現在の装備はソフトではない");
            //何もない
            equipSoft.Show(0);
            SetCurrentLure(currentSelect,0);
            currentChangeSelect=0;
        }else{
            Debug.Log("現在の装備はソフト");
            currentChangeSelect=0;
            int currentRigID=equipSoft.GetEquippedRigID(currentChangeSelect);
            equipSoft.Show(currentChangeSelect);
            SetCurrentLure(currentSelect,currentRigID);
        }

        OnSelect_Soft(currentChangeSelect);
     
        WaitAndCover.Instance.StopWait();
        WaitAndCover.Instance.UnCoverAll();

    }

    public void OnEquipSoft(){
        StartCoroutine(OnEquipSoftInvoke());
    }



    public  IEnumerator OnEquipLineInvoke(){
        WaitAndCover.Instance.ShowWait();
        WaitAndCover.Instance.CoverAll(false);
        equipBtn.SetDelegate(OnEquip_Line);
        affecter.SetMode(BottomMenu.Btn);
        equipBtn.rader.InitRader( PSGameUtils.SplitStringData(Localization.Get("TotalLureParams_Tittles"),new char[]{';'}),transform);
        //現在のルアー
        int i=DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lineNum;
        currentChangeSelect=i;

        equipBtn.SetAishouTotal(GetAishouLure(currentSelect,DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lureNum,
            DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft),GetAishouLine(currentSelect,currentChangeSelect));
        equipBtn.SetAishouCurrent(GetAishouLine(currentSelect,currentChangeSelect));
     
        yield return new WaitForSeconds(0.5f);
        if(currentChangeSelect==-1){
            //何もない
            equipLine.Show(0);
            equipBtn.SetCurrent("","", equipLine.itemAtlas);
            currentChangeSelect=0;

        }else{
            currentChangeSelect=0;
            equipLine.Show(currentChangeSelect);
            //レーダーを表示する。 現在のライン→セレクトされたライン
            equipBtn.SetCurrent(Constants.LineDatas.itemTittles[i],Constants.LineDatas.itemSprites[i], equipLine.itemAtlas);

        }

       


        OnSelect_Line(currentChangeSelect);
      
        WaitAndCover.Instance.StopWait();
        WaitAndCover.Instance.UnCoverAll();

    }

    public void OnEquipLine(){
      
        StartCoroutine(OnEquipLineInvoke());

    }

    public EquipLine equipLine;
    public EquipSoft equipSoft;
    public EquipHard equipHard;

    public EquipBtn equipBtn;

    public void OnSelect_Line(int i){
        Debug.Log("実装しろ　OnEquip_Line "+i);
        currentChangeSelect=i;
                    equipBtn.SetKouho(Constants.LineDatas.itemTittles[currentChangeSelect],Constants.LineDatas.itemSprites[currentChangeSelect],equipLine.itemAtlas,
                        isEquippable( DataManger.Instance.GAMEDATA.lureHas_Line[Constants.LineDatas.itemTittles[currentChangeSelect]],
                            DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lineNum==currentChangeSelect?true:false));

        //0 なし1=ハード 2＝ソフト 3＝ライン
        equipBtn.rader.SetRaderData(GetTotal(currentSelect,DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lineNum,3
            ,DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft),GetTotal(currentSelect,currentChangeSelect,3,DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft));
        
        equipBtn.SetAishouTotal(GetAishouLure(currentSelect,DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lureNum,
            DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft),
            GetAishouLine(currentSelect,currentChangeSelect));
        equipBtn.SetAishouChanged(GetAishouLine(currentSelect,currentChangeSelect));
    }
    public void OnSelect_Soft(int i){
        Debug.Log("実装しろ　OnEquip_Soft "+i);
        currentChangeSelect=i;
      
   
        int currentRigID=equipSoft.GetEquippedRigID(currentChangeSelect);
        Debug.Log("リグは"+currentRigID);
        equipBtn.SetKouho(Constants.SoftLureDatas.itemTittles[currentChangeSelect],Constants.SoftLureDatas.itemSprites[currentChangeSelect],equipSoft.itemAtlas

            ,Constants.RigDatas.itemSprites[currentRigID],
            Constants.RigPositionDefines.GetShouldShowBack(currentRigID,Constants.SoftLureDatas.itemType[currentChangeSelect]),
            Constants.RigPositionDefines.GetRigPos(currentRigID,Constants.SoftLureDatas.itemType[currentChangeSelect]),
            Constants.RigPositionDefines.GetRigRot(currentRigID,Constants.SoftLureDatas.itemType[currentChangeSelect]),
            isEquippable( DataManger.Instance.GAMEDATA.lureHas_Soft[Constants.SoftLureDatas.itemTittles[currentChangeSelect]],
                DataManger.Instance.HowManyEquipped_Lure(currentChangeSelect,true)));


        /*
        if(DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft){
            equipBtn.SetKouho(Constants.SoftLureDatas.itemTittles[currentChangeSelect],Constants.SoftLureDatas.itemSprites[currentChangeSelect],equipSoft.itemAtlas

                ,Constants.RigDatas.itemSprites[currentRigID],
                Constants.RigPositionDefines.GetShouldShowBack(currentRigID,Constants.SoftLureDatas.itemType[currentChangeSelect]),
                Constants.RigPositionDefines.GetRigPos(currentRigID,Constants.SoftLureDatas.itemType[currentChangeSelect]),
                Constants.RigPositionDefines.GetRigRot(currentRigID,Constants.SoftLureDatas.itemType[currentChangeSelect]),
                isEquippable( DataManger.Instance.GAMEDATA.lureHas_Soft[Constants.SoftLureDatas.itemTittles[currentChangeSelect]],
                    DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lureNum==currentChangeSelect?true:false));
        }else{
            equipBtn.SetKouho(Constants.SoftLureDatas.itemTittles[currentChangeSelect],Constants.SoftLureDatas.itemSprites[currentChangeSelect],equipSoft.itemAtlas

                ,Constants.RigDatas.itemSprites[currentRigID],
                Constants.RigPositionDefines.GetShouldShowBack(currentRigID,Constants.SoftLureDatas.itemType[currentChangeSelect]),
                Constants.RigPositionDefines.GetRigPos(currentRigID,Constants.SoftLureDatas.itemType[currentChangeSelect]),
                Constants.RigPositionDefines.GetRigRot(currentRigID,Constants.SoftLureDatas.itemType[currentChangeSelect]),
                isEquippable( DataManger.Instance.GAMEDATA.lureHas_Soft[Constants.SoftLureDatas.itemTittles[currentChangeSelect]],
                    false));
        }*/


        //0 なし1=ハード 2＝ソフト 3＝ライン
        /*if(!DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft){
            equipBtn.rader.SetRaderData( new float[5]{0.0f,0.0f,0.0f,0.0f,0.0f},GetTotal(currentSelect,currentChangeSelect,2));
        }else{
            equipBtn.rader.SetRaderData(GetTotal( currentSelect,DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lureNum,0),GetTotal(currentSelect,currentChangeSelect,2));
        }*/
        equipBtn.rader.SetRaderData(GetTotal( currentSelect,DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lureNum,2,DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft
        ),GetTotal(currentSelect,currentChangeSelect,2,true));


        equipBtn.SetAishouTotal(GetAishouLure(currentSelect,currentChangeSelect,true),GetAishouLine(currentSelect,DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lineNum));
        equipBtn.SetAishouChanged(GetAishouLure(currentSelect,currentChangeSelect,true));

    }
    public void OnSelect_Hard(int i){
        Debug.Log("ハード　"+i);
        currentChangeSelect=i;


        equipBtn.SetKouho(Constants.LureDatas.itemTittles[currentChangeSelect],Constants.LureDatas.itemSprites[currentChangeSelect],equipHard.itemAtlas,
            isEquippable( DataManger.Instance.GAMEDATA.lureHas_Hard[Constants.LureDatas.itemTittles[currentChangeSelect]],
                DataManger.Instance.HowManyEquipped_Lure(currentChangeSelect,false)));


        /*
        if(!DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft){
            equipBtn.SetKouho(Constants.LureDatas.itemTittles[currentChangeSelect],Constants.LureDatas.itemSprites[currentChangeSelect],equipHard.itemAtlas,
                isEquippable( DataManger.Instance.GAMEDATA.lureHas_Hard[Constants.LureDatas.itemTittles[currentChangeSelect]],
                    DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lureNum==currentChangeSelect?true:false));
        }else{
            equipBtn.SetKouho(Constants.LureDatas.itemTittles[currentChangeSelect],Constants.LureDatas.itemSprites[currentChangeSelect],equipHard.itemAtlas,
                isEquippable( DataManger.Instance.GAMEDATA.lureHas_Hard[Constants.LureDatas.itemTittles[currentChangeSelect]],
                  false));
        }*/
        //0 なし1=ハード 2＝ソフト 3＝ライン
        /*if(DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft){
            Debug.Log("ハード　現在は　ソフトが装備されてる");
            equipBtn.rader.SetRaderData( new float[5]{0.0f,0.0f,0.0f,0.0f,0.0f},GetTotal(currentSelect,currentChangeSelect,1));
        }else{
           
            equipBtn.rader.SetRaderData(GetTotal( currentSelect,DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lureNum,1),GetTotal(currentSelect,currentChangeSelect,1));
        }*/

        equipBtn.rader.SetRaderData(GetTotal( currentSelect,DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lureNum,1,DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft
        ),GetTotal(currentSelect,currentChangeSelect,1,false));

        equipBtn.SetAishouTotal(GetAishouLure(currentSelect,currentChangeSelect,false),
            GetAishouLine(currentSelect,DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lineNum));


        equipBtn.SetAishouChanged(GetAishouLure(currentSelect,currentChangeSelect,false));

    }

    public int currentChangeSelect=0;
    public void OnEquip_Hard(){
        Debug.Log("実装しろ　OnEquip_Hard "+currentChangeSelect);
        DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft=false;
        DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lureNum=currentChangeSelect;
        AudioManager.Instance.Equip();
        WaitAndCover.Instance.ShowInfoList(Localization.Get("Info_Equip")+ equipHard.GetTittle(currentSelect));
        Show(currentSelect);
    }
    public void OnEquip_Soft(){
        Debug.Log("実装しろ　OnEquip_Hard "+currentChangeSelect);
        DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].isSoft=true;
        DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lureNum=currentChangeSelect;
        AudioManager.Instance.Equip();
        WaitAndCover.Instance.ShowInfoList(Localization.Get("Info_Equip")+ equipSoft.GetTittle(currentSelect));
        Show(currentSelect);
    }
    public void OnEquip_Line(){
        Debug.Log("実装しろ　OnEquip_Hard "+currentChangeSelect);
        DataManger.Instance.GAMEDATA.tackleSlots[currentSelect].lineNum=currentChangeSelect;
        AudioManager.Instance.Equip();
        WaitAndCover.Instance.ShowInfoList(Localization.Get("Info_Equip")+ equipLine.GetTittle(currentSelect));
        Show(currentSelect);
    }

    public void OnCloseEquip(){
        Debug.Log("OnCloseEquip");
        AudioManager.Instance.Cancel();
        Show(currentSelect);
    }


    bool isEquippable(int has,int isEquipped){

        Debug.Log("isEquippable "+has+" "+isEquipped);
        if(isEquipped>=has){
            return false;
        }else{
            if(has<=0){
                return false;
            }
        }
        return true;
    }
    bool isEquippable(int has,bool isEquipped){

        //Debug.Log("isEquippable "+has+" "+isEquipped);
        if(isEquipped){
            return false;
        }else{
            if(has<=0){
                return false;
            }
        }
        return true;
    }

    //0 なし1=ハード 2＝ソフト 3＝ライン
    float[] GetTotal(int rod,int current,int itemType,bool isSoftData){
        //TotalLureParams_Tittles,飛距離;感度;適合サイズ;フッキング;強度,Distance;Fish Size;Stock;Foocking;Strength

        Debug.Log("GetTotal　"+rod +" "+current+" "+itemType);

        float[] vals=new float[5]{0.0f,0.0f,0.0f,0.0f,0.0f};
        //総計して1.0に持ってく
        if(DataManger.Instance.GAMEDATA.tackleSlots[rod].lineNum==-1){
            if(itemType==3 && current!=-1){
            }else{
                Debug.Log("ラインを装備していない　return");
                return vals;
            }
        }
        if(DataManger.Instance.GAMEDATA.tackleSlots[rod].lureNum==-1){
            if(itemType<=2 && current!=-1){
            }else{
                Debug.Log("ルアーを装備していない　return");
                return vals;
            }
        }
        bool isSoft=isSoftData;

        int line=DataManger.Instance.GAMEDATA.tackleSlots[rod].lineNum;

        if(itemType==3 && current!=-1 && line!=current){
            Debug.LogError("ラインオーバーライド");
            line=current;
        }
        int lure=DataManger.Instance.GAMEDATA.tackleSlots[rod].lureNum;
        if(itemType<3 && current!=-1&& lure!=current){
            Debug.LogError("ルアーオーバーライド");
            lure=current;
        }
        int rigNum=0;



        if(isSoft)rigNum=equipSoft.GetEquippedRigID(lure);

        if(lure==-1 || line==-1 || rigNum==-1){
            Debug.LogError("やばいやつ");
            return vals;
        }

        float aishou=1.0f;

        aishou+=GetAishouLure(rod,lure,isSoft);
        Debug.Log("相性　ルアーとライン "+aishou.ToString("F2"));
        aishou+=GetAishouLine(rod,line);
        Debug.Log("相性　ロッドとライン "+aishou.ToString("F2"));

        //飛距離　重いルアーほど飛ぶ 軽いラインほど飛ぶ　ロッドの性能もあるがベイトは飛ばない　　(60:30:10)
        if(!isSoft){
            vals[0]=PSGameUtils.WariaiVals(new float[]{Constants.LureDatas.graph_2[lure],Constants.LineDatas.graph_2[line],Constants.RodsDatas.graph_2[rod]},new int[]{60,30,10});
        }else{
            //ソフトはリグ込みで出す
            vals[0]=PSGameUtils.WariaiVals(new float[]{Constants.SoftLureDatas.graph_2[lure]+Constants.RigDatas.graph_2[rigNum],Constants.LineDatas.graph_2[line],Constants.RodsDatas.graph_2[rod]},new int[]{60,30,10});
        }


        //感度　ラインの感度　ロッドの感度(60:40)
        if(!isSoft){
            vals[1]=PSGameUtils.WariaiVals(new float[]{Constants.LineDatas.graph_5[line],Constants.RodsDatas.graph_5[rod]},new int[]{60,40});
        }else{
            //ソフトはリグ込みで出す
            vals[1]=PSGameUtils.WariaiVals(new float[]{Constants.LineDatas.graph_5[line],Constants.RodsDatas.graph_5[rod]},new int[]{60,40});
        }

        //サイズ(サイズに対する難易度を決定する)  ライン強度　ロッド強度　(70:30) ルアーのサイズは最低を決める
        if(!isSoft){
            vals[2]=PSGameUtils.WariaiVals(new float[]{Constants.LineDatas.graph_3[line],Constants.RodsDatas.graph_3[rod]},new int[]{70,30});
        }else{
            //ソフトはリグ込みで出す
            vals[2]=PSGameUtils.WariaiVals(new float[]{Constants.LineDatas.graph_3[line],Constants.RodsDatas.graph_3[rod]},new int[]{70,30});
        }

        //フッキング  ロッド性能　ルアーのフック性能　ライン性能　(20:30:50) ルアーのサイズは最低を決める
        if(!isSoft){
            vals[3]=PSGameUtils.WariaiVals(new float[]{Constants.LineDatas.graph_4[line],Constants.LureDatas.graph_4[lure],Constants.RodsDatas.graph_4[rod]},new int[]{50,30,20});
        }else{
            //ソフトはリグ込みで出す
            vals[3]=PSGameUtils.WariaiVals(new float[]{Constants.LineDatas.graph_4[line],Constants.RigDatas.graph_4[rigNum],Constants.RodsDatas.graph_4[rod]},new int[]{50,30,20});
        }

        //強度　強いほど切れない　引いてこれる　ロッド　ライン（20:80）
        if(!isSoft){
            vals[4]=PSGameUtils.WariaiVals(new float[]{Constants.LineDatas.graph_3[line],Constants.RodsDatas.graph_3[rod]},new int[]{20,80});
        }else{
            //ソフトはリグ込みで出すs
            vals[4]=PSGameUtils.WariaiVals(new float[]{Constants.LineDatas.graph_3[line],Constants.RodsDatas.graph_3[rod]},new int[]{20,80});
        }

        Debug.Log(" "+vals[0]+" "+vals[1]+" "+vals[2]+" "+vals[3]+" "+vals[4]);

        //飛距離;感度;適合サイズ;フッキング;強度
        vals[0]=vals[0]*aishou;
        vals[1]=vals[1]*aishou;
        vals[2]=vals[2]*aishou;
        vals[3]=vals[3]*aishou;
        vals[4]=vals[4]*aishou;

        Debug.LogError("飛距離"+vals[0]+" 感度"+vals[1]+" 適合サイズ"+vals[2]+" フッキング"+vals[3]+" 強度"+vals[4]);

        return vals;


    }
    int GetHeavyAishou(Constants.Heavy rod , Constants.Heavy two){
        return rod-two;
    }


    float GetAishouLine(int rod ,int line){
        float vals=-100.0f;
        if(rod==-1 || line==-1){
            Debug.LogError("やばいやつ");
            return vals;
        }
        Debug.Log("ロッど＝ "+rod+"ライン "+line);
        Constants.Heavy  lineHeavy=Constants.Heavy.h;
        Constants.Heavy rodHeavy=Constants.Heavy.h;
        //Heavy{ul,l,ml,m,mh,h,xh}
        rodHeavy=Constants.RodsDatas.heavyCategory[rod];
        lineHeavy=Constants.LineDatas.heavyCategory[line];


        //飛距離;感度;適合サイズ;フッキング;強度
        int  aishouTemp=GetHeavyAishou(rodHeavy,lineHeavy);
        Debug.Log("ロッドの重さ＝ "+rodHeavy.ToString()+"ラインの重さ＝ "+lineHeavy.ToString());
        Debug.Log("相性 "+aishouTemp);
        switch(Mathf.Abs(aishouTemp)){
        case 0:
            vals=-0.0f;
            break;
        case 1:
            vals=-0.25f;
            break;
        case 2:
            vals=-0.1f;
            break;
        case 3:
            vals=-0.15f;
            break;
        case 4:
            vals=-0.25f;
            break;
        case 5:
            vals=-0.3f;
            break;
        case 6:
            vals=-0.4f;
            break;
        }
        Debug.Log("＝ "+vals);
        return vals;
    }


    float GetAishouLure(int rod ,int lure,bool isSoft){
        float vals=-100.0f;
        if(lure==-1){
            Debug.LogError("やばいやつ");
            return vals;
        }
        int rigNum=isSoft?equipSoft.GetEquippedRigID(lure):0;

        Constants.Heavy  lureHeavy=Constants.Heavy.h;
        Constants.Heavy rodHeavy=Constants.Heavy.h;
        //Heavy{ul,l,ml,m,mh,h,xh}

        Debug.Log("GetAishouLure "+rod+" ルアー "+lure+" "+isSoft);


        rodHeavy=Constants.RodsDatas.heavyCategory[rod];

       
        lureHeavy=!isSoft? Constants.LureDatas.heavyCategory[lure]:Constants.RigDatas.heavyCategory[rigNum];


        Debug.Log("ロッドの重さ＝ "+rodHeavy.ToString()+"ルアーの重さ＝ "+lureHeavy.ToString());
        //飛距離;感度;適合サイズ;フッキング;強度
        int  aishouTemp=GetHeavyAishou(rodHeavy,lureHeavy);

        switch(Mathf.Abs(aishouTemp)){
        case 0:
            vals=-0.0f;
            break;
        case 1:
            vals=-0.25f;
            break;
        case 2:
            vals=-0.1f;
            break;
        case 3:
            vals=-0.15f;
            break;
        case 4:
            vals=-0.25f;
            break;
        case 5:
            vals=-0.3f;
            break;
        case 6:
            vals=-0.4f;
            break;
        }
        return vals;
    }

}
