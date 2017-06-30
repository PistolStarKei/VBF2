using UnityEngine;
using System.Collections;

public class CameraTankOrbit : MonoBehaviour
{
	public Transform target;
	public bool autoRotate = false;
	public bool isMouseControl = true;
	public float x = 0f;
	public float y = 2f;
	public float distance = 3f;
	public float distanceMin = 1f;
	public float distanceMax = 25f;
	public float offsetY = 0.5f;
	public float xSpeed = 0.1f;
	public float ySpeed = 0.25f;
	public float rotAngleX = 180f;
	public float rotAngleY = 180f;
	public float rotAngleYMin = 5f;
	public float rotAngleYMax = 45f;
	
	void Update()
	{
		if (isMouseControl)
		{
			if (Input.GetMouseButton(1))
				Rotate(Input.GetAxis("Mouse X") * xSpeed, Input.GetAxis("Mouse Y") * ySpeed);
			
			Zoom(Input.GetAxis("Mouse ScrollWheel") * 2);
		}
	}
	
	
	/// <summary>
	/// Zoom camera
	/// </summary>
	public void Zoom(float zoom)
	{
		if (target && zoom != 0)
		{
			distance += zoom;
			if (distance < distanceMin)
				distance = distanceMin;
			if (distance > distanceMax)
				distance = distanceMax;
		}
	}


	/// <summary>
	/// Rotate camera around target
	/// </summary>
	public void Rotate(float x1, float y1)
	{
		x += x1 * xSpeed;
		y -= y1 * ySpeed;
		y = ClampAngle(y, rotAngleYMin, rotAngleYMax);
	}

	void LateUpdate()
	{
		if (target)
		{
			if (autoRotate)
				x += Time.deltaTime * xSpeed;
			
			// Move camera in target position
			transform.position = target.position;
			
			// Rotate camera
			transform.rotation = Quaternion.Euler(y, 
												  target.transform.rotation.eulerAngles.y + x, 
												  transform.rotation.eulerAngles.z);
			
			// Move camera back to distance
			transform.position = transform.position -(transform.forward * distance) +(transform.up * offsetY);
		}
	}

	static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}
}