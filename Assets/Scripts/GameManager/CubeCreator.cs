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
	public Transform CubeParent;

	void Start () 
	{
		tagManager = GetComponent<TagManager> ();
	}

	public QuuppaData CreateCube(string quuppaId)
	{
		GameObject cube = (GameObject)MonoBehaviour.Instantiate(CubePrefab);
		QuuppaData quuppaData = cube.GetComponent<QuuppaData> ();
		quuppaData.TagData = tagManager.TagDataDictionary[quuppaId];

		cube.GetComponent<CubeRotation> ().Initialize ();
		cube.GetComponent<CubeTexture>().Initialize() ;

		cube.transform.parent = CubeParent;
	
		return quuppaData;
	}
}
