using UnityEngine;
using System.Collections;

public class PGUI_Utilities : MonoBehaviour {

		public static IEnumerator WaitForRealTime(float delay){
			while(true){
				float pauseEndTime = Time.realtimeSinceStartup + delay;
				while (Time.realtimeSinceStartup < pauseEndTime){
					yield return 0;
				}
				break;
			}
		}

}
