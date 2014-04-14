using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// CubePosition.cs
/// Script is added to each from the StartScript.cs
/// </summary>
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
	private Queue<Vector3> previousPositions = new Queue<Vector3>();

    #endregion

	/// <summary>
	/// Initialize this instance.
	/// The method is called once from the StartScript.cs on creation
	/// </summary>*
	public void Init () 
   	{
        transform = base.transform;
		PositionCube(0.7f,0.2f,0.7f);
        GridManager.obstacleList.Add(gameObject);
	}
    
    /// <summary>
    /// Position the cube on the center of the four squares it occupies
    /// Constrain the cube within the boundaries of the game
    /// </summary>
    public void PositionCube(float x, float y , float z)
    {
		Vector3 pos = new Vector3(x,y,z);
		pos.x = CheckValue(x, Axis.X);
		pos.z = CheckValue (z, Axis.Y);

		// Convert to game grid
		// The key is defined to be unique
		int key = (int)(pos.x * 1000f + pos.z*10f);
		// The dictionary contains the equivalent vector 3 in game grid
		try
		{
			pos = GridManager.gridDict[key];
		}
		catch(System.Exception e)
		{
			print (e.Message);
		}
		if(y <= 0.40)pos.y = 1;
		else if (y > 0.40f && y <= 0.80f)pos.y = 3;
		else if (y > 0.80f) pos.y = 5;
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

	Vector3 SolveAveragePosition(Vector3 currentPos)
	{
		previousPositions.Enqueue(currentPos);

		while (previousPositions.Count > 20){
			previousPositions.Dequeue();
		}

		float sumX = 0.0f;
		float sumY = 0.0f;
		float sumZ = 0.0f;

		foreach(Vector3 pos in previousPositions){
			sumX += pos.x;
			sumY += pos.y;
			sumZ += pos.z;
		}

		float averageX = sumX / previousPositions.Count;
		float averageY = sumY / previousPositions.Count;
		float averageZ = sumZ / previousPositions.Count;

		return new Vector3(averageX, averageY, averageZ);
	}

 	/*
		The input value is checked against the array of known position for the play area grid
		When a range is found to contain the input, the median value is returned.
		This consists in taking the fact that each cube should be at a certain position in the grid:
		Those values are contained in the static values array. Using the fact that the cube has width 0.2 from center,
		we consider the value returned by Quuppa to be within that range considering error of max 0.2. 
	 */
	private enum Axis{X, Y}
	private float CheckValue(float input, Axis axis)
	{
		float offset = 0.2f;
		float [] values = null;
		if(axis == Axis.X)values = GridManager.valuesX;
		else if(axis == Axis.Y) values  = GridManager.valuesY;
		// Values are {0.715f, 1.115f, 1.515f, 1.915f, 2.315f, 2.715f};
		int length = values.Length;
		if(input < values[0]) return values[0];
		if (input > values[length - 1])return values[length - 1];
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

	/*Vector2 GetCell (float x, float z)
	{
		Vector2 cell = new Vector2();
		int i = 0;
		// Values are {0.715f, 1.115f, 1.515f, 1.915f, 2.315f, 2.715f};
		float [] values = GridManager.values;
		int length = values.Length;
		for(; i < length; i++)
		{
			if(x == values[i])cell.x = values[i];
		}
		for(i = 0; i < length; i++)
		{
			if(z == values[i])cell.y = values[i];
		}
		return cell;
	}*/
}
