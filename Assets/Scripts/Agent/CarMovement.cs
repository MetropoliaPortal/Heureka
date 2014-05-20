using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarMovement : MonoBehaviour 
{
	public float speed = 4f;							
	public float height = 1.0f;				
	// Last waypoint script used
	// It is used when updating the path on movement of a cube
	// This helps figure out what road the agent is on
	private WaypointScript ws;							

	// Cache of the transform
	private new Transform trans;					
	private Vector3 [] m_path;

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
		trans.position = pos;
	}
	
	void Update () 
	{
		Vector3 pathPos = m_path[i_index];
		pathPos.y = height;

		Vector3 dir = (pathPos - transform.position);

		trans.Translate (dir.normalized * Time.deltaTime * speed, Space.World);
		Vector3 pos = transform.position;
		trans.position = new Vector3(pos.x, height, pos.z);

		if(Vector3.Distance(trans.position, pathPos) < 0.1f)
		{
			if(++i_index == m_path.Length)i_index = 0;
			CheckIfOnRoad();
		}
	}

	/// <summary>
	/// Check if ray-cast down hits ground or cube and places the car to new position if needed.
	/// </summary>
	private void CheckIfOnRoad()
	{
		RaycastHit hit;
		Ray ray = new Ray(trans.position, Vector3.down);
		if(Physics.Raycast(ray, out hit))
		{
			if(hit.collider.name == "Ground" || hit.collider.tag == "Obstacle")
			{
				RepositionCar();
			}
		}
	}

	void LateUpdate()
	{
		carSprite.ApplySprite( m_path, i_index );
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
}
