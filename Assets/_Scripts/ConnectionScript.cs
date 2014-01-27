using UnityEngine;
using System.Collections;

public class ConnectionScript : MonoBehaviour {
	
	public SecBoxScript script;
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void SendMessageToSecCube()
	{
		script.SetModel();
	}
}
