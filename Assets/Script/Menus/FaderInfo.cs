using UnityEngine;
using System.Collections;

public class FaderInfo : MonoBehaviour {

    public void Show(string info,string icon, WaitAndCover wait){
        this.wait=wait;
        NGUITools.SetActive(gameObject,true);
        sps.spriteName=icon;
        infoLabel.text=info;
        gameObject.GetComponent<UIWidget>().alpha=1.0f;
        tp.ResetToBeginning();
        tp.PlayForward();
        isTappable=false;
    }

    public UILabel infoLabel;
    public UISprite sps;
    WaitAndCover wait;
    public TweenPosition tp;
    public bool isTappable=false;
    public void OnMoved(){
        if(tp.direction==AnimationOrTween.Direction.Forward){
            isTappable=true;
            if(sps.spriteName=="ic_warning"){
                AudioController.Play("warning");
            }else{
                AudioController.Play("levelup");
            }

           

        }else{

        }
    }
    public TweenAlpha ta;
    public void OnAlphaed(){
        if(ta.direction==AnimationOrTween.Direction.Forward){
            Despawn();
        }else{

        }
    }
    public void Despawn(){
        NGUITools.SetActive(gameObject,false);
    }
    public void OnTapped(){
        if(isTappable){
            isTappable=false;
            wait.OnHideTapInfo();
           
            wait.UnCoverAll();
           
            ta.ResetToBeginning();
            ta.PlayForward();
        }
    }
}
