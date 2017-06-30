using UnityEngine;
using System.Collections;

public class CastBtn : PS_SingletonBehaviour<CastBtn>  {


    public void ShowCastBtn(bool isShow){
        if(gameObject.GetComponent<Collider>().enabled==isShow){
            return;
        }
        gameObject.GetComponent<Collider>().enabled=isShow;
        Show(false);
    }
	void Show(bool isShow){
		ta.enabled=false;
		isPressed=false;
		isCasted=false;
        dragPoint.transform.localPosition=Vector3.zero;
        if(LureController.Instance==null){
            Debug.LogError("Rure is not setted show user to set the rure");
      
        }
		if(isShow){
            lbl.text="↓DRAG↓";

            guid.transform.localRotation=Quaternion.Euler(Vector3.zero);
            guid.fillAmount=0.4f;
            guid.height=204;
            guid.width=126;
            guid.alpha=1.0f;

            guidNavi.fillAmount= 0.0f;
            guidNavi.height= guid.height;
            guidNavi.width=guid.width;
            guidNavi.alpha=0.7f;
            if(!controlls.activeSelf)NGUITools.SetActiveSelf(controlls,isShow);

		}else{
            if(controlls.gameObject.activeSelf)NGUITools.SetActiveSelf(controlls,isShow);
            if(this.guid.gameObject.activeSelf) NGUITools.SetActive(guid.gameObject,false);
            if(this.guidNavi.gameObject.activeSelf) NGUITools.SetActive(guidNavi.gameObject,false);
		}
	}
    public GameObject controlls;

    public UILabel lbl;
	bool isPressed=false;
	bool isCasted=false;

	float castPower=0.0f;
	

	void OnPress(bool pressed)
	{

       
        if(LureController.Instance.lureOBJ==null || !LureController.Instance.isLureActive()){
            WaitAndCover.Instance.ShowInfoPopup(Localization.Get("WarningNoLure"),Info_IC.Warning);
            return;
        }
        Debug.Log("OnPress"+  pressed +" "+isPressed);
		if(!isCasted){
			if (pressed && !isPressed)
			{
                controlls.transform.position=UICamera.currentCamera.ScreenToWorldPoint(UICamera.lastEventPosition);
                if(!this.guid.gameObject.activeSelf) NGUITools.SetActive(guid.gameObject,true);
                if(!this.guidNavi.gameObject.activeSelf) NGUITools.SetActive(guidNavi.gameObject,true);
                Show(true);
                if(ta.enabled)ta.enabled=false;
                isCasted=false;
				isPressed=true;
				castPower=0.01f;
			}
			else
			{
                
                if(ta.enabled)ta.enabled=false;
                if(this.guid.gameObject.activeSelf) NGUITools.SetActive(guid.gameObject,false);
                if(this.guidNavi.gameObject.activeSelf) NGUITools.SetActive(guidNavi.gameObject,false);
                if(controlls.gameObject.activeSelf)NGUITools.SetActiveSelf(controlls,false);
                if(castPower<=0.2f){
                    
                    isPressed=false;
                    dragPoint.transform.localPosition=Vector3.zero;
                  
                }else{
                    isCasted=true;
                    isPressed=false;
                    dragPoint.transform.localPosition=Vector3.zero;
                    gameObject.GetComponent<Collider>().enabled=false;
                    OnCast(castPower-0.4f);
                    Show(false);
                }
               
			}
		}
	}
    private float castPow=0.0f;
    public void OnCast(float power){
        //0.1f-1.0f
        Debug.Log("Cast"+power);

        if(power<0.1f)power=0.1f;
        FishingStateManger.Instance.ChangeStateTo(GameMode.Throwing);

        castPow=power* (TackleParams.Instance.tParams.Cast_Range* ((4500.0f-700.0f)/(40.0f-3.0f)));

        //throw anime
        CastInvoke();
    }

    public void CastInvoke(){
        if(castPow<700.0f)castPow=700.0f;
        #if UNITY_EDITOR
        #endif
        float pow=1.0f-(0.09f*TackleParams.Instance.tParams.Cast_WindFactor);


        LureController.Instance.CastLure((Player.Instance.gameObject.transform.forward+(Vector3.up/2.0f))* castPow,pow*LakeEnvironmentalParamas.Instance.weather.WindDirection);
        ZoomCamera.Instance.SetZoomCamera();
    }

    public UISprite dragPoint;
    public UISprite guid;
    public UISprite guidNavi;
    float dist=0.0f;

    Vector3 previousPos;
    void OnDrag(Vector2 delta)
    {
        if(isCasted)return;
        previousPos= dragPoint.transform.position;
        dragPoint.transform.position= UICamera.currentCamera.ScreenToWorldPoint(UICamera.lastEventPosition);

       

        dist = Vector3.Distance(UICamera.currentCamera.WorldToViewportPoint(dummyBtn.position),UICamera.currentCamera.WorldToViewportPoint(dragPoint.transform.position));
        castPower=dist*5.0f;

        if(castPower>1.4f)castPower=1.4f;
       
        int percent=(int)((castPower-0.2f)*101.0f);
        if(percent-20<=0){
            if(ta.enabled)ta.enabled=false;
            lbl.text="CANCEL";
           
        }else{
            if(percent-20>=100){
                if(!ta.enabled)ta.enabled=true;
                lbl.text="MAX";
            }else{
                if(ta.enabled)ta.enabled=false;
                lbl.text=(percent-20)+"%";
            }
           
        }
        if(percent-5<=0){
            guid.transform.localRotation=Quaternion.identity;
        }else{
            Vector3 vectorToTarget = UICamera.currentCamera.WorldToViewportPoint(dragPoint.transform.position)-UICamera.currentCamera.WorldToViewportPoint(dummyBtn.position);
            float angle = Mathf.Atan2(vectorToTarget.y,vectorToTarget.x) * Mathf.Rad2Deg;

            guid.transform.localRotation=Quaternion.Euler(new Vector3(0.0f,0.0f,angle+90.0f));
        }
       
       

        guid.fillAmount=0.34f+((1.2f-0.34f)*castPower );
        guid.height=200+(int)( ((200 -148)*castPower));
        guid.width=130-(int)( ((200 -148)*castPower));


        guidNavi.fillAmount= castPower;
        guidNavi.height= guid.height;
        guidNavi.width=guid.width;
    }
    private float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
    {
        Vector2 diference = vec2 - vec1;
        float sign = (vec2.y < vec1.y)? -1.0f : 1.0f;
        return Vector2.Angle(Vector2.right, diference) * sign;
    }
    public TweenAlpha ta;

    public Transform dummyBtn;


}
