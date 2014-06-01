using UnityEngine;
using System.Collections;

public class CubeCollision : MonoBehaviour 
{
	void OnTriggerEnter(Collider c)
	{
		if( c.tag == "Obstacle")
		{
			if( c.GetComponent<CubePosition>().IsMoving)
				c.renderer.enabled = false;
		}
	}

	void OnTriggerExit(Collider c)
	{
		if( c.tag == "Obstacle")
		{
			c.renderer.enabled = true;
		}
	}
}
