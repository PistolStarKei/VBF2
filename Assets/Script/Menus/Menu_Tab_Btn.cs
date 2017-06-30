using UnityEngine;
using System.Collections;

public class Menu_Tab_Btn : MonoBehaviour {

    public UILabel lbl;
    public UISprite sp;
    public Menu_Tab tab;
    public void SetTabs(string menu,Menu_Tab tab){
        this.tab=tab;
        lbl.text=menu;   
        SetState(false);
    }
    public void OnClicked(){
        Debug.Log("OnClicked");
        tab.OnClicked(lbl.text);
    }
    public bool isSelected=false;
    public void SetState(bool isOn){
        //Debug.Log("SetState"+isOn);
        isSelected=isOn;
        if(isOn){
            transform.localPosition=new Vector3(15.0f, transform.localPosition.y, transform.localPosition.z);
            sp.color= GUIColors.Instance.GUI_Tab_Selected;
            lbl.color= GUIColors.Instance.GUI_Tab_Selected_Lb;
        }else{
            transform.localPosition=new Vector3(15.0f, transform.localPosition.y, transform.localPosition.z);
            sp.color= GUIColors.Instance.GUI_Tab_UnSelected;
            lbl.color= GUIColors.Instance.GUI_Tab_UnSelected_Lb;
        }
    }
}
