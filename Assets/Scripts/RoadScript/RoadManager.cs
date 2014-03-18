using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
public delegate void HandlerEvent();
/// <summary>
/// Class is attached to empty game object to create and take care of the roads
/// </summary>
public class RoadManager : MonoBehaviour 
{
	private Transform startPos, endPos;
	private Node startNode;
	private Node goalNode;
	public GameObject wp;
	
	public List<Node> pathArray = new List<Node>();
	public Transform[] waypoints;

	public GameObject road;
	public GameObject car;

    StackPool m_stack;
	int i;
    GameObject objRoadEdge;
    GameObject objRoadCenter;
	Transform [][] roads = new Transform[4][];


	public static event HandlerEvent OnRoadChange = new HandlerEvent(()=>{});


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
				// FindObjectOfType(...) returns the first RoadManager object in the scene.
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

	void Awake() 
	{
        objRoadEdge = new GameObject("EdgeRoadParent");         // Parent object for edge roads
        objRoadEdge.transform.position = Vector3.zero;          // Position at origin

        objRoadCenter = new GameObject("CenterRoadParent");     // Parent object for center roads
        objRoadCenter.transform.position = Vector3.zero;        // Position at origin

        m_stack = new StackPool(                                // Create stack for center roads, 
            road,                                               // road prefab
            objRoadCenter.transform);                           // Parent object

        DrawEdgeRoads();  
        DrawCenterRoads();                                      // Draw center roads
        CubePosition.OnMove += SolveRoad;                       // Register the solving of the road to the movement of a cube		
	}

    private void DrawEdgeRoads()
    {   
        // Get corner points tagged as CornerWp
        GameObject[] cornerWp = GameObject.FindGameObjectsWithTag("CornerWp");
        // Sort them in order since Unity has no logic to find objects in scene
        cornerWp = cornerWp.OrderBy(x => x.name).ToArray();

        // Iterate to join each consecutive point
        // Last point gets joined with first
        for (i = 0; i < cornerWp.Length; i++)
        {
            if (i != cornerWp.Length - 1)
                DrawStraightRoad(cornerWp[i].transform.position, cornerWp[i + 1].transform.position, tag);
            else
                DrawStraightRoad(cornerWp[i].transform.position, cornerWp[0].transform.position, tag);
        }
    }

	public void SolveRoad()
    {
		DeleteRoad ();
		GridManager.instance.ResolveObstacles ();
		DrawCenterRoads ();
		//wp.BroadcastMessage("UpdatePath",SendMessageOptions.DontRequireReceiver);
	}

    void DeleteRoad()
    {
        // This does not delete them anymore, it just deactivates them and stores them back on the stack
        // Only the two roads in the middle are concerned anymore
        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");
        for (int i = 0; i < roads.Length; i++)
        {
            m_stack.Push(roads[i]);
        }	
    }

	void DrawCenterRoads()
    {
		// Get dynamic paths and store in array
		if (Physics.Linecast(waypoints[1].position, waypoints[5].position))
		{
            roads[0] = DrawRoadAStar(waypoints[1].position, waypoints[5].position);
		}
		else
		{
			roads[0] = DrawStraightRoad(waypoints[1].position, waypoints[5].position);
		}

		if (Physics.Linecast(waypoints[3].position, waypoints[7].position))
		{
			roads[1] = DrawRoadAStar(waypoints[3].position, waypoints[7].position);
		}
		else
		{
			roads[1] = DrawStraightRoad(waypoints[3].position, waypoints[7].position);
		}

		// Store reverse array.
		roads[2] = ReverseArray(roads[0]);
		roads[3] = ReverseArray(roads[1]);;

		//Call all events
		OnRoadChange();
	}
	Transform[] ReverseArray(Transform[]arr)
	{
		int index = 0;
		int length = arr.Length;
		Transform[] tr = new Transform[length];
		for(int i = length - 1; i >= 0; i--)
		{
			tr[index] = arr[i];
			index++;
		}
		return tr;
	}
	Transform[] DrawRoadAStar(Vector3 start, Vector3 end) 
	{
		startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(start)));
		goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(end)));
		pathArray = AStar.FindPath(startNode, goalNode, true);
		List<Transform> arrayRoad = new List<Transform>();
		
		// Place first
		Vector3 pos = start;
		pos.y += 0.01f;
		GameObject o = (GameObject)Instantiate(road, pos, Quaternion.identity);
        o.tag = "Road";

		arrayRoad.Add(o.transform);
		
		for (int i = 0; i < pathArray.Count; i++)
		{
			pos = pathArray[i].position;
			pos.y += 0.01f;
			o = (GameObject)Instantiate(road, pos, Quaternion.identity);
			arrayRoad.Add(o.transform);
			pathArray[i].isRoad = true;
		}
		return arrayRoad.ToArray();
	}
	
	Transform[] DrawStraightRoad(Vector3 start, Vector3 end,string tag = "Road") 
	{
		Vector3 first = start;
		List<Transform> arrayRoad = new List<Transform>();
		while (first != end)
		{
			Vector3 pos = first;
			pos.y += 0.01f;
			GameObject o = (GameObject)Instantiate(road, pos, Quaternion.identity);
            o.tag = tag;
			arrayRoad.Add(o.transform);
			first = Vector3.MoveTowards(first, end, 1.0f);
		}
		return arrayRoad.ToArray();
	}
	public Transform[]GetDynamicPath(int index)
	{
		if(index < roads.Length)
			return roads[index];
		else
		{
			Debug.LogError ("Index out of bounds");
			return null;
		}
	}
}