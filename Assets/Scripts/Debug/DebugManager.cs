using UnityEngine;
using System.Collections;

public class DebugManager : MonoBehaviour 
{
	public bool IsDebugMode;
	CubePosition cubePosition;
	CubeRotation cubeRotation;

	public Collider[] colliders;

	void Start()
	{
		QuuppaStart quuppaStart = GameObject.Find("GameManager").GetComponent<QuuppaStart>();

		if(IsDebugMode)
		{
			quuppaStart.GetFileBuilding();
		}
		else
		{
			quuppaStart.StartQuuppa();
		}
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

	/// <summary>
	/// Pressing cube with 2nd mousebutton and either x, y or z key will result in cube rotating
	/// left-ctrl can be used to make inverse rotation
	/// </summary>
	private void CheckIfRotate()
	{
		if(Input.GetMouseButtonDown(1))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast (ray, out hit))
			{
				if(hit.collider.tag == "Obstacle")
				{
					cubeRotation = hit.collider.gameObject.GetComponent<CubeRotation>();

					if(Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl)){
						cubeRotation.ProcessRotation(new Vector3(0,0,-65f));}
					else if(Input.GetKey(KeyCode.Z)){
						cubeRotation.ProcessRotation(new Vector3(0,0,65f));}


					else if(Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.LeftControl))
						cubeRotation.ProcessRotation(new Vector3(-65f,0,0));
					else if(Input.GetKey(KeyCode.X))
						cubeRotation.ProcessRotation(new Vector3(65f,0,0));

					else if(Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.LeftControl))
						cubeRotation.ProcessRotation(new Vector3(0,-65f,0));
					else if(Input.GetKey(KeyCode.Y))
						cubeRotation.ProcessRotation(new Vector3(0,65f,0));
				}
			}
		}
	}
}
