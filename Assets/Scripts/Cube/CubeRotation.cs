using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeRotation : MonoBehaviour 
{
	public delegate void RotationChangedEvent();
	public event RotationChangedEvent rotationChanged = delegate{};

	public int CompareAmountTriggered = 0;
	public int CompareAmountDefault = 0;
	public int AccelerationLimit = 35;
	public int CurrentIndex{get;set;}

	private int _currentCompareAmount;
	private Queue<int> _previousRotations = new Queue<int> ();
	private QuuppaData _quuppaData;
	private Vector3 _defaultAcceleration;
	private float _xLimit = 0;
	private float _yLimit = 0;
	private float _zLimit = 0;

	void Start()
	{
		_quuppaData = GetComponent<QuuppaData>();
		_quuppaData.accelerationChanged += ProcessRotation;
		_quuppaData.tagStateChanged += ChangeCompareAmount;
		_currentCompareAmount = CompareAmountDefault;
	}

	public void Initialize()
	{
		_quuppaData.TagData.defaultAccelerationChanged += ChangeDefaultAcceleration;
		_defaultAcceleration = _quuppaData.TagData.DefaultAcceleration;
	}
	
	/// <summary>
	/// Process the cube rotation and apply corresponding texture
	/// The method is called from the ConnectScript.cs where the Quuppa json file is requested
	/// // Define which axis is receiving acceleration
	/// // polarity indicates whether the axis is up or down
	/// </summary>
	/// <param name="acceleration">The Vector3 containing the 3 acceleration values from the tag</param>
	public void ProcessRotation (Vector3 acceleration)
	{
		int axis = 0;	
		int polarity = 0;	

		for(; axis < 3 ; axis++)						
		{
			if( axis == 0 )
			{
				if(Mathf.Abs(acceleration[axis]) > AccelerationLimit - _xLimit)break;
			}
			else if( axis == 1 )
			{
				if(Mathf.Abs(acceleration[axis]) > AccelerationLimit - _yLimit)break;
			}
			else if( axis == 2 )
			{
				if(Mathf.Abs(acceleration[axis]) > AccelerationLimit - _zLimit)break;
			}
		}

		/*
		for(; axis < 3 ; axis++)						
		{
			if(Mathf.Abs(acceleration[axis]) > AccelerationLimit)break;
														
		}
		*/
		if(axis == 3) return;						

		polarity = (acceleration[axis] >= 0) ? 1: -1;

		if( Helpers.ComparePreviousValues( _previousRotations, axis * polarity, _currentCompareAmount) )
		{
			CheckIfRotationChanged(axis, polarity);
		}
	}

	// The switch case uses the value of the axis, then once inside the statement, 
	// we check if the polarity is positive or negative
	private void CheckIfRotationChanged(int axis, int polarity)
	{
		int materialIdx = -1;
		switch(axis)
		{
		case 0:
			materialIdx = polarity > 0 ? 0 : 1; 
			break;
		case 1:
			materialIdx = polarity > 0 ? 2 : 3; 
			break;
		case 2:
			materialIdx = polarity > 0 ? 4 : 5;
			break;
		}

		if( materialIdx != CurrentIndex )
		{
			CurrentIndex = materialIdx;
			rotationChanged();
		}
	}

	private void ChangeCompareAmount(string tagState)
	{ 
		if ( tagState == "t" ) 
		{
			_currentCompareAmount = CompareAmountTriggered;
		}
		else
		{
			_currentCompareAmount = CompareAmountDefault;
		}
	}

	private void ChangeDefaultAcceleration(Vector3 newDefAcc)
	{
		//TODO: check what is number for "default" state
		_defaultAcceleration = newDefAcc;
		_xLimit = (Vector3.Dot (Vector3.right, _defaultAcceleration) /  65) - 65;
		_yLimit = Vector3.Dot (Vector3.up, _defaultAcceleration) /  65;
		_zLimit = Vector3.Dot (Vector3.forward, _defaultAcceleration) / 65;
	}
}