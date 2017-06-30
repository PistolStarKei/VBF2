using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StoreItemsParams{
    public string[] prices;
    public string[] tittles;
    public string[] descs;
    public bool[] isConsumable;
}
public class StoreListener :  PS_SingletonBehaviour<StoreListener> 
	{
        //まず　Startで来るのでハンドルしろ
        void OnPurchaceRecovered(string id){
            if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( " OnPurchaceRecovered"+id );
        }


        public delegate void Callback_purchasFailledEvent(string s);
        public delegate void Callback_purchasedEvent(string s);
        public StoreItemsParams items;

        public bool isConsumableProduct(string skusName){
        
        int i=Array.IndexOf(Constants.Params.skus,skusName);
        if(i<Constants.Params.skus.Length){
                return items.isConsumable[i];
            }else{
                Debug.LogError("isConsumableProduct sukus is over isConsumable Length");
                return true;
            }
        }
        
	    public bool isDebugLog=false;
		//購入の成功時
		public  event Callback_purchasedEvent purchasedEvent;
		//購入時のキャンセル時
		public  event Callback_purchasFailledEvent purchasFailledEvent;


		//--------------------------------------
		//  PUBLIC METHODS
		//--------------------------------------
		public void ClearAllEventListeners(){
			if(purchasedEvent!=null)purchasedEvent=null;
			if(purchasFailledEvent!=null)purchasFailledEvent=null;
		}
		public void PurchaseProduct(string id){
		    if(!isInvoking && AndroidInAppPurchaseManager.Client.IsConnected){
				isInvoking=true;
				AndroidInAppPurchaseManager.Client.Purchase (id);
			}else{
                Debug.LogError("PurchaseProduct GPGにコネクトされていない");
			}
		}


        public bool IsStoreAvaillable(){

            if(!isInvoking && AndroidInAppPurchaseManager.Client.IsConnected){
                Debug.Log("ストアにコネクトされていない");
                return false;
            }

        if( this.items.prices.Length<=0 || this.items.prices[0]=="--"){
                Debug.Log("ストアがクエリされていない");
                return false;
            }
            return true;
        }
		bool isInvoking=false;

		public void Init(){
                Debug.Log("ストア　初期化");
        if(IsStoreAvaillable()){
            PS_Plugin.Instance.OnStoreInitComplete(true);
            return;
        }
            this.items.prices=new string[Constants.Params.skus.Length];
            this.items.tittles=new string[Constants.Params.skus.Length];
            this.items.descs=new string[Constants.Params.skus.Length];
            this.items.isConsumable=new bool[Constants.Params.skus.Length];
            
                for(int i=0;i<this.items.prices.Length;i++){
                    this.items.prices[i]="--";
                    this.items.tittles[i]="--";
                    this.items.descs[i]="--";
                    this.items.isConsumable[i]=true;
				}

                for(int i=0;i<Constants.Params.skus.Length;i++){
                    AndroidInAppPurchaseManager.Client.AddProduct(Constants.Params.skus[i]);
                }


				AndroidInAppPurchaseManager.ActionProductPurchased += OnProductPurchased;
				AndroidInAppPurchaseManager.ActionProductConsumed += OnProductConsumed;
				AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnBillingConnected;
				AndroidInAppPurchaseManager.Client.Connect();

		}


		
		public void consume(string SKU) {
			AndroidInAppPurchaseManager.Client.Consume (SKU);
		}


        //Init 1
		void OnBillingConnected(BillingResult result) {
		
			AndroidInAppPurchaseManager.ActionBillingSetupFinished -= OnBillingConnected;
            if(result.IsSuccess  &&  !result.IsFailure) {
			    if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log ("ストア　コネクト: Success"+result.IsSuccess );
				//Store connection is Successful. Next we loading product and customer purchasing details
				AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinised;
				AndroidInAppPurchaseManager.Client.RetrieveProducDetails();

    		} else{
                if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.LogError ("ストア　コネクト: "+result.IsSuccess );
                PS_Plugin.Instance.OnStoreInitComplete(false);
    		}
			
		}


        //Init ２
		void OnRetrieveProductsFinised(BillingResult result) {
			AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinised;
              if(result.IsSuccess &&  !result.IsFailure) {
			    if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log ("OnRetrieveProductsFinised  Success");
           
			} else {
			    if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.LogError ("OnRetrieveProductsFinised "+result.IsSuccess );
                 PS_Plugin.Instance.OnStoreInitComplete(false);
				return;
			}
			
			foreach(GoogleProductTemplate p in AndroidInAppPurchaseManager.Client.Inventory.Products) {
				if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log("プロダクトのリストをロードしました: " + p.Title);
				if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( p.SKU + "\n" );

                OnItemsQuery(p.SKU,p.LocalizedPrice.ToString(),p.Description,p.Title,p.ProductType);

				if(isConsumableProduct(p.SKU)){
					if(AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased(p.SKU)) {
						if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log("consume 忘れ " +p.SKU);
                        if(ES2.Exists(DataManger.DataFilename+"?tag=SPOfferLeft"))consume(p.SKU);
					}
				}else{
				    if(AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased(p.SKU)) {
						if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log("リストア　 " +p.SKU);
                    if(ES2.Exists(DataManger.DataFilename+"?tag=SPOfferLeft"))OnPurchaceRecovered(p.SKU);
					}
				}
			}
            PS_Plugin.Instance.OnStoreInitComplete(true);

		}

		

		

        void OnItemsQuery(string id,string price,string desc,string tittle,AN_InAppType type){
			int i=0;
            i=Array.IndexOf(Constants.Params.skus,id);
			if(i>=0){
                this.items.prices[i]= price;
                this.items.tittles[i]=tittle;
                this.items.descs[i]=desc;
                this.items.isConsumable[i]=type==AN_InAppType.NonConsumable?false:true;
			}else{
				if(PS_Plugin.Instance.isDebugMode && isDebugLog)if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.LogError( "not match id for this items"+id );
			}

		}




		void OnProductPurchased(BillingResult result)
		{
			
		if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "OnProductPurchased: "+result.IsSuccess);
			

			if(result.Purchase==null || !result.IsSuccess){
				if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "OnProductPurchased null result: ");
				OnPurchased("",true);
			}else{
			
				if(PS_Plugin.Instance.isDebugMode && isDebugLog){
					if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "OnProductPurchased: "+result.Purchase.SKU);
				}
				if(result.Purchase.State==GooglePurchaseState.PURCHASED){
					if(isConsumableProduct(result.Purchase.SKU)){
						if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "consume first ");
						consume(result.Purchase.SKU);
					}else{
						OnPurchased(result.Purchase.SKU,false);
					}
						
				}else if(result.Purchase.State==GooglePurchaseState.CANCELED){
						OnPurchased(result.Purchase.SKU,true);
				}
			}
             isInvoking=false;
		}
		
        
		void OnProductConsumed(BillingResult result) {

		    if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.LogError ("OnProductConsumed "+result.IsSuccess );

			if(result.IsSuccess) {
				OnPurchased(result.Purchase.SKU,false);
			} else {
                if(purchasFailledEvent!=null)purchasFailledEvent("購入完了も、消費に失敗　再起動せよ");
			}

		}
        //まず　Cosumeして成功したら呼ばれる
       public void OnPurchased(string id,bool isCancel){
            if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "OnPurchased "+id+" "+isCancel);
            if(isCancel){
                if(purchasFailledEvent!=null)purchasFailledEvent(id);
            }else{
           
                if(ES2.Exists(DataManger.DataFilename+"?tag=SPOfferLeft")){
                    if(id==Constants.Params.skus[0]){
                                //90
                    DataManger.Instance.GAMEDATA.Gold+= Constants.Params.purchaseGold[0];
                   
                        }else if(id==Constants.Params.skus[1]){
                             //330
                    ES2.Save(ES2.Load<long>(DataManger.DataFilename+"?tag=Currency_Gold")+ Constants.Params.purchaseGold[1],DataManger.DataFilename+"?tag=Currency_Gold");
                            ES2.Save(true,DataManger.DataFilename+"?tag=isAdFree");
                        }else if(id==Constants.Params.skus[2]){
                            //710
                            ES2.Save(true,DataManger.DataFilename+"?tag=isAdFree");
                    ES2.Save(ES2.Load<long>(DataManger.DataFilename+"?tag=Currency_Gold")+ Constants.Params.purchaseGold[2],DataManger.DataFilename+"?tag=Currency_Gold");
                        }else if(id==Constants.Params.skus[3]){
                            //1900
                            

                    ES2.Save(ES2.Load<long>(DataManger.DataFilename+"?tag=Currency_Gold")+ Constants.Params.purchaseGold[3],DataManger.DataFilename+"?tag=Currency_Gold");
                            ES2.Save(true,DataManger.DataFilename+"?tag=isAdFree");
                        }
                        if(purchasedEvent!=null)purchasedEvent(id);
                }
        }
            ClearAllEventListeners();
        }


		
}

