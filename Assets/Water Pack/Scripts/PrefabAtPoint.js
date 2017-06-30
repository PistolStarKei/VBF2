//PrefabAtPoint.js

var object : Transform;
var rate : float;
private var nextCopy : float;

function Update () {
    if(Input.GetKey("mouse 0") && Time.time > nextCopy){
    	nextCopy = Time.time + rate;
    	var ray = GetComponent.<Camera>().ScreenPointToRay(Input.mousePosition);
    	var hit : RaycastHit;
    	if(Physics.Raycast(ray, hit)){
       		var rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
        	Instantiate(object, hit.point, rot);
    	}
    }
}