using UnityEngine;
using System.Collections;

public class CubeEffect : MonoBehaviour 
{
	public GameObject particleEffect;
	private ParticleSystem particleSystem;
	
	void Start () 
	{
		GameObject go = (GameObject)Instantiate( particleEffect, transform.position, Quaternion.identity );
		particleSystem = go.GetComponent<ParticleSystem>();
		go.transform.parent = transform;

		CubeRotation cr = GetComponent<CubeRotation>();
		cr.rotationChanged += EmitParticles;
	}

	private void EmitParticles()
	{
		particleSystem.Emit(120);
	}
}
