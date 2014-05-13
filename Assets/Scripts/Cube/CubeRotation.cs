using UnityEngine;
using System.Collections;

/// <summary>
/// Cube rotation script is attached to each cube.
/// The rotation defines the texture to ba applied
/// The script is added from the StartScript.cs
/// The data comes from ConnectScript.cs
/// </summary>
using System.Collections.Generic;


public class CubeRotation : MonoBehaviour 
{
	public GameObject particleEffect;
	public int category;
	private ParticleSystem particleSystem;

	public Material[] materials; 
	int i_prevIndex = -1;
	int i_prevPolarity = -1;
	Vector3 v_prevRotation;
	private Queue<int> v_prevRotations = new Queue<int> ();

	/// <summary>
	/// Initialized the object.
	/// Method is called once from the StartScript.cs when creating the new object
	/// </summary>
	/// <param name="tagQuuppa">Quuppa tag quuppa.</param>
	/// <param name="type">The type of building, Official by default</param>
	public void Initialize (string tagQuuppa, BuildingType type = BuildingType.Leisure) 
	{
		string tempType = type.ToString ();				// Convert type to string
		string url = "Materials/" + tempType;			// Append type to url
		materials = Resources.LoadAll<Material>(url);	// Get corresponding materials from Resources folder

		//Instantiate particle effect
		GameObject go = (GameObject)Instantiate( particleEffect, transform.position, Quaternion.identity );
		particleSystem = go.GetComponent<ParticleSystem>();
		go.transform.parent = transform;
	}

	/// <summary>
	/// Process the cube rotation and apply corresponding texture
	/// The method is called from the ConnectScript.cs where the Quuppa json file is requested
	/// </summary>
	/// <param name="acceleration">The Vector3 containing the 3 acceleration values from the tag</param>
	public void ProcessRotation (Vector3 acceleration)
	{
		// Define which axis is receiving acceleration
		int axis = 0;	
		// polarity indicates whether the axis is up or down
		int polarity = 0;								

		for(; axis < 3 ; axis++)						
		{
			if(Mathf.Abs(acceleration[axis]) > 30)break;
														
		}
		if(axis == 3) return;						

		polarity = (acceleration[axis] >= 0) ? 1: -1;

		if(ComparePreviousRotations(axis, polarity))
			SwapTexture(axis, polarity);

	}

	private bool ComparePreviousRotations(int axis, int polarity)
	{
		if (v_prevRotations.Count < 3) 
		{
			v_prevRotations.Enqueue (axis * polarity);
		} 
		else 
		{
			v_prevRotations.Dequeue();
		}

		foreach (int i in v_prevRotations) 
		{
			if( i != axis * polarity)
				return false;
		}

		return true;
	}

	/// <summary>
	/// Swaps the texture.
	/// Method is called when axis or polarity have changed
	/// </summary>
	private void SwapTexture(int axis, int polarity)
	{
		// The switch case uses the value of the axis, then once inside the statement, we check if the polarity is positive or negative
		// This will define what texture is to be applied
		int materialIdx = -1;
		switch(axis)
		{
		case 0:
			materialIdx = polarity > 0 ? 0 : 1; 
			break;
		case 1:
			materialIdx = polarity > 0 ? 2 : 3; 
			break;
		case 2:
			materialIdx = polarity > 0 ? 4 : 5;
			break;
		}
	
		string matName = renderer.material.name.Replace(" (Instance)","");

		if( matName != materials[materialIdx].name )
		{
			renderer.material = materials[materialIdx];
			particleSystem.Emit(120);
		}
	}

	#region Debug

	void Start()
	{
		if(category == 0)
			Initialize("0202020", BuildingType.Leisure);
		else if (category == 1)
			Initialize("0202020", BuildingType.Official);
		else if (category == 2)
			Initialize("0202020", BuildingType.Residential);
		else if (category == 3)
			Initialize("0202020", BuildingType.Shop);

		renderer.material = materials[0];
	}

	#endregion

	#region NOT IN USE

	List<Vector3>list = new List<Vector3>();
	
	Vector3 ? CheckForValue (Vector3 value)
	{
		if (list.Count == 0)
		{
			list.Add (value);
			return v_prevRotation;
		}
		for(int i = 0; i < list.Count; i++)
		{
			if(value != list[i])
			{
				list.Clear();
				list.Add (value);
				return v_prevRotation;
			}
		}
		list.Add (value);
		if(list.Count == 3)
		{
			list.Clear();
			v_prevRotation = value;
			return value;
		}
		return v_prevRotation;
	}

	#endregion
}