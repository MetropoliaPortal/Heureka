using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarManager : MonoBehaviour {


	public GameObject _car;
	const int AMOUNT_CARS = 5;
	Stack<GameObject> _stack = null;
	
	public CarManager(){
		_stack = new Stack<GameObject>();
	}

	public void InitializeCars(){
		for(int i = 0; i < AMOUNT_CARS; i++){
			Push ( (GameObject)Instantiate( _car ) );
		}

	}

	void Start(){

		PathManager.GeneratePaths();
		InitializeCars();
		InvokeRepeating("AddCar", 2.0f, 1.0f);

	}

	void AddCar(){
		Pop ();
	}

	public void RemoveCar(GameObject o){
		Push( o );
	}

	public void Push(GameObject obj) {
		obj.SetActive(false);
		_stack.Push(obj);
	}


	public GameObject Pop() {
		if (_stack.Count > 0){
			GameObject o = _stack.Pop();
			o.SetActive(true);
			//o.GetComponent<CarAgent>().Initialize();
			return o;
		}
		else{
			return null;
		}
	}
}
