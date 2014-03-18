using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarScript : MonoBehaviour {

	public GameObject waypoints;
	private Transform [] path;
	private Vector3 prevDirection;
	public Texture2D front, back, sideLeft, sideRight; 
	public Vector3 target;
	public bool DynamicRoadOn{get;set;}
	int index = 0;
	// Use this for initialization

	void Start () {
		List<Transform>list = new List<Transform>();
		foreach(Transform t in waypoints.transform)
		{
			list.Add (t);
		}
		// Get one waypoint at random
		Transform start = list[Random.Range(0,list.Count)];
	 	// Get the WaypointScript
		WaypointScript ws = start.GetComponent<WaypointScript>();
		// Get a random item for the connection array of the waypoint
		path = ws.GetPath(this);
		index  = 0;

		transform.position = start.position;
		Vector3 direction = (path[0].position - transform.position).normalized;
		prevDirection = direction;
		CubePosition.OnMoveSecond += UpdatePath;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 direction = (path[index].position - transform.position).normalized;
		target = path[index].position;
		transform.Translate (direction * Time.deltaTime * 4f, Space.World);
		transform.LookAt(Camera.main.transform.position);

		// Check distance with target point
		if(Vector3.Distance(transform.position, path[index].position)< 0.2f)
		{
			if(++index == path.Length)index = 0;
		}
		// Define orientation and texture to apply
		if(prevDirection != direction)
			GetDirection(direction);
		prevDirection = direction;

		// Debug for agent path
		for(int i = 0; i < path.Length - 1; i++)
		{
			Debug.DrawLine(path[i].position, path[i+1].position, Color.white);
		}
	}

	// Appropriate 2D dot with x and z
	float Dot(Vector3 vec, Vector3 direction)
	{
		return vec.x * direction.x + vec.z * direction.z;
	}

	// Apply the corresponding texture based on direction
	void GetDirection(Vector3 direction)
	{
		Vector3 [] directions = {Vector3.right, Vector3.left,Vector3.forward, Vector3.back};
		int ind = -1;
		for(int i = 0; i < directions.Length; i++ )
		{
			float dot = Dot (direction, directions[i]);
			if(IsEqual (dot, 1))ind = i;
		}
		switch(ind){
		case 0:
			//print ("Right");
			break;
		case 1 :
			//print ("Left");
			break;
		case 2:
			//print ("Up");
			break;
		case 3:
			//print ("Down");
			break;
		}
	}

	bool IsEqual(float a, float b) {
		if (a >= b - 0.05f && a <= b + 0.05f)
			return true;
		else
			return false;
	}

	WaypointScript ws;
	// When Car enters a trigger, the Waypoint script of the waypoint is accessed to get a new path.
	void OnTriggerEnter(Collider col)
	{
		if(col.CompareTag("CornerWp")||col.CompareTag("CenterWp"))
		{
			ws = col.gameObject.GetComponent<WaypointScript>();
			path = ws.GetPath(this);
			index  = 0;
		}
	}
	void UpdatePath()
	{
		// If not on a Dynamic road quit the method
		if(DynamicRoadOn == false)return;

		// Current direction of the agen
		Vector3 direction = (path[index].position - transform.position).normalized;
		// get the new path
		path = ws.GetDynamicPath();
		float distance = Mathf.Infinity;
		int ind = 0;
		Vector3 position = transform.position;
		for(int i = 0 ; i < path.Length; i++)
		{
			float dist = (position - path[i].position).sqrMagnitude;
			if(dist < distance)
			{
				distance = dist;
				ind = i;
			}
		}
		float dot = Vector3.Dot (direction, (transform.position - path[ind].position).normalized);
		if(dot < 0 )ind++;
		RaycastHit hit;
		Ray ray = new Ray(transform.position, Vector3.down);
		Physics.Raycast(ray, out hit);
		if(hit.collider.name == "Ground")
		{
			transform.position = path[ind].position;
		}
		
	}
}
