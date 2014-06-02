using UnityEngine;
using System.Collections;

public class CubeTexture : MonoBehaviour 
{
	public int category;
	private Material[] _materials; 
	private CubeRotation _cubeRotation;
	private QuuppaData _quuppaData;

	void Awake()
	{
		_cubeRotation = GetComponent<CubeRotation> ();
		_cubeRotation.rotationChanged += ChangeTexture;
	}

	public void Initialize()
	{
		_quuppaData = GetComponent<QuuppaData>();
		_quuppaData.TagData.buildingTypeChanged += ChangeBuildingType;
		ChangeBuildingType ();
		ChangeTexture ();
	}

	private void ChangeBuildingType()
	{
		string tempType = _quuppaData.TagData.BuildingType.ToString ();				// Convert type to string
		string url = "Materials/" + tempType;			// Append type to url
		_materials = Resources.LoadAll<Material>(url);	// Get corresponding materials from Resources folder
		renderer.material = _materials [0];
	}

	private void ChangeTexture()
	{
		renderer.material = _materials[ _cubeRotation.CurrentIndex ];
	}
}
