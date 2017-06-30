using UnityEngine;
using System.Collections;

public class BuyButtons : MonoBehaviour {

    public UILabel lbl;
    public UILabel lbl2;
    public GameObject objTop;
    public GameObject objBtm;

    //1 line
    public void SetBtns(string text,string btnText,bool showBtn){
        lbl.text=text;
        lbl2.text="";
        btnLbl.text=btnText;
        btnLblBtm.text="";
        ShowTopBtn(showBtn);
        ShowBottomBtn(false);
        ShowTop(true);
        ShowBtm(false);
    }
    //2 line
    public void SetBtns(string text,string btnText,bool showBtn,string text2,string btnText2,bool showBtn2){
        lbl.text=text;
        lbl2.text=text2;
        btnLbl.text=btnText;
        btnLblBtm.text=btnText2;
        ShowTopBtn(showBtn);
        ShowBottomBtn(showBtn2);
        ShowTop(true);
        ShowBtm(true);
    }

    void ShowTop(bool isShowBtn){
        if(objTop.activeSelf!=isShowBtn)NGUITools.SetActive(objTop,isShowBtn);
    }
    void ShowBtm(bool isShowBtn){
        if(objBtm.activeSelf!=isShowBtn)NGUITools.SetActive(objBtm,isShowBtn);
    } 
    public GameObject btnTop;
    public UILabel btnLbl;
    void ShowTopBtn(bool isShowBtn){
        if(btnTop.activeSelf!=isShowBtn)NGUITools.SetActive(btnTop,isShowBtn);
        if( isShowBtn){
            lbl.pivot=UIWidget.Pivot.Left;
            lbl.transform.localPosition=new Vector3(94.0f,0.0f,0.0f);
        }else{
            lbl.pivot=UIWidget.Pivot.Center;
            lbl.transform.localPosition=new Vector3(586.97f,0.0f,0.0f);
        }
    }
    public GameObject btnBtm;
    public UILabel btnLblBtm;
    void ShowBottomBtn(bool isShowBtn){
        Debug.Log("ShowBottomBtn"+isShowBtn);
        if(btnBtm.activeSelf!=isShowBtn)NGUITools.SetActive(btnBtm,isShowBtn);
        if( isShowBtn){
            lbl2.pivot=UIWidget.Pivot.Left;
            lbl2.transform.localPosition=new Vector3(94.0f,0.0f,0.0f);
        }else{
            lbl2.pivot=UIWidget.Pivot.Center;
            lbl2.transform.localPosition=new Vector3(586.97f,0.0f,0.0f);
        }
    }

}
