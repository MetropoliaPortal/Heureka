using UnityEngine;
using System.Collections;

public class BuildingStorage : MonoBehaviour {

	public GameObject[]house;
	public GameObject[]largeBuilding2B;
	public GameObject[]environment;

	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

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
}
