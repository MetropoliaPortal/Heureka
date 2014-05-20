using UnityEngine;
using System.Collections;

public class CubeDebug : MonoBehaviour 
{
	void Start () 
	{
		TagInfo info = GetComponent<TagInfo>();
		info.BuildingType = BuildingType.Leisure;
		GameObject.Find("GameManager").GetComponent<TagManager>().tagInfos.Add( info );
	}
}
