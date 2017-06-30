using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RainbowLabel : MonoBehaviour {


	public List<Color> cols=new List<Color>();
	int num=0;
	Color col;
	public void SetToColor(Color col){
		if(label==null)GetLabel();
		label.color=col;
	}
	UILabel label;
	void GetLabel(){
		if(cols.Count<=0){
			cols.Add(new Color(255.0f/255.0f,96.0f/255.0f,152.0f/255.0f));
			cols.Add(new Color(252.0f/255.0f,69.0f/255.0f,255.0f/255.0f));
			cols.Add(new Color(132.0f/255.0f,73.0f/255.0f,255.0f/255.0f));
			cols.Add(new Color(92.0f/255.0f,113.0f/255.0f,255.0f/255.0f));
			cols.Add(new Color(96.0f/255.0f,209.0f/255.0f,255.0f/255.0f));
			cols.Add(new Color(79.0f/255.0f,255.0f/255.0f,109.0f/255.0f));
			cols.Add(new Color(219.0f/255.0f,255.0f/255.0f,92.0f/255.0f));
			cols.Add(new Color(255.0f/255.0f,146.0f/255.0f,86.0f/255.0f));
		}
		label=gameObject.GetComponent<UILabel>();
	}
	void Start(){
		

		if(label==null)GetLabel();
		col=label.color;
	}
	void OnDisable(){
		SetToDefault();
	}
    public void Invoke(){
        if(label==null)GetLabel();
        num=0;
        if(isGradient){
            label.gradientTop=cols[num];
            label.gradientBottom=cols[num+1];
        }else{
            label.color=cols[num];
        }
        InvokeRepeating("NextColot",0.0f,repeatRate);
        label.applyGradient=true;
    }
	public float repeatRate=0.1f;
	public void Destroy(){
		Destroy(this);
	}
	public void SetToDefaultColor(){
		if(label==null)GetLabel();
        label.applyGradient=false;
		label.color=col;
	}
	void SetToDefault(){
		if(label==null)GetLabel();
		label.color=col;
		CancelInvoke("NextColot");
	}
	public bool isGradient=true;
	public void NextColot(){
		if(label==null)GetLabel();
		if(!enabled){
			
			return;
		}
		num=num>=cols.Count-1? 0:++num;
		if(isGradient){
			label.gradientTop=cols[num];
			label.gradientBottom=cols[num+1>=cols.Count-1? 0:num+1];
		}else{
			label.color=cols[num];
		}
	}
}
