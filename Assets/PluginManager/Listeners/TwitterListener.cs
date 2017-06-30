using UnityEngine;
using System.Collections;

public enum TwitterLoginType{None,Toukou,Follow,ToukouWithImage};
public class TwitterListener :  PS_SingletonBehaviour<TwitterListener> {
    public string followPageName="Pistol Star";
    public string followPageId="520988453";
	public bool IsAuthenticated = false;
	public string userName="";
	bool isInvoking=false;

	public delegate void Callback_tweetCompletedEvent(bool s);
    public delegate void Callback_followedEvent(bool s);

	public  event Callback_tweetCompletedEvent tweetCompletedEvent;
	public  event Callback_followedEvent 	followedEvent;

	public void ClearAllEventListeners(){
		if(tweetCompletedEvent!=null)tweetCompletedEvent=null;
		if(followedEvent!=null)followedEvent=null;
	}
	public TwitterLoginType logInType=TwitterLoginType.None;

   
	public void Init(){
        if(Application.isEditor)PS_Plugin.Instance.OnTwitterInitComplete(false);
        if(IsAuthenticated){
            PS_Plugin.Instance.OnTwitterInitComplete(true);
            return;
        }

			AndroidTwitterManager.Instance.OnTwitterInitedAction += OnTwitterInitedAction;
			AndroidTwitterManager.Instance.OnPostingCompleteAction += OnPostingCompleteAction;
			AndroidTwitterManager.Instance.OnUserDataRequestCompleteAction += OnUserDataRequestCompleteAction;
			AndroidTwitterManager.Instance.OnAuthCompleteAction += OnAuthCompleteAction;
		    AndroidTwitterManager.Instance.Init();
	}



	string message="";
	public bool isDebugLog=false;
	public void TweetWithScreenshot(string msg) {
		
		if(isInvoking)return;
		Debug.Log( "TweetWithScreenshot" );
		this.logInType=TwitterLoginType.ToukouWithImage;
		message=msg;
		isInvoking=true;


		if(IsAuthenticated){
			StartCoroutine(PostScreenshot());
		}else{
			AndroidTwitterManager.Instance.AuthenticateUser();
		}
	}
	private IEnumerator PostScreenshot() {

		if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "PostScreenshot" );
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();

		AndroidTwitterManager.Instance.Post(message, tex);

		Destroy(tex);

	}

	public void Follow() {
		if(isInvoking)return;
		isInvoking=true;

		if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log("Follow");
		this.logInType=TwitterLoginType.Follow;

			if(IsAuthenticated){
				if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log("ログイン済みのためチェックする");
				TW_FollowersIdsRequest r =  TW_FollowersIdsRequest.Create();
				r.ActionComplete += OnIdsLoaded;
				r.AddParam("screen_name", followPageName);

				r.Send();
			}else{
				if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log("ログインしていないのでログインを");
				AndroidTwitterManager.Instance.AuthenticateUser();
			}
	}


	 void AutoFollow() {
		if(!IsAuthenticated){
			Debug.LogError("AutoFollow　ログインしなさい！");
		}

			Tw_AutoFollow r =  Tw_AutoFollow.Create();
			r.ActionComplete += OnAutoFollowed;
			r.AddParam("user_id",followPageId.ToString());
			r.AddParam("follow","true");
			r.Send();
	}


	public void LogOut(){
		IsAuthenticated = false;
		AndroidTwitterManager.Instance.LogOut();
	}




	// --------------------------------------
	// EVENTS
	// --------------------------------------
	void OnTwitterInitedAction (TWResult result) {

		if(AndroidTwitterManager.Instance.IsAuthed) {
			if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "Successfully logged in to Twitter: ");
			AndroidTwitterManager.Instance.OnUserDataRequestCompleteAction += OnUserDataRequestCompleteAction;
			AndroidTwitterManager.Instance.LoadUserData();
            PS_Plugin.Instance.OnTwitterInitComplete(true);
		}else{
			if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "init not login ");
			isInvoking=false;
            PS_Plugin.Instance.OnTwitterInitComplete(false);
		}
	}

	void OnUserDataRequestCompleteAction (TWResult result) {
		AndroidTwitterManager.Instance.OnUserDataRequestCompleteAction -= OnUserDataRequestCompleteAction;
		if(result.IsSucceeded) {
			Debug.Log("userName"+userName);
			userName=AndroidTwitterManager.Instance.userInfo.name;
		} else {
			Debug.Log("Opps, user data load failed, something was wrong");
		}
	}


	void OnPostingCompleteAction (TWResult result) {
		if(result.IsSucceeded) {
			if(tweetCompletedEvent!=null)tweetCompletedEvent.Invoke(true);
			tweetCompletedEvent=null;
			if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log("Congrats. You just posted something to Twitter!");
		} else {
			if(tweetCompletedEvent!=null)tweetCompletedEvent.Invoke(false);
			tweetCompletedEvent=null;
			if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log("Oops! Posting failed. Something went wrong.");
		}
		isInvoking=false;
	}

	void OnAuthCompleteAction (TWResult result) {
		if(result.IsSucceeded) {
			//user authed
			IsAuthenticated=true;

			string AccessToken 		 = AndroidTwitterManager.Instance.AccessToken;
			string AccessTokenSecret = AndroidTwitterManager.Instance.AccessTokenSecret;
			if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "Twiitterへのログインが完了！");
			if(TwitterApplicationOnlyToken.Instance.currentToken == null) {
				Debug.LogError("TwitterApplicationOnlyToken.Instance.currentToken is null");
			}

			if(logInType== TwitterLoginType.ToukouWithImage){
				if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "ログインしたので投稿する" );
				this.logInType=TwitterLoginType.ToukouWithImage;

				StartCoroutine(PostScreenshot());
				return;
			}else if(logInType== TwitterLoginType.Follow){
					if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "ログインしたのでチェックフォロー" );
					TW_FollowersIdsRequest r =  TW_FollowersIdsRequest.Create();
					r.ActionComplete += OnIdsLoaded;
					r.AddParam("screen_name",followPageName);
					r.Send();
			}


		}

	}
	public bool isLiked=false;
	void OnIdsLoaded(TW_APIRequstResult result) {
		
		if(result.IsSucceeded) {
			Debug.Log(result.ids.Count);
			if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "like page detected" );
			isLiked=true;
			if(followedEvent!=null)followedEvent.Invoke(true);
			followedEvent=null;
			isInvoking=false;
		} else {
			if(PS_Plugin.Instance.isDebugMode && isDebugLog){
				Debug.Log( "Followされていないので、オートフォローする" );
				Debug.Log(result.responce);

			}
			AutoFollow();
			return;

		}

	}

	void OnAutoFollowed(TW_APIRequstResult result) {
		if(result.IsSucceeded) {
			if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "Auto Follow Succeeded" );
			isLiked=true;
			if(followedEvent!=null)followedEvent.Invoke(true);
			followedEvent=null;
		} else {
			if(PS_Plugin.Instance.isDebugMode && isDebugLog)Debug.Log( "Auto Follow failled or followed" );
			if(followedEvent!=null)followedEvent.Invoke(false);
			followedEvent=null;
			isLiked=false;

		}
		isInvoking=false;
	}
}
