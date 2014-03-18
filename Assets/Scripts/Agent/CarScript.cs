using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarScript : MonoBehaviour {

	public float speed = 4f;							// The speed of the movement 
	public Texture2D front, back, sideLeft, sideRight; 	// texture to ba applied
	public bool DynamicRoadOn{get;set;}					// Is the agent on a dynamic road, 
														// is used to skip the updating of the path 
														//if the agent is not on dynamic road

	private WaypointScript ws;							// Last waypoint script used
	                                           			// It is used when updating the path on movement of a cube
														// This helps figure out what road the agent is on
	private new Transform transform;					// Cache of the transform
	private Transform [] m_path;						// Current path
	private Vector3 v_prevDirection;					// Previous direction to see if direction has changed
	private int i_index = 0;							// Current index

	void Start () 
	{
		transform = base.transform;

		// Get the main waypoint object
		GameObject waypoints = GameObject.Find ("Wp");
		// get all children waypoints
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
		m_path = ws.GetPath(this);
		i_index  = 0;

		// Place the agent at start position and get the initial direction
		transform.position = start.position;
		v_prevDirection = (m_path[0].position - transform.position).normalized;

		// Subscribe to event on movement of the cube
		CubePosition.OnMoveSecond += UpdatePath;
	}

	void Update () 
	{
		// Update direction
		Vector3 direction = (m_path[i_index].position - transform.position).normalized;
		// Move agent in the dierction
		transform.Translate (direction * Time.deltaTime * speed, Space.World);
		// Agen always face camera
		transform.LookAt(Camera.main.transform.position);

		// Check distance with target point
		if(Vector3.Distance(transform.position, m_path[i_index].position)< 0.2f)
		{
			if(++i_index == m_path.Length)i_index = 0;
		}
		// Define orientation and texture to apply
		if(v_prevDirection != direction)
			GetDirection(direction);
		v_prevDirection = direction;

		// Debug for agent path
		for(int i = 0; i < m_path.Length - 1; i++)
		{
			Debug.DrawLine(m_path[i].position, m_path[i+1].position, Color.white);
		}
	}


	// Apply the corresponding texture based on direction
	void GetDirection(Vector3 direction)
	{
		// Get all four directions
		Vector3 [] directions = {Vector3.right, Vector3.left,Vector3.forward, Vector3.back};
		int ind = -1;
		// Check current direction against all directions to find which has dot close to 1
		for(int i = 0; i < directions.Length; i++ )
		{
			float dot =  direction.x * directions[i].x + direction.z * directions[i].z;
			if(dot >= 1 - 0.05f && dot <= 1 + 0.05f)ind = i;
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


	// When Car enters a trigger, the Waypoint script of the waypoint is accessed to get a new path.
	void OnTriggerEnter(Collider col)
	{
		if(col.CompareTag("CornerWp")||col.CompareTag("CenterWp"))
		{
			ws = col.gameObject.GetComponent<WaypointScript>();
			m_path = ws.GetPath(this);
			i_index  = 0;
		}
	}
	void UpdatePath()
	{
		// If not on a Dynamic road quit the method
		if(DynamicRoadOn == false)return;

		// Current direction of the agen
		Vector3 direction = (m_path[i_index].position - transform.position).normalized;
		// get the new dynamic path
		m_path = ws.GetDynamicPath();
		// Get closest node of the path
		float distance = Mathf.Infinity;
		int ind = 0;
		Vector3 position = transform.position;
		for(int i = 0 ; i < m_path.Length; i++)
		{
			float dist = (position - m_path[i].position).sqrMagnitude;
			if(dist < distance)
			{
				distance = dist;
				ind = i;
			}
		}

		// Get direction to check if found node is ahead or behind
		float dot = Vector3.Dot (direction, (transform.position - m_path[ind].position).normalized);
		// if behind get next node
		if(dot < 0 )ind++;
		// Throw raycast downward to check if the agent is still on road.
		RaycastHit hit;
		Ray ray = new Ray(transform.position, Vector3.down);
		Physics.Raycast(ray, out hit);
		// if the ray hits the ground the agen tis repositioned on the nearest node
		if(hit.collider.name == "Ground")
		{
			transform.position = m_path[ind].position;
		}
	}
}
