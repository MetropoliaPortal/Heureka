using UnityEngine;
using System.Collections;

public class BoxScript : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		transform.position = new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z)); 
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z));
	}
}
