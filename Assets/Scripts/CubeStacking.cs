using UnityEngine;
using System.Collections;

public class CubeStacking : MonoBehaviour {

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

	/// <summary>
	/// Decreases the value of old position in the array
	/// Increases the value of new position in the array
	/// </summary>
	/// <returns>The occupied array.</returns>
	/// <param name="oldPos">Old position.</param>
	/// <param name="newPos">New position.</param>
	public int UpdateOccupiedArray (Vector3 oldPos, Vector3 newPos) 
	{
		occupiedArray[(int)oldPos.x, (int)oldPos.z] -= 1;
		occupiedArray[(int)newPos.x, (int)newPos.z] += 1;

		return occupiedArray[(int)newPos.x, (int)newPos.z];
	}
}
