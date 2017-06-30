using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; 
static public class PSGameUtils {

    // transform
    public static Bounds TransformBounds(Transform self, Bounds bounds)
    {
        var center = self.TransformPoint(bounds.center);
        var points = bounds.GetCorners();

        var result = new Bounds(center, Vector3.zero);
        foreach (var point in points)
            result.Encapsulate(self.TransformPoint(point));
        return result;
    }
    /*public static Bounds InverseTransformBounds(Transform self, Bounds bounds)
    {
        var center = self.InverseTransformPoint(bounds.center);
        var points = bounds.GetCorners(bounds);

        var result = new Bounds(center, Vector3.zero);
        foreach (var point in points)
            result.Encapsulate(self.InverseTransformPoint(point));
        return result;
    }*/

    // bounds
    public static List<Vector3> GetCorners(this Bounds obj)
    {
        var result = new List<Vector3>();

        /*for (int x = -1; x <= 1; x += 2)
            for (int y = -1; y <= 1; y += 2)
                for (int z = -1; z <= 1; z += 2)
                    //result.Add( obj.center + (obj.size/ 2)*new Vector3(x, y, z));*/
        return result;
    }

    //precent=そうでない確率
    public static float GetChusenRitsuFromKakuritsu(float kaisu,float percent){

        //（1-1/大当たり確率）^遊戯回転数*100%

        if(kaisu<=0.0f)return 0.0f;
        if(percent<=0.0f)return 0.0f;
        float num=(100.0f-percent)/100.0f;
        float kai=Mathf.Pow(num,1.0f/kaisu);
        kai=1.0f/(1.0f-kai);
        kai=(100.0f/kai);
        return kai/100.0f;
    }

    public static float GetTousenRitsuST(float kaisu,float bunbo){

        //（1-1/大当たり確率）^遊戯回転数*100%
        float num=(float)(1.0f-(1.0f/bunbo));
        float kai=Mathf.Pow(num,kaisu);
        return 100.0f-(kai*100.0f);
    }

    public static bool Chusen(float kakuritsu){
        
        if(kakuritsu>=UnityEngine.Random.value){
            return true;
        }else{
            return false;
        }
    }
    public static float localEulerangletoDec(float angle){

        return (angle > 180) ? angle - 360 : angle;
    }
    public static string GetPercent(float val) {
        val=val*100.0f;
        int valI=Mathf.FloorToInt(val);

        if(val<0.0f){
            return "-"+Mathf.Abs(valI)+"%";
        }else{
            return "+"+Mathf.Abs(valI)+"%";
        }
    }


    public static bool[] MergeAvility(string avil1,string avil2){
        bool[] str1=StringToBoolArray(avil1);
        bool[] str2=StringToBoolArray(avil2);

        for(int i=0;i<str1.Length;i++){
            if(str2[i]) str1[i]=true;
        }
        return str1;

    }


    public static void SetUISprite (UISprite sp,string name) {
        if(sp.spriteName!=name)sp.spriteName=name;
    }
    public static void ActiveNGUIObject (GameObject go,bool state) {
        if(go.activeSelf!=state)NGUITools.SetActive(go,state);
    }

    public static int SetPercent (int val,int weightPercent) {
        weightPercent=ClampInte(weightPercent,1,100);
        return val/(101-weightPercent);
    }

    public static int ClampInte (int val,int min ,int max) {
        return Mathf.Clamp (val, min, max);
    }

    public static float ClampAngle (float angle,float min ,float max) {
        if (angle < -360.0f)angle += 360.0f;
        if (angle > 360.0f)angle -= 360.0f;
        return Mathf.Clamp (angle, min, max);
    }
    public static string DateTimeToString(DateTime time){
        return time.ToString();
    }
    public static DateTime StringToDateTime(string timeString){

        DateTime date;
        DateTime.TryParse(timeString,out date);
        return date;

    }
    public static int GetKeikaSecondsSinceLast(DateTime lastTimeOpen){

        TimeSpan ts = DateTime.Now - lastTimeOpen;
        return Mathf.FloorToInt((float)ts.TotalSeconds);

    }

    public static bool IsRenzoku(DateTime lastTimeOpen){

        TimeSpan ts = DateTime.Now - lastTimeOpen;
        if(Mathf.FloorToInt((float)ts.TotalSeconds)<=172800){
            if(DateTime.Now.AddDays(-1).Day==lastTimeOpen.Day){
                return true;
            }else{
                return false;
            }
        }else{
            return false;
        }
    }
    public static bool IsSameDayLogin(DateTime lastTimeOpen){
        TimeSpan ts = DateTime.Now - lastTimeOpen;
        if(Mathf.FloorToInt((float)ts.TotalSeconds)<=172800){
            if(DateTime.Now.Day==lastTimeOpen.Day){
                return true;
            }else{
                return false;
            }
        }else{
            return false;
        }
    }




