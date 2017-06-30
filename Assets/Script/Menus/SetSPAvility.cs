using UnityEngine;
using System.Collections;

public class SetSPAvility : MonoBehaviour {

    public SPAvility[] avils;
    public void InitAvility(string[] strs){
        descLabel.text="";
        for(int i=0;i<strs.Length;i++){
            avils[i].SetActive(true);
            avils[i].SetText(strs[i]);
        }
    }
    public UILabel descLabel;
    public void SetAvilities(string str){
        descLabel.text=str;
    }
    public void SetAvilities(bool[] nums){
        for(int i=0;i<avils.Length;i++){
            avils[i].SetEnabled(nums[i]);
        }
    }
    public void SetAvilities(){
        for(int i=0;i<avils.Length;i++){
            avils[i].SetActive(false);
        }
    }
}
