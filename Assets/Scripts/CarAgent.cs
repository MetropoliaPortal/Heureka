﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarAgent : MonoBehaviour {

	private int _step;
	private int _amountSteps;
	private Vector3 _nextStepPosition;
	private Vector3 _positionFix;
	private int _direction;
	private Collider _currentRoad;
	private int _layerMask;
	private int _forbiddenDirection;


	private enum Directions{
		LEFT = 0, RIGHT = 1, UP = 2, DOWN = 3
	}

	
	void Start () {

		_step = 0;
		_amountSteps = Random.Range(20,40);
		_nextStepPosition = new Vector3(0,0,0);
		_positionFix = new Vector3(0,0,0);
		_direction = (int)Directions.LEFT;
		_currentRoad = null;
		_layerMask = 1 << 8;

		SolvePositionFix();
		SolveStartingPosition();
	}


	void Update () {

		Move();
		CheckRoadUnder();
		CheckStepsRemaining();
		CheckStepReached();
	}
	
	void SolveStartingPosition(){
		
		List<GameObject> list = new List<GameObject>();
		GameObject[] edgeRoads = GameObject.FindGameObjectsWithTag("EdgeRoad");
		GameObject[] centreRoads = GameObject.FindGameObjectsWithTag("Road");
		list.AddRange (edgeRoads);
		list.AddRange (centreRoads);
		GameObject[] roads = list.ToArray();
		
		Vector3 startPos = roads [Random.Range (0, roads.Length - 1)].transform.position;
		startPos += _positionFix;
		transform.position = startPos;
		
	}
	
	void Move(){

		//Make sure next step is resolved
		if(_nextStepPosition != new Vector3(0,0,0))
			transform.position = Vector3.MoveTowards (transform.position, _nextStepPosition, 0.05f);
		else{
			if( _currentRoad != null)
				SolveNextStep();
		}
	}
	 

	void CheckRoadUnder(){
		
		Vector3 castEnd = transform.position;
		castEnd.y -= 1.0f;
		
		Vector3 castStart = transform.position;
		castStart.y += 0.5f;
		
		if ( !Physics.Linecast (castStart, castEnd, _layerMask) ){
			Destroy();
		}
	}
	
	
	void CheckStepsRemaining(){
		
		if (_step > _amountSteps)
			GameObject.Destroy (gameObject);
	}
	
	
	void CheckStepReached(){
		
		if (CompareApproximate(transform.position, _nextStepPosition)) {
			SolveNextStep();
		}
	}

	
	void SolveNextStep(){

		List<int> routes = new List<int>();
		int r;

		SolveForbiddenDirection();

		for(int i = 0; i <= 3; i++){
			if( i != _forbiddenDirection ){
				if ( CheckForRoad(i) ){
					routes.Add( i );
				}
			}
		}

		if( routes.Count != 0){
			r = Random.Range(0, routes.Count);

			_direction = routes[r];
			ChangeDirection();

			//Solve target point for next step and position fix corresponding to direction
			SolvePositionFix();
			_nextStepPosition = SolveNextPosition( _direction );
			_nextStepPosition += _positionFix;

			//Adjust position to make it effective right away when turning
			AdjustPosition();

			_step++;
		} else {
			//Destroy if no road found
			Destroy();
		}
	}


	bool CheckForRoad(int dir){
		Vector3 nextPosition = SolveNextPosition(dir);
		return Physics.Linecast (transform.position, nextPosition, _layerMask);	
	}


 	Vector3 SolveNextPosition(int dir){

		Vector3 nextPos;
		switch ( dir ) {
		case((int)Directions.LEFT):
			nextPos = new Vector3 (_currentRoad.transform.position.x - 1, 
			                    _currentRoad.transform.position.y, 
			                    _currentRoad.transform.position.z);
			break;
		case((int)Directions.UP):
			nextPos = new Vector3 (_currentRoad.transform.position.x, 
			                    _currentRoad.transform.position.y, 
			                    _currentRoad.transform.position.z + 1);;
			break;
		case((int)Directions.RIGHT):
			nextPos = new Vector3 (_currentRoad.transform.position.x + 1, 
			                    _currentRoad.transform.position.y, 
			                    _currentRoad.transform.position.z);
			break;
		case((int)Directions.DOWN):
			nextPos = new Vector3 (_currentRoad.transform.position.x, 
			                    _currentRoad.transform.position.y, 
			                    _currentRoad.transform.position.z - 1);
			break;
		default:
			nextPos = new Vector3 (_currentRoad.transform.position.x, 
			                    _currentRoad.transform.position.y, 
			                    _currentRoad.transform.position.z);
			break;
		}

		return nextPos;
	}


	void SolveForbiddenDirection(){

		switch ( _direction ) {
		case((int)Directions.LEFT):
			_forbiddenDirection = (int) Directions.RIGHT;
			break;
		case((int)Directions.UP):
			_forbiddenDirection = (int) Directions.DOWN;
			break;
		case((int)Directions.RIGHT):
			_forbiddenDirection = (int) Directions.LEFT;
			break;
		case((int)Directions.DOWN):
			_forbiddenDirection = (int) Directions.UP;
			break;
		default:
			_forbiddenDirection = (int) Directions.LEFT;
			break;
		}
	}


	void ChangeDirection(){

		switch ( _direction ) {
		case((int)Directions.LEFT):
			transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
			break;
		case((int)Directions.RIGHT):
			transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
			break;
		case((int)Directions.UP):
			transform.rotation = Quaternion.Euler(new Vector3(0,-90,0));
			break;
		case((int)Directions.DOWN):
			transform.rotation = Quaternion.Euler(new Vector3(0,90,0));
			break;
		default:
			transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
			break;
		}
	}


	void AdjustPosition(){

		Vector3 roadPos = _currentRoad.transform.position;
		transform.position = roadPos + _positionFix;
	}


	/// <summary>
	/// Position fix to make cars drive on right side of the street
	/// </summary>
	void SolvePositionFix(){

		switch(_direction){
		case((int)Directions.LEFT):
			_positionFix = new Vector3(0, 0, 0.3f);
			break;
		case((int)Directions.RIGHT):
			_positionFix = new Vector3(0, 0, -0.3f);
			break;
		case((int)Directions.UP):
			_positionFix = new Vector3(0.3f, 0, 0);
			break;
		case((int)Directions.DOWN):
			_positionFix = new Vector3( -0.3f, 0, 0);
			break;
		default:
			_positionFix = new Vector3(0, 0, 0);
			break;
		}
	}


	bool CompareApproximate(Vector3 a, Vector3 b){

		float tolerance = 0.1f;

		if( !(Mathf.Abs(a.x - b.x) < tolerance) )
			return false;
		if( !(Mathf.Abs(a.y - b.y) < tolerance) )
			return false;
		if( !(Mathf.Abs(a.z - b.z) < tolerance) )
			return false;
		return true;
	}


	void OnTriggerEnter(Collider d){

		_currentRoad = d;
	}


	void Destroy(){

		GameObject.Destroy (gameObject);
	}
}