using UnityEngine;
using System.Collections;

public class BackHomeBtn : MonoBehaviour {

    public void OnTapped(){
        Debug.LogError("OnTapped");
        MenuManager.Instance.BackHome();
        Hide();
    }
    public TweenPosition tp;
    public BoxCollider col;
    public void OnShowed(){
        
        if(tp.direction==AnimationOrTween.Direction.Forward){
            col.enabled=true;
            Debug.LogError("OnShowed 1");
        }else{
            col.enabled=false;
            Debug.LogError("OnShowed 2");
            NGUITools.SetActive(gameObject,false);
        }
    }
    public void Show(){
        Debug.LogError("Show");
        if(col==null)col=gameObject.GetComponent<BoxCollider>();
        if(!col.enabled){
            NGUITools.SetActive(gameObject,true);
            tp.PlayForward();
        }
       
    }

    public void Hide(){
        Debug.LogError("Hide");
        if(col.enabled){
            col.enabled=false;
            tp.PlayReverse();
         }
    }
}
