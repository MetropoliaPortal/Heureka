using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class RoadManager : MonoBehaviour 
{
	private Transform startPos, endPos;
	private Node startNode;
	private Node goalNode;
	
	public List<Node> pathArray = new List<Node>();
	public Transform[] waypoints;

	public GameObject road;
	public GameObject car;

	GameObject[][] roadArray = new GameObject[2][];
    StackPool m_stack;
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

	void Start() 
	{
        m_stack = new StackPool(road);
        // Here values is arbitrary, it could be lower, dunno
        for (int i = 0; i < 50; i++)
        {
            GameObject o = (GameObject)Instantiate(road);
            m_stack.Push(o);
        }
        // Draw edge roads.
        DrawEdgeRoad();
        // Register the solving of the road to the movement of a cube
        CubePosition.OnMove += SolveRoad;


		InvokeRepeating("AddCar", 2.0f, 1.0f);
		//Physics.IgnoreLayerCollision (0, 8);
		//AddCar();
	}

	private void AddCar(){
		GameObject c = (GameObject)Instantiate (car);
	}

	//Not in use
	/*
    private void DrawCentreRoad()
    {
        // Get corner points tagged as CornerWp
        GameObject[] centerWp = GameObject.FindGameObjectsWithTag("CenterWp");
        // Sort them in order since Unity has no logic to find objects in scene
        centerWp = centerWp.OrderBy(x => x.name).ToArray();
        new GameObject("RoadCenter");

    }*/

    private void DrawEdgeRoad()
    {
        // Get corner points tagged as CornerWp
        GameObject[] cornerWp = GameObject.FindGameObjectsWithTag("CornerWp");
        // Sort them in order since Unity has no logic to find objects in scene
        cornerWp = cornerWp.OrderBy(x => x.name).ToArray();
        GameObject o = new GameObject("Road");
        // Tag them differently so they are not considered when redrawing
        roadObj = o.transform;
        roadObj.position = new Vector3(0f, 0f, 0f);
        // Iterate to join each consecutive point
        // Last point gets joined with first
        string tag = "EdgeRoad";
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
        //print("Call");
		DeleteRoad ();
		GridManager.instance.ResolveObstacles ();
		SolveCrossingRoads ();
		
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
	void SolveCrossingRoads()
    {
		if (Physics.Linecast(waypoints[1].position, waypoints[5].position))
		{
			roadArray[0] = DrawRoadAStar(waypoints[1].position, waypoints[5].position);
		}
		else
		{
			roadArray[0] = DrawStraightRoad(waypoints[1].position, waypoints[5].position);
		}
		
		if (Physics.Linecast(waypoints[3].position, waypoints[7].position))
		{
			roadArray[1] = DrawRoadAStar(waypoints[3].position, waypoints[7].position);
		}
		else
		{
			roadArray[1] = DrawStraightRoad(waypoints[3].position, waypoints[7].position);
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
        o.tag = "Road";

		arrayRoad.Add(o);
		o.transform.parent = roadObj;
		
		for (int k = 0; k < pathArray.Count; k++)
		{
			pos = pathArray[k].position;
			pos.y += 0.01f;
			o = (GameObject)Instantiate(road, pos, Quaternion.identity);
			if (i == 2) o.name = "New";
			arrayRoad.Add(o);
		}
		return arrayRoad.ToArray();
	}
	
	GameObject[] DrawStraightRoad(Vector3 start, Vector3 end, string tag = "Road") 
	{
		Vector3 first = start;
		List<GameObject> arrayRoad = new List<GameObject>();
		while (first != end)
		{
			Vector3 pos = first;
			pos.y += 0.01f;
			GameObject o = (GameObject)Instantiate(road, pos, Quaternion.identity);
            o.tag = tag;
			arrayRoad.Add(o);
			first = Vector3.MoveTowards(first, end, 1.0f);
		}
		return arrayRoad.ToArray();
	}
}