using UnityEngine;
using System.Collections;
using PathologicalGames;

public class SplashParticle : MonoBehaviour {

	public ParticleEmitter[] particleEmitters;
	// Use this for initialization
	private bool firstUpdate = true;
	private float[] minsize;
	private float[] maxsize;
	private Vector3[] worldvelocity;
	private Vector3[] localvelocity;
	private Vector3[] rndvelocity;
	private Vector3[] scaleBackUp;

	void DestroyMe(){
        Debug.Log("DestroyMe()");
        PoolManager.Pools["Non_GUI_Effects"].Despawn(transform);
	}
	public void UpdateScale (float scale) {  
        Debug.Log("UpdateScale");
		int length = particleEmitters.Length;
			minsize = new float[length];
			maxsize = new float[length];
			worldvelocity = new Vector3[length];
			localvelocity = new Vector3[length];
			rndvelocity = new Vector3[length];
			scaleBackUp = new Vector3[length];

		for (var i = 0; i < particleEmitters.Length; i++) { 
				minsize[i] = particleEmitters[i].minSize;
				maxsize[i] = particleEmitters[i].maxSize;
				worldvelocity[i] = particleEmitters[i].worldVelocity;
				localvelocity[i] = particleEmitters[i].localVelocity;
				rndvelocity[i] = particleEmitters[i].rndVelocity;
				scaleBackUp[i] = particleEmitters[i].transform.localScale;
			particleEmitters[i].minSize = minsize[i] * scale;
			particleEmitters[i].maxSize = maxsize[i] * scale;
			particleEmitters[i].worldVelocity = worldvelocity[i] * scale;
			particleEmitters[i].localVelocity = localvelocity[i] * scale;
			particleEmitters[i].rndVelocity = rndvelocity[i] * scale;
			particleEmitters[i].transform.localScale = scaleBackUp[i] * scale;

		}
		Invoke("DestroyMe",1.0f);
	}
}
