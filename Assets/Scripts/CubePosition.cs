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
#if DEBUGMODE
        //InvokeRepeating("PositionCube", 0.001f, 0.016f);
		InvokeRepeating("CheckPosition", 0.001f, 0.01f);
#endif
	}
    
    /// <summary>
    /// Position the cube on the center of the four squares it occupies
    /// Constrain the cube within the boundaries of the game
    /// </summary>
    public void PositionCube(float x, float y , float z)
    {
        // Drop the decimal part
		x *= (row - 2) / 5.0f;
		y *= (column - 2) / 5.0f;
        x = Mathf.Round(x);
        y = Mathf.Round(y);
		z = Mathf.Round(z);
        // Store the value
		prevPos = transform.position;
		transform.position = new Vector3(Mathf.Clamp(x,2,row - 2),z,Mathf.Clamp(y, 2, column - 2));	
    }
#if DEBUGMODE
    void CheckPosition() 
    {
        // Check if Cube has moved
        if (prevPos != transform.position)
        {
            //PositionCube();
        }
    }
#endif
}
