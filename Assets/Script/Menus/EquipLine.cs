using UnityEngine;
using System.Collections;
using System.Collections;

public class EquipLine : TackeMenuParentEquip {

  

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
        if(!isInit) rods.OnSelect_Line(num);
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
        rods.ShowItemList(false);
        cor=StartCoroutine(SetItemLists());
        yield return cor;


        currenEquippedLure=currentLure;

        currentSelect=0;
        OnTappedItem(currentSelect,true);
        yield return new WaitForSeconds(0.5f);
        Debug.Log(GetMethodName()+this.name+"End");
        WaitAndCover.Instance.StopWait();
        WaitAndCover.Instance.UnCoverAll();

    }
    public UIAtlas itemAtlas;
    public  override IEnumerator SetItemLists(){

        Debug.Log(GetMethodName()+this.name);
        yield return null;
        itemList.view.ResetPosition();

        foreach(LureItemContainer it in  itemList.items){
            it.isUsed=false;
        }
        int num=0;

        for(int i=0;i<GetItemSpriteLength()/2;i++){
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
                eqp=DataManger.Instance.IsEquipped_Line(num);
                eqp++;
                if(eqp>0){
                    //装備中
                    itemList.items[i].SetItems(num, GetTittle(num), GetHasNum(num), 
                        GetMaxSlots(num), GetItemSprite(num),0,isHas(num), GetItemMarkSprite(num),"E:"+eqp,OnTappedItem,itemAtlas);
                }else{
                    //装備していない
                    itemList.items[i].SetItems(num, GetTittle(num), GetHasNum(num), 
                        GetMaxSlots(num), GetItemSprite(num),0,isHas(num), GetItemMarkSprite(num),OnTappedItem,itemAtlas);

                }



                num++;
                //Debug.Log("Start 2");
                if(num< GetItemSpriteLength()){
                    //if(num==13)Debug.Log("Start- 1");
                    if(itemList.items[i].gameObject.activeSelf!=true)NGUITools.SetActive(itemList.items[i].gameObject,true);
                    itemList.items[i].SetActive(1,true);
                    //if(num==13)Debug.Log("Start- 2");

                    eqp=DataManger.Instance.IsEquipped_Line(num);
                    eqp++;
                    if(eqp>0){
                        //装備中
                        itemList.items[i].SetItems(num, GetTittle(num), GetHasNum(num), 
                            GetMaxSlots(num), GetItemSprite(num),1,isHas(num), GetItemMarkSprite(num),"E:"+eqp,OnTappedItem,itemAtlas);
                    }else{
                        //装備していない
                        itemList.items[i].SetItems(num, GetTittle(num), GetHasNum(num), 
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
        Debug.Log(GetMethodName()+this.name);
        //タイトル　画像　マーク

    }

    public override bool isHas(int num){

        if(DataManger.Instance.GAMEDATA.playerLevel>=  GetItemUnloclRank(num)){
            //アンロックされている
            if(  GetHasNum(num)>0){
                //アンロックされていないが持っている
                return true;
            }
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
            return Constants.LineDatas.graph_1[current];
            break;
        case 2:
            return  Constants.LineDatas.graph_2[current];
            break;
        case 3:
            return  Constants.LineDatas.graph_3[current];
            break;
        case 4:
            return Constants.LineDatas.graph_4[current];
            break;
        case 5:
            return  Constants.LineDatas.graph_5[current];
            break;
        }
        return 0.0f;
    }
    public  int GetPrice(int current){
        return Constants.LineDatas.itemPrices[current];
    }
    public int GetMaxSlots(int current){
        return Constants.LineDatas.itemNumsMax[current];

    }
    public int GetItemUnloclRank(int current){
        return Constants.LineDatas.itemUnlockAt[current];

    }
    public string GetTittle(int current){
        return Constants.LineDatas.itemTittles[current];
    }
    public string GetItemSprite(int current){
        return Constants.LineDatas.itemSprites[current];
    }
    public int GetItemSpriteLength(){
        return Constants.LineDatas.itemSprites.Length;
    }
    public string GetItemMarkSprite(int current){
        return  Constants.Params.HeavyString[(int)Constants.LineDatas.heavyCategory[current]];
    }
    public string GetSPAvility(int current){
        return Constants.LineDatas.avility[current];
    }
    public int GetHasNum(int current){
        return DataManger.Instance.GAMEDATA.lureHas_Line[ GetTittle(current)];
    }
}
