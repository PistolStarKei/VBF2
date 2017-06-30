using UnityEngine;

[ExecuteInEditMode]
public class RenderReflRefr : MonoBehaviour {
	public LayerMask ReflectLayers = -1;
	public LayerMask RefractLayers = -1;
	public float ClipPlaneOffset = 0.07f;
	private RenderTexture reflTex = null;
	private RenderTexture refrTex = null;
	private Camera reflCam = null;
	private Camera refrCam = null;
	private static bool SafeGuard = false;
	
	public void OnWillRenderObject() {
		//Check if all necessities are supported
		if(!enabled || !GetComponent<Renderer>() || !GetComponent<Renderer>().sharedMaterial || !GetComponent<Renderer>().enabled)
			return;

		if( !SystemInfo.supportsRenderTextures)
			return;

		Camera cam = Camera.current;
		if(!cam)
			return;

		// Safeguard from recursive reflections.		
		if(SafeGuard)
			return;
		SafeGuard = true;

		//We are good to go
		CreateTextures(256, 256, 16);
		CreateCameras(cam);
		UpdateCameras(cam);

		//REFLECTION
		//Get plane and calculate reflection
		Vector3 pos = transform.position;
		Vector3 normal = transform.up;
		Vector4 plane = GetPlaneEquation(normal, pos, ClipPlaneOffset);
		Matrix4x4 reflMtx = Matrix4x4.zero;
		SetReflectionMatrix(ref reflMtx, plane);
		Vector3 oldpos = cam.transform.position;
		Vector3 newpos = reflMtx.MultiplyPoint(oldpos);
		reflCam.worldToCameraMatrix = cam.worldToCameraMatrix * reflMtx;
		//Setup oblique projection matrix so that near plane is our reflection
		//plane. This way we clip everything below/above it for free.
		Vector4 reflClipPlane = CameraSpacePlane(reflCam, pos, normal, ClipPlaneOffset, 1.0f);
		Matrix4x4 reflProjMtx = cam.projectionMatrix;
		SetObliqueMatrix(ref reflProjMtx, reflClipPlane);
		reflCam.projectionMatrix = reflProjMtx;
		//Render the reflection here
		GL.SetRevertBackfacing(true);
		reflCam.transform.position = newpos;
		Vector3 euler = cam.transform.eulerAngles;
		reflCam.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);
		reflCam.Render();
		reflCam.transform.position = oldpos;
		GL.SetRevertBackfacing(false);
		//Set the reflection variable in the shader
		GetComponent<Renderer>().sharedMaterial.SetTexture("_ReflectionTex", reflTex);

		//REFRACTION
		refrCam.worldToCameraMatrix = cam.worldToCameraMatrix;
		// Setup oblique projection matrix so that near plane is our reflection
		// plane. This way we clip everything below/above it for free.
		Vector4 refrClipPlane = CameraSpacePlane(refrCam, pos, normal, ClipPlaneOffset, -1.0f );
		Matrix4x4 refrProjMtx = cam.projectionMatrix;
		SetObliqueMatrix(ref refrProjMtx, refrClipPlane);
		refrCam.projectionMatrix = refrProjMtx;
		//Render the refraction here
		refrCam.transform.position = cam.transform.position;
		refrCam.transform.rotation = cam.transform.rotation;
		refrCam.Render();
		GetComponent<Renderer>().sharedMaterial.SetTexture("_RefractionTex", refrTex);
		
