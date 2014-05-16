using UnityEngine;
using System.Collections;

public class DebugInput : MonoBehaviour 
{
	CubePosition cubePosition;
	CubeRotation cubeRotation;
	
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
				cubePosition.MoveCubeInDebug(hit.point);
				cubePosition = null;
			}
		}

		CheckIfRotate();
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
