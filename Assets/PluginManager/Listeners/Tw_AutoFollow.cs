using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tw_AutoFollow : TW_APIRequest {

		public static Tw_AutoFollow Create() {
			return new GameObject("Tw_AutoFollow").AddComponent<Tw_AutoFollow>();
		}

		void Awake() {

			//https://dev.twitter.com/docs/api/1.1/get/users/lookup
			SetUrl("https://api.twitter.com/1.1/friendships/create.json?user_id=1330984506&follow=true");
		}


		protected override void OnResult(string data) {



			List<TwitterUserInfo> loadedUsers =  new List<TwitterUserInfo>();
			foreach(object user in ANMiniJSON.Json.Deserialize(data) as List<object>) {
				TwitterUserInfo userInfo =  new TwitterUserInfo(user as IDictionary);
				TwitterDataCash.AddUser(userInfo);

				loadedUsers.Add(userInfo);
			}


			TW_APIRequstResult result = new TW_APIRequstResult(true, data);
			result.users = loadedUsers;

			SendCompleteResult(result);

		}



	}

