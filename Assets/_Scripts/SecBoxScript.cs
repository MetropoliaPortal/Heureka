using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecBoxScript : MonoBehaviour 
{
	public Transform boxTransform;
	private Transform m_transform;
	private GameObject [] m_boxes;
	private BuildingStorage buildStorage;
	private Vector3 m_prevPosition;

	void Start () 
	{
		m_boxes = GameObject.FindGameObjectsWithTag("Box");

		m_transform = GetComponent<Transform>();
		Vector3 pos = boxTransform.position;
		pos.z -= 50f;
		m_transform.position = pos;
		buildStorage = GameObject.Find ("Manager").GetComponent<BuildingStorage>();
		SetModel ();
	}

	void Update () 
	{
		Vector3 pos = boxTransform.position;
		pos.z -= 50f;
		m_transform.position = pos;
	}
	public void SetModel()
	{
		GameObject obj;
		BuildingType bt = boxTransform.GetComponent<TagScript>().GetTag();
		if(CheckNearItem() == true)
		{
			switch(bt)
			{
				case BuildingType.House:
					obj = (GameObject)Instantiate(buildStorage.GetSmallBuilding());
					obj.transform.position = m_transform.position;
					obj.transform.parent = m_transform;
					break;
				case BuildingType.Environment:
					obj = (GameObject)Instantiate(buildStorage.GetSmallEnvironment());
					obj.transform.position = m_transform.position;
					obj.transform.parent = m_transform;	
					break;
			}
		}
	}
	private bool CheckNearItem()
	{
		float range = 1.1f * 1.1f;
		List<GameObject>objList = new List<GameObject>();
		for (int i = 0; i <m_boxes.Length; i++)
		{
			if((m_transform.position - m_boxes[i].transform.position).sqrMagnitude < range)
			{
				if(m_boxes[i].transform == m_transform)
				{
					continue;
				}
				objList.Add (m_boxes[i]);
			}
		}
		print (objList.Count);
		if(objList.Count == 0)return false;
		Vector3 newPos = new Vector3();
		if(objList.Count == 1)
		{
			// Check if near by object is same tag
			if(objList[0].GetComponent<SecBoxScript>().GetComponent<TagScript>().GetTag() 
			   != boxTransform.GetComponent<TagScript>().GetTag())
			{
				return false;
			}

			// Check if it is 
			if(objList[0].transform.position.x == m_transform.position.x)
			{
				newPos.x = m_transform.position.x;
			}
			else
			{
				newPos.x = (m_transform.position.x - objList[0].transform.position.x) / 2f;
			}
			if(objList[0].transform.position.z == m_transform.position.z)
			{
				newPos.z = m_transform.position.z;
			}
			else
			{
				newPos.z = (m_transform.position.z - objList[0].transform.position.z) / 2f;
			}
			newPos.y = 0.5f;
		}
		/*GameObject obj = new GameObject("BigBuilding");
		obj.transform.position = newPos;
		obj.AddComponent<ExtraBuildingScript>();
		m_transform.parent = obj.transform;

		m_transform.renderer.enabled = false;
		objList[0].transform.parent = obj.transform;
		objList[0].renderer.enabled = false;*/
		return true;
	}
}

