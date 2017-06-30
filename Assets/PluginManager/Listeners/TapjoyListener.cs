using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TapjoyUnity;

public class TapjoyListener :PS_SingletonBehaviour<TapjoyListener>
	{

    public TJPlacement offerwallPlacement;
    public TJPlacement BannerAd;
    public TJPlacement IntterAd;
    public delegate void Callback_Earned(int amount);
    public  event Callback_Earned earnedEvent;


    void OnEnable()
    {
        Tapjoy.OnConnectSuccess += HandleConnectSuccess;
        Tapjoy.OnConnectFailure += HandleConnectFailure;
        TJPlacement.OnRequestSuccess += HandlePlacementRequestSuccess;
        TJPlacement.OnRequestFailure += HandlePlacementRequestFailure;
        TJPlacement.OnContentReady += HandlePlacementContentReady;
        TJPlacement.OnContentShow += HandlePlacementContentShow;
        TJPlacement.OnContentDismiss += HandlePlacementContentDismiss;
        TJPlacement.OnPurchaseRequest += HandleOnPurchaseRequest;
        TJPlacement.OnRewardRequest += HandleOnRewardRequest;
        // Currency Delegatesåå
        Tapjoy.OnGetCurrencyBalanceResponse += HandleGetCurrencyBalanceResponse;
        Tapjoy.OnEarnedCurrency += HandleEarnedCurrency;
        if(isInitted){
            if (BannerAd == null) {
                BannerAd = TJPlacement.CreatePlacement("Video");
                if (BannerAd != null) {
                    BannerAd.RequestContent();
                }
            }

            if (offerwallPlacement == null) {
                offerwallPlacement = TJPlacement.CreatePlacement("Wall");
                if (offerwallPlacement != null) {
                    offerwallPlacement.RequestContent();
                }
            }
            if (IntterAd == null) {
                IntterAd = TJPlacement.CreatePlacement("Pop");
                if (IntterAd != null) {
                    IntterAd.RequestContent();
                }
            }


        }

    }


    void OnDisable()
    {
        Tapjoy.OnConnectSuccess -= HandleConnectSuccess;
        Tapjoy.OnConnectFailure -= HandleConnectFailure;
        // Placement delegates
        TJPlacement.OnRequestSuccess -= HandlePlacementRequestSuccess;
        TJPlacement.OnRequestFailure -= HandlePlacementRequestFailure;
        TJPlacement.OnContentReady -= HandlePlacementContentReady;
        TJPlacement.OnContentShow -= HandlePlacementContentShow;
        TJPlacement.OnContentDismiss -= HandlePlacementContentDismiss;
        TJPlacement.OnPurchaseRequest -= HandleOnPurchaseRequest;
        TJPlacement.OnRewardRequest -= HandleOnRewardRequest;

        // Tapjoy Placement Video Delegates

        // Currency Delegates
        Tapjoy.OnGetCurrencyBalanceResponse -= HandleGetCurrencyBalanceResponse;
        Tapjoy.OnEarnedCurrency -= HandleEarnedCurrency;
    }

    private static bool  created =false;


    void Awake () {

        if(!created){
            // this is the first instance -make it persist
            DontDestroyOnLoad(this.gameObject);
            created = true;
        } else{
            // this must be aduplicate from a scene reload  - DESTROY!
            Destroy(this.gameObject);
        }
    }

    void Start(){
        Init();
    }
    public void Show_Offer_Wall(){
        if(!isInitted){
            Debug.LogError("Tapjoy Not Initted");
            return;
        }

        #if UNITY_IPHONE ||UNITY_ANDROID 


        if (offerwallPlacement != null) {
            if(offerwallPlacement.IsContentReady()){
                offerwallPlacement.ShowContent();
            } else{
                StartCoroutine(ShowOffer());
            }
        }
        #endif
    }

    bool ShowOffInvoing=false;
    bool offerReady=false;

    IEnumerator ShowOffer(){
        if(ShowOffInvoing)yield break;
        ShowOffInvoing=true;
        offerReady=false;
        offerwallPlacement.RequestContent();
        float time2=0.0f;
        while(!offerReady){

            time2+=Time.deltaTime;
            if(time2>10.0f){
                offerReady=false;
                ShowOffInvoing=false;
                yield break;

            }
            yield return null;
        }
        offerwallPlacement.ShowContent();
        yield return null;
        ShowOffInvoing=false;

    }

    public void Show_Inter(){
        if(!isInitted){
            Debug.LogError(" Show_Inter Tapjoy Not Initted");
            return;
        }

        if (IntterAd!= null) {
            Debug.LogError("IntterAd not null");
            if(IntterAd.IsContentReady()){
                Debug.LogError("IntterAd show");
                IntterAd.ShowContent();
            } else{
                StartCoroutine(ShowVid());
            }
        }else{
            Debug.LogError("IntterAd null");
            IntterAd = TJPlacement.CreatePlacement("InterSticial");
        }
    }

    bool ShowInterInvoing=false;
    bool IntterAdReady=false;

    IEnumerator ShowInter(){
        Debug.LogError("IntterAd show coroutine");
        if(ShowVidInvoing)yield break;
        ShowInterInvoing=true;
        IntterAdReady=false;
        IntterAd.RequestContent();
        float time=0.0f;
        while(!IntterAdReady){

            time+=Time.deltaTime;
            if(time>10.0f){
                Debug.LogError("IntterAd show time up");
                IntterAdReady=false;
                ShowInterInvoing=false;
                yield break;

            }
            yield return null;
        }
        IntterAd.ShowContent();
        yield return null;
        ShowInterInvoing=false;

    }


    public void Show_Video(){
        if(!isInitted){
            Debug.LogError("Tapjoy Not Initted");
            return;
        }

        if (BannerAd != null) {
            Debug.LogError("BannerAd not null");
            if(BannerAd.IsContentReady()){
                Debug.LogError("BannerAd show");
                BannerAd.ShowContent();
            } else{
                StartCoroutine(ShowVid());
            }
        }else{
            Debug.LogError("BannerAd null");
            BannerAd = TJPlacement.CreatePlacement("Ad_Video");
        }
    }

    bool ShowVidInvoing=false;
    bool videoReady=false;

    IEnumerator ShowVid(){
        Debug.LogError("BannerAd show coroutine");
        if(ShowVidInvoing)yield break;
        ShowVidInvoing=true;
        videoReady=false;
        BannerAd.RequestContent();
        float time=0.0f;
        while(!videoReady){

            time+=Time.deltaTime;
            if(time>10.0f){
                Debug.LogError("BannerAd show time up");
                videoReady=false;
                ShowVidInvoing=false;
                yield break;

            }
            yield return null;
        }
        BannerAd.ShowContent();
        yield return null;
        ShowVidInvoing=false;

    }

    public bool isInitted=false;
    public void Init(){

        if(Application.isEditor)PS_Plugin.Instance.OnTJInitComplete(true);

        if(Tapjoy.IsConnected){
            PS_Plugin.Instance.OnTJInitComplete(true);
            return;
        }
        #if UNITY_ANDROID
         Tapjoy.Connect( Constants.Params.tapjoyID);

        #else
            Tapjoy.Connect(Constants.Params.tapjoyID_iOS);

        #endif

    }


    public void ClearAllEventListeners(){
        if(earnedEvent!=null)earnedEvent=null;
    }


    public void HandleConnectSuccess() {

        Debug.Log( "tapjoy Connect Success " );
        Tapjoy.GetCurrencyBalance();

        isInitted=true;
        if(isInitted){
            if (BannerAd == null) {
                BannerAd = TJPlacement.CreatePlacement("Ad_Video");
                if (BannerAd != null) {
                    BannerAd.RequestContent();
                }
            }

            if (offerwallPlacement == null) {
                offerwallPlacement = TJPlacement.CreatePlacement("Ad_OfferWall");
                if (offerwallPlacement != null) {
                    offerwallPlacement.RequestContent();
                }
            }
            if (IntterAd == null) {
                IntterAd = TJPlacement.CreatePlacement("InterSticial");
                if (IntterAd != null) {
                    IntterAd.RequestContent();
                }
            }
        }
        PS_Plugin.Instance.OnTJInitComplete(true);
    }


    public void HandleConnectFailure() {
        Debug.LogError( "tapjoy Connect fail " );
        isInitted=false;
        PS_Plugin.Instance.OnTJInitComplete(false);
    }


    public bool loadVideoAdOnInit=true;
    public bool loadVideoAdOnWatched=true;




    public void HandlePlacementRequestSuccess(TJPlacement placement) {
        Debug.Log("C#: Content available for " + placement.GetName());

        if(placement.GetName()=="Ad_Video"){
            videoReady=true;
        }else if(placement.GetName()=="InterSticial"){
            IntterAdReady=true;
        }else{
            offerReady=true;
        }
    }



    public void HandleEarnedCurrency(string currencyName, int amount) {
        Debug.Log("C#: HandleEarnedCurrency: currencyName: " + currencyName + ", amount: " + amount);

       
        if(amount>0){
            if(ES2.Exists(DataManger.DataFilename+"?tag=SPOfferLeft")){
                ES2.Save(ES2.Load<int>( DataManger.DataFilename+"?tag=gettedGolds")+amount, DataManger.DataFilename+"?tag=gettedGolds");
                Tapjoy.SpendCurrency(amount);
                if(earnedEvent!=null)earnedEvent(amount);
            }

        }
      
    }
    public void HandlePlacementRequestFailure(TJPlacement placement, string error) {
        Debug.Log("C#: HandlePlacementRequestFailure");
        Debug.Log("C#: Request for " + placement.GetName() + " has failed because: " + error);


    }

    public void HandlePlacementContentReady(TJPlacement placement) {
        Debug.Log("C#: HandlePlacementContentReady");

    }

    public void HandlePlacementContentShow(TJPlacement placement) {
        Debug.Log("C#: HandlePlacementContentShow");
    }

    public void HandlePlacementContentDismiss(TJPlacement placement) {
        Debug.Log("C#: HandlePlacementContentDismiss");
        Tapjoy.GetCurrencyBalance();
    }

    void HandleOnPurchaseRequest (TJPlacement placement, TJActionRequest request, string productId)
    {
        Debug.Log ("C#: HandleOnPurchaseRequest");
        request.Completed();
    }

    void HandleOnRewardRequest (TJPlacement placement, TJActionRequest request, string itemId, int quantity)
    {
        Debug.Log ("C#: HandleOnRewardRequest");
        request.Completed();
    }


    public void HandleGetCurrencyBalanceResponse(string currencyName, int balance) {
        Debug.Log("C#: HandleGetCurrencyBalanceResponse: currencyName: " + currencyName + ", balance: " + balance);

    }



}
