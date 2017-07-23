using UnityEngine;
using System.Collections;

public class BoatBtn : MonoBehaviour {

	public void OnClicks(){
		if(isOn){
			GameController.Instance.OnBoat(isBoat?true:false);
			SetState(isBoat?false:true);

		}

	}
	public BoxCollider col;
	public bool isBoat=true;
	bool isChange=false;
	public UIButtonColor color;
	public Color[] cols;
	public void SetState(bool isBoat){

		if(!isInited){
			isInited=true;
			this.isBoat=isBoat;
			if(isBoat){
				yeslbl.text="Boat";
				color.defaultColor=cols[1];
				color.pressed=cols[0];
			}else{
				yeslbl.text="Fishing";
				color.defaultColor=cols[0];
				color.pressed=cols[1];

			}
			isOn=false;
			NGUITools.SetActive(yeslbl.gameObject,false);
			NGUITools.SetActive(gameObject,true);
			gameObject.GetComponent<TweenPosition>().PlayForward();

			return;
		}
		if(isInvoing) return;
		isInvoing=true;
		NGUITools.SetActive(yeslbl.gameObject,false);
		NGUITools.SetActive(gameObject,true);
		if(isBoat){
			Debug.Log("モードボタン　SetState"+ "ボートへ");
		}else{
			Debug.Log("モードボタン　SetState"+ "釣りへ");
		}

		this.isBoat=isBoat;
		if(isBoat){
			yeslbl.text="Boat";
			color.defaultColor=cols[1];
			color.pressed=cols[0];
		}else{
			yeslbl.text="Fishing";
			color.defaultColor=cols[0];
			color.pressed=cols[1];

		}

		isChange=true;
		col.enabled=false;
		isOn=false;
		if(yeslbl.text!=""){
			yeslbl.gameObject.gameObject.GetComponent<TweenScale>().PlayReverse();
		}
		gameObject.GetComponent<TweenPosition>().PlayReverse();
	}

	public bool isOn=false;
	public bool isInited=false;
	public UILabel yeslbl;



	public void Hide(){
		if(isInvoing)return;

		if(!isOn)return;
		isChange=false;
		col.enabled=false;
		Debug.Log("Show cast Hide()");

		isOn=false;
		if(yeslbl.text!=""){
			yeslbl.gameObject.gameObject.GetComponent<TweenScale>().PlayReverse();
		}
		gameObject.GetComponent<TweenPosition>().PlayReverse();
	}
	public bool isInvoing=false;
	public void OnShowed(){
		if(gameObject.GetComponent<TweenPosition>().direction==AnimationOrTween.Direction.Forward){
			Debug.Log("Show cast OnShowed　見せた");
			isOn=true;
			col.enabled=true;
			isInited=true;
			isInvoing=false;
			if(yeslbl.text!=""){
				NGUITools.SetActive(yeslbl.gameObject,true);
				yeslbl.gameObject.gameObject.GetComponent<TweenScale>().PlayForward();
			}

		}else{
			Debug.Log("Show cast OnShowed　見せてない"+isChange);
			if(isChange){
				isChange=false;
				gameObject.GetComponent<TweenPosition>().PlayForward();
			}else{
				isInvoing=false;
				NGUITools.SetActive(gameObject,false);
			}
		}
	}
}
