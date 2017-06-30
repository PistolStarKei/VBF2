using UnityEngine;
using System.Collections;

public class YesNoPopup : MonoBehaviour {

    public UILabel tittle;
    public UILabel desc;
    public void ShowYesNo(string tittle,string desc,string yes,string no){
        NGUITools.SetActive(gameObject,true);
        this.tittle.text=tittle;
        this.desc.text=desc;
        this.tittle.enabled=false;
        this.desc.enabled=false;
        WaitAndCover.Instance.CoverAll(true);
        gameObject.GetComponent<TweenScale>().PlayForward();
        gameObject.GetComponent<TweenScale>().duration=0.3f;
        gameObject.GetComponent<TweenScale>().from=new Vector3(0.2f,1.0f,1.0f);
        this.yes=yes;
        this.no=no;
    }
    public void HideYesNo(){
        this.tittle.enabled=false;
        this.desc.enabled=false;
        gameObject.GetComponent<TweenScale>().from=new Vector3(1.0f,0.0f,1.0f);
        gameObject.GetComponent<TweenScale>().duration=0.1f;
        gameObject.GetComponent<TweenScale>().PlayReverse();
    }
    public string yes;
    public string no;
    public void OnYesNoShowed(){
        if(gameObject.GetComponent<TweenScale>().direction==AnimationOrTween.Direction.Forward){
            this.tittle.enabled=true;
            this.desc.enabled=true;
            btns.ShowYesNo(yes,no);
            AudioManager.Instance.ShowWindow();
        }else{
            
            WaitAndCover.Instance.UnCoverAllTween();
            NGUITools.SetActive(gameObject,false);
        }
    }

   
    public YesNoBtn btns;


}
