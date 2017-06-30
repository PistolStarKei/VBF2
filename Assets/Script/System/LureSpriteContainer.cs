using UnityEngine;
using System.Collections;

public class LureSpriteContainer : MonoBehaviour {



    public UISprite lureSp;
    public UISprite AdonSp;
    public UILabel tittleLb;
    public UISprite logo;

    public void SetLureParams(string tittle,string spName,UIAtlas at){
        if(tittle==""){
            tittleLb.text=tittle;
            lureSp.atlas=at;
            logo.enabled=true;
            lureSp.material=at.spriteMaterial;
            lureSp.spriteName=spName;
            PSGameUtils.SetUISprite(AdonSp,"");
        }else{
            logo.enabled=false;
            tittleLb.text=tittle;
            lureSp.atlas=at;
            lureSp.material=at.spriteMaterial;
            lureSp.spriteName=spName;
            PSGameUtils.SetUISprite(AdonSp,"");
        }

    }

    public void SetRig(string addOnSp,bool shouldShowBack,Vector3 AdonLocalTrans,Vector3 AdonLocalRot){
        PSGameUtils.SetUISprite(AdonSp,addOnSp);
        if(shouldShowBack){
            AdonSp.depth=lureSp.depth-1;
        }else{
            AdonSp.depth=lureSp.depth+1;
        }
        AdonSp.transform.localPosition=AdonLocalTrans;
        AdonSp.transform.localRotation=Quaternion.Euler(AdonLocalRot);

    }
}
