using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class CubeStacking : MonoBehaviour 
{
	public int CompareFrequency = 5;
	private int updateCount = 0;

	/// <summary>
	/// Limit comparing to happen only on every n:th frame 
	/// </summary>
	void LateUpdate()
	{
		updateCount++;
		if( updateCount == CompareFrequency)
		{
			CompareStackedCubes ();
			updateCount = 0;
		}
	}

	/// <summary>
	/// Get cube transforms which share the same 2d-position. Add them in to array and sort it according to
	/// actual height coordinate from quuppa.
	/// </summary>
	private void CompareStackedCubes () 
	{
		List<Transform> ts = new List<Transform>();
	
		foreach(GameObject o in GridManager.obstacleList) 
		{
			Vector3 otherCubePos = o.transform.position;
			if( otherCubePos.x == transform.position.x && otherCubePos.z == transform.position.z )
			{
				ts.Add( o.transform );
			}
		}

		Transform[] transformArray = ts.ToArray();
		System.Array.Sort(transformArray, delegate(Transform first, Transform second){
			return (first.GetComponent<TagInfo>().HeightQuuppa.CompareTo(second.GetComponent<TagInfo>().HeightQuuppa));
		});

		for (int i = 0; i < transformArray.Length; i++)
		{
			Vector3 pos = transformArray[i].position;

			float newY;
			if(i == 0)
				newY = (i + 1);
			else
				newY = (i + i + 1);

			if( transform == transformArray[i])
			{
				transformArray[i].position = new Vector3(pos.x, newY , pos.z);
			}
		}
	}
}


