using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointScript : MonoBehaviour {

	public Transform[]connections;
	public bool dynamic = false;
	private RoadManager m_manager;
	public int indexArray;
	private Transform [][] roads;
	//public Color color;

	//public event HandlerEvent OnNewDynamicRoad = new HandlerEvent(()=>{});

	void Awake()
	{
		m_manager = GameObject.Find ("RoadManager").GetComponent<RoadManager>();
		if(dynamic)
		{
			roads = new Transform[3][];
		}
		else{
			roads = new Transform[2][];
		}
		for (int i = 0; i < connections.Length; i++ )
		{
			Transform[] path = {connections[i]};
			roads[i] = path;  
		}
		if(dynamic)
		{
			UpdatePath ();
		}
		RoadManager.OnRoadChange += UpdatePath;
	}
	void Update()
	{
		if(dynamic)
		{
			for(int i = 0; i < roads[2].Length - 1; i++)
			{
				Debug.DrawLine(roads[2][i].position, roads[2][i+1].position, Color.blue);
			}
		}
		/*RaycastHit hit;
		Physics.Raycast (transform.position, Vector3.down, out hit)*/
	}

	public Transform [] GetPath(CarScript sc)
	{	
		int r = Random.Range(0,roads.Length);
		if(dynamic)r = 2;
		if(r == 2)
		{
			sc.DynamicRoadOn = true;
		}
		else
		{
			sc.DynamicRoadOn = false;
		}
		return roads[r];
	}
	private  void UpdatePath()
	{
		if(dynamic)
		{
			roads[2] = m_manager.GetDynamicPath(indexArray);
		}
	}
	public Transform[] GetDynamicPath()
	{
		return roads[2];
	}
}
