using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ShakeCam : MonoBehaviour {

	#region Internal Classes
	
	[System.Serializable]
	public class ShakeEffect
	{
		public Transform camera;
		public int repeats = 20;
		public float speed = 40;
		public Vector2 distance = new Vector2(0.2f, 0.2f);
		[HideInInspector]
		public int currentRepeats = 0;
		[HideInInspector]
		public Vector3 mCameraPos;
		[HideInInspector]
		public Vector3 shakePos = Vector3.zero;
	}
	
	#endregion
	
	#region Public Variables
	public float refreshRate = 0.01f;
	public ShakeEffect shakeEffect = new ShakeEffect();
	public bool ignoreTimeScale=true;
	#endregion
	
	#region Private Variables
	static bool shaking = false;

	#endregion
	
	#region Private Methods

	void Start()
	{
		shakeEffect.currentRepeats = shakeEffect.repeats;
		if (shakeEffect.camera == null) shakeEffect.camera = Camera.main.transform;
		shakeEffect.mCameraPos = shakeEffect.camera.localPosition;
	}
	

	


	IEnumerator ShakeCamera()
	{
		shakeEffect.mCameraPos = shakeEffect.camera.localPosition;

		while (shaking)
		{
			shakeEffect.currentRepeats--;
			//Calculate percentage of blend between stationary and bobbing camera positions
			float percentage = 1;// Mathf.Min(1, Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal")));
			
			//Calculate desired x position
			float desiredPosX=0.0f;
			if(ignoreTimeScale){
				desiredPosX = shakeEffect.distance.x * Mathf.Sin(Time.realtimeSinceStartup * shakeEffect.speed + Mathf.PI / 2);

			}else{
				desiredPosX = shakeEffect.distance.x * Mathf.Sin(Time.time * shakeEffect.speed + Mathf.PI / 2);

			}
			//Blend between stationary and desired x position
			float newX = ((1 - percentage)) + (desiredPosX * (percentage));
			
			//Calculate desired y position
			float desiredPosY=0.0f;
			if(ignoreTimeScale){
				desiredPosY = shakeEffect.distance.y * Mathf.Sin(Time.realtimeSinceStartup * 2 * shakeEffect.speed);
			
			}else{
				desiredPosY = shakeEffect.distance.y * Mathf.Sin(Time.realtimeSinceStartup * 2 * shakeEffect.speed);
			}
			//Blend between stationary and desired y position
			float newY = ((1 - percentage)) + (desiredPosY * (percentage));
			
			shakeEffect.shakePos.x = shakeEffect.mCameraPos.x + newX;
			shakeEffect.shakePos.y = shakeEffect.mCameraPos.y + newY;
			shakeEffect.shakePos.z = shakeEffect.mCameraPos.z;
			
			shakeEffect.camera.localPosition = shakeEffect.shakePos;
			
			if (shakeEffect.currentRepeats <= 0)
			{
				if (shakeEffect.camera.localPosition != shakeEffect.mCameraPos) shakeEffect.camera.localPosition = shakeEffect.mCameraPos;
				shaking = false;
			}
			if(ignoreTimeScale){
				yield return WaitForSecondsIgnoreTimeScale (refreshRate);
			}else{
				yield return new WaitForSeconds(refreshRate);
			}
		}

	}
	IEnumerator WaitForSecondsIgnoreTimeScale (float pauseTime)
	{
		float pauseFinishTime = Time.realtimeSinceStartup + pauseTime;
		while (Time.realtimeSinceStartup < pauseFinishTime) {
			yield return 0;
		}
	}
	#endregion
	
	#region Public Methods

	public void Shake()
	{
		if (!shaking)
		{

			shakeEffect.currentRepeats = shakeEffect.repeats;
			shaking = true;
			StartCoroutine(ShakeCamera());
		}
	}
	
	#endregion
}
