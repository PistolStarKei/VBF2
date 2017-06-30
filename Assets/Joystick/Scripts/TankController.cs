using UnityEngine;
using System.Collections;

public class TankController : MonoBehaviour
{
	public float kSpeed = 50f;
	public float angleRotation = 45f;
	public float turretAngleRotation = 5f;
	public float powerShoot = 250f;
	public float delayShoot = 0.5f;
	float timeLastShoot = 0f;
	bool isShoot = false;
	
	public Object shellPrefab;
	public Transform gun;
	public Transform turret;
	
	void Start(){
		Time.timeScale = 0.0f;
	}
	void Update()
	{
		timeLastShoot += Time.deltaTime;
		if (isShoot && timeLastShoot >= delayShoot)
		{
			// Shoot
			GameObject newShell = (GameObject) Instantiate(shellPrefab, gun.position + gun.forward * 1.8f, Quaternion.identity);
			Physics.IgnoreCollision(GetComponent<Collider>(), newShell.GetComponent<Collider>());
			newShell.GetComponent<Rigidbody>().AddRelativeForce(turret.forward * powerShoot, ForceMode.Impulse);
			isShoot = false;
			timeLastShoot = 0f;
		}
	}
	
	
	public void Move (float x, float y)
	{
		// Move forward
		GetComponent<Rigidbody>().velocity = transform.forward * kSpeed * y;
		
		// Rotate
		if (x > 0)
		{
			// right
			float newAngleRotation = angleRotation * Time.deltaTime;
			GetComponent<Rigidbody>().MoveRotation(GetComponent<Rigidbody>().rotation * Quaternion.Euler (0, newAngleRotation, 0));
		}
		else if (x < 0)
		{
			// left
			float newAngleRotation = -1 * angleRotation * Time.deltaTime;
			GetComponent<Rigidbody>().MoveRotation(GetComponent<Rigidbody>().rotation * Quaternion.Euler (0, newAngleRotation, 0));
		}
	}
	
	
	public void Shoot()
	{
		isShoot = true;
	}
	
	
	public void TurretRotate(float rot)
	{
		Vector3 eulerAngles = turret.localRotation.eulerAngles;
		
		float angle = eulerAngles.y;
		if (angle > 180)
			angle = angle - 360f;
		
		float f = rot * Mathf.Rad2Deg;
		float delta = 0f;
		if (f > angle)
		{
			if (angle < -90f && f > 90f)
				delta = -turretAngleRotation * Time.deltaTime;
			else
				delta = turretAngleRotation * Time.deltaTime;
		}
		else if (f < angle)
		{
			if (f < -90f && angle > 90f)
				delta = turretAngleRotation * Time.deltaTime;
			else
				delta = -turretAngleRotation * Time.deltaTime;
		}
		
		turret.localRotation = Quaternion.Euler(eulerAngles.x, angle + delta, eulerAngles.z);
	}
}
