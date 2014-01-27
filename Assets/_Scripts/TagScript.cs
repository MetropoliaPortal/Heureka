using UnityEngine;
using System.Collections;

public class TagScript : MonoBehaviour {

	public BuildingType buildingType;


	public BuildingType GetTag()
	{
		return buildingType;
	}
	public void SetTag(BuildingType bt)
	{
	 	buildingType = bt;
	}
}

public enum BuildingType
{ 
	House, Environment
}
