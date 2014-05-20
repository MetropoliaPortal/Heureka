using UnityEngine;
using System.Collections;

public class DebugInput : MonoBehaviour 
{
	private CubePosition cubePosition;
	private CubeRotation cubeRotation;
	private TagInfo tagInfo;
	
	void Update () 
	{
		SolveMovement();
		SolveRotation();
	}

	private void SolveMovement()
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
					tagInfo = hit.collider.gameObject.GetComponent<TagInfo>();
				}
			}
		}
		if(Input.GetMouseButton(0) && cubePosition!= null)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast (ray, out hit);
			cubePosition.MoveInDebug(hit.point);
			//tagInfo.Position = hit.point;
		}
		if (Input.GetMouseButtonUp (0))
		{
			if(cubePosition != null)
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Physics.Raycast (ray, out hit);
				cubePosition.MoveInDebug(hit.point);
				//tagInfo.Position = hit.point;
				cubePosition = null;
			}
		}
	}

	/// <summary>
	/// Textures of a cube can be changed with keys Q,W,E,R,T,Y
	/// </summary>
	private void SolveRotation()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast (ray, out hit))
		{
			if(hit.collider.tag == "Obstacle")
			{
				cubeRotation = hit.collider.gameObject.GetComponent<CubeRotation>();
				tagInfo = hit.collider.gameObject.GetComponent<TagInfo>();

				Vector3 acceleration = Vector3.zero;

				if(Input.GetKey(KeyCode.Q) )
				{
					acceleration = new Vector3(0,0,-65f);
				}
				else if(Input.GetKey(KeyCode.W))
				{
					acceleration = new Vector3(0,0,65f);
				}

				else if(Input.GetKey(KeyCode.E))
				{
					acceleration = new Vector3(-65f,0,0);
				}
				else if(Input.GetKey(KeyCode.R))
				{
					acceleration = new Vector3(65f,0,0);
				}
				else if(Input.GetKey(KeyCode.T))
				{
					acceleration = new Vector3(0,-65f,0);
				}
				else if(Input.GetKey(KeyCode.Y))
				{
					acceleration = new Vector3(0,65f,0);
				}

				if( acceleration != Vector3.zero )
				{
					tagInfo.Acceleration = acceleration;
				}
			}
		}
	}
}
