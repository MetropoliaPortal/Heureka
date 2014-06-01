using UnityEngine;
using System.Collections;

public class CubeDebug : MonoBehaviour 
{
	public bool InitializeCube;
	public bool ShowTriggered;
	private Material _triggeredHighLightMaterial;
	private Material _previousMaterial;

	void Start () 
	{
		QuuppaData quuppaData = GetComponent<QuuppaData> ();
		_previousMaterial = renderer.material;
		_triggeredHighLightMaterial = Resources.Load("Materials/CubeTriggeredHighlight") as Material;

		if( ShowTriggered )
		{
			quuppaData.tagStateChanged += EnableHighlight;
		}

		if( InitializeCube )
		{
			///tagInfo.BuildingType = BuildingType.Leisure;
			//tagInfo.QuuppaId = "123456789";
			GameObject.Find("GameManager").GetComponent<TagManager>().QuuppaDataDictionary.Add( "123456789", quuppaData );
		}
	}

	private void EnableHighlight(string tagState)
	{
		if ( tagState == "t" )
		{
			_previousMaterial = renderer.material;
			renderer.material = _triggeredHighLightMaterial;
		}
		else
		{
			renderer.material = _previousMaterial;
		}
		//else Debug.LogError("Tagstate not handled properly");
	}
}
