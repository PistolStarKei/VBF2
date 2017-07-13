using UnityEngine;
using System.Collections;

public class SoubiBtn : MonoBehaviour {


	public void SetEnabled(bool isEnable){
		isEnableBtn=isEnable;
		isSetEnable=true;
        Debug.LogError("SetEnable "+isEnable);
        this.col.enabled=isEnable;
        this.color.SetState(isEnable?UIButtonColor.State.Normal:  UIButtonColor.State.Disabled,true);
        yeslbl.color=isEnable?Color.white:Color.grey;
    }
    public EquipMenu_Rod rod;
    public void OnClicks(){
        if(isOn){
            rod.OnEquip();
        }

    }
    public BoxCollider col;

    public UIButtonColor color;
    public void Show(string text){
        Debug.LogError("Show  "+text+" "+isOn+" "+isInvoing);
        if(this.isInvoing || isOn) return;
        isInvoing=true;
        NGUITools.SetActive(gameObject,true);

        yeslbl.text=text;
        col.enabled=false;
        isOn=false;
        Debug.Log("Show"+text);
        gameObject.GetComponent<TweenPosition>().PlayForward();
    }

    public bool isOn=false;
    public UILabel yeslbl;


	bool isEnableBtn=false;
	bool isSetEnable=false;
    public void Hide(){
        if(isInvoing)return;
        isInvoing=true;

        if(!isOn)return;
        col.enabled=false;
        Debug.Log("Show cast Hide()");

        isOn=false;

        if(yeslbl.text!=""){
			yeslbl.gameObject.gameObject.GetComponent<TweenScale>().PlayReverse();
        }
        gameObject.GetComponent<TweenPosition>().PlayReverse();
    }
     bool isInvoing=false;
    public void OnShowed(){
        if(gameObject.GetComponent<TweenPosition>().direction==AnimationOrTween.Direction.Forward){
            Debug.Log("Show cast OnShowed　見せた");
            isOn=true;
            
            isInvoing=false;


			if(isSetEnable){
				this.color.SetState(isEnableBtn?UIButtonColor.State.Normal:  UIButtonColor.State.Disabled,true);
				isSetEnable=false;

			}else{
				col.enabled=true;

			}
			if(yeslbl.text!="")yeslbl.gameObject.gameObject.GetComponent<TweenScale>().PlayForward();
        }else{
            isInvoing=false;
            NGUITools.SetActive(gameObject,false);
        }
    }
}
