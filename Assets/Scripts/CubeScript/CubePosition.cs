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
    public delegate void EventMove();
    public static event EventMove OnMove = new EventMove(() => { });
	public static event EventMove OnMoveSecond = new EventMove(() => { });
    private bool b_fireEvent = false;
    #endregion

    public void Start () 
    {
        transform = base.transform;
        GameObject obj = GameObject.Find("GridManager");
        GridManager gm = obj.GetComponent<GridManager>();
        row = gm.numOfRows;
        column = gm.numOfColumns;
        GridManager.obstacleList.Add(gameObject);
        //PositionCube(0,0,0);
        
#if DEBUGMODE
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
        x = Mathf.Round(x);
        y = Mathf.Round(y);
        z = Mathf.Round(z);
		x *= (row - 2) / 5.0f;
		y *= (column - 2) / 5.0f;

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
            PositionCube();
            b_fireEvent = true;
        }
        else if (prevPos == transform.position && b_fireEvent == true)
        {
            b_fireEvent = false;
            OnMove();
			OnMoveSecond();
        }
        
    }
    public void PositionCube()
    {
        Vector3 pos = transform.position;
        // Drop the decimal part
        pos.x = Mathf.Round(pos.x);
        pos.z = Mathf.Round(pos.z);
        // Constrain to the grid
        pos.x = Mathf.Clamp(pos.x, 2, row - 2);
        pos.z = Mathf.Clamp(pos.z, 2, column - 2);
        // Store the value
        prevPos = transform.position;
        transform.position = pos;
    }
#endif
}