	public static Vector3 WolrdToNGUIPosition(Camera worldCamera,Camera guiCamera,Vector3 pos){
		Vector3 UIPos = worldCamera.WorldToScreenPoint(pos);
		UIPos.z = 1.0f;
		return guiCamera.ScreenToWorldPoint(UIPos);
	}

	public static Vector3 GetPointAroundPosition(Vector3 center, float radius) { 
		float ang = UnityEngine.Random.value * 360.0f; 
		Vector3 pos;
		pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad); pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad); pos.z = center.z; 
		return pos; 
	}
	public static IEnumerator WaitForRealSeconds(float time)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
	}
	private static string GetCallerMethodName() {
		System.Diagnostics.StackTrace stack  = new System.Diagnostics.StackTrace(false);
		return stack.GetFrame(2).GetMethod().Name;
	}




   
        
    public static int GetStringLength(string data,char[] delimiterChars){
        string[] words=data.Split(delimiterChars);
        return words.Length;
    }

    public static string[] SplitStringData(string dataString,char[] delimiterChars){


        string[] words=dataString.Split(delimiterChars);

        return words;

    }


    public static string Vector3ToString(Vector3 vec){
        string str = "";
        str = vec.x.ToString ("F2") + "," + vec.y.ToString ("F2") + "," + vec.z.ToString ("F2");
        return str;

    }
    public static Vector3 StringToVector3(string data){
        char[] delimiterChars = {','};
        string[] vec=data.Split (delimiterChars);
        return new Vector3 (float.Parse(vec[0]),float.Parse(vec[1]),float.Parse(vec[2]));
    }

    public static string BoolArrayToString(bool[] num){
        string returnVal="";

        for(int i=0;i<num.Length;i++){
            if(i==0){
                if(num[i]){
                    returnVal="1";
                }else{
                    returnVal="0";
                }
            }else{
                if(num[i]){
                    returnVal+="1";
                }else{
                    returnVal+="0";
                }
            }


        }

        return returnVal;

    }
    public static string FloatArrayToString(float[] num){
        string returnVal="";
        for(int i=0;i<num.Length;i++){
            if(i==0){
                returnVal=num[i].ToString();

            }else{
                returnVal+=","+num[i].ToString();

            }


        }

        return returnVal;

    }
    public static float[] StringToFloatArray(string num){
        char[] delimiterChars = {','};
        string[] str=num.Split (delimiterChars);
        float[] data=new float[str.Length];
        for(int i=0;i<str.Length;i++){
            data[i]=(float)System.Convert.ToDouble(str[i]);

        }

        return data;

    }

    public static string IntArrayToString(int[] num){
        string returnVal="";
        for(int i=0;i<num.Length;i++){
            if(i==0){
                returnVal=num[i].ToString();

            }else{
                returnVal+=","+num[i].ToString();

            }


        }

        return returnVal;

    }
    public static bool[] StringToBoolArray(string num,char[] delimiterChars){

        string[] str=num.Split (delimiterChars);
        bool[] data=new bool[str.Length];
        for(int i=0;i<str.Length;i++){
            data[i]=System.Convert.ToBoolean(str[i]);

        }
        return data;

    }

    public static int[] StringToIntArray(string num,char[] delimiterChars){
        
        string[] str=num.Split (delimiterChars);
        int[] data=new int[str.Length];
        for(int i=0;i<str.Length;i++){
            data[i]=System.Convert.ToInt32(str[i]);

        }

        return data;

    }

    public static bool[] StringToBoolArray(string num){

        char[] chara = num.ToCharArray ();
        bool[] data = new bool[chara.Length];
        char trueChara = '1';
        for(int i=0;i<data.Length;i++){
            data[i]=false;
            if(chara[i]==trueChara){
                data[i]=true;
            }else{
                data[i]=false;
            }

        }

        return data;

    }


    public static float WariaiVals(float[] paramsVal,int[] percent){
        if(paramsVal.Length!=percent.Length){
            Debug.LogError("WariaiVals not match length error");
            return 0.0f;
        }
       
        float data = 0.0f;
        for(int i=0;i<percent.Length;i++){
            data+=paramsVal[i]*(percent[i]/100.0f);
            if(data>100.0f)data=100.0f;
            if(data<0.0f)data=0.0f;

        }
        return data;

    }
}
