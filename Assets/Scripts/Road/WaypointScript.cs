using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointScript : MonoBehaviour 
{
	public Transform[] connections;
	public int indexArray;
	public bool isDynamic = false;

	private RoadManager roadManager;
	private Vector3 [][] roadPositions;

	void Awake()
	{
		roadManager = GameObject.Find ("GameManager").GetComponent<RoadManager>();
		if(isDynamic)
		{
			roadPositions = new Vector3[3][];
		}
		else
		{
			roadPositions = new Vector3[2][];
		}
		for (int i = 0; i < connections.Length; i++ )
		{
			Vector3[] path = {connections[i].position};
			roadPositions[i] = path;  
		}
		if(isDynamic)
		{
			UpdatePath ();
		}
		RoadManager.OnRoadChange += UpdatePath;
	}

	public Vector3[] GetPath(CarMovement carMovement)
	{	
		int r = Random.Range(0,roadPositions.Length);
		return roadPositions[r];
	}

	private void UpdatePath()
	{
		if(isDynamic)
		{
			roadPositions[2] = roadManager.GetDynamicPath(indexArray);
		}
	}

	public Vector3[] GetDynamicPath()
	{
		return roadPositions[2];
	}
}
