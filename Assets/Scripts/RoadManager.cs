using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadManager : MonoBehaviour 
{
	private Transform startPos, endPos;
	private Node startNode;
	private Node goalNode;
	
	public List<Node> pathArray = new List<Node>();
	public Transform[] waypoints;
	public GameObject road;
	GameObject[][] roadArray = new GameObject[9][];
	int i;
	Transform roadObj;

	#region Singleton implementation
	// s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
	private static RoadManager s_Instance = null;
	
	// This defines a static instance property that attempts to find the manager object in the scene and
	// returns it to the caller.
	public static RoadManager instance
	{
		get
		{
			if (s_Instance == null)
			{
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first RoadManager object in the scene.
				s_Instance = FindObjectOfType(typeof(RoadManager)) as RoadManager;
				if (s_Instance == null)
					Debug.Log("Could not locate an RoadManager object. \n You have to have exactly one RoadManager in the scene.");
			}
			return s_Instance;
		}
	}
	
	// Ensure that the instance is destroyed when the game is stopped in the editor.
	void OnApplicationQuit()
	{
		s_Instance = null;
	}

	#endregion

	void Start() 
	{
		//Solve road after repeatedly after each timestep. Solving the road is currently too heavy procedure to be done
		//after every time a cube is moved
		InvokeRepeating("SolveRoad", 0.001f, 1.0f);
	}

	void DeleteRoad(){
		GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");
		for (int i=0; i<roads.Length; i++) {
			GameObject.Destroy(roads[i]);
		}
		//GameObject.Destroy (road);
		roadArray = new GameObject[9][];
	}

	public void SolveRoad(){
		DeleteRoad ();
		GridManager.instance.DoStuff ();
		SolveSurroundingRoads ();
		SolveCrossingRoads ();
	}

	void SolveSurroundingRoads(){
		for (i = 0; i < waypoints.Length; i++)
		{
			if(i == waypoints.Length - 1){
				if (Physics.Linecast(waypoints[i].position, waypoints[0].position))
				{
					roadArray[i] = DrawRoadAStar(waypoints[i].position, waypoints[0].position);
				}
				else
				{
					roadArray[i] = DrawStraightRoad(waypoints[i].position, waypoints[0].position);
				}
			}
			else if (Physics.Linecast(waypoints[i].position, waypoints[i + 1].position))
			{
				roadArray[i] = DrawRoadAStar(waypoints[i].position, waypoints[i + 1].position);
			}
			else
			{
				roadArray[i] = DrawStraightRoad(waypoints[i].position, waypoints[i + 1].position);
			}
		}
	}

	void SolveCrossingRoads(){
		if (Physics.Linecast(waypoints[1].position, waypoints[5].position))
		{
			roadArray[7] = DrawRoadAStar(waypoints[1].position, waypoints[5].position);
		}
		else
		{
			roadArray[7] = DrawStraightRoad(waypoints[1].position, waypoints[5].position);
		}
		
		if (Physics.Linecast(waypoints[3].position, waypoints[7].position))
		{
			roadArray[8] = DrawRoadAStar(waypoints[3].position, waypoints[7].position);
		}
		else
		{
			roadArray[8] = DrawStraightRoad(waypoints[3].position, waypoints[7].position);
		}
	}

	GameObject[] DrawRoadAStar(Vector3 start, Vector3 end) 
	{
		startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(start)));
		goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(end)));
		pathArray = AStar.FindPath(startNode, goalNode);
		List<GameObject> arrayRoad = new List<GameObject>();
		
		// Place first
		Vector3 pos = start;
		pos.y += 0.01f;
		GameObject o = (GameObject)Instantiate(road, pos, Quaternion.identity);
		arrayRoad.Add(o);
		o.transform.parent = roadObj;
		
		for (int k = 0; k < pathArray.Count; k++)
		{
			pos = pathArray[k].position;
			pos.y += 0.01f;
			o = (GameObject)Instantiate(road, pos, Quaternion.identity);
			if (i == 2) o.name = "New";
			arrayRoad.Add(o);

			//What is this?
			/*
			o.transform.parent = roadObj;
			pos = pathArray[k + 1].position - pathArray[k].position;
			pos = pathArray[k].position + 0.5f * pos;
			pos.y += 0.01f;
			o = (GameObject)Instantiate(road, pos, Quaternion.identity);
			arrayRoad.Add(o);
			o.transform.parent = roadObj;
			if (i == 2) o.name = "New";
			*/
		}
		return arrayRoad.ToArray();
	}
	
	GameObject[] DrawStraightRoad(Vector3 start, Vector3 end) 
	{
		Vector3 first = start;
		List<GameObject> arrayRoad = new List<GameObject>();
		while (first != end)
		{
			Vector3 pos = first;
			pos.y += 0.01f;
			GameObject o = (GameObject)Instantiate(road, pos, Quaternion.identity);
			arrayRoad.Add(o);
			first = Vector3.MoveTowards(first, end, 1.0f);
			
		}
		return arrayRoad.ToArray();
	}
}