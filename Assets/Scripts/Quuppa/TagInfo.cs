using UnityEngine;
using System.Collections;

/// <summary>
/// Events and variables from quuppa server and tag info file
/// todo: make all events for all dynamic
/// </summary>
public class TagInfo : MonoBehaviour 
{
	public delegate void ChangedEvent();
	public event ChangedEvent buildingTypeChanged = delegate{};

	public delegate void ChangedEventVector3(Vector3 param1);
	public event ChangedEventVector3 accelerationChanged = delegate{};

	public event ChangedEventVector3 positionChanged = delegate{};
	public event ChangedEvent tagStateChanged = delegate{};

	
	public int RefId{ get; set;}
	public string QuuppaId{ get; set;}
	public float BatteryVoltage{ get; set;}
	public float HeightQuuppa{ get; set; }
	
	private BuildingType buildingType;
	public BuildingType BuildingType
	{ 
		get
		{
			return buildingType;
		}
		set
		{
			buildingType = value;
			buildingTypeChanged();
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

	//event not used atm
	private string tagState;
	public string TagState
	{
		get
		{
			return tagState;
		}
		set
		{
			tagState = value;
			tagStateChanged();
		}
	}
}
