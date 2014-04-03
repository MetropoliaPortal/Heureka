//#define DEBUGMODE
using UnityEngine;
using System.Collections.Generic;

public class CubePosition : MonoBehaviour
{
    #region MEMBERS
    // Cache the transform for faster performance
    private new Transform transform;

    private Vector3 prevPos;
    public delegate void EventMove();
    public static event EventMove OnMove = new EventMove(() => { });
	public static event EventMove OnMoveSecond = new EventMove(() => { });
    private bool b_fireEvent = false;

    #endregion

    public void Start () 
    {
        transform = base.transform;
        GridManager.obstacleList.Add(gameObject);
        PositionCube(0,0,0);
        
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
		Vector3 pos = new Vector3();
		pos.x = CheckValue(x);
		pos.y = CheckValue (y);

		// Check what cell is used
		Vector2 cell = GetCell(pos.x,pos.y);
		// Convert to game grid
		int key = (int)(cell.x * 1000 + cell.y*10);
		pos = GridManager.gridDict[key];
		if(pos.z <= 0.40)pos.z = 1;
		else if (pos.z > 0.40 && pos.z <= 0.80)pos.z = 3;
		else if (pos.z > 0.80) pos.z = 5;
        // Store the value
		prevPos = transform.position;
			
		transform.position = pos;
		CheckPosition();
    }

	void CheckPosition() 
    {
        // Check if Cube has moved
        if (prevPos != transform.position)
        {
            b_fireEvent = true;
        }
        else if (prevPos == transform.position && b_fireEvent == true)
        {
            b_fireEvent = false;
            OnMove();
			OnMoveSecond();
        }  
    }


	private float CheckValue(float input)
	{
		float offset = 0.2f;
		float [] values = GridManager.values;
		int length = values.Length;
		for(int i = 0 ; i < length; i++)
		{
			float v = values[i];
			if((v - offset)  < input && input <= (v + offset))
			{
				return v;
			}
		}
		return input;
	}

	Vector2 GetCell (float x, float y)
	{
		Vector2 cell = new Vector2();
		int i = 0;
		float [] values = GridManager.values;
		int length = values.Length;
		for(; i < length; i++)
		{
			if(x == values[i])cell.x = i;
		}
		for(i = 0; i < length; i++)
		{
			if(y == values[i])cell.y = i;
		}
		return cell;
	}
}
