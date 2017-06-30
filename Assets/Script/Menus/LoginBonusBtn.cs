using UnityEngine;
using System.Collections;

public class LoginBonusBtn : MonoBehaviour {

    public UILabel lbl;
    public Animation anime;
    public void Show(bool isLogin){
        CancelInvoke("Huruero");
        if(isLogin){
            lbl.text=Localization.Get("LoginB");
        }else{
            lbl.text=Localization.Get("FreeG");
        }
        InvokeRepeating("Huruero",0.0f,2.0f);
    }

    public void Huruero(){
        anime.Play();
    }
}
