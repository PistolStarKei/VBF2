using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserName : PS_SingletonBehaviour<UserName> {

    public void SetPosition(bool isLeft){
        if(isLeft){
            transform.localPosition=new Vector3(-825.85f,-36.91f,0.0f);
        }else{
            transform.localPosition=new Vector3(326.3f,-36.91f,0.0f);
        }
    }


    public UILabel goldLb;
    public UILabel dolLb;

    public void UpdateCurrencyNums(){
        
        goldLb.text=DataManger.Instance.GetFormattedLong(DataManger.Instance.GAMEDATA.Gold);
        dolLb.text=DataManger.Instance.GetFormattedLong(DataManger.Instance.GAMEDATA.Doller);
    }
    public void SetNames(string name){
        lbl.text=name;

       if(DataManger.Instance.GAMEDATA.Countly==""){
            DetectUserLocation();
        }
        string result;
        /*if (COUTRY_NAMES.TryGetValue (DataManger.Instance.GAMEDATA.Countly, out result)) {
            lbl_cc.text=result;
        } else {
            lbl_cc.text=COUTRY_NAMES["gv"];
        }*/

        sp.spriteName=DataManger.Instance.GAMEDATA.Countly+"_mini";
        DataManger.Instance.SaveData();

    }

    public UILabel lbl;
   // public UILabel lbl_cc;
    public UISprite sp;
    void DetectUserLocation(){
        string str= ToCountryCode(Application.systemLanguage);
        ES2.Save(str,  DataManger.DataFilename+"?tag=Countly");
    }


    public static string ToCountryCode(SystemLanguage language) {
        string result;
        if (COUTRY_CODES.TryGetValue (language, out result)) {
            return result;
        } else {
            return COUTRY_CODES[SystemLanguage.Unknown];
        }
    }

    private static readonly Dictionary<SystemLanguage, string>  COUTRY_CODES = new Dictionary<SystemLanguage, string>
    {
        { SystemLanguage.Belarusian, "be"},
        { SystemLanguage.Bulgarian, "bg"},
        { SystemLanguage.Catalan, "ca"},
        { SystemLanguage.Chinese, "cn"},
        { SystemLanguage.Czech, "cz"},
        { SystemLanguage.Dutch, "nl"},
        { SystemLanguage.English, "uk"},
        { SystemLanguage.Estonian, "et"},
        { SystemLanguage.Finnish, "fi"},
        { SystemLanguage.French, "fr"},
        { SystemLanguage.German, "de"},
        { SystemLanguage.Greek, "gr"},
        { SystemLanguage.Hungarian, "hu"},
        { SystemLanguage.Icelandic, "is"},
        { SystemLanguage.Indonesian, "id"},
        { SystemLanguage.Italian, "it"},
        { SystemLanguage.Japanese, "jp"},
        { SystemLanguage.Korean, "kr"},
        { SystemLanguage.Latvian, "lv"},
        { SystemLanguage.Lithuanian, "lt"},
        { SystemLanguage.Norwegian, "no"},
        { SystemLanguage.Polish, "pl"},
        { SystemLanguage.Portuguese, "pt"},
        { SystemLanguage.Romanian, "ro"},
        { SystemLanguage.Russian, "ru"},
        { SystemLanguage.Slovak, "sk"},
        { SystemLanguage.Slovenian, "sl"},
        { SystemLanguage.Spanish, "es"},
        { SystemLanguage.Swedish, "sv"},
        { SystemLanguage.Thai, "th"},
        { SystemLanguage.Turkish, "tr"},
        { SystemLanguage.Vietnamese, "vi"},
        { SystemLanguage.Unknown, "gv" }
    };

    private static readonly Dictionary<string, string>  COUTRY_NAMES = new Dictionary<string, string>
    {
        { "be","BEL"},
        { "bg","BUL"},
        { "ca","CAT"},
        { "cn","CHN"},
        { "cz","CZE"},
        { "dk","NLD"},
        { "uk","ENG"},
        { "et","EST"},
        { "fi","FIN"},
        { "fr","FRA"},
        { "de","DEU"},
        { "gr","GRC"},
        { "hu","HUN"},
        { "is","ISL"},
        { "id","IDN"},
        { "it","ITA"},
        { "jp","JPN"},
        { "kr","KOR"},
        { "lv","LVA"},
        { "lt","LTU"},
        { "no","NOR"},
        { "pl","POL"},
        { "pt","PRT"},
        { "ro","RMN"},
        { "ru","RUS"},
        { "sk","SVK"},
        { "sl","SVN"},
        { "es","ESP"},
        { "sv","SWE"},
        { "th","THA"},
        { "tr","TUR"},
        { "vi","VNM"},
        { "gv", "GLB" }
    };
}
