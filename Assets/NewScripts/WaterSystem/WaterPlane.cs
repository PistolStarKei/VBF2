using UnityEngine;
using System.Collections;

public class WaterPlane : PS_SingletonBehaviour<WaterPlane> {

    public Color[] col;
    WAVETYPE waveTextureName=WAVETYPE.NONE;
    string[] Shaderparams=new string[]{"_Color","_WaveTex","_WaveSpeed","_Refraction"};
    //STILL,NORMAL,TALL
    string[] waveTexs=new string[]{"WaterSurface/Water3_N","WaterSurface/MediumWaves_norm","WaterSurface/Water1_N"};

    public Transform suimenTrans;

    //offset 与えただけ早めに
    public bool isOnSuime(float positionY,float offset){
        if((suimenTrans.position.y-positionY)<offset){
            return true;
        }
        return false;
    }
    public bool isUnderWater(float positionY){

        if(suimenTrans.position.y>positionY){
            return true;
        }
        return false;
    }


    public void SetWater(float clearness,WAVETYPE_COLOR coltype,float wavePower,WAVETYPE waveTall){
        gameObject.GetComponent<Renderer>().material.SetFloat(Shaderparams[3],1.0f-clearness);

        gameObject.GetComponent<Renderer>().material.SetColor(Shaderparams[0],col[(int)coltype]);




        if(waveTall==WAVETYPE.NONE)return;
        if(waveTextureName==WAVETYPE.NONE || waveTextureName!=waveTall){
            Texture tex=Resources.Load(waveTexs[(int)waveTall]) as Texture;
            gameObject.GetComponent<Renderer>().material.SetTexture(Shaderparams[1],tex);
            waveTextureName=waveTall;
            switch(waveTall){
                case WAVETYPE.STILL:
                gameObject.GetComponent<Renderer>().material.SetFloat(Shaderparams[2], 0.013f);
                gameObject.GetComponent<Renderer>().material.mainTextureScale=new Vector2(8,8);
                    break;
                    break;
                case WAVETYPE.NORMAL:
                gameObject.GetComponent<Renderer>().material.SetFloat(Shaderparams[2], Random.Range(0.013f,0.02f));
                gameObject.GetComponent<Renderer>().material.mainTextureScale=new Vector2(4,4);
                    break;
                case WAVETYPE.TALL:
                gameObject.GetComponent<Renderer>().material.SetFloat(Shaderparams[2],0.02f);
                gameObject.GetComponent<Renderer>().material.mainTextureScale=new Vector2(2,2);
                    break;
            }
        }


    }

}
