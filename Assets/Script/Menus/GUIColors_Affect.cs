using UnityEngine;
using System.Collections;

public class GUIColors_Affect : MonoBehaviour {

    public GUIColor color;
	// Use this for initialization
	void Start () {
        gameObject.GetComponent<UIWidget>().color=GUIColors.Instance.GetColor(color);
	}
	
}
