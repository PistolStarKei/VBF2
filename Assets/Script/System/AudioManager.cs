using UnityEngine;
using System.Collections;

public class AudioManager : PS_SingletonBehaviour<AudioManager>  {

    public void Alert(bool isOn){
        if(isOn){
            if(!AudioController.IsPlaying("alert"))AudioController.Play("alert");
        }else{
            if(AudioController.IsPlaying("alert"))AudioController.Stop("alert");
        }
       
    }
    public void OnBassKubihuri(){
            AudioController.Play("jabajaba");
    }

    public void OnBassJumpInWater(){
        //AudioController.Play( "popwater",LureController.Instance.gameObject.transform.position, null); 
        AudioController.Play("bassjump");

    }

    public void OnPopWater(){
        //AudioController.Play( "popwater",LureController.Instance.gameObject.transform.position, null); 
        AudioController.Play("popwater");

    }

    public void OnLureInWater(){
        AudioController.Play("chapun");
       
    }
    public void ShowWindow(){
        
        AudioController.Play("showWindow");
    }
    public void SelectItemList(){

        AudioController.Play("selectItem");
    }
    public void ButtonYes(){
        AudioController.Play("btn");
    }
    public void Cancel(){
        AudioController.Play("cancel");
    }
    public void LevelUp1(){
        AudioController.Play("levelup");
    }
    public void LevelUp2(){
        AudioController.Play("rankup");
    }
    public void Warning(){
        AudioController.Play("warning");
    }
    public void Tab(){
        AudioController.Play("tab");
    }
    public void Shutter(){
        AudioController.Play("shutter");
    }
    public void LoopScore(bool isOn){
        if(isOn){
            AudioController.Play("loop");
        }else{
            AudioController.Stop("loop");
        }
       
    }
    public void Equip(){
        AudioController.Play("equiplure");
    }
    public void BuyAndEquip(){
        AudioController.Play("levelup");
    }

    public void Contact(){
        AudioController.Play("contact_wood");
    }
}
