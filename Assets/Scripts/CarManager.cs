using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// No need for that, just use the stack already existin
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
        // Not storing the game object
		Pop ();
	}

	public void RemoveCar(GameObject o){
		Push( o );
	}

	public void Push(GameObject obj) {
		obj.SetActive(false);
		_stack.Push(obj);
	}

    // why should this return a value, it is not storing it anywhere
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
