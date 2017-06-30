using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EquipHard : TackeMenuParentEquip{
  



    public override void OnTappedItem(int num,bool isInit){
        //Debug.Log(GetMethodName()+this.name);
        if(!isInit && num==currentSelect)return;
        if(!isInit)AudioManager.Instance.SelectItemList();
        int row=Mathf.FloorToInt(currentSelect/2.0f);

        if( GetItemSpriteLength()>=currentSelect)itemList.items[row].GetItem(currentSelect%2).SetSelect(false);
        currentSelect= num;
        row=Mathf.FloorToInt(currentSelect/2.0f);
        if( GetItemSpriteLength()>=currentSelect)itemList.items[row].GetItem(num%2).SetSelect(true);

        SetContents(num);
        if(!isInit)rods.OnSelect_Hard(num);
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
        Debug.Log(GetMethodName()+this.name);
        Coroutine cor;
        rods.ShowItemList(false);
        cor=StartCoroutine(SetItemLists());
        yield return cor;

        currenEquippedLure=currentLure;

        currentSelect=0;
        OnTappedItem(currentSelect,true);

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


                int eqp=-1;
                eqp=DataManger.Instance.IsEquipped_Lure(num,false);
                eqp++;
                if(eqp>0){
                    //装備中
                    //int id,string tittle,int num,int maxhas,string itemSp,int isOne,bool isAvaillable,string markSp,string extra,Callback_OnTappedItem tapEvent,UIAtlas at,Material mat
                    itemList.items[i].SetItems(num,  GetTittle(num), GetHasNum(num),GetHasNum(num), DataManger.Instance.HowManyEquipped_Lure(num,false),
                        GetMaxSlots(num), GetItemSprite(num),0,isHas(num),  GetItemMarkSprite(num),"ROD"+eqp,OnTappedItem,itemAtlas);

                }else{
                    //装備していない
                    itemList.items[i].SetItems(num,  GetTittle(num), GetHasNum(num),GetHasNum(num), DataManger.Instance.HowManyEquipped_Lure(num,false),
                        GetMaxSlots(num), GetItemSprite(num),0,isHas(num),  GetItemMarkSprite(num),OnTappedItem,itemAtlas);

                }


                num++;
                if(num< GetItemSpriteLength()){
                    if(itemList.items[i].gameObject.activeSelf!=true)NGUITools.SetActive(itemList.items[i].gameObject,true);
                    itemList.items[i].SetActive(1,true);
                    eqp=DataManger.Instance.IsEquipped_Lure(num,false);
                    eqp++;
                    if(eqp>0){
                        //装備中

                        itemList.items[i].SetItems(num,  GetTittle(num), GetHasNum(num),GetHasNum(num), DataManger.Instance.HowManyEquipped_Lure(num,false),
                            GetMaxSlots(num), GetItemSprite(num),1,isHas(num),  GetItemMarkSprite(num),"ROD"+eqp,OnTappedItem,itemAtlas);

                    }else{
                        //装備していない
                        itemList.items[i].SetItems(num,  GetTittle(num), GetHasNum(num),GetHasNum(num), DataManger.Instance.HowManyEquipped_Lure(num,false),
                            GetMaxSlots(num), GetItemSprite(num),1,isHas(num),  GetItemMarkSprite(num),OnTappedItem,itemAtlas);

                    }
                }else{
                    itemList.items[i].SetActive(1,false);
                }
                num++;
            }else{
                if(itemList.items[i].gameObject.activeSelf!=false)NGUITools.SetActive(itemList.items[i].gameObject,false);
            }

        }
        //Debug.Log("item list "+num);
        itemList.itemCount=num;
        foreach(LureItemContainer it in  itemList.items){
            if(!it.isUsed)NGUITools.SetActive(it.gameObject,false);
        }
        itemList.AffectGrid();
        itemList.view.ResetPosition();
    }

    public  override void SetContents(int i){
        //タイトル　画像　マーク

    }



   
    public override bool isHas(int num){


        if(DataManger.Instance.GAMEDATA.playerLevel>=  GetItemUnloclRank(num)){
            //アンロックされている
            if( GetHasNum(num)>0){
                //アンロックされていないが持っている
                return true;
            }
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

    public float GetGraph(int current,int num){
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
   public int GetPrice(int current){
        return Constants.LureDatas.itemPrices[current];
    }
   public int GetMaxSlots(int current){
        return Constants.LureDatas.itemNumsMax[current];

    }
   public int GetItemUnloclRank(int current){
        return Constants.LureDatas.itemUnlockAt[current];

    }
   public string GetTittle(int current){
        return Constants.LureDatas.itemTittles[current];
    }
   public string GetItemSprite(int current){
        return Constants.LureDatas.itemSprites[current];
    }
   public int GetItemSpriteLength(){
        return Constants.LureDatas.itemSprites.Length;
    }
   public string GetItemMarkSprite(int current){
        return Constants.Params.HeavyString[(int)Constants.LureDatas.heavyCategory[current]];
    }
   public LureRangePivot GetRangePivot(int current){
        return (LureRangePivot) Constants.LureDatas.rangePivot[current];
    }
   public float GetRangeSize(int current){
        return Constants.LureDatas.rangeSize[current];
    }
   public bool[] GetSPAvility(int current){
        return PSGameUtils.StringToBoolArray(Constants.LureDatas.avility[current]);
    }
}
