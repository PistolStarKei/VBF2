using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour
{
	static public float life = 2.0f;
	float timer = 0f;

	// Update is called once per frame
	void Update ()
	{
		if (timer < life)
			timer += Time.deltaTime;
		else
			Destroy(gameObject);
	}
}
