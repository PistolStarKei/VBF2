using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
public class TackeMenuParentEquip : MonoBehaviour {

    public ItemListDynamic itemList;
    public LureShop_Menu_Rods rods;

    public int currentSelect=0;
    public int currenEquippedLure=0;

    public  virtual void SetContents(int i){ }
    public virtual bool isHas(int num){return false;}

    public virtual void OnTappedItem(int num,bool isInit){}
    public virtual void Show(int currentLure){}
    public virtual IEnumerator Init(int currentLure){yield return null;
    }
    public  virtual IEnumerator SetItemLists(){
        yield return null;
    }
    public static string GetMethodName () {
        MethodBase methodBase = MethodBase.GetCurrentMethod();
        return methodBase.Name;
    }

}

