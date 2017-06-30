using UnityEngine;
using System.Collections;
using PathologicalGames;
public class InfoItem : MonoBehaviour {
    public void SetItems(string info,string icon, InfoLauncher launcher,float dulation){
        this.launcher=launcher;
        sps.spriteName=icon;
        infoLabel.text=info;
        tp.ResetToBeginning();
        tp.PlayForward();
        Invoke("Despawn",dulation);
    }

    public UILabel infoLabel;
    public UISprite sps;
    InfoLauncher launcher;
    public TweenPosition tp;
    public void Despawn(){
        if(launcher!=null)launcher.Despawn(this);
    }
}
