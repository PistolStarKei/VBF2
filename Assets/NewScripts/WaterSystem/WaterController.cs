using UnityEngine;
using System.Collections;

public class WaterController : PS_SingletonBehaviour<WaterController> {


	void Awake(){
		suimenTrans=transform.Find("SuimenPosition").gameObject.transform;
	}

    public Color[] col;
    WAVETYPE waveTextureName=WAVETYPE.NONE;
    string[] Shaderparams=new string[]{"_Color","_WaveTex","_WaveSpeed","_Refraction"};
    //STILL,NORMAL,TALL
    string[] waveTexs=new string[]{"WaterSurface/Water3_N","WaterSurface/MediumWaves_norm","WaterSurface/Water1_N"};

   Transform suimenTrans;

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

		//透明度のセット
		SetClearness(clearness);

		//水の色の設定
		SetWaterColor(coltype);

		//波の高さの設定
		SetWaveTall(waveTall);
	}

	void SetClearness(float clearness){
		gameObject.GetComponent<Renderer>().material.SetFloat(Shaderparams[3],1.0f-clearness);
	}
	void SetWaterColor(WAVETYPE_COLOR coltype){
		gameObject.GetComponent<Renderer>().material.SetColor(Shaderparams[0],col[(int)coltype]);
	}
	void SetWaveTall(WAVETYPE waveTall){
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
    



	//以下、波紋エフェクト用のメソッド

	void OnTriggerEnter(Collider other ) {

		Debug.Log("OnTriggerExit"+other.gameObject.name);
		if( other.gameObject.layer==LayerMask.NameToLayer("Rure")){
			if(GameController.Instance.currentMode==GameMode.Throwing||GameController.Instance.currentMode==GameMode.ReelingOnLand)
				SplashAt(other.gameObject.transform.position,other.gameObject.transform.localScale.x/2.0f);

			if(GameController.Instance!=null)GameController.Instance.OnLureInWater();

			if(!GameController.Instance.isStateWithin(GameMode.Cast)) LureController.Instance.OnEnterWater();
		}

	}

	public void SplashAt(Vector3 position,float scale){
		AudioManager.Instance.OnLureInWater();
		LureSpawner.Instance.OnJumpInWater(position,scale);
	}



	/*void OnTriggerExit  ( Collider other) {
		Debug.Log("OnTriggerExit"+other.gameObject.name);
	}*/

}
