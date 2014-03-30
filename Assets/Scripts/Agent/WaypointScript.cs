using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointScript : MonoBehaviour {

	public Transform[]connections;
	public int indexArray;
	public bool isDynamic = false;

	private RoadManager m_manager;
	private Transform [][] m_roads;

	void Awake()
	{
		m_manager = GameObject.Find ("RoadManager").GetComponent<RoadManager>();
		if(isDynamic)
		{
			m_roads = new Transform[3][];
		}
		else{
			m_roads = new Transform[2][];
		}
		for (int i = 0; i < connections.Length; i++ )
		{
			Transform[] path = {connections[i]};
			m_roads[i] = path;  
		}
		if(isDynamic)
		{
			UpdatePath ();
		}
		RoadManager.OnRoadChange += UpdatePath;
	}
	void Update()
	{
		if(isDynamic)
		{
			for(int i = 0; i < m_roads[2].Length - 1; i++)
			{
				Debug.DrawLine(m_roads[2][i].position, m_roads[2][i+1].position, Color.blue);
			}
		}
	}

	public Transform [] GetPath(CarScript sc)
	{	
		int r = Random.Range(0,m_roads.Length);
		if(r == 2)
		{
			sc.DynamicRoadOn = true;
		}
		else
		{
			sc.DynamicRoadOn = false;
		}

		return m_roads[r];
	}
	private  void UpdatePath()
	{
		if(isDynamic)
		{
			m_roads[2] = m_manager.GetDynamicPath(indexArray);
		}
	}
	public Transform[] GetDynamicPath()
	{
		return m_roads[2];
	}
}
