using UnityEngine;
using System.Collections;

public class LureBtn : MonoBehaviour {

    public delegate void Callback_onClick();
    public  event Callback_onClick onClick;

    public void OnClickedBtn(){
        if(onClick!=null)onClick();
    }
    public void SetBtnCallback(Callback_onClick onClick){
        this.onClick=null;
        this.onClick=onClick;
    }
    public UILabel descLabel;
    public UILabel priceLabel;
    public UILabel btnLabel;
    public GameObject btnObj;
    public void SetBtns(string desc,string price,string showBtn){
        descLabel.text=desc;
        priceLabel.text=price;
        NGUITools.SetActive(gameObject,true);

        if(showBtn!=""){
            //通常購入を表示する場合
            NGUITools.SetActive(btnObj,true);
            btnLabel.text=showBtn;
            descLabel.pivot=UIWidget.Pivot.Right;
            descLabel.gameObject.transform.localPosition=new Vector3(687.9f,0.0f,0.0f);
            priceLabel.gameObject.transform.localPosition=new Vector3(805.4f,0.0f,0.0f);
        }else{
            
            //アンロックを表示する場合
            NGUITools.SetActive(btnObj,false);
            priceLabel.gameObject.transform.localPosition=new Vector3(1004.0f,0.0f,0.0f);

            descLabel.pivot=UIWidget.Pivot.Left;
            descLabel.gameObject.transform.localPosition=new Vector3(90.0f,0.0f,0.0f);

        }
        if(price==""){
            NGUITools.SetActive(priceLabel.gameObject,false);
        }else{
            NGUITools.SetActive(priceLabel.gameObject,true);
        }
       

    }
    public void HideBtns(){
        //何も表示しない場合
        NGUITools.SetActive(gameObject,false);

       
    }

    public Vector3 center;
    public Vector3 defaultPos;
    public void SetCenter(){
        transform.localPosition=center;
    }
    public void SetDefault(){
        transform.localPosition=defaultPos;
    }
}
