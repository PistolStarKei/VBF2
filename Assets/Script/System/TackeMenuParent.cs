using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
public class TackeMenuParent : MonoBehaviour {

    public ItemListDynamic itemList;


    public GameObject lurelistObj;
    public GameObject rodlistObj;
    public void ShowItemList(bool isRod){
        if(isRod){
            PSGameUtils.ActiveNGUIObject(lurelistObj,false);
            PSGameUtils.ActiveNGUIObject(rodlistObj,true);
        }else{
            PSGameUtils.ActiveNGUIObject(lurelistObj,true);
            PSGameUtils.ActiveNGUIObject(rodlistObj,false);
        }
    }



    public LureParamsAffecter affecter;
    public List<int> earlyAccessAvaillable=new List<int>();
    public int currentSelect=0;
    public RaderGraph rader;
    public int currenEquippedLure=0;

   

    public virtual void OnBtnBuy(){}
    public virtual void OnTapHatena(){}
    public virtual void OnBtnBuySP(){}

    public virtual void CheckEarlyAccess(){}

    public  virtual void SetContents(int i){ }
    public virtual bool isHasOrAvaillable(int num){return false;}

    public virtual void OnTappedItem(int num,bool isInit){}
    public virtual void Show(int currentLure){}
    public virtual IEnumerator Init(int currentLure){yield return null;
    }
    public  virtual IEnumerator SetItemLists(){
        yield return null;
    }

    public SetSPAvility avils;
    public void InitAvility(string[] strs){
        avils.InitAvility(strs);
    }
    public void SetAvilities(string str){
        avils.SetAvilities(str);
    }
    public void SetAvilities(bool[] nums){
        avils.SetAvilities(nums);
    }
    public void HideAvilities(){
        avils.SetAvilities();
    }

    public bool canEarlyAccess(int i){
        if(earlyAccessAvaillable.Contains(i)){
            return true;
        }
        return false;

    }
    public bool isMax(int currentHas,int currentMax){

        if(currentMax>currentHas){
            return false;
        }
        return true;

    }

    public static string GetMethodName () {
        MethodBase methodBase = MethodBase.GetCurrentMethod();
        return methodBase.Name;
    }

}
