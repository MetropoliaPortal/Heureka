using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarScript : MonoBehaviour {
	
	public float speed = 4f;							// The speed of the movement 
	public Texture2D front, back, sideLeft, sideRight; 	// texture to ba applied
	private Material mat;
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
	private float height = 0.3f;						// Car height
	
	void Start () 
	{
		transform = base.transform;
		mat = renderer.material;
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
		Vector3 pos = start.position;
		//pos.y += height;
		transform.position = pos;
		v_prevDirection = (m_path[0].position - transform.position).normalized;
		
		// Subscribe to event on movement of the cube
		CubePosition.OnMoveSecond += UpdatePath;
	}
	
	void Update () 
	{
		// Update direction
		Vector3 direction = (m_path[i_index].position - transform.position);
		// Move agent in the dierction
		transform.Translate (direction.normalized * Time.deltaTime * speed, Space.World);
		Vector3 pos = transform.position;
		pos.y = height;
		transform.position = pos;
		// Agent always face camera
		//transform.LookAt(Camera.main.transform.position);
		// Check distance with target point
		if(Vector3.Distance(transform.position, m_path[i_index].position)< 0.51f)
		{
			if(++i_index == m_path.Length)i_index = 0;
		}
		// Define orientation and texture to apply
		//if(v_prevDirection != direction) 
		//GetDirection(direction);
		//v_prevDirection = direction;
		
		// Debug for agent path
		for(int i = 0; i < m_path.Length - 1; i++)
		{
			Debug.DrawLine(m_path[i].position, m_path[i+1].position, Color.white);
		}
	}
	void LateUpdate(){
		Vector3 direction = (m_path[i_index].position - transform.position);
		GetDirection(direction);
	}
	
	// Apply the corresponding texture based on direction
	void GetDirection(Vector3 direction)
	{
		print (direction);
		// Get all four directions
		Vector3 [] directions = {Vector3.right, Vector3.left,Vector3.forward, Vector3.back};
		int ind = -1;
		Vector3 d = direction.normalized;
		// Check current direction against all directions to find which has dot close to 1
		for(int i = 0; i < directions.Length; i++ )
		{
			float dot =  d.x * directions[i].x + d.z * directions[i].z;
			if(dot >= 1 - 0.25f && dot <= 1 + 0.25f)
			{
				ind = i;
				break;
			}
		}
		if(ind != -1)
		{
			Vector3 dir = directions[ind];
			Vector3 targetPoint = m_path[i_index].position;
			float dot = Vector3.Dot(dir, direction);
			Vector3 a = -dot * dir;
			
			Vector3 closest = targetPoint + a;
			Vector3 v = new Vector3(directions[ind].z, directions[ind].y, -directions[ind].x);
			Vector3 b =closest + v * 0.2f;
			b.y = 0.3f;
			transform.position = b;
		}
		switch(ind){
		case 0:
			//print ("Right");
			mat.mainTexture = sideRight;
			break;
		case 1 :
			//print ("Left");
			mat.mainTexture = sideLeft;
			break;
		case 2:
			//print ("Up");
			mat.mainTexture = back;
			break;
		case 3:
			//print ("Down");
			mat.mainTexture = front;
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
	
	// Update path is called by the event of the Cube hen moved
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
		if(Physics.Raycast(ray, out hit))
		{
			// if the ray hits the ground the agent is repositioned on the nearest node
			if(hit.collider.name == "Ground")
			{
				transform.position = m_path[ind].position;
			}
		}
	}
}
