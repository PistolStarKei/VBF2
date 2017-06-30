using UnityEngine;
using System.Collections;

public class WidgetOnObject : MonoBehaviour {

    public void Show(){

        gameObject.transform.localScale=Vector3.one;
        isEnableFolow=true;
    }

    public void Hide(){

        gameObject.transform.localScale=Vector3.one;
        isEnableFolow=false;
    }
    public float offset=0.5f;
    public bool isEnableFolow=false;
    public Transform target;
    public Camera camera;
    // Update is called once per frame
    void Update () {

        if(isEnableFolow && target!=null){
            UIPos = camera.WorldToScreenPoint(target.position);
            UIPos.z = 1.0f;
            UIPos.y+=offset;
            //文字の座標をスクリーン座標からOrthographicカメラのワールド座標に反映
            transform.position =  UICamera.mainCamera.ScreenToWorldPoint(UIPos);
        }
    }
    Vector3 UIPos ;

}
