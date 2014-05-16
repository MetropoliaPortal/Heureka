using UnityEngine;
using System.Collections;

public class CubeTexture : MonoBehaviour 
{
	public int category;
	private Material[] materials; 

	private CubeRotation cubeRotation;

	void Start()
	{
		cubeRotation = GetComponent<CubeRotation> ();
		cubeRotation.rotationChanged += ChangeTexture;

		//without quuppa connection
		if(category == 0)
			Initialize( BuildingType.Leisure);
		else if (category == 1)
			Initialize( BuildingType.Official);
		else if (category == 2)
			Initialize( BuildingType.Residential);
		else if (category == 3)
			Initialize( BuildingType.Shop);
		
		renderer.material = materials[0];
	}

	public void Initialize (BuildingType type = BuildingType.Leisure) 
	{
		string tempType = type.ToString ();				// Convert type to string
		string url = "Materials/" + tempType;			// Append type to url
		materials = Resources.LoadAll<Material>(url);	// Get corresponding materials from Resources folder
		
	}

	private void ChangeTexture()
	{
		renderer.material = materials[ cubeRotation.currentIndex ];
	}

}
