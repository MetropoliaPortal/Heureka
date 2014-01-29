using UnityEngine;
using System.Collections;

public class BuildingStorage : MonoBehaviour {

	public GameObject[]house;
	public GameObject[]largeBuilding2B;
	public GameObject[]environment;
	public GameObject[]tallBuilding2B;

	public GameObject GetSmallBuilding()
	{
		int rand = Random.Range (0,house.Length);
		return house[rand];
	}

	public GameObject GetSmallEnvironment()
	{
		int rand = Random.Range (0,environment.Length);
		return environment[rand];
	}

	public GameObject GetLargeBulding2B()
	{	
		int rand = Random.Range (0, largeBuilding2B.Length);
		return largeBuilding2B[rand];
	}
	public GameObject GetTallBuilding2B()
	{
		int rand = Random.Range (0, tallBuilding2B.Length);
		return tallBuilding2B[rand];
	}
}
