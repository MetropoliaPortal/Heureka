using UnityEngine;
using System.Collections;

public class CubeHighlight : MonoBehaviour 
{
	private Material previousMaterial;
	private Material highlightMaterial;
	private bool isOn = false;

	void Start()
	{
		highlightMaterial = Resources.Load("Materials/CubeHighlight") as Material;
		previousMaterial = renderer.material;
	}

	public void ChangeHighlight()
	{
		if( !isOn )
		{
			previousMaterial = renderer.material;
			renderer.material = highlightMaterial;
			isOn = true;
		}
		else
		{
			renderer.material = previousMaterial;
			isOn = false;
		}
	}
}
