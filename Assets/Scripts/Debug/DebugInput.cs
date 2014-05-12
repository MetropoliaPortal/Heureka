﻿using UnityEngine;
using System.Collections;

public class DebugInput : MonoBehaviour 
{
	public bool IsDebugMode;
	CubePosition cubePosition;
	CubeRotation cubeRotation;

	public Collider[] colliders;

	void Awake()
	{
		//SetLights();
	}

	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast (ray, out hit))
			{
				if(hit.collider.tag == "Obstacle")
				{
					cubePosition = hit.collider.gameObject.GetComponent<CubePosition>();
					SetColliders(false);
				}
			}
		}
		if(Input.GetMouseButton(0) && cubePosition!= null)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast (ray, out hit);
			cubePosition.MoveCubeInDebug(hit.point);
		}
		if (Input.GetMouseButtonUp (0))
		{
			if(cubePosition != null)
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Physics.Raycast (ray, out hit);
				SetColliders(true);
				cubePosition.MoveCubeInDebug(hit.point);
				cubePosition = null;
			}
		}

		CheckIfRotate();
	}

	void SetColliders(bool value)
	{
		for(int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = value;
		}
	}
	private void SetLights()
	{
		Light [] lights = new Light[5];
		Transform [] trs = new Transform[5]; 
		GameObject obj = new GameObject("Light");
		obj.transform.position = new Vector3(0,0,0);
		for (int i = 0 ; i < 5 ; i++)
		{
			GameObject go = new GameObject("The Light");
			lights[i] = go.AddComponent<Light>();
			lights[i].intensity = 2f;
			trs[i] = go.transform;
		}
		trs[0].position = new Vector3(0,5,0);
		trs[1].position = new Vector3(0,5,12);
		trs[2].position = new Vector3(12,5,0);
		trs[3].position = new Vector3(12,5,12);
		trs[4].position = new Vector3(6,5,6);
		for(int i = 0; i < 5 ; i++){trs[i].parent = obj.transform;}
	}

	/// <summary>
	/// Textures of a cube can be changed with keys Q,W,E,R,T,Y
	/// </summary>
	private void CheckIfRotate()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast (ray, out hit))
		{
			if(hit.collider.tag == "Obstacle")
			{
				cubeRotation = hit.collider.gameObject.GetComponent<CubeRotation>();

				if(Input.GetKey(KeyCode.Q) ){
					cubeRotation.ProcessRotation(new Vector3(0,0,-65f));}
				else if(Input.GetKey(KeyCode.W)){
					cubeRotation.ProcessRotation(new Vector3(0,0,65f));}

				else if(Input.GetKey(KeyCode.E))
					cubeRotation.ProcessRotation(new Vector3(-65f,0,0));
				else if(Input.GetKey(KeyCode.R))
					cubeRotation.ProcessRotation(new Vector3(65f,0,0));

				else if(Input.GetKey(KeyCode.T))
					cubeRotation.ProcessRotation(new Vector3(0,-65f,0));
				else if(Input.GetKey(KeyCode.Y))
					cubeRotation.ProcessRotation(new Vector3(0,65f,0));
			}
		}

	}
}