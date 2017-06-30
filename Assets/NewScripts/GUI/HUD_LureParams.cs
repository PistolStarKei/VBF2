using UnityEngine;
using System.Collections;

public class HUD_LureParams : PS_SingletonBehaviour<HUD_LureParams> {

    // Use this for initialization
    void Start () {

        Localization.language="Japanese";
        if(Localization.language=="Japanese"){
            tani_kyori="m";
            tani_Range="m";
            rangeMulti=1.0f;
        }else{
            tani_kyori="yd.";
            tani_Range="ft.";
            rangeMulti=3.3f;
        }

        SetDistanceFromPlayer(0.12345f);
        SetRangeInWater(0.12345f);
    }

    public UILabel label_Kyori;
    public UILabel lable_Range;
    float rangeMulti=0.0f;
    string tani_kyori="m";
    string tani_Range="m";
    public void SetDistanceFromPlayer(float val){
        label_Kyori.text="Dist "+val.ToString("N2")+tani_kyori;
    }
     void SetRangeInWater(float val){
        lable_Range.text="Depth "+(val*rangeMulti).ToString("N2")+tani_Range;
    }

    public void UpdateLineGUI(float distance,float range){
        if(label_Kyori.gameObject.activeSelf)  SetDistanceFromPlayer(distance);
        if(lable_Range.gameObject.activeSelf) SetRangeInWater(range);
    }

    public void SetRangeAndDistToZero(){
        if(label_Kyori.gameObject.activeSelf) SetDistanceFromPlayer(0.0f);
        if(lable_Range.gameObject.activeSelf)SetRangeInWater(0.0f);
    }

    public void Show(){
            //投げた瞬間に呼べ、見せる。フォローする 見せるのは距離
            if(Constants.Params.isShowDepth)NGUITools.SetActive(label_Kyori.gameObject,true);
            if(Constants.Params.isShowDepth)NGUITools.SetActive(lable_Range.gameObject,true);
            
    }

    public void Hide(){
            //投げた瞬間に呼べ、見せる。フォローする 見せるのは距離
            NGUITools.SetActive(label_Kyori.gameObject,false);
            if(Constants.Params.isShowDepth)NGUITools.SetActive(lable_Range.gameObject,false);
   
    }


    public bool isEnableFolow=false;
    public Camera camera;
    Vector3 UIPos ;
    Vector3 PrevUIPos ;
    // Update is called once per frame
    public float dumping=0.5f;
    void Update () {

        if(isEnableFolow && LureController.Instance.lureOBJ!=null){
            
            UIPos = camera.WorldToScreenPoint(LureController.Instance.lureOBJ.transform.position);
            UIPos.z = 1.0f;
            UIPos= UICamera.mainCamera.ScreenToWorldPoint(UIPos);
            if(Mathf.Abs(PrevUIPos.y-UIPos.y)>dumping){
                transform.position =  UIPos;
            }
            //文字の座標をスクリーン座標からOrthographicカメラのワールド座標に反映
            PrevUIPos=UIPos;
        }
    }
   

}
