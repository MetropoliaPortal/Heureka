using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarMovement : MonoBehaviour 
{
	public float speed = 4f;							
	public float height = 1.0f;	

	// Is the agent on a dynamic road, 
	// is used to skip the updating of the path 
	//if the agent is not on dynamic road
	public bool DynamicRoadOn{get;set;}					

	// Last waypoint script used
	// It is used when updating the path on movement of a cube
	// This helps figure out what road the agent is on
	private WaypointScript ws;							

	// Cache of the transform
	private new Transform trans;					
	private Transform [] m_path;

	private int i_index = 0;	
						
	private List<Transform>list;
	private CarSprite carSprite;

	void Start () 
	{
		trans = base.transform;
		GameObject waypoints = GameObject.Find ("WayPoints");
		list = new List<Transform>();
		foreach(Transform t in waypoints.transform)
		{
			list.Add (t);
		}

		RepositionCar ();

		carSprite = GetComponentsInChildren<CarSprite>()[0];
		
		// Subscribe to event on movement of the cube
		CubePosition.OnMoveSecond += UpdatePath;
	}

	// Get a random item for the connection array of the waypoint
	// Place the agent at start position and get the initial direction
	private void RepositionCar()
	{
		Transform start = list[Random.Range(0,list.Count)];
		WaypointScript ws = start.GetComponent<WaypointScript>();

		m_path = ws.GetPath(this);
		i_index  = 0;

		Vector3 pos = new Vector3(start.position.x, height, start.position.z);
		transform.position = pos;
	}
	
	void Update () 
	{
		Vector3 pathPos = m_path[i_index].position;
		pathPos.y = height;
		//pathPos.z -= 0.4f;

		Vector3 direction = (pathPos - transform.position);

		trans.Translate (direction.normalized * Time.deltaTime * speed, Space.World);
		Vector3 pos = transform.position;
		trans.position = new Vector3(pos.x, height, pos.z);

		if(Vector3.Distance(trans.position, pathPos) < 0.1f)
		{
			if(++i_index == m_path.Length)i_index = 0;
		}

	}

	void LateUpdate()
	{
		carSprite.GetDirection( m_path, i_index );
	}


	// When Car enters a trigger, the Waypoint script of the waypoint is accessed to get a new path.
	void OnTriggerEnter(Collider col)
	{
		if(col.CompareTag("CornerWp") || col.CompareTag("CenterWp"))
		{
			ws = col.gameObject.GetComponent<WaypointScript>();
			m_path = ws.GetPath(this);
			i_index  = 0;
		}
	}
	
	// Update path is called by the event of the Cube hen moved
	// If not on a Dynamic road quit the method
	private void UpdatePath()
	{
		if(DynamicRoadOn == false)return;
		RepositionCar();
	}
}
