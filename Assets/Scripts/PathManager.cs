using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PathManager
{
	static Dictionary<int, List<Node>> paths;

	/// <summary>
	/// TEMPORARY: Generates hard-coded paths
	/// Registers path checking to movement of a cube
	/// TODO: Logic for creating needed paths
	/// </summary>
	public static void Initialize(){

		paths = new Dictionary<int, List<Node>>();

		//path 1
		Node startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex( new Vector3(0.5f, 0, 3.5f) )));
		Node goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex( new Vector3(5.5f, 0, 0.5f) )));
		paths.Add(0, AStar.FindPath(startNode, goalNode, false) );

		//path 2
		startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex( new Vector3(39.5f, 0, 36.5f) )));
		goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex( new Vector3(30.5f, 0, 39.5f) )));
		paths.Add(1, AStar.FindPath(startNode, goalNode, false) );

		//path 3
		startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex( new Vector3(21.5f, 0, 39.5f) )));
		goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex( new Vector3(21.5f, 0, 0.5f) )));
		paths.Add(2, AStar.FindPath(startNode, goalNode, false) );

		CubePosition.OnMove += CheckPathsChanged;
	}

	/// <summary>
	/// Checks every created path node by node to find if any of them contains nodes 
	/// which are not marked with isRoad == true. Paths which have nodes without road
	/// on them are recalculated.
	/// </summary>
	public static void CheckPathsChanged(){
		//Debug.Log("Checking changed paths");
		List<int> pathsToRecalculate = new List<int>();
		foreach(KeyValuePair<int, List<Node>> entry in paths)
		{
			List<Node> nodeList = entry.Value;
			for(int i = 0; i < nodeList.Count; i++){
				if( !nodeList[i].isRoad ){
					pathsToRecalculate.Add( entry.Key );
					break;
				}
			}	
		}

		//Can not change dictionary entries during foreach
		for(int i = 0; i < pathsToRecalculate.Count; i++){
			RecalculatePath( pathsToRecalculate[i] );
		}

	}

	/// <summary>
	/// Recalculates a path which is not usable anymore
	/// </summary>
	/// <param name="pathIndex">Path index.</param>
	static void RecalculatePath(int id){
		List<Node> nodeList;
		paths.TryGetValue( id, out nodeList );
		Node startNode = nodeList[0];
		Node goalNode = nodeList[ nodeList.Count-1 ];
		List<Node> pathList = AStar.FindPath(startNode, goalNode, false);
		paths[id] = pathList;
	}

	public static List<Node> GetUpdatedPath( int id ){
		List<Node> nodeList;
		paths.TryGetValue( id, out nodeList );
		return nodeList;
	}

	/// <summary>
	/// Get a path with id 
	/// </summary>
	/// <returns>The new path.</returns>
	/// <param name="id">Identifier.</param>
	public static List<Node> GetNewPath( int id ){
		List<Node> nodeList;
		paths.TryGetValue( id, out nodeList );
		return nodeList;
	}

	/// <summary>
	/// Randomly generates new path id
	/// </summary>
	/// <returns>The new path identifier.</returns>
	public static int GetNewPathId(){
		return Random.Range(0, paths.Count );
	}
}
