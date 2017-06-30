using UnityEngine;
using System.Collections;

public class PGUI_Wait : MonoBehaviour {

	bool isInvoking=false;

	public void ShowWait(float time,string msg){
		if(isInvoking)return;
		isInvoking=true;
		this.msg=msg;
		waitLabel.text=msg;
		labelString=msg;
		closeAt=time;
		NGUITools.SetActiveSelf(gameObject,true);
		num=0;
		isOn=false;
		elapsedTime=0.0f;
		NGUITools.SetActiveSelf(faderTween.gameObject,true);
		faderTween.PlayForward();
	}
	public UISprite fader;
	public TweenAlpha faderTween;
	public UISprite waiter;
	public UILabel waitLabel;
	public int dotsNum=7;
	public float dotsInterbal=0.5f;
	int num=0;
	float elapsedTime=0.0f;
	bool isOn=false;
	string labelString="LOADING";
	string msg="LOADING";
	void DoLabel(){
		if(num>dotsNum){
			labelString=msg;
			num=0;
		}
		num++;
		labelString+=".";
		waitLabel.text=labelString;
	}
	bool invoked=false;
	
	public void OnFaded(){
		if(faderTween.direction==AnimationOrTween.Direction.Forward){
			
					NGUITools.SetActive(waiter.gameObject,true);
					NGUITools.SetActive(waitLabel.gameObject,true);
					isOn=true;
					StartCoroutine(StartWait());

		}else{

			isOn=false;

			NGUITools.SetActive(gameObject,false);
			NGUITools.SetActiveChildren(gameObject,false);
			isInvoking=false;
		}

	}
	public float closeAt=3.0f;
	public void StopWait(){
		isOn=false;
		faderTween.PlayReverse();
	}
	IEnumerator StartWait(){
		while(isOn){

			if(closeAt>0.0f){
				if(elapsedTime>closeAt){
					isOn=false;

					faderTween.PlayReverse();
					yield break;
				}
			}
			DoLabel();
			elapsedTime+=dotsInterbal;
			yield return StartCoroutine(PGUI_Utilities.WaitForRealTime(dotsInterbal));
		}

	}

	// Use this for initialization
	void Start () {
		EventDelegate.Add(faderTween.onFinished, OnFaded);
	}

}
