using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathManager
{
	static Dictionary<int, List<Node>> paths;

	public static void GeneratePaths(){

		paths = new Dictionary<int, List<Node>>();

		//Couple of hard-coded path start and end points for now

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
	}

	public static void CheckPathsChanged(){
		Debug.Log("Checking changed paths");
		List<int> pathsToRecalculate = new List<int>();
		foreach(KeyValuePair<int, List<Node>> entry in paths)
		{
			List<Node> nodeList = entry.Value;
			for(int i = 0; i < nodeList.Count; i++){
				if( !nodeList[i].isRoad && !nodeList[i].isEdgeRoad ){
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

	static void RecalculatePath(int pathIndex){
		List<Node> nodeList;
		paths.TryGetValue( pathIndex, out nodeList );
		Node startNode = nodeList[0];
		Node goalNode = nodeList[ nodeList.Count-1 ];
		List<Node> pathList = AStar.FindPath(startNode, goalNode, false);
		paths[pathIndex] = pathList;
	}

	public static List<Node> GetNewPath(){
		List<Node> nodeList;
		paths.TryGetValue( Random.Range(0, paths.Count), out nodeList );
		return nodeList;
	}
}
