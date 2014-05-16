using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// CubePosition.cs
/// Script is added to each from the StartScript.cs
/// </summary>
using System.Collections;


public class CubePosition : MonoBehaviour
{
    #region MEMBERS
	
    public delegate void EventMove();
    public static event EventMove OnMove = new EventMove(() => { });
	public static event EventMove OnMoveSecond = new EventMove(() => { });
	
    
	// Cache the transform for faster performance
	private new Transform transform;
	//private Vector3 prevPos;
	private float positionYQuuppa;
	public float PositionYQuuppa
	{
		get
		{
			return positionYQuuppa;
		}
	}
	//private bool b_fireEvent = false;
	private CubeStacking cubeStacking;
	private string tagState;
	private Vector3 _previousPosition;

    #endregion

	void Start () 
    {
		cubeStacking = GameObject.Find("GameManager").GetComponent<CubeStacking>();

		if(cubeStacking == null)
			Debug.LogError("CubeStacking script is null");
        transform = base.transform;

        GridManager.obstacleList.Add(gameObject);
	}

	void LateUpdate()
	{
		cubeStacking.CompareStackedCubes (transform);
	}
	
	public void PositionCube(float x, float y , float z)
	{
		// If the cube is already on the move, we discard any other moves
		// Next move will be considered once the cube has reached its destiantion
		if(b_cubeOnMove)return;

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

			//save the y position from quuppa to separate variable
			positionYQuuppa = y;

			// Store the value
			if(!b_cubeOnMove)
				StartCoroutine(MoveCubeToPosition(pos));

			_previousPosition = pos;
		}
		catch(System.Exception e)
		{
			print (e.Message);
		}
	}
    
	private bool b_cubeOnMove = false;
	IEnumerator MoveCubeToPosition(Vector3 position)
	{
		b_cubeOnMove = true;
		float ratio = 0;
		float duration = 0.6f;
		float multiplier = 1 / duration;
	
		//2D coordinates to disable effect to Y coordinate
		Vector2 cube2dPos = new Vector2 (transform.position.x, transform.position.z);
		Vector2 targetPos = new Vector2 (position.x, position.z);

		while (cube2dPos != targetPos)
		{
			ratio += Time.deltaTime * multiplier;
			cube2dPos = Vector2.Lerp(cube2dPos, targetPos, ratio);
			transform.position = new Vector3(cube2dPos.x, transform.position.y, cube2dPos.y);
			yield return null;
		}

		//make sure transform is on the exactly correct position
		transform.position = new Vector3 (targetPos.x, position.y, targetPos.y);

		b_cubeOnMove = false;

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

	/// <summary>
	/// Fire OnMove events when tag state is set back to d
	/// </summary>
	public void SetTagState(string c)
	{
		if( tagState != c )
		{
			tagState = c;
			if( tagState == "d" )
			{
				OnMove();
				OnMoveSecond();
			}
		}
	}
	
	#region DEBUG
	
	public void MoveCubeInDebug(Vector3 position)
	{
		position.y++;
	
		float [] values = {2,4,6,8,10,12};
		float x = 0;
		float z = 0;
		if(position.x < values[0] - 1)
			x = values[0];
		if(position.x > values[values.Length - 1] + 1)
			x = values[values.Length - 1];
		if(position.z < values[0] - 1)
			z = values[0];
		if(position.z > values[values.Length - 1] + 1)
			z = values[values.Length - 1];

		for (int i = 0; i < values.Length ; i++)
		{
			if(position.x >= values[i] - 1 && position.x <= values[i] + 1)
				x = values[i];
			if(position.z >= values[i] - 1 && position.z <= values[i] + 1)
				z = values[i];
		}
		Vector3 targetPos = new Vector3(x,position.y,z);

		if(!b_cubeOnMove)
			StartCoroutine(MoveCubeToPosition(targetPos));

		OnMove ();
		OnMoveSecond ();
	}
	 
	#endregion
}
