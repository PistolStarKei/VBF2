using UnityEngine;
using System.Collections;

public class EquippMenu : MonoBehaviour {

    public void SetEquipped(string spriteName_Hard,string tittle_Hard,string spriteName_Soft,string spriteName_RigSoft,Vector3 rigpos,Vector3 rigrot,string tittle_Soft,bool shoulShowBack,string spriteName_Line,string tittle_Line){
        SetHard(spriteName_Hard,tittle_Hard);
        SetSoft(spriteName_Soft,spriteName_RigSoft,rigpos,rigrot,tittle_Soft,shoulShowBack);
        SetLine(spriteName_Line,tittle_Line);
    }
    public void SetEquipped(string spriteName_Hard,string tittle_Hard,string spriteName_Soft,string tittle_Soft,string spriteName_Line,string tittle_Line){
        SetHard(spriteName_Hard,tittle_Hard);
        SetSoft("","",Vector3.zero,Vector3.zero,tittle_Soft,true);
        SetLine(spriteName_Line,tittle_Line);
    }

    void SetHard(string spriteName,string tittle){
        
        if(spriteName==""){
            lureHard.spriteName=spriteName;
            lureHardLb.text=tittle;
            lureHardBG_LOGO.enabled=true;
        }else{
            lureHard.spriteName=spriteName;
            lureHardLb.text=tittle;
            lureHardBG_LOGO.enabled=false;
        }
        SetBG(lureHardBG,spriteName==""?false:true);
    }

    void SetSoft(string spriteName,string spriteName_RigSoft,Vector3 rigpos,Vector3 rigrot,string tittle,bool shoulShowBack){
        
        if(spriteName==""){
            lureSoft.spriteName=spriteName;
            lureSoftLb.text=tittle;
            lureSoftRig.spriteName=spriteName;
            lureSoftBG_LOGO.enabled=true;

        }else{
            if(shoulShowBack){
                lureSoftRig.depth=lureSoft.depth-1;
            }else{
                lureSoftRig.depth=lureSoft.depth+1;
            }
            lureSoft.spriteName=spriteName;
            lureSoftLb.text=tittle;
            lureSoftRig.spriteName=spriteName_RigSoft;
            lureSoftRig.transform.localPosition=rigpos;
            lureSoftRig.transform.localRotation=Quaternion.Euler(rigrot);
            lureSoftBG_LOGO.enabled=false;
        }
        SetBG(lureSoftBG,spriteName==""?false:true);


    }

    void SetLine(string spriteName,string tittle){
        if(spriteName==""){
            line.spriteName=spriteName;
            lineLb.text=tittle;
            lineBG_LOGO.enabled=true;
        }else{
            line.spriteName=spriteName;
            lineLb.text=tittle;
            lineBG_LOGO.enabled=false;

        }
        SetBG(lineBG,spriteName==""?false:true);
    }
    public Color setted;
    public Color unsetted;
    void SetBG(UISprite sp,bool isSetted){
        if(isSetted){
            sp.color=setted;
        }else{
            sp.color=unsetted;
        }
    }

    public UISprite lureHard;
    public UISprite lureSoft;
    public UISprite lureSoftRig;
    public UISprite line;

    public UILabel lureHardLb;
    public UILabel lureSoftLb;
    public UILabel lineLb;

    public UISprite lureHardBG;
    public UISprite lureSoftBG;
    public UISprite lineBG;

    public UISprite lureHardBG_LOGO;
    public UISprite lureSoftBG_LOGO;
    public UISprite lineBG_LOGO;

	


    public LureShop_Menu_Rods rods;
    public void OnTapHard(){
        rods.OnEquipHard();
    }
    public void OnTapSoft(){
        rods.OnEquipSoft();
    }
    public void OnTapLine(){
        rods.OnEquipLine();
    }


    public UILabel aishouHard;
    public UILabel aishouSoft;
    public UILabel aishouLine;
   
    public void SetAishouHard(float num){
        //Debug.Log("SetAishouCurrent"+num);
        SetAishouLbl(aishouHard, num);
    }
    public void SetAishouSoft(float num){

        SetAishouLbl(aishouSoft, num);
    }
    public void SetAishouLine(float num){

        SetAishouLbl(aishouLine, num);
    }

    public void SetAishouLbl(UILabel lbl, float aishou){

        if(aishou<-10.0f){
            lbl.text="--";
            lbl.color=Color.white;
        }else{
            if(aishou==0.0f){
                lbl.text=Localization.Get("MatchHeavy");
                lbl.color=GUIColors.Instance.MatchColor_Plus;
            }else if(aishou<0.0f){
                lbl.color=GUIColors.Instance.MatchColor_Minus;
                lbl.text=PSGameUtils.GetPercent(aishou);
            }else{
                lbl.color=GUIColors.Instance.MatchColor_Plus;
                lbl.text=PSGameUtils.GetPercent(aishou);
            }

        }

    }
}
