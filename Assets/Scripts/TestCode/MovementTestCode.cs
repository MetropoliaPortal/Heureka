using UnityEngine;
using System.Collections;

public class MovementTestCode : MonoBehaviour {

	CubePosition cb;
	public Collider[] colliders;
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast (ray, out hit))
			{
				if(hit.collider.name == "Obstacle")
				{
					cb = hit.collider.gameObject.GetComponent<CubePosition>();
					SetColliders(false);
				}
			}
		}
		if(Input.GetMouseButton(0) && cb!= null)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast (ray, out hit);
			cb.MoveCubeInDebug(hit.point);
		}
		if (Input.GetMouseButtonUp (0))
		{
			if(cb != null)
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Physics.Raycast (ray, out hit);
				SetColliders(true);
				cb.MoveCubeInDebug(hit.point);
				cb = null;
			}
		}
	}
	void SetColliders(bool value)
	{
		for(int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = value;
		}
	}
}
