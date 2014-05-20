using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System;

public class CubeCreator : MonoBehaviour 
{
	private TagManager tagManager;
	public GameObject CubePrefab;

	void Start () 
	{
		tagManager = GetComponent<TagManager> ();
	}

	public TagInfo CreateCube( string quuppaId, BuildingType type )
	{
		GameObject cube = (GameObject)MonoBehaviour.Instantiate(CubePrefab);
		CubeTexture textureScript = cube.GetComponent<CubeTexture>();
		QuuppaConnection connectScript = cube.GetComponent<QuuppaConnection>();
		connectScript.Initialize( quuppaId );
		//textureScript.Initialize( type );
	
		return cube.GetComponent<TagInfo>();
	}
}
