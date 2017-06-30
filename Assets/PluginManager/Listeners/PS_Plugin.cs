using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PS_Plugin: PS_SingletonBehaviour<PS_Plugin> {
	
    public bool needToSingleton=false;
    void DestroyAll(){
        foreach (Transform childTransform in gameObject.transform) Destroy(childTransform.gameObject);
        Destroy(gameObject);
    }
    void Awake()
    {
        if(needToSingleton){
            if(this != Instance)
            {
                DestroyAll();
                return;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }



	public bool isDebugMode=false;

	
	

	public void InitAll(){
        Debug.Log("****Pluginの初期化");
        AndroidNotificationManager.Instance.CancelAllLocalNotifications();

        StartCoroutine(InitInvoker());
        //ここでコルーチンを行い、全部のデータがあることを確認してDataManagerを呼び出す
        //ダメならダメで良い、store落ち、とかでも良いかも

	}


    IEnumerator InitInvoker(){
        //最悪なくても良いもの
        LoadingMenuManager.Instance.SetProgress(1,"connecting to store");
            isInitted_Store=false;
            storeListener.Init();

            while(!isInitted_Store){
                yield return null;
            }
        yield return new WaitForSeconds(0.2f);
        LoadingMenuManager.Instance.SetProgress(1,"connecting to social");
            isInitted_Twitter=false;
            twListener.Init();

            //Tapjoyもここでやる
            //if(tjListener!=null)tjListener.Init();
            while(!isInitted_Twitter){
                yield return null;
            }
        yield return new WaitForSeconds(0.2f);

        LoadingMenuManager.Instance.SetProgress(1,"connecting to social");
        isInitted_TJ=false;
        tjListener.Init();
        while(!isInitted_TJ){
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);

        LoadingMenuManager.Instance.SetProgress(1,"connecting to readerboad");
        isInitted_Readerboad=false;
        readerboadListener.Init();

        Debug.Log("****Pluginの初期化 2");
        yield return null;

        while(!isInitted_Readerboad){
            yield return null;
        }
        Debug.Log("****Pluginの初期化 3");
        if(!isConnected_Readerboad){
            //GPGにコネクトできない場合
            DataManger.Instance.OnPluginNotReady();
            yield break;
        }
        Debug.Log("****Pluginの初期化 4");
        yield return new WaitForSeconds(0.2f);
        LoadingMenuManager.Instance.SetProgress(1,"check if app is uptodate");
        year="not ready";
        readerboadListener.IsThisYear();
        while(year=="not ready"){
            yield return null;
        }
        Debug.Log("****Pluginの初期化 5");
        DataManger.Instance.OnPluginReady(this.year);
    }
    string year="not ready";
    public void OnYearChecked(string tittle){
        year=tittle;
    }

	//ストア
    public StoreListener storeListener;
    public bool isInitted_Store=false;
    public bool isConnected_Store=false;
    public void OnStoreInitComplete(bool isSuccess){
        isInitted_Store=true;
        isConnected_Store=isSuccess;
    }

    //Twitter
    public TwitterListener twListener;
    public bool isInitted_Twitter=false;
    public bool isConnected_Twitter=false;
    public void OnTwitterInitComplete(bool isSuccess){
        isInitted_Twitter=true;
        isConnected_Twitter=isSuccess;
    }
    //Readerboad
    public GPGSListener readerboadListener;
    public bool isInitted_Readerboad=false;
    public bool isConnected_Readerboad=false;
    public void OnGpgInitComplete(bool isSuccess){
        isInitted_Readerboad=true;
        isConnected_Readerboad=isSuccess;
    }
    //TJ
    public TapjoyListener tjListener;
    public bool isInitted_TJ=false;
    public bool isConnected_TJ=false;
    public void OnTJInitComplete(bool isSuccess){
        isInitted_TJ=true;
        isConnected_TJ=isSuccess;
    }




	public void ClearAllCallbacks() {
		if(tjListener!=null)tjListener.ClearAllEventListeners();
		if(storeListener!=null)storeListener.ClearAllEventListeners();
        if(readerboadListener!=null)readerboadListener.ClearAllEventListeners();
		if(twListener!=null)twListener.ClearAllEventListeners();
	}



	public void SetNotification(string tittle,string desc,int sec){
		if(AndroidNotificationManager.Instance!=null)AndroidNotificationManager.Instance.ScheduleLocalNotification(tittle,desc,sec);
	}
	void OnApplicationQuit() {
		//SetNotification("連続起動ボーナス","今日ペカつく！をひらくと"+(ES2.Load<int>(DataManger.Instance.filename+"?tag=RenzokuLogin")+1)+"連続ログインでボーナスをGET! "+"",86400);


	}




    //SceneManager
    public string GetCurrentSceneName(){
        return SceneManager.GetActiveScene().name;
    }

    public void LoadScene(string sceneName){
        Debug.Log("LoadScene");
        tjListener.ClearAllEventListeners();
        SceneManager.LoadScene(sceneName);
        //SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
