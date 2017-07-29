using UnityEngine;
using System.Collections;

public class RodParameter : MonoBehaviour {

	//ロッドを持つ手が来るところ
    public Transform gripController;
	//回転軸
    public Transform reelCenter;

	//リールの掴むところ
    public Transform reelHundleToGrab;

	//回転軸
    public Transform reelHundleToRotate;

	//ローカルポジション スポーンするときに使え
   	public Vector3 startLocalPosition;
    public Vector3 startLocalRotation;

}
