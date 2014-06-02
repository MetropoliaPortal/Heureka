using UnityEngine;
using System.Collections;

public class CubeHider : MonoBehaviour 
{
	private CubePosition cubePosition;
	private bool isHidden = false;

	void Start () 
	{
		cubePosition = GetComponent<CubePosition> ();
		cubePosition.OnOutsideGrid += HandleOnOutsideGrid;
	}

	void HandleOnOutsideGrid (bool isOutside)
	{
		//if( isOutside == isHidden )
			//return;

		if( isOutside )
		{
			renderer.enabled = false;
		}
		else
		{
			renderer.enabled = true;
		}
	}
}
