using UnityEngine;
using System.Collections;

public class YesNoBtn : MonoBehaviour {

    public void OnYes(){
       HideYesNo();
        WaitAndCover.Instance.OnYes();
    }
    public void OnNo(){
        HideYesNo();
        WaitAndCover.Instance.OnNo();
    }

    public UILabel yeslbl;
    public UILabel nolbl;
    public void ShowYesNo(string yes,string no){
        yeslbl.text=yes;  nolbl.text=no;
        NGUITools.SetActive(yeslbl.gameObject,false);
        NGUITools.SetActive(nolbl.gameObject,false);
        NGUITools.SetActive(gameObject,true);
        gameObject.GetComponent<TweenPosition>().PlayForward();
    }
    public void ShowYesNo(){
        yeslbl.text=""; nolbl.text="";
        NGUITools.SetActive(yeslbl.gameObject,false);
        NGUITools.SetActive(nolbl.gameObject,false);
        NGUITools.SetActive(gameObject,true);
        gameObject.GetComponent<TweenPosition>().PlayForward();
    }
    public void HideYesNo(){
        if(yeslbl.text!=""){
            yeslbl.gameObject.gameObject.GetComponent<TweenScale>().PlayReverse();
        }

        if(nolbl.text!=""){
            nolbl.gameObject.gameObject.GetComponent<TweenScale>().PlayReverse();
        }

        gameObject.GetComponent<TweenPosition>().PlayReverse();
    }
    public void OnYesNoShowed(){
        if(gameObject.GetComponent<TweenPosition>().direction==AnimationOrTween.Direction.Forward){


            if(yeslbl.text!=""){
                NGUITools.SetActive(yeslbl.gameObject,true);
                yeslbl.gameObject.gameObject.GetComponent<TweenScale>().PlayForward();
            }

            if(nolbl.text!=""){
                NGUITools.SetActive(nolbl.gameObject,true);
                nolbl.gameObject.gameObject.GetComponent<TweenScale>().PlayForward();
            }


        }else{
            NGUITools.SetActive(gameObject,false);
        }
    }

}