		SafeGuard = false;
	}

	void OnBecameVisible() {
		enabled = true;
	}

	void OnBecameInvisible() {
		enabled = false;
	}

	void OnDisable() {
		if(reflTex) {
			DestroyImmediate(reflTex);
			reflTex = null;
		}
		if(reflCam) {
			DestroyImmediate(reflCam.gameObject);
			reflCam = null;
		}
		if(refrTex) {
			DestroyImmediate(refrTex);
			refrTex = null;
		}
		if(refrCam) {
			DestroyImmediate(refrCam.gameObject);
			refrCam = null;
		}
	}

	void OnDestroy() {
		//Just in case
		if(reflTex) {
			DestroyImmediate(reflTex);
			reflTex = null;
		}
		if(reflCam) {
			DestroyImmediate(reflCam.gameObject);
			reflCam = null;
		}
		if(refrTex) {
			DestroyImmediate(refrTex);
			refrTex = null;
		}
		if(refrCam) {
			DestroyImmediate(refrCam.gameObject);
			refrCam = null;
		}
	}

	//Create Textures
	private void CreateTextures(int width, int height, int depth) {
		if(!reflTex) {
			reflTex = new RenderTexture(width, height, depth);
			reflTex.name = "ReflectTexture";
			reflTex.isPowerOfTwo = true;
			reflTex.hideFlags = HideFlags.DontSave;
		}
		if(!refrTex) {
			refrTex = new RenderTexture(width, height, depth);
			refrTex.name = "RefractTexture";
			refrTex.isPowerOfTwo = true;
			refrTex.hideFlags = HideFlags.DontSave;
		}
	}

	//Create cameras
	private void CreateCameras(Camera current) {
		if(!reflCam) {
			GameObject go = new GameObject("ReflectCamera" + GetInstanceID(), typeof(Camera), typeof(Skybox));
			reflCam = go.GetComponent<Camera>();
			reflCam.enabled = false;
			reflCam.transform.position = transform.position;
			reflCam.transform.rotation = transform.rotation;
			reflCam.gameObject.AddComponent<FlareLayer>();
			go.hideFlags = HideFlags.HideAndDontSave;
		}
		if(!refrCam) {
			GameObject go = new GameObject("RefractCamera" + GetInstanceID(), typeof(Camera), typeof(Skybox));
			refrCam = go.GetComponent<Camera>();
			refrCam.enabled = false;
			refrCam.transform.position = transform.position;
			refrCam.transform.rotation = transform.rotation;
			refrCam.gameObject.AddComponent<FlareLayer>();
			go.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	//Update cameras information
	private void UpdateCameras(Camera current) {
		if (reflCam == null || refrCam == null)
			return;
		reflCam.backgroundColor = current.backgroundColor;
		refrCam.backgroundColor = current.backgroundColor;
		if(current.clearFlags == CameraClearFlags.Skybox) {
			Skybox sky = current.GetComponent(typeof(Skybox)) as Skybox;
			Skybox reflSky = reflCam.GetComponent(typeof(Skybox)) as Skybox;
			Skybox refrSky = refrCam.GetComponent(typeof(Skybox)) as Skybox;
			if(!sky || !sky.material) {
				reflSky.enabled = false;
				refrSky.enabled = false;
			}
			else {
				reflSky.enabled = true;
				reflSky.material = sky.material;
				refrSky.enabled = true;
				refrSky.material = sky.material;
			}
		}
		reflCam.farClipPlane = current.farClipPlane;
		reflCam.nearClipPlane = current.nearClipPlane;
		reflCam.orthographic = current.orthographic;
		reflCam.fieldOfView = current.fieldOfView;
		reflCam.aspect = current.aspect;
		reflCam.orthographicSize = current.orthographicSize;
		reflCam.cullingMask = ~(1<<4) & ReflectLayers.value;
		reflCam.targetTexture = reflTex;

		refrCam.farClipPlane = current.farClipPlane;
		refrCam.nearClipPlane = current.nearClipPlane;
		refrCam.orthographic = current.orthographic;
		refrCam.fieldOfView = current.fieldOfView;
		refrCam.aspect = current.aspect;
		refrCam.orthographicSize = current.orthographicSize;
		refrCam.cullingMask = ~(1<<4) & RefractLayers.value;
		refrCam.targetTexture = refrTex;
	}

	//Plane equation from a normal and a position
	private static Vector4 GetPlaneEquation(Vector3 normal, Vector3 position, float offset) {
		float d = -Vector3.Dot(normal, position) - offset;
		return new Vector4(normal.x, normal.y, normal.z, d);
	}

	// Given position/normal of the plane, calculates plane in camera space.
	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 norm, float offs, float side) {
		Vector3 offsetPos = pos + (norm * offs);
		Matrix4x4 mtx = cam.worldToCameraMatrix;
		Vector3 cPos = mtx.MultiplyPoint(offsetPos);
		Vector3 cNorm = mtx.MultiplyVector(norm).normalized * side;
		return new Vector4(cNorm.x, cNorm.y, cNorm.z, -Vector3.Dot(cPos,cNorm));
	}

	// Extended sign: returns -1, 0 or 1 based on sign of a
	private static float Sgn(float a) {
		if (a > 0.0f) return 1.0f;
		if (a < 0.0f) return -1.0f;
		return 0.0f;
	}

	// http://aras-p.info/texts/obliqueortho.html
	private static void SetObliqueMatrix(ref Matrix4x4 proj, Vector4 plane) {
		Vector4 q = proj.inverse * new Vector4(Sgn(plane.x), Sgn(plane.y), 1.0f, 1.0f);
		Vector4 c = plane * (2.0f / (Vector4.Dot(plane, q)));
		// third row = clip plane - fourth row
		proj[2] = c.x - proj[3]; 	proj[6] = c.y - proj[7];
		proj[10] = c.z - proj[11]; 	proj[14] = c.w - proj[15];
	}

	// Calculates reflection matrix around the given plane
	private static void SetReflectionMatrix(ref Matrix4x4 mtx, Vector4 plane) {
		mtx.m00 = (1f - 2f * plane[0] * plane[0]); 	mtx.m01 = (-2f * plane[0] * plane[1]);
		mtx.m02 = (-2f * plane[0] * plane[2]);		mtx.m03 = (-2f * plane[3] * plane[0]);
		mtx.m10 = (-2f * plane[1] * plane[0]); 		mtx.m11 = (1f - 2f * plane[1] * plane[1]);
		mtx.m12 = (-2f * plane[1] * plane[2]); 		mtx.m13 = (-2f * plane[3] * plane[1]);
		mtx.m20 = (-2f * plane[2] * plane[0]); 		mtx.m21 = (-2f * plane[2] * plane[1]);
		mtx.m22 = (1f - 2f * plane[2] * plane[2]);	mtx.m23 = (-2f * plane[3] * plane[2]);
		
		mtx.m30 = 0f; mtx.m31 = 0f; mtx.m32 = 0f; mtx.m33 = 1f;
	}
}