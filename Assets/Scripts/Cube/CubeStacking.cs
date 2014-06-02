using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class CubeStacking : MonoBehaviour 
{
	public bool IsComparingOnUpdate;
	public float CompareFrequency = 0;
	public int CompareAmount = 0;

	private QuuppaData quuppaData;
	private float timer = 0;
	public float quuppaHeight{ get; private set;}
	private Queue<float> previousValues = new Queue<float>();

	void Start()
	{
		quuppaData = GetComponent<QuuppaData>();
		quuppaData.heightQuuppaChanged += SolveHeight;
	}
	
	void Update()
	{
		if( IsComparingOnUpdate )
		{
			timer += Time.deltaTime;
			if( timer >= CompareFrequency)
			{
				CompareStackedCubes ();
				timer = 0;
			}
		}
	}

	private void SolveHeight(float height)
	{
		quuppaHeight = Helpers.SolveAverage ( previousValues, height, CompareAmount );
	}

	/// <summary>
	/// Get cube transforms which share the same 2d-position. Add them in to array and sort it according to
	/// actual height coordinate from quuppa.
	/// </summary>
	public void CompareStackedCubes () 
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
		/*
		System.Array.Sort(transformArray, delegate(Transform first, Transform second){
			return (first.GetComponent<QuuppaData>().HeightQuuppa.CompareTo(second.GetComponent<QuuppaData>().HeightQuuppa));
		});
		*/
		System.Array.Sort(transformArray, delegate(Transform first, Transform second){
			return (first.GetComponent<CubeStacking>().quuppaHeight.CompareTo(second.GetComponent<CubeStacking>().quuppaHeight));
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


