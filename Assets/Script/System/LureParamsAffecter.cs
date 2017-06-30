using UnityEngine;
using System.Collections;
public enum LureRangePivot{TOP,MID,BTM};
public enum BottomMenu{Buy,AddOn,Equip,Btn};
public class LureParamsAffecter : MonoBehaviour {

    public PS_Toggle mainRodToggle;
    public UILabel tittleLb;
    public UISprite lureSp;
    public UISprite AdonSp;
    public UILabel markSp;

    public LureBtn topBtn;
    public LureBtn btmBtn;
    public void SetLureParams(string tittle,string spName,string heavy,UIAtlas at){
        tittleLb.text=tittle;
        lureSp.atlas=at;
        lureSp.material=at.spriteMaterial;
        lureSp.spriteName=spName;
        PSGameUtils.SetUISprite(AdonSp,"");
        markSp.text=heavy;
    }

    public void SetRig(string addOnSp,bool shouldShowBack,Vector3 AdonLocalTrans,Vector3 AdonLocalRot){
        PSGameUtils.SetUISprite(AdonSp,addOnSp);
        if(shouldShowBack){
            AdonSp.depth=lureSp.depth-1;
        }else{
            AdonSp.depth=lureSp.depth+1;
        }
        AdonSp.transform.localPosition=AdonLocalTrans;
        AdonSp.transform.localRotation=Quaternion.Euler(AdonLocalRot);

    }


    public SetRange rangeAffecter;
    public void SetRangeValue(LureRangePivot pivot,float percent){
        rangeAffecter.Show();
        rangeAffecter.SetRangeValue(pivot,percent);
    }
    public void SetRangeValue(){
        rangeAffecter.Hide();
    }

    public SetNums numAffecter;
    public void SetNum(int has,int maxhas){
        numAffecter.SetNum(has,maxhas);
    }
    public void SetNum(int has,int reallyhas,int equip,int maxhas){
        numAffecter.SetNum(has,reallyhas,equip,maxhas);
    }

    public void SetBuyBtns(bool isLocked,int priceDol,bool canEarlyAccess,int priceGold,int rankToUnlock,bool isMaxHas){
        SetMode(BottomMenu.Buy);
        if(isLocked){

            //ロック　アンロックレベルを表示する
            if(canEarlyAccess){
                //ロック　アーリーアクセスを表示する
                topBtn.SetDefault();
                topBtn.SetBtns("Unlocks At Rank Of",rankToUnlock.ToString(),"");
                btmBtn.SetDefault();
                btmBtn.SetBtns("Early Access For",priceGold.ToString()+"G","Buy");
            }else{
                topBtn.SetCenter();
                topBtn.SetBtns("Unlocks At Rank Of",rankToUnlock.ToString(),"");
                btmBtn.SetDefault();
                btmBtn.HideBtns();
            }
        }else{
            //購入可能　通常の価格を提示する
            if(isMaxHas){
                topBtn.SetCenter();
                topBtn.SetBtns("Already Have Full","","");
                btmBtn.SetCenter();
                btmBtn.HideBtns();

            }else{
                if(DataManger.Instance.canBuyDolller(priceDol)){
                    //通常のみで購入可能
                    topBtn.SetCenter();
                    topBtn.SetBtns("Buy This Item For",priceDol.ToString()+"$","Buy");
                    btmBtn.SetCenter();
                    btmBtn.HideBtns();

                }else{
                    //通常のみで足りない　
                    topBtn.SetDefault();
                    topBtn.SetBtns("Buy This Item For",priceDol.ToString()+"$","No\nMoney");
                    btmBtn.SetDefault();
                    btmBtn.SetBtns("Buy This Item For",priceGold.ToString()+"G","Buy");
                }
            }

        }
    }


    //アドオンの設定
    public void SetAdonItems(int num,string tittle,string desc,string price,string itemSp,bool isBuyed,bool isEquipped){
        Debug.Log("SetAdonItems"+price);
        SetMode(BottomMenu.AddOn);
        adons.SetAdonItems(num,tittle,desc,price,itemSp,isBuyed,isEquipped);
    }
    public void SetAdonItemsToUA(int num){
        Debug.Log("SetAdonItemsToUA"+num);
        SetMode(BottomMenu.AddOn);
        adons.SetAdonItemsToUA(num);
    }
    public SoftAddOns adons;



    public UIScrollBar rangeSlider;
    void SetRange(LureRangePivot pivot,float percent){
        switch(pivot){
            case LureRangePivot.TOP:
                rangeSlider.value=0.0f;
                break;
            case LureRangePivot.MID:
                rangeSlider.value=0.5f;
                break;
            case LureRangePivot.BTM:
                rangeSlider.value=1.0f;
                break;
        }
        rangeSlider.barSize=percent;
    }

    //1 購入ボタン 2 アドオン 3 装備
    public void SetMode(BottomMenu menu){


        switch(menu){
            case BottomMenu.Buy:
                PSGameUtils.ActiveNGUIObject(bottomBuyBtns,true);
                PSGameUtils.ActiveNGUIObject(bottomAddons,false);
                PSGameUtils.ActiveNGUIObject(bottomEquipped,false);
            PSGameUtils.ActiveNGUIObject(bottomEquipBtn,false);
                break;
            case BottomMenu.AddOn:
                PSGameUtils.ActiveNGUIObject(bottomBuyBtns,false);
                PSGameUtils.ActiveNGUIObject(bottomAddons,true);
                PSGameUtils.ActiveNGUIObject(bottomEquipped,false);
            PSGameUtils.ActiveNGUIObject(bottomEquipBtn,false);
                break;
            case BottomMenu.Equip:
                PSGameUtils.ActiveNGUIObject(bottomBuyBtns,false);
                PSGameUtils.ActiveNGUIObject(bottomAddons,false);
                PSGameUtils.ActiveNGUIObject(bottomEquipped,true);
            PSGameUtils.ActiveNGUIObject(bottomEquipBtn,false);
                break;
            case BottomMenu.Btn:
                PSGameUtils.ActiveNGUIObject(bottomBuyBtns,false);
                PSGameUtils.ActiveNGUIObject(bottomAddons,false);
            PSGameUtils.ActiveNGUIObject(bottomEquipped,false);
            PSGameUtils.ActiveNGUIObject(bottomEquipBtn,true);
                break;

        }
    }

    public GameObject bottomEquipBtn;
    public GameObject bottomBuyBtns;
    public GameObject bottomAddons;
    public GameObject bottomEquipped;

    public void SetEquipped(string spriteName_Hard,string tittle_Hard,string spriteName_Soft,string tittle_Soft,string spriteName_Line,string tittle_Line){
        SetMode(BottomMenu.Equip);
        bottomEquipped.GetComponent<EquippMenu>().SetEquipped(spriteName_Hard,
            tittle_Hard,spriteName_Soft,tittle_Soft,spriteName_Line,tittle_Line);
    }

    public void SetEquipped(string spriteName_Hard,string tittle_Hard,string spriteName_Soft,string spriteName_RigSoft,Vector3 rigpos,Vector3 rigrot,string tittle_Soft,bool shoulShowBack,string spriteName_Line,string tittle_Line){
        SetMode(BottomMenu.Equip);
        bottomEquipped.GetComponent<EquippMenu>().SetEquipped(spriteName_Hard,
            tittle_Hard,spriteName_Soft,spriteName_RigSoft,rigpos,rigrot,tittle_Soft,shoulShowBack,spriteName_Line,tittle_Line);
    }

    public void SetAishou(float hard,float soft,float line){
        bottomEquipped.GetComponent<EquippMenu>().SetAishouHard(hard);
        bottomEquipped.GetComponent<EquippMenu>().SetAishouSoft(soft);
        bottomEquipped.GetComponent<EquippMenu>().SetAishouLine(line);

    }

}
