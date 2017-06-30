using UnityEngine;
using System.Collections;

public class DFStyle : MonoBehaviour {



	UILabel label;
	void Start(){

		label=gameObject.GetComponent<UILabel>();
		SetByPixelSize(Screen.height/800.0f,label);
	}
	public int minimumSize=10;
	public int maximumSize=100;
	void SetByPixelSize(float bai ,UILabel label){
		if(label.overflowMethod==UILabel.Overflow.ShrinkContent){
			label.keepCrispWhenShrunk=UILabel.Crispness.Always;
			minimumSize=label.fontSize;
		}
		int prevSize=label.fontSize;
		if(Mathf.FloorToInt(label.fontSize*bai)<=minimumSize){
			bai=(float)minimumSize/(float)label.fontSize;
		}else if(Mathf.FloorToInt(label.fontSize*bai)>=maximumSize){
			bai=(float)maximumSize/(float)label.fontSize;
		}
			label.fontSize=Mathf.FloorToInt(label.fontSize*bai);
		if(label.overflowMethod!=UILabel.Overflow.ShrinkContent)gameObject.transform.localScale=new Vector3(gameObject.transform.localScale.x/bai,gameObject.transform.localScale.y/bai,1.0f);
		label.MarkAsChanged();
	}

}
