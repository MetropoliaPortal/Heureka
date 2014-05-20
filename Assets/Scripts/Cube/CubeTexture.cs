using UnityEngine;
using System.Collections;

public class CubeTexture : MonoBehaviour 
{
	public int category;
	private Material[] materials; 

	private CubeRotation cubeRotation;
	private TagInfo tagInfo;

	void Awake()
	{
		cubeRotation = GetComponent<CubeRotation> ();
		cubeRotation.rotationChanged += ChangeTexture;

		tagInfo = GetComponent<TagInfo>();
		tagInfo.buildingTypeChanged += ChangeBuildingType;
	}
	
	private void ChangeBuildingType()
	{
		string tempType = tagInfo.BuildingType.ToString ();				// Convert type to string
		string url = "Materials/" + tempType;			// Append type to url
		materials = Resources.LoadAll<Material>(url);	// Get corresponding materials from Resources folder
	}

	private void ChangeTexture()
	{
		renderer.material = materials[ cubeRotation.currentIndex ];
	}
}
