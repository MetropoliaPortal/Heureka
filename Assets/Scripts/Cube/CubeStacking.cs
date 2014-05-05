using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class CubeStacking : MonoBehaviour 
{
	public void CompareStackedCubes (Transform t) 
	{
		List<Transform> ts = new List<Transform>();
	
		foreach(GameObject o in GridManager.obstacleList) 
		{
			Vector3 otherCubePos = o.transform.position;
			if( otherCubePos.x == t.position.x && otherCubePos.z == t.position.z )
			{
				ts.Add( o.transform );
			}
		}

		//Sort array from smallest to largest
		Transform[] transformArray = ts.ToArray();
		System.Array.Sort(transformArray, delegate(Transform first, Transform second){
			return (second.GetComponent<CubePosition>().PositionYQuuppa.CompareTo(first.GetComponent<CubePosition>().PositionYQuuppa));
		});

		for (int i = 0; i < transformArray.Length; i++)
		{
			Vector3 pos = transformArray[i].position;

			float newY;
			if(i == 0)
				newY = (i + 1);
			else
				newY = (i + i + 1);

			transformArray[i].position = new Vector3(pos.x, newY , pos.z);
		}
	}
}

#region NOT IN USE

/*
	private int[,] occupiedArray;

	void Start () 
	{
		GridManager gm = GameObject.Find("GridManager").GetComponent<GridManager>();
		int width = gm.numOfColumns;
		int height = gm.numOfRows;
		occupiedArray = new int[width,height];

		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				occupiedArray[i,j] = 0;
			}
		}
	}
	*/

/*
	public int UpdateOccupiedArray (Vector3 oldPos, Vector3 newPos) 
	{
		occupiedArray[(int)oldPos.x, (int)oldPos.z] -= 1;
		occupiedArray[(int)newPos.x, (int)newPos.z] += 1;

		//Debug.Log("new: " +occupiedArray[(int)newPos.x, (int)newPos.z]);

		return occupiedArray[(int)newPos.x, (int)newPos.z];
	}
	*/

/*
public class CubeStacking : MonoBehaviour {

	private Dictionary<int,int> usedCell = new Dictionary<int, int>();

	void Awake () 
	{
		foreach(KeyValuePair<int, Vector3> entry in GridManager.gridDict)
		{
			Vector3 vec = entry.Value;
			int i = Mathf.RoundToInt(vec.x) * 100 + Mathf.RoundToInt(vec.z);
			usedCell.Add (i,0);
		}
	}
	void Start(){}
	/// <summary>
	/// Decreases the value of old position in the array
	/// Increases the value of new position in the array
	/// </summary>
	/// <returns>The occupied array.</returns>
	/// <param name="oldPos">Old position.</param>
	/// <param name="newPos">New position.</param>
	public int UpdateOccupiedArray (Vector3 oldPos, Vector3 newPos) 
	{
		// Convert old position to key
		int i = Mathf.RoundToInt (oldPos.x) * 100 + Mathf.RoundToInt(oldPos.z);
		// Decrease by 1
		try{
			usedCell[i] -= 1;
			// Make sure the value is not negative
			if(usedCell[i] < 0) usedCell[i] = 0;
			// Convert new position to key
			i = Mathf.RoundToInt(newPos.x) * 100 + Mathf.RoundToInt(newPos.z);
			// Increase by 1
			usedCell[i] += 1;
		}
		catch(Exception )
		{
			return 1;
		}
		// return the new value
		return usedCell[i];
	}
}
*/

#endregion

