using UnityEngine;
using System.Collections;

public class TagSystem : MonoBehaviour {

    public BuildingType buildingType;
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {

	}
    public void SwaptTag(BuildingType newType) 
    {
        buildingType = newType;
    }
    void OnGUI() 
    {
    
    }
}
public enum BuildingType
{ 
    House, Environment
}
