using UnityEngine;
using System.Collections;

public class ResolutionResizer : MonoBehaviour {

    public int resolutionDefault=1280;
    public bool isLandScape=true;
	// Use this for initialization
	void Start () {
        float screenRate = 0f;
        if(isLandScape){
            screenRate = (float)resolutionDefault/ Screen.width;
        }else{
            screenRate = (float)resolutionDefault/ Screen.height;
        }
        if( screenRate > 1 ) screenRate = 1;
        int width = (int)(Screen.width * screenRate);
        int height = (int)(Screen.height * screenRate);
        Screen.SetResolution( width , height, true,15);
	}
}
