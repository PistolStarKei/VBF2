using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScrollLabel : MonoBehaviour {
	
	public GameObject scrollLabel;
	UILabel uiLabel;
	bool current_onshot=false;
	string current_key="";
	string default_key="";
	bool isEnable=false;
	public float initY=182.1f;
	public float initX=446.6f;
	public float speed=0.02f;
	List<string> stockKeys= new List<string>();
	List<string> soundKeys= new List<string>();
	void Start(){
		uiLabel=scrollLabel.GetComponent<UILabel> ();
	}
	
	// public method ,"nekoNormal"=normaldesc "success"=success "neko"=alart
	public void ScrollEternally(string message,string sound){
		//StockMessage(message,false,sound);
		default_key=message;
		
		if (!isEnable){
			//not scroll
			if(sound!="")AudioController.Play (sound);
			current_key=message;
			current_onshot=false;
			isEnable=true;
			StartScroll(message);
			
		}else{
			//scrolling	
			if(current_onshot){
				
			}else{
				isCancel=true;
			}
		}
		
		
	}
	public void ScrollOneshot(string message,string sound){

		StockMessage(message,sound);
		isCancel = true;
		if (!isEnable){
			current_key=message;
			current_onshot=true;
			isEnable=true;
			StartScroll(message);
		}
		
	}
	
	
	public void HideScroll(){
		
		isEnable=false;
		current_key="";
		uiLabel.text = current_key;
		stockKeys.Clear();
		soundKeys.Clear ();
		scrollLabel.transform.localPosition=new Vector3(initX,initY,0.0f);
		
	}
	
	
	
	void StockMessage(string key,string sound){
		stockKeys.Add(key);
		soundKeys.Add (sound);
		
	}
	void RemoveStock(){
		if(stockKeys.Count>0)stockKeys.RemoveAt(0);
		if (soundKeys.Count > 0)soundKeys.RemoveAt (0);
		
	}
	string soundKey="neko";
	void SetNextStock(){
		//remove current
		
		
		if(stockKeys.Count>0){
			//has oneshot
			
			if(stockKeys.Count>0){
				current_key=stockKeys[0];
				soundKey=soundKeys[0];
				current_onshot=true;
				if(soundKey!="")AudioController.Play (soundKey);
				RemoveStock();
				StartScroll(current_key);
			}
		}else{
			//back to default
			current_key=default_key;
			StartScroll(current_key);
			
		}
		
	}
	void StartScroll(string message){
		speed=0.09f;
		time=0.0f;
		uiLabel.pivot=UIWidget.Pivot.Left;
		scrollLabel.transform.localPosition=new Vector3(initX,initY,0.0f);
		if(message.Length>=7){
			if(message.Substring(0,7).Equals("Achieve")){
				uiLabel.text =Localization.Get("TextTassei")+ Localization.Get(message);
			}else{
				uiLabel.text = Localization.Get(message);
			}
		}else{
			uiLabel.text = Localization.Get(message);
		}
		
	}
	
	public Transform stopPoint;
	public Transform startPoint;
	bool isCancel=false;
	float time=0.0f;
	void Update () {
		if(isEnable){
			
			if(isCancel){
				isCancel=false;
				
				SetNextStock();
			}
			
			if(scrollLabel.transform.localPosition.x<startPoint.localPosition.x){
				if(time==0.0f)time=Time.realtimeSinceStartup;
				if(speed>=0.02f){
					if(Time.realtimeSinceStartup-time>1.0f){
						speed=0.005f;
						
						if(uiLabel.pivot!=UIWidget.Pivot.Right)uiLabel.pivot=UIWidget.Pivot.Right;
					}
					
				}else{
					scrollLabel.transform.Translate(Vector3.left*speed,Space.Self);
					
				}
				if(scrollLabel.transform.localPosition.x<stopPoint.localPosition.x){
					
					uiLabel.pivot=UIWidget.Pivot.Left;
					scrollLabel.transform.localPosition=new Vector3(initX,initY,0.0f);
					speed=0.09f;
					time=0.0f;
					if(current_onshot){
						SetNextStock();
					}else{
						//do nothing just scroll mugen
						if(stockKeys.Count>0){
							//has oneshotStock
							SetNextStock();
						}else{
							
						}
					}
				}
			}else{
				
				scrollLabel.transform.Translate(Vector3.left*speed,Space.Self);
				
			}
		}
	}

	
	
}
