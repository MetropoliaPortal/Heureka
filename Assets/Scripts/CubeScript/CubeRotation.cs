﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Cube rotation script is attached to each cube.
/// The rotation defines the texture to ba applied
/// The script is added from the StartScript.cs
/// The data comes from ConnectScript.cs
/// </summary>
public class CubeRotation : MonoBehaviour {

	public Material[] material; 
	int i_prevIndex = -1;
	int i_prevPolarity = -1;

	/// <summary>
	/// Initialized the object.
	/// Method is called once from the StartScript.cs when creating the new object
	/// </summary>
	/// <param name="tagQuuppa">Quuppa tag quuppa.</param>
	/// <param name="type">The type of building, Official by default</param>
	public void Init (string tagQuuppa, BuildingType type = BuildingType.Official) 
	{
		string tempType = type.ToString ();				// Convert type to string
		string url = "Textures/" + tempType;			// Append type to url
		material = Resources.LoadAll<Material>(url);	// Get corresponding materials from Resources folder
	}

	/// <summary>
	/// Process the cube rotation and apply corresponding texture
	/// The method is called from the ConnectScript.cs where the Quuppa json file is requested
	/// </summary>
	/// <param name="acceleration">The Vector3 containing the 3 acceleration values from the tag</param>
	public void ProcessRotation (Vector3 acceleration)
	{
		int axis = 0;									// Define which axis is receiving acceleration
		int polarity = 0;								// polarity indicates whether the axis is up or down

		for(; axis < 3 ; axis++)						// For loop iterates through the 
		{
			if(Mathf.Abs(acceleration[axis]) > 60)break;// If the value is greater than 60, we found the axis getting acceleration
														// Since the axis may receive positive or negative accelration we use the absolut value
		}
		if(axis == 3) return;							// if we did not find any axis with value matching our request we quit the method
		 												// this means the cube is not totally resting flat, to avoid flickering of texture we return
														// The latest texture should remain on
		polarity = (axis >> 31 != 0) ? 1: -1; 				// We found an axis being greater than 60 we need to know if the value is positive or negative
														// we check the 32nd bit to be 1 or 0. 

		if(polarity != i_prevPolarity || axis != i_prevIndex)	// if the polarity or the axis have changed from previous, we swap the texture
		{
			SwapTexture(axis, polarity);
		}
	}

	/// <summary>
	/// Swaps the texture.
	/// Method is called when axis or polarity have changed
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="polarity">Polarity.</param>
	private void SwapTexture(int axis, int polarity)
	{
		// The switch case uses the value of the axis, then once inside the statement, we check if the polarity is positive or negative
		// This will define what texture is to be applied
		int textureIndex = -1;
		switch(axis)
		{
		case 0:
			textureIndex = polarity > 0 ? 0 : 1; 
			break;
		case 1:
			textureIndex = polarity > 0 ? 2 : 3; 
			break;
		case 2:
			textureIndex = polarity > 0 ? 4 : 5;
			break;
		}
		renderer.material = material[textureIndex];
		// Record values for next round
		i_prevIndex = axis;
		i_prevPolarity = polarity;
	}
}
