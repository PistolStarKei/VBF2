using UnityEngine;
using System.Collections;
using System;
public class Menu_Tab : MonoBehaviour {

    public void SetTabState(string current){
        Debug.Log("タブをセットする "+strs.Length);
        ClearState();
        if(currentTabs!=null)currentTabs.SetState(false);
        Debug.Log("タブをセットする "+Array.IndexOf(strs,current));
        currentTabs=tabs[Array.IndexOf(strs,current)];
        currentTabs.SetState(true);
    }


    public void ClearState(){
        Debug.Log("タブをクリアする"+gameObject.name);
        for(int i=0;i<tabs.Length;i++){
            tabs[i].SetState(false);
        }
        currentTabs=null;
       
    }

    public delegate void Callback_onTabChanged(string menu);
    //購入時のキャンセル時
    public  event Callback_onTabChanged onTabChanged;

    public Menu_Tab_Btn[] tabs;
    public Menu_Tab_Btn currentTabs;
    public void OnClicked(string menu){
        if(currentTabs!=null){
            if(currentTabs==tabs[Array.IndexOf(strs,menu)])return;
            currentTabs.SetState(false);
        }
        currentTabs=tabs[Array.IndexOf(strs,menu)];
        currentTabs.SetState(true);
        AudioController.Play("tab");
        if(onTabChanged!=null)onTabChanged(menu);
    }
    public string[] strs;
   public void SetTabs(string[] strs){
        
       this.strs=strs;
        currentTabs=null;
        switch(strs.Length){
        case 2:
            SetLocalPostion( tabs[0].transform,new Vector3(0.0f,200.0f,0.0f));
            SetLocalPostion( tabs[1].transform,new Vector3(0.0f,-200.0f,0.0f));
            break;
        case 3:
            SetLocalPostion( tabs[0].transform,new Vector3(0.0f,350.0f,0.0f));
            tabs[1].transform.localPosition=Vector3.zero;
            SetLocalPostion( tabs[2].transform,new Vector3(0.0f,-350.0f,0.0f));
            break;
        case 4:
            SetLocalPostion( tabs[0].transform,new Vector3(0.0f,450.0f,0.0f));
            SetLocalPostion( tabs[1].transform,new Vector3(0.0f,150.0f,0.0f));
            SetLocalPostion( tabs[2].transform,new Vector3(0.0f,-150.0f,0.0f));
            SetLocalPostion( tabs[3].transform,new Vector3(0.0f,-450.0f,0.0f));
            break;
        }
        for(int i=0;i<tabs.Length;i++){
            if(i<strs.Length){
                if(!tabs[i].gameObject.activeSelf)NGUITools.SetActive( tabs[i].gameObject,true);
                tabs[i].SetTabs(strs[i],this);
            }else{
                if(tabs[i].gameObject.activeSelf)NGUITools.SetActive( tabs[i].gameObject,false);
            }
        }
       
    }
    void SetLocalPostion(Transform pos,Vector3 vec){
        
        pos.localPosition=new Vector3(vec.x!=0.0f?vec.x:pos.localPosition.x,vec.y!=0.0f?vec.y:pos.localPosition.y,vec.z!=0.0f?vec.z:pos.localPosition.z);
    }

}
