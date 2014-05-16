using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeRotation : MonoBehaviour 
{
	public int ComparePreviousAmount = 4;
	int i_prevIndex = -1;
	int i_prevPolarity = -1;
	private Queue<int> previousRotations = new Queue<int> ();

	public int currentIndex;

	public delegate void RotationChangedEvent();
	public event RotationChangedEvent rotationChanged = delegate{};


	/// <summary>
	/// Process the cube rotation and apply corresponding texture
	/// The method is called from the ConnectScript.cs where the Quuppa json file is requested
	/// // Define which axis is receiving acceleration
	/// // polarity indicates whether the axis is up or down
	/// </summary>
	/// <param name="acceleration">The Vector3 containing the 3 acceleration values from the tag</param>
	public void ProcessRotation (Vector3 acceleration)
	{
		int axis = 0;	
		int polarity = 0;								

		for(; axis < 3 ; axis++)						
		{
			if(Mathf.Abs(acceleration[axis]) > 35)break;
														
		}
		if(axis == 3) return;						

		polarity = (acceleration[axis] >= 0) ? 1: -1;

		if(ComparePreviousRotations(axis, polarity))
			SwapTexture(axis, polarity);

	}

	private bool ComparePreviousRotations(int axis, int polarity)
	{
		if (previousRotations.Count < ComparePreviousAmount) 
		{
			previousRotations.Enqueue (axis * polarity);
		} 
		else 
		{
			previousRotations.Dequeue();
		}
		foreach (int i in previousRotations) 
		{
			if( i != axis * polarity)
				return false;
		}
		return true;
	}

	// The switch case uses the value of the axis, then once inside the statement, we check if the polarity is positive or negative
	// This will define what texture is to be applied
	private void SwapTexture(int axis, int polarity)
	{
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

		if( materialIdx != currentIndex )
		{
			currentIndex = materialIdx;
			rotationChanged();
		}
	}
}