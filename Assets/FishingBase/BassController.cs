using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BassController : MonoBehaviour {

	public float _spawnSphere = 3f;				// Range around the spawner waypoints will created //changed to box
	public float _spawnSphereDepth = 3f;			
	public float _spawnSphereHeight = 1.5f;		
	public float _childSpeedMultipler = 2f;		// Adjust speed of entire school
	public float _minSpeed = 6f;					// minimum random speed
	public float _maxSpeed = 10f;				// maximum random speed
	public AnimationCurve _speedCurveMultiplier = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
	public float _minScale = .7f;				// minimum random size
	public float _maxScale = 1f;					// maximum random size
	public float _minDamping = 1f;				// Rotation tween damping, lower number = smooth/slow rotation (if this get stuck in a loop, increase this value)
	public float _maxDamping = 2f;
	public float _waypointDistance = 1f;			// How close this can get to waypoint before creating a new waypoint (also fixes stuck in a loop)
	public float _minAnimationSpeed = 2f;
	public float _maxAnimationSpeed = 4f;		
	public float _randomPositionTimerMax = 10f;	// When _autoRandomPosition is enabled
	public float _randomPositionTimerMin = 4f;	
	public float _acceleration = .025f;			// How fast child speeds up
	public float _brake = .01f;					// How fast child slows down 
	public float _positionSphere = 25f;			// If _randomPositionTimer is bigger than zero the controller will be moved to a random position within this sphere
	public float _positionSphereDepth = 5f;		// Overides height of sphere for more controll
	public float _positionSphereHeight = 5f;		// Overides height of sphere for more controll
	public bool  _autoRandomPosition;			// Automaticly positions waypoint based on random values (_randomPositionTimerMin, _randomPositionTimerMax)
	public float _forcedRandomDelay = 1.5f;		// Random delay added before forcing new waypoint
	public float _schoolSpeed;					// Value multiplied to child speed
	public Vector3 _posBuffer;

	///AVOIDANCE
	public bool _avoidance;				//Enable/disable avoidance
	public float _avoidAngle = 0.35f; 		//Angle of the rays used to avoid obstacles left and right
	public float _avoidDistance = 1f;		//How far avoid rays travel
	public float _avoidSpeed = 75f;			//How fast this turns around when avoiding	
	public float _stopDistance	= 0.5f;		//How close this can be to objects directly in front of it before stopping and backing up. This will also rotate slightly, to avoid "robotic" behaviour
	public float _stopSpeedMultiplier = 2f;	//How fast to stop when within stopping distance

	///PUSH
	public bool _push;					//Enable/disable push
	public float _pushDistance;				//How far away obstacles can be before starting to push away	
	public float _pushForce = 5;			//How fast/hard to push away

	//FRAME SKIP
	public int _updateDivisor = 1;				//Skip update every N frames (Higher numbers might give choppy results, 3 - 4 on 60fps , 2 - 3 on 30 fps)
	public float _newDelta;
	public int _updateCounter;
	public int _activeChildren;

	void Start () {
		_posBuffer = transform.position;	
		_schoolSpeed = Random.Range(1 , _childSpeedMultipler);
		Invoke("AutoRandomWaypointPosition", RandomWaypointTime());

		SetRandomScale();			
		LocateRequiredChildren();
		RandomizeStartAnimationFrame();
		SkewModelForLessUniformedMovement();
		_speed = Random.Range(_minSpeed, _maxSpeed);
		Wander(0);
		SetRandomWaypoint();
		_instantiated = true;
		GetStartPos();
		FrameSkipSeedInit();
		_activeChildren++;
	}

	void Update () {
			if(_updateDivisor > 1){
				_updateCounter++;
				_updateCounter = _updateCounter % _updateDivisor;	
				_newDelta = Time.deltaTime*_updateDivisor;	
			}else{
				_newDelta = Time.deltaTime;
			}

		if (_updateDivisor <=1 || _updateCounter == _updateSeed){
			CheckForDistanceToWaypoint();
			RotationBasedOnWaypointOrAvoidance();
			ForwardMovement();
			RayCastToPushAwayFromObstacles();
			SetAnimationSpeed();
		}
	}
		

	//Set waypoint randomly inside box
	void SetRandomWaypointPosition() {
		_schoolSpeed = Random.Range(1 , _childSpeedMultipler);
		Vector3 t;
		t.x = Random.Range(-_positionSphere, _positionSphere) + transform.position.x;
		t.z = Random.Range(-_positionSphereDepth, _positionSphereDepth) + transform.position.z;
		t.y = Random.Range(-_positionSphereHeight, _positionSphereHeight) + transform.position.y;
		_posBuffer = t;	
	}

	void AutoRandomWaypointPosition () {
		if(_autoRandomPosition && _activeChildren > 0){
			SetRandomWaypointPosition();
		}
		CancelInvoke("AutoRandomWaypointPosition");
		Invoke("AutoRandomWaypointPosition", RandomWaypointTime());
	}

	float RandomWaypointTime(){
		return Random.Range(_randomPositionTimerMin, _randomPositionTimerMax);
	}

	void OnDrawGizmos () {
		if(!Application.isPlaying && _posBuffer != transform.position) _posBuffer = transform.position;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube (_posBuffer,new  Vector3(_spawnSphere*2, _spawnSphereHeight*2 ,_spawnSphereDepth*2));
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube (transform.position,new Vector3((_positionSphere*2)+_spawnSphere*2, (_positionSphereHeight*2)+_spawnSphereHeight*2 ,(_positionSphereDepth*2)+_spawnSphereDepth*2));
	}
		
	private Vector3 _wayPoint ;
	public float _speed= 10;				//Fish Speed
	private float _stuckCounter;			//prevents looping around a waypoint
	private float _damping;					//Turn speed
	public Transform _model;				//Model with animations
	private float _targetSpeed;				//Fish target speed
	private float tParam = 0;				//
	private float _rotateCounterR;			//Used to increase avoidance speed over time
	private float _rotateCounterL;			
	public Transform _scanner;				//Scanner object used for push, this rotates to check for collisions
	private bool _scan= true;			
	private bool _instantiated;			//Has this been instantiated
	private static int  _updateNextSeed = 0;	//When using frameskip seed will prevent calculations for all fish to be on the same frame
	private int _updateSeed = -1;


	#if UNITY_EDITOR
	static bool _sWarning;
	#endif



	void FrameSkipSeedInit(){
		if(_updateDivisor > 1){
			int _updateSeedCap = _updateDivisor -1;
			_updateNextSeed++;
			this._updateSeed = _updateNextSeed;
			_updateNextSeed = _updateNextSeed % _updateSeedCap;
		}
	}

	void LocateRequiredChildren(){
		if(!_model) _model = transform.FindChild("Model");
		if(!_scanner){
			_scanner = new GameObject().transform;
			_scanner.transform.parent = this.transform;
			_scanner.transform.localRotation = Quaternion.identity;
			_scanner.transform.localPosition = Vector3.zero;
			#if UNITY_EDITOR
			if(!_sWarning){
				Debug.Log("No scanner assigned: creating... (Increase instantiate performance by manually creating a scanner object)");
				_sWarning = true;
			}
			#endif
		}
	}

	void SkewModelForLessUniformedMovement () {
		// Adds a slight rotation to the model so that the fish get a little less uniformed movement	
		_model.transform.rotation =	Quaternion.Euler(new Vector3(0f, 0f , Random.Range(-25f, 25f)));
	}

	void SetRandomScale(){
		float sc = Random.Range(_minScale, _maxScale);
		transform.localScale=Vector3.one*sc;
	}

	void RandomizeStartAnimationFrame(){
		foreach (AnimationState state  in _model.GetComponent<Animation>()) {
			state.time = Random.value * state.length;
		}
	}

	void GetStartPos(){
		RaycastHit hit ;
		if (Physics.Raycast(transform.position, _wayPoint,out hit, Vector3.Distance(transform.position, _wayPoint))){	
			transform.position = hit.point;
			return;
		}
		//-Vector is to avoid zero rotation warning
		transform.position = _wayPoint - new Vector3(.1f,.1f,.1f);
	}

	Vector3 findWaypoint(){
		Vector3 t;
		t.x = Random.Range(-_spawnSphere, _spawnSphere) + _posBuffer.x;
		t.z = Random.Range(-_spawnSphereDepth, _spawnSphereDepth) + _posBuffer.z;
		t.y = Random.Range(-_spawnSphereHeight, _spawnSphereHeight) + _posBuffer.y;
		return t;
	}

	//Uses scanner to push away from obstacles
	void RayCastToPushAwayFromObstacles() {
		if(_push){
			RotateScanner();
			RayCastToPushAwayFromObstaclesCheckForCollision();
		}
	}

	void RayCastToPushAwayFromObstaclesCheckForCollision () {
		RaycastHit hit;
		float d;
		Vector3 cacheForward = _scanner.forward;
		if (Physics.Raycast(transform.position, cacheForward,out hit, _pushDistance)){		
				
			d = (_pushDistance - hit.distance)/_pushDistance;	// Equals zero to one. One is close, zero is far	
			_speed -= .01f*_newDelta;
			if(_speed < .1f)
				_speed = .1f;
				transform.position -= cacheForward*_newDelta*d*_pushForce*2;
				//Tell scanner to rotate slowly
				_scan = false;
		}else{
			//Tell scanner to rotate randomly
			_scan = true;
		}
	}

	void RotateScanner() {
		//Scan random if not pushing
		if(_scan){
			_scanner.rotation = Random.rotation;
			return;
		}
		//Scan slow if pushing
		_scanner.Rotate(new Vector3(150f*_newDelta,0f,0f));
	}

	bool Avoidance (){
		//Avoidance () - Returns true if there is an obstacle in the way
		if(!_avoidance)
			return false;		
		RaycastHit hit;
		float d;
		Quaternion rx = transform.rotation;
		Vector3 ex = transform.rotation.eulerAngles;
		Vector3 cacheForward = transform.forward;
		Vector3 cacheRight = transform.right;
		//Up / Down avoidance
		if (Physics.Raycast(transform.position, -Vector3.up+(cacheForward*.1f),out hit, _avoidDistance)){			
			//Debug.DrawLine(transform.position,hit.point);
			d = (_avoidDistance - hit.distance)/_avoidDistance;
			ex.x -= _avoidSpeed*d*_newDelta*(_speed +1);
			rx.eulerAngles = ex;
			transform.rotation = rx;
		}
		if (Physics.Raycast(transform.position, Vector3.up+(cacheForward*.1f),out hit, _avoidDistance)){
			//Debug.DrawLine(transform.position,hit.point);
			d = (_avoidDistance - hit.distance)/_avoidDistance;			
			ex.x += _avoidSpeed*d*_newDelta*(_speed +1);	
			rx.eulerAngles = ex;
			transform.rotation = rx;	
		}

		//Crash avoidance //Checks for obstacles forward
		if (Physics.Raycast(transform.position, cacheForward+(cacheRight*Random.Range(-0.1f, 0.1f)),out hit, _stopDistance)){		
			//					Debug.DrawLine(transform.position,hit.point);
			d = (_stopDistance - hit.distance)/_stopDistance;				
			ex.y -= _avoidSpeed*d*_newDelta*(_targetSpeed +3);
			rx.eulerAngles = ex;
			transform.rotation = rx;
			_speed -= d*_newDelta*_stopSpeedMultiplier*_speed;				
			if(_speed < 0.01f){
				_speed = 0.01f;	
			}
			return true;
		}else if (Physics.Raycast(transform.position, cacheForward+(cacheRight*(_avoidAngle+_rotateCounterL)),out hit, _avoidDistance)){
			//				Debug.DrawLine(transform.position,hit.point);
			d = (_avoidDistance - hit.distance)/_avoidDistance;				
			_rotateCounterL+=.1f;
			ex.y -= _avoidSpeed*d*_newDelta*_rotateCounterL*(_speed +1);
			rx.eulerAngles = ex;
			transform.rotation = rx;				
			if(_rotateCounterL > 1.5f)
				_rotateCounterL = 1.5f;				
			_rotateCounterR = 0;
			return true;		
		}else if (Physics.Raycast(transform.position, cacheForward+(cacheRight*-(_avoidAngle+_rotateCounterR)),out hit, _avoidDistance)){
			//			Debug.DrawLine(transform.position,hit.point);
			d = (_avoidDistance - hit.distance)/_avoidDistance;
			if(hit.point.y < transform.position.y){
				ex.y -= _avoidSpeed*d*_newDelta*(_speed +1);
			}
			else{
				ex.x += _avoidSpeed*d*_newDelta*(_speed +1);
			}
			_rotateCounterR +=.1f;
			ex.y += _avoidSpeed*d*_newDelta*_rotateCounterR*(_speed +1);
			rx.eulerAngles = ex;
			transform.rotation = rx;	
			if(_rotateCounterR > 1.5f)
				_rotateCounterR = 1.5f;	
			_rotateCounterL = 0;
			return true;
		}else{
			_rotateCounterL = 0;
			_rotateCounterR = 0;
		}
		return false;																	    																																				    																				
	}

	void ForwardMovement(){
		transform.position += transform.TransformDirection(Vector3.forward)*_speed*_newDelta;
		if (tParam < 1) {
			if(_speed > _targetSpeed){
				tParam += _newDelta * _acceleration;
			}else{
				tParam += _newDelta * _brake;		
			}
			_speed = Mathf.Lerp(_speed, _targetSpeed,tParam);	
		}
	}

	void RotationBasedOnWaypointOrAvoidance (){
		Quaternion rotation;
		rotation = Quaternion.LookRotation(_wayPoint - transform.position);
		if(!Avoidance()){
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _newDelta * _damping);
		}
		//Limit rotation up and down to avoid freaky behavior
		float angle = transform.localEulerAngles.x;
		angle = (angle > 180) ? angle - 360 : angle;
		Quaternion rx = transform.rotation;
		Vector3 rxea = rx.eulerAngles;
		rxea.x = ClampAngle(angle, -50.0f , 50.0f);
		rx.eulerAngles = rxea;
		transform.rotation = rx;
	}

	void CheckForDistanceToWaypoint(){
		if((transform.position - _wayPoint).magnitude < _waypointDistance+_stuckCounter){
			Wander(0);	//create a new waypoint
			_stuckCounter=0;
			CheckIfThisShouldTriggerNewFlockWaypoint();
			return;
		}
		_stuckCounter+=_newDelta*(_waypointDistance*.25f);
	}

	void CheckIfThisShouldTriggerNewFlockWaypoint(){
			SetRandomWaypointPosition();

	}

	static float ClampAngle (float angle ,float min,float max) {
		if (angle < -360f)angle += 360f;
		if (angle > 360f)angle -= 360f;
		return Mathf.Clamp (angle, min, max);
	}

	void SetAnimationSpeed(){
		foreach (AnimationState state  in _model.GetComponent<Animation>()) {
			state.speed = (Random.Range(_minAnimationSpeed, _maxAnimationSpeed)*_schoolSpeed*this._speed)+.1f;   		   
		}
	}

	void Wander(float delay){
		_damping = Random.Range(_minDamping, _maxDamping);
		_targetSpeed = Random.Range(_minSpeed, _maxSpeed)*_speedCurveMultiplier.Evaluate(Random.value)*_schoolSpeed;
		Invoke("SetRandomWaypoint", delay);
	}

	void SetRandomWaypoint(){
		tParam = 0;
		_wayPoint = findWaypoint();
	}
}
