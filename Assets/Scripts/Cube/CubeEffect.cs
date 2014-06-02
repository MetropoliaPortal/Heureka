using UnityEngine;
using System.Collections;

public class CubeEffect : MonoBehaviour 
{
	public GameObject particleEffect;
	private ParticleSystem particleSystem;
	private bool isDisabled = false;
	
	void Start () 
	{
		GameObject go = (GameObject)Instantiate( particleEffect, transform.position, Quaternion.identity );
		particleSystem = go.GetComponent<ParticleSystem>();
		go.transform.parent = transform;

		CubeRotation cr = GetComponent<CubeRotation>();
		cr.rotationChanged += EmitParticles;

		CubePosition cp = GetComponent<CubePosition> ();
		cp.OnOutsideGrid += HandleOnOutsideGrid;
	}

	void HandleOnOutsideGrid (bool par)
	{
		isDisabled = par;
	}

	private void EmitParticles()
	{
		if(!isDisabled)
			particleSystem.Emit(120);
	}
}
