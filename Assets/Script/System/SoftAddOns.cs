using UnityEngine;
using System.Collections;

public class SoftAddOns : MonoBehaviour {

    public LureShop_Menu_Soft sof;
    public void OnClicked(int num,bool isEquippedd){
        if(isEquippedd){
            Debug.LogError("装備中なので無理");
            return;
        }

        sof.OnTappedAddonItem(num
        );
    }
    public void SetAdonItems(int num,string tittle,string desc,string price,string itemSp,bool isBuyed,bool isEquipped){
        addons[num].SetItems(tittle,desc,price,itemSp,isBuyed,isEquipped);
    }
    public void SetAdonItemsToUA(int num){
        addons[num].SetItems();
    }
    public void SetEquipped(int num,bool isOn){
        addons[num].SetEquipped(isOn);
    }
    public AddOnBtn[] addons;


}
