using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class CubeStacking : MonoBehaviour 
{
	public void CompareStackedCubes (Transform t) 
	{
		List<Transform> ts = new List<Transform>();
	
		foreach(GameObject o in GridManager.obstacleList) 
		{
			Vector3 otherCubePos = o.transform.position;
			if( otherCubePos.x == t.position.x && otherCubePos.z == t.position.z )
			{
				ts.Add( o.transform );
			}
		}

		//Sort array from smallest to largest
		Transform[] transformArray = ts.ToArray();
		System.Array.Sort(transformArray, delegate(Transform first, Transform second){
			return (first.GetComponent<CubePosition>().PositionYQuuppa.CompareTo(second.GetComponent<CubePosition>().PositionYQuuppa));
		});

		//Debug.Log ("tra array len: " + transformArray.Length);

		for (int i = 0; i < transformArray.Length; i++)
		{
			Vector3 pos = transformArray[i].position;

			float newY;
			if(i == 0)
				newY = (i + 1);
			else
				newY = (i + i + 1);

			if( t == transformArray[i])
				transformArray[i].position = new Vector3(pos.x, newY , pos.z);
		}
	}
}


