using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecBoxScript : MonoBehaviour 
{
    private GameObject m_large;
    private GameObject m_other;
    public GameObject GetLarge() 
    {
        return m_large;
    }
    public void SetLarge(GameObject obj) 
    {
        m_large = obj;
    }
    public GameObject GetOther()
    {
        return m_other;
    }
    public void SetOther(GameObject obj)
    {
        m_other = obj;
    }

    private BuildingStorage buildStorage;
    void Start() 
    {
        buildStorage = GameObject.Find("Manager").GetComponent<BuildingStorage>();
    }

	public GameObject SetModel()
	{
        GameObject go_obj = null;
		BuildingType bt = GetComponent<TagScript>().GetTag();
        Vector3 pos;
			switch(bt)
			{
				case BuildingType.House:
                    go_obj = (GameObject)Instantiate(buildStorage.GetSmallBuilding());
                    pos = transform.position;
		            pos.z -= 50f;
                    pos.y = 1f;
                    go_obj.transform.position = pos;
					break;
				case BuildingType.Environment:
                    go_obj = (GameObject)Instantiate(buildStorage.GetSmallEnvironment());
                    pos = transform.position;
		            pos.z -= 50f;
                    pos.y = 1f;
                    go_obj.transform.position = pos;	
					break;
			}
            return go_obj;
	}
}

