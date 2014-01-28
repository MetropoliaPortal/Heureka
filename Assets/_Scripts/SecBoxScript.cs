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

    //private GameObject go_obj;
    private Transform m_transform;
    private BuildingStorage buildStorage;
    void Start() 
    {
        buildStorage = GameObject.Find("Manager").GetComponent<BuildingStorage>();
        m_transform = GetComponent<Transform>();
    }
    /*
	
	private GameObject [] m_boxes;
	
	private Vector3 m_prevPosition;
    
	private GameObject go_large;

	private List<SecBoxScript> scripts = new List<SecBoxScript>();
    

	void Start () 
	{
		m_boxes = GameObject.FindGameObjectsWithTag("Box");

		m_transform = GetComponent<Transform>();
		Vector3 pos = boxTransform.position;
		pos.z -= 50f;
		m_transform.position = pos;

        go_obj = (GameObject)Instantiate(buildStorage.GetSmallBuilding());
        go_obj.transform.position = m_transform.position;
        go_obj.transform.parent = m_transform;
	}
    */

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
    /*
	public void CheckNearItem()
	{
        List<GameObject> listObj = new List<GameObject>();
        float range = 1.3f;
        for (int i = 0; i < m_boxes.Length; i++)
        {
            float distance = Vector3.Distance(m_transform.position , m_boxes[i].transform.position);
            if (distance < range)
            {
                if (m_transform == m_boxes[i].transform) continue;
                listObj.Add(m_boxes[i]);
            }
        }
        if (listObj.Count == 0) 
        {
			if(go_obj == null)
			{

			}
            return;
        }
        if (listObj.Count == 1)
        {
            Vector3 vec = listObj[0].transform.position - m_transform.position;
            Vector3 pos = m_transform.position + 0.5f * vec;
			Destroy(go_obj);
			go_obj = null;
			SecBoxScript script = listObj[0].GetComponent<SecBoxScript>();
			Destroy (script.GetObj());
			script.SetObj(null);

			GameObject o = (GameObject)Instantiate(buildStorage.GetLargeBulding2B());
			o.transform.position = pos;
			go_large = o;
			script.SetLargeObj(o);

            return;
        }
	}

    public void SetPosition(Vector3 position)
    {
        Vector3 pos = position;
        pos.y = 1.0f;
        pos.z -= 50f;
        m_transform.position = pos;
    }
	public GameObject GetObj()
	{
		return go_obj;		
	}
	public void SetObj(GameObject obj)
	{
	 	go_obj = obj;		
	}
	public GameObject GetLargeObj()
	{
		return go_large;	
	}
	public void SetLargeObj(GameObject obj)
	{
		go_large = obj;		
	}*/
}

