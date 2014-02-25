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
    #endregion
    void Start () 
    {
        GameObject obj = GameObject.Find("GridManager");
        GridManager gm = obj.GetComponent<GridManager>();
        row = gm.numOfRows;
        column = gm.numOfColumns;
        transform = base.transform;
        PositionCube();
#if DEBUGMODE
        InvokeRepeating("PositionCube", 0.001f, 0.016f);
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
        pos.x = Mathf.Floor(pos.x);
        pos.z = Mathf.Floor(pos.z);
        // Constrain to the grid
        pos.x = Mathf.Clamp(pos.x,1,row - 1);
        pos.z = Mathf.Clamp(pos.z, 1, column - 1);
        // Store the value
        transform.position = pos;
        prevPos = pos;
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
