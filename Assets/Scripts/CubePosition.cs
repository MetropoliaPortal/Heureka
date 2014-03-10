#define DEBUGMODE
using UnityEngine;
using System.Collections;

public class CubePosition : MonoBehaviour
{
    #region MEMBERS
    // Cache the transform for faster performance
    private new Transform transform;
    // Cache the grid
    private Vector3 prevPos;
    private int row;
    private int column;
	private RoadManager roadManager;
    #endregion

    public void AwakeCubePosition () 
    {
        GameObject obj = GameObject.Find("GridManager");
        GridManager gm = obj.GetComponent<GridManager>();
		roadManager = GameObject.Find ("RoadManager").GetComponent<RoadManager>();
        row = gm.numOfRows;
        column = gm.numOfColumns;
        transform = base.transform;
        PositionCube();
#if DEBUGMODE
		InvokeRepeating("CheckPosition", 0.001f, 0.01f);
#endif
	}
    
    /// <summary>
    /// Position the cube on the center of the four squares it occupies
    /// Constrain the cube within the boundaries of the game
    /// </summary>
    public void PositionCube()
    {
        Vector3 pos = transform.position;
        // Drop the decimal part
        pos.x = Mathf.Round(pos.x);
        pos.z = Mathf.Round(pos.z);
        // Constrain to the grid
        pos.x = Mathf.Clamp(pos.x,2,row - 2);
        pos.z = Mathf.Clamp(pos.z, 2, column - 2);
        // Store the value
		prevPos = transform.position;
        transform.position = pos;

		//Debug.Log ("Pos: " + transform.position.x + ", " + transform.position.z);
		
    }
#if DEBUGMODE
    void CheckPosition() 
    {
        // Check if Cube has moved
        if (prevPos != transform.position)
        {
            PositionCube();
        }
    }
#endif
}
