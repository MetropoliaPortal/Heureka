using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CubePosition : MonoBehaviour
{
    #region MEMBERS
    public delegate void EventMove();
    public static event EventMove OnMove = new EventMove(() => { });
	public int CompareAmountTriggered = 0;
	public int CompareAmountDefault = 0;
	public float PositionAccuracyLimit = 0;

	[HideInInspector]
	public bool IsMoving = false;

	private int currentCompareAmount;
	// Cache the transform for faster performance
	private new Transform transform;

	private bool isNoiseTooHigh = false;
	private Queue<int> previousPositions = new Queue<int> ();
	private QuuppaData tagInfo;
	private CubeStacking cubeStacking;
	private int previousKey = 0;
    #endregion

	void Start () 
    {
        transform = base.transform;
        GridManager.obstacleList.Add(gameObject);
		tagInfo = GetComponent<QuuppaData>();
		tagInfo.positionChanged += SolvePosition;
		tagInfo.tagStateChanged += ChangeCompareAmount;
		tagInfo.positionAccuracyChanged += SolveIsNoiseTooHigh;
		cubeStacking = GetComponent<CubeStacking> ();
		currentCompareAmount = CompareAmountDefault;
	}

	private void SolvePosition(Vector3 pos)
	{
		// If the cube is already on the move, we discard any other moves
		if(IsMoving)return;

		pos.x = GetGridPosition (pos.x, Axis.X);
		pos.z = GetGridPosition (pos.z, Axis.Y);
		
		// Convert to game grid
		// The key is defined to be unique
		int key = (int)(pos.x * 1000f + pos.z*10f);
		// The dictionary contains the equivalent vector 3 in game grid
		try
		{
			//key = Helpers.SolveMode(previousPositions, key, currentCompareAmount);
			pos = GridManager.gridDictionary[key];

			if( Helpers.ComparePreviousValues( previousPositions, key, currentCompareAmount) )
			{
				if( key != previousKey && !isNoiseTooHigh)
				{
					StartCoroutine(MoveToPosition(pos));
					previousKey = key;
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError (e.Message);
		}
	}

	//2D coordinates to disable effect to Y coordinate
	//after lerp, makes sure transform is on the exactly correct position
	private IEnumerator MoveToPosition(Vector3 position)
	{
		IsMoving = true;
		float ratio = 0;
		float duration = 0.6f;
		float multiplier = 1 / duration;

		Vector2 cube2dPos = new Vector2 (transform.position.x, transform.position.z);
		Vector2 targetPos = new Vector2 (position.x, position.z);

		while (cube2dPos != targetPos)
		{
			ratio += Time.deltaTime * multiplier;
			cube2dPos = Vector2.Lerp(cube2dPos, targetPos, ratio);
			transform.position = new Vector3(cube2dPos.x, transform.position.y, cube2dPos.y);
			yield return null;
		}

		transform.position = new Vector3 (targetPos.x, position.y, targetPos.y);
		cubeStacking.CompareStackedCubes ();

		IsMoving = false;
		OnMove ();
	}

 	/*
		The input value is checked against the array of known position for the play area grid
		When a range is found to contain the input, the median value is returned.
		This consists in taking the fact that each cube should be at a certain position in the grid:
		Those values are contained in the static values array. Using the fact that the cube has width 0.2 from center,
		we consider the value returned by Quuppa to be within that range considering error of max 0.2. 
	 */
	private enum Axis{X, Y}
	private float GetGridPosition(float input, Axis axis)
	{
		float offset = 0.2f;
		float [] values = null;
		if(axis == Axis.X)values = GridManager.valuesX;
		else if(axis == Axis.Y) values  = GridManager.valuesY;
		// Values are {0.715f, 1.115f, 1.515f, 1.915f, 2.315f, 2.715f};
		int length = values.Length;


		//put cubes on a stack if not on screen
		//if(input < values[0] - offset) return values[0];
		//if (input > values[length - 1] + offset) return values[length - 1];
		if(input < values[0] - offset || input > values[length - 1] + offset)
		{
			if(axis == Axis.X) return -2;
			else return 2;
		}

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

	public void MoveInDebug(Vector3 position)
	{
		if(IsMoving)return;

		position.y = 1.0f;
	
		float [] values = {2,4,6,8,10,12};
		float x = 0;
		float z = 0;
	

		for (int i = 0; i < values.Length ; i++)
		{
			if(position.x >= values[i] - 1 && position.x <= values[i] + 1)
				x = values[i];
			if(position.z >= values[i] - 1 && position.z <= values[i] + 1)
				z = values[i];
		}

		if(position.x < values[0] - 1)
		{
			//x = values[0];
			x = -2;
			z = 2;
		}
		
		if(position.x > values[values.Length - 1] + 1)
		{
			//x = values[values.Length - 1];
			x = -2;
			z = 2;
		}
		
		if(position.z < values[0] - 1)
		{
			//z = values[0];
			x = -2;
			z = 2;
		}
		
		if(position.z > values[values.Length - 1] + 1)
		{
			//z = values[values.Length - 1];
			x = -2;
			z = 2;
		}
		Vector3 targetPos = new Vector3(x,position.y,z);

		StartCoroutine(MoveToPosition(targetPos));
	}

	private void ChangeCompareAmount(string tagState)
	{ 
		if ( tagState == "t" ) 
		{
			currentCompareAmount = CompareAmountTriggered;
		}
		else
		{
			currentCompareAmount = CompareAmountDefault;
		}
	}

	private void SolveIsNoiseTooHigh(float posAcc)
	{
		if( posAcc > PositionAccuracyLimit)
		{
			isNoiseTooHigh = true;
			Debug.LogError("Position accuracy too bad");
		}
		else
		{
			isNoiseTooHigh = false;
		}
	}
}
