using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Events and variables from quuppa server and tag info file
/// </summary>
public class QuuppaData : MonoBehaviour
{
	public delegate void ChangedEvent();
	public delegate void ChangedEventString(string param1);
	public delegate void ChangedEventVector3(Vector3 param1);
	public delegate void ChangedEventFloat(float param1);
	
	public event ChangedEventVector3 positionChanged = delegate{};
	public event ChangedEventString tagStateChanged = delegate{};
	public event ChangedEventFloat positionAccuracyChanged = delegate{};
	public event ChangedEventFloat heightQuuppaChanged = delegate{};
	public event ChangedEventVector3 accelerationChanged = delegate{};

	public TagData TagData{get;set;}
	public float BatteryVoltage{ get; set;}
	
	private float heightQuuppa;
	public float HeightQuuppa
	{
		get
		{
			return heightQuuppa;
		}
		set
		{
			heightQuuppa = value;
			heightQuuppaChanged( heightQuuppa );
		}
	}

	private Vector3 acceleration;
	public Vector3 Acceleration
	{ 
		get
		{
			return acceleration;
		} 
		set
		{
			acceleration = value;
			accelerationChanged( acceleration );
		}
	}

	private Vector3 position;
	public Vector3 Position
	{ 
		get
		{
			return position;
		} 
		set
		{
			position = value;
			positionChanged( position );
		}
	}
	
	private string tagState;
	public string TagState
	{
		get
		{
			return tagState;
		}
		set
		{
			if( tagState != value)
			{
				tagState = value;
				tagStateChanged( tagState );
			}
		}
	}

	private float positionAccuracy;
	public float PositionAccuracy
	{
		get
		{
			return positionAccuracy;
		}
		set
		{
			positionAccuracy = value;
			positionAccuracyChanged( positionAccuracy );
		}
	}	
}
