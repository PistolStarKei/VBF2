using UnityEngine;
using System.Collections;

public class EquipSoft : TackeMenuParentEquip{


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
        if(!isInit)rods.OnSelect_Soft(num);
        //0 1 0
        //2 3 1
    }



    //Public Methods
    public override void Show(int currentLure){
        //Debug.Log(GetMethodName()+this.name);
        WaitAndCover.Instance.ShowWait();
        WaitAndCover.Instance.CoverAll(true);
        StartCoroutine(Init(currentLure));
    }
    public override IEnumerator Init(int currentLure){
        Debug.Log(GetMethodName()+this.name);
        Coroutine cor;
        rods. ShowItemList(false);
        cor=StartCoroutine(SetItemLists());
        yield return cor;

        currenEquippedLure=currentLure;

        currentSelect=0;
        OnTappedItem(currentSelect,true);
        yield return new WaitForSeconds(0.5f);
        //Debug.Log(GetMethodName()+this.name+"End");
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
            //Debug.Log(GetMethodName()+this.name+"Set"+i);
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
                    //Debug.LogError("装備中"+num);
                    itemList.items[i].SetItems(num, GetTittle(num),GetHasNum(num),GetMaxSlots(num), DataManger.Instance.HowManyEquipped_Lure(num,true),
                        GetMaxSlots(num), GetItemSprite(num),0,isHas(num), GetItemMarkSprite(num),"E:"+eqp,OnTappedItem,itemAtlas);
                }else{
                    //装備していない
                    itemList.items[i].SetItems(num, GetTittle(num), GetHasNum(num),GetMaxSlots(num), DataManger.Instance.HowManyEquipped_Lure(num,true),
                        GetMaxSlots(num), GetItemSprite(num),0,isHas(num), GetItemMarkSprite(num),OnTappedItem,itemAtlas);

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
                        itemList.items[i].SetItems(num, GetTittle(num), GetHasNum(num),GetMaxSlots(num), DataManger.Instance.HowManyEquipped_Lure(num,true),
                            GetMaxSlots(num), GetItemSprite(num),1,isHas(num), GetItemMarkSprite(num),"E:"+eqp,OnTappedItem,itemAtlas);
                    }else{
                        //装備していない
                        itemList.items[i].SetItems(num, GetTittle(num), GetHasNum(num),GetMaxSlots(num), DataManger.Instance.HowManyEquipped_Lure(num,true),
                            GetMaxSlots(num), GetItemSprite(num),1,isHas(num), GetItemMarkSprite(num),OnTappedItem,itemAtlas);

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

    public  bool isHas(int num){
        if( GetHasNum(num)>0){
            //持っている
            return true;
        }

        return false;
    }
        

    public int GetEquippedRigID(int current){
        
        int[] adons=GetAvaillableAdons(current);
        if(GetEquippedRig(current)>=adons.Length)Debug.LogError("GetEquippedRig Error length over");

        return adons[GetEquippedRig(current)];


    }
    public int[] GetAvaillableAdons(int current){
        //Debug.Log("GetAvaillableAdons"+current+" "+Constants.SoftLureDatas.AvaillableRig[current]);
        return PSGameUtils.StringToIntArray(Constants.SoftLureDatas.AvaillableRig[current],new char[]{';'});
    }

    public float GetAddonGraph(int current,int num){

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
    public string GetRigDesc(int current){
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

    public float GetGraph(int current,int num){
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
    public int GetPrice(int current){
        return Constants.SoftLureDatas.itemPrices[current];
    }
    public int GetRigPrice(int current){
        return Constants.RigDatas.itemPrices[current];
    }
    public int GetMaxSlots(int current){
        return Constants.SoftLureDatas.itemNumsMax[current];

    }
    public int GetItemUnloclRank(int current){
        return Constants.SoftLureDatas.itemUnlockAt[current];

    }
    public string GetTittle(int current){
        return Constants.SoftLureDatas.itemTittles[current];
    }
    public string GetRigTittle(int current){
        return Constants.RigDatas.itemTittles[current];
    }
    public string GetItemSprite(int current){
        return Constants.SoftLureDatas.itemSprites[current];
    }
    public int GetItemSpriteLength(){
        return Constants.SoftLureDatas.itemSprites.Length;
    }
    public string GetItemMarkSprite(int current){
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
    public bool[] GetSPAvility(int current){
        int currentRig=GetEquippedRigID(current);

        return PSGameUtils.MergeAvility(Constants.RigDatas.avilitys[currentRig],Constants.SoftLureDatas.avilitys[current]);
    }
    public int GetHasNum(int current){
        return DataManger.Instance.GAMEDATA.lureHas_Soft[ GetTittle(current)];
    }
}
