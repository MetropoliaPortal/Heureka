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
	public List<Node> PathList = new List<Node>();
	public Transform[] Waypoints;
	public GameObject RoadPrefab;
	private Node startNode;
	private Node goalNode;
    private StackPool roadStack;
	private int i;
	private GameObject roadContainer;
	private Vector3 [][] roads = new Vector3[4][];

	public static event HandlerEvent OnRoadChange = new HandlerEvent(()=>{});

	// The boolean to set for redrawing
	private bool b_redraw = false;

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
        roadContainer = new GameObject("Roads");
        roadContainer.transform.position = Vector3.zero;
		roadContainer.transform.parent = GameObject.Find("DynamicObjects").transform;

        roadStack = new StackPool( RoadPrefab, roadContainer.transform );                        

        DrawEdgeRoads();  
        DrawCenterRoads();
        CubePosition.OnMove += SetRedrawRoad;                   // Register the method to redraw the road		
	}
	// LateUpdate is called once all update are done
	// Once all cubes have been checked for movement, the boolean is checked and the road redrawn if necessary.
	void LateUpdate()
	{
		if(b_redraw)
		{
			SolveRoad();
			b_redraw = false;
		}
	}

	// The method used to subscribe to the movement of the cubes
	private void SetRedrawRoad()
	{
		b_redraw = true;
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
	}

    private void DeleteRoad()
    {
        // This does not delete them anymore, it just deactivates them and stores them back on the stack
        // Only the two roads in the middle are concerned anymore
        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");
        for (int i = 0; i < roads.Length; i++)
        {
            roadStack.Push(roads[i]);
        }	
    }

	private void DrawCenterRoads()
    {
		// Get dynamic paths and store in array
		RaycastHit hit;
		if (Physics.Linecast(Waypoints[1].position, Waypoints[5].position, out hit/*,m_layer*/))
		{
            roads[0] = DrawRoadAStar(Waypoints[1].position, Waypoints[5].position);
		}
		else
		{
			roads[0] = DrawStraightRoad(Waypoints[1].position, Waypoints[5].position);
		}

		if (Physics.Linecast(Waypoints[3].position, Waypoints[7].position,out hit/*,m_layer*/))
		{
			roads[1] = DrawRoadAStar(Waypoints[3].position, Waypoints[7].position);
		}
		else
		{
			roads[1] = DrawStraightRoad(Waypoints[3].position, Waypoints[7].position);
		}

		// Store reverse array.
		roads[2] = ReverseArray(roads[0]);
		roads[3] = ReverseArray(roads[1]);
		//Call all events
		OnRoadChange();
	}

	private Vector3[] ReverseArray(Vector3[]arr)
	{
		int index = 0;
		int length = arr.Length;
		Vector3[] tr = new Vector3[length];
		for(int i = length - 1; i >= 0; i--)
		{
			tr[index] = arr[i];
			index++;
		}
		return tr;
	}

	private Vector3[] DrawRoadAStar(Vector3 start, Vector3 end) 
	{

		//startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(start)));
		//goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(end)));
		startNode = new Node(start);
		goalNode = new Node(end);
		PathList = AStar.FindPath(startNode, goalNode, true);
		List<Vector3> arrayRoad = new List<Vector3>();
		
		// Place first
		Vector3 pos = start;
		pos.y = 0.01f;
		GameObject o = roadStack.Pop();
		o.transform.position = pos;
        o.tag = "Road";

		arrayRoad.Add(o.transform.position);
		
		for (int i = 0; i < PathList.Count; i++)
		{
			pos = PathList[i].position;
			pos.y = 0.01f;
			o = roadStack.Pop();
			o.transform.position = pos;
			arrayRoad.Add(o.transform.position);
			PathList[i].isRoad = true;
		}
		return arrayRoad.ToArray();
	}
	
	private Vector3[] DrawStraightRoad(Vector3 start, Vector3 end,string tag = "Road") 
	{

		Vector3 first = start;
		List<Vector3> arrayRoad = new List<Vector3>();
		while (first != end)
		{
			Vector3 pos = first;
			pos.y = 0.01f;
			GameObject o = roadStack.Pop();
			o.transform.position = pos;
            o.tag = tag;
			arrayRoad.Add(o.transform.position);
			first = Vector3.MoveTowards(first, end, 1.0f);
		}
		return arrayRoad.ToArray();
	}

	public Vector3[] GetDynamicPath(int index)
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