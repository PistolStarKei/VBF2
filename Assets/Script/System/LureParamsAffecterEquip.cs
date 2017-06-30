using UnityEngine;
using System.Collections;

public class LureParamsAffecterEquip : MonoBehaviour {

    public PS_Toggle mainRodToggle;
    public UILabel tittleLb;
    public UISprite lureSp;
    public UISprite AdonSp;
    public UILabel markSp;

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

    public UILabel notbuyedLbl;
    public BotomSetter guiSetter;

    public void NotBuyedORNoLure(bool isShow){
        Debug.Log("Show N/A");
        if(isShow){
           

            notbuyedLbl.text=Localization.Get("LureNotAvaillable");
        }else{
            notbuyedLbl.text="";
        }
       
       
       
    }


    //アドオンの設定
    public void SetAdonItems(int num,string tittle,string desc,string price,string itemSp,bool isBuyed,bool isEquipped){
      
        adons.SetAdonItems(num,tittle,desc,price,itemSp,isBuyed,isEquipped);
    }
    public void SetAdonItemsToUA(int num){
        adons.SetAdonItemsToUA(num);
    }
    public SoftAddOns_Equip adons;



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



    public EquippMenu_Equip equipMenu;

    public void SetEquipped(string spriteName_Hard,string tittle_Hard,string spriteName_Soft,string tittle_Soft,string spriteName_Line,string tittle_Line){
        
        equipMenu.SetEquipped(spriteName_Hard,
            tittle_Hard,spriteName_Soft,tittle_Soft,spriteName_Line,tittle_Line);
    }

    public void SetEquipped(string spriteName_Hard,string tittle_Hard,string spriteName_Soft,string spriteName_RigSoft,Vector3 rigpos,Vector3 rigrot,string tittle_Soft,bool shoulShowBack,string spriteName_Line,string tittle_Line){
        
        equipMenu.SetEquipped(spriteName_Hard,
            tittle_Hard,spriteName_Soft,spriteName_RigSoft,rigpos,rigrot,tittle_Soft,shoulShowBack,spriteName_Line,tittle_Line);
    }

    public void SetAishou(float hard,float soft,float line){
        equipMenu.SetAishouHard(hard);
        equipMenu.SetAishouSoft(soft);
        equipMenu.SetAishouLine(line);

    }

}
