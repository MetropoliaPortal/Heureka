using UnityEngine;
using System.Collections;

public class TagInfo : MonoBehaviour 
{
	public int RefId{ get; set;}
	public string QuuppaId{ get; set;}
	public float BatteryVoltage{ get; set;}
	public BuildingType BuildingType{ get; set;}
	public Vector3 Rotation{ get; set;}
	public Vector3 Position{ get; set;}
	public string TagState{ get; set;}
}
