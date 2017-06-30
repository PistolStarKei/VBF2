using UnityEngine;
using System.Collections;

public class FPSLogger : MonoBehaviour {

    int frameCount;
    float prevTime;

    void Start()
    {
        Application.targetFrameRate = 60;
        frameCount = 0;
        prevTime = 0.0f;
    }
    public UILabel lbl;
    public UILabel lbl2;
    void Update()
    {
        ++frameCount;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f) {
            lbl.text=(frameCount / time).ToString("F0")+"fps";

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
            lbl2.text=Application.targetFrameRate==-1?"AUTO":Application.targetFrameRate.ToString()+"FPS";
        }
    }
}
