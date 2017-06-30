using UnityEngine;
using System.Collections;

public class Tetsr : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
            //int kaisu=time/kankaku;

        float val1=299.0f/300.0f;
        Debug.Log(""+val1);
        float val=Mathf.Pow(val1,600.0f);
        val=val*100.0f;
        Debug.Log(""+val);

        float chusenRitsu=GetChusenRitsu((20.0f/(0.2f/6.0f)),100.0f-76.6f);

        Debug.Log("回の抽選中　　1/kaisu "+chusenRitsu);

	}
    //5% 10% 15% 20% 25% 30% 35% 40% 45% 50% 60% 70% 80% 90% 95%
    //float[] percent=new float[12]{20.0f,10.0f,6.0f,4.5f,3.5f,2.8f,2.3f,2.0f,1.7f,1.45f,1.1f,0.85f,0.6f,0.45f,0.33f};

    float GetChusenRitsu(float kaisu,float percent){

        //int kaisu=Mathf.FloorToInt(time/kankaku);
        //float val=Mathf.Pow(val,1.0f/600.0f);

       
        Debug.Log("回数"+kaisu);

        float num=(float)(kaisu-1)/(float)kaisu;

        float rp=percent/100.0f;
        float kai=Mathf.Pow(rp,1.0f/kaisu);

        return kai/100.0f;
    }
	// Update is called once per frame
	void Update () {
	
	}
}
