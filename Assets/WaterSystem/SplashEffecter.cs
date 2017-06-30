using UnityEngine;
using System.Collections;

public class SplashEffecter : MonoBehaviour {

public bool alwaysEmitRipples = false;
public int maxEmission = 5000;



//var splashPrefab1 : Transform;
//var splashPrefab2Med : Transform;
//var splashRingsPrefab1 : Transform;
//var splashdirtPrefab1 : Transform;
//var splashdropsPrefab1 : Transform;

private bool isinwater  = false;
private float atDepth = 0.0f;

private Transform splash_rings;
private Transform splash_small;
private Transform splash_med ;
private Transform splash_dirt ;
private Transform splash_drops ;

private float  isPlayingTimer = 0.0f;

private float  setvolumetarget = 0.65f;
private float  setvolume = 0.65f;

private ParticleSystem ringsSystem ;
private ParticleSystem.Particle[] ringsParticles ;
	
private int ringsParticlesNum = 1;

private ParticleSystem ringFoamSystem  ;
private ParticleSystem.Particle[] ringFoamParticles ;
private int ringFoamParticlesNum  = 1;

private ParticleSystem splashSystem ;
private ParticleSystem.Particle[] splashParticles ;
private int splashParticlesNum = 1;

private ParticleSystem splashDropSystem ;
private ParticleSystem.Particle[] splashDropParticles ;
private int splashDropParticlesNum  = 1;



void Start(){
		
		ringsSystem = GameObject.Find("splash_rings_normal_prefab").gameObject.GetComponent<ParticleSystem>();
		ringFoamSystem = GameObject.Find("splash_ringsFoam_prefab").gameObject.GetComponent<ParticleSystem>();
		splashSystem = GameObject.Find("splash_prefab").gameObject.GetComponent<ParticleSystem>();
		splashDropSystem = GameObject.Find("splash_droplets_prefab").gameObject.GetComponent<ParticleSystem>();


	
}









public void AddEffect(string addMode ,  Vector3 effectPos,int addRate ,float addSize , float addRot ){

int px = 0;
	if (addMode == "rings"){
	ringsSystem.Emit(addRate);
	//get particles
	ringsParticles = new ParticleSystem.Particle[ringsSystem.particleCount];
			
	ringsSystem.GetParticles(ringsParticles);
	//set particles
	for (px = (ringsSystem.particleCount-addRate); px < ringsSystem.particleCount; px++){
			//set position
			ringsParticles[px].position=new Vector3(effectPos.x,effectPos.y,effectPos.z);
			//set variables
			ringsParticles[px].size = addSize;
			ringsParticles[px].rotation = addRot;
	}
	ringsSystem.SetParticles(ringsParticles,ringsParticles.Length);
	ringsSystem.Play();
}


if (addMode == "ringfoam"){
	ringFoamSystem.Emit(addRate);
	//get particles
	ringFoamParticles = new ParticleSystem.Particle[ringFoamSystem.particleCount];
	ringFoamSystem.GetParticles(ringFoamParticles);
	//set particles
	for (px = (ringFoamSystem.particleCount-addRate); px < ringFoamSystem.particleCount; px++){
			//set position
			ringFoamParticles[px].position=new Vector3(effectPos.x,effectPos.y,effectPos.z);
		
			//set variables
			ringFoamParticles[px].size = addSize;
			ringFoamParticles[px].rotation = addRot;
	}
	ringFoamSystem.SetParticles(ringFoamParticles,ringFoamParticles.Length);
	ringFoamSystem.Play();
}




if (addMode == "splash"){
	splashSystem.Emit(addRate);
	//get particles
	splashParticles = new ParticleSystem.Particle[splashSystem.particleCount];
	splashSystem.GetParticles(splashParticles);
	//set particles
	for (px = (splashSystem.particleCount-addRate); px < splashSystem.particleCount; px++){
			//set position
			splashParticles[px].position=new Vector3(effectPos.x,effectPos.y,effectPos.z);
			
	}
	splashSystem.SetParticles(splashParticles,splashParticles.Length);
	splashSystem.Play();
}



if (addMode == "splashDrop"){
	splashDropSystem.Emit(addRate);
	//get particles
	splashDropParticles = new ParticleSystem.Particle[splashDropSystem.particleCount];
	splashDropSystem.GetParticles(splashDropParticles);
	//set particles
	for (px = (splashDropSystem.particleCount-addRate); px < splashDropSystem.particleCount; px++){
			//set position
			splashDropParticles[px].position=new Vector3(effectPos.x,effectPos.y,effectPos.z);
			
	}
	splashDropSystem.SetParticles(splashDropParticles,splashDropParticles.Length);
	splashDropSystem.Play();
}


}

}
