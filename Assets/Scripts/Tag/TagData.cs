using UnityEngine;
using System.Collections;

public class TagData
{
	public delegate void TagEvent();
	public delegate void TagEventVec3(Vector3 par);
	public event TagEvent buildingTypeChanged = delegate{};
	public event TagEventVec3 defaultAccelerationChanged = delegate{};

	public string RefId{ get; private set;}
	public string QuuppaId{ get; private set;}

	public TagData(string refId, string quuppaId)
	{
		RefId = refId;
		QuuppaId = quuppaId;
	}

	private BuildingType _buildingType;
	public BuildingType BuildingType
	{ 
		get
		{
			return _buildingType;
		}
		set
		{
			_buildingType = value;
			buildingTypeChanged();
		}
	}

	private Vector3 _defaultAcceleration;
	public Vector3 DefaultAcceleration
	{
		get
		{
			return _defaultAcceleration;
		}
		set
		{
			_defaultAcceleration = value;
			defaultAccelerationChanged( _defaultAcceleration );
		}
	}
}
