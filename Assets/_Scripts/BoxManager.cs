using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxManager : MonoBehaviour 
{
    public BuildingStorage buildStorage;
   
    private List<GameObject> list = new List<GameObject>();
    private Dictionary<GameObject, GameObject> dict = new Dictionary<GameObject, GameObject>();
    
    
	void Start () 
    {
        GameObject[] m_boxes = GameObject.FindGameObjectsWithTag("Box");

        for (int i = 0; i < m_boxes.Length; i++)
        {
            BuildingType bt = m_boxes[i].GetComponent<TagScript>().GetTag();
            GameObject go_obj = null;
            Vector3 pos;
            switch (bt)
            {      
                case BuildingType.House:
                    go_obj = (GameObject)Instantiate(buildStorage.GetSmallBuilding());
                    pos = m_boxes[i].transform.position;
                    pos.z -= 50;
                    go_obj.transform.position = pos;
                    break;
                case BuildingType.Environment:
                    go_obj = (GameObject)Instantiate(buildStorage.GetSmallEnvironment());
                    pos = m_boxes[i].transform.position;
                    pos.z -= 50;
                    go_obj.transform.position = pos;
                    break;
            }
            dict.Add(m_boxes[i], go_obj);
        }
	}

    public void SetPosition(GameObject obj)
    {
        GameObject o = dict[obj];
        Vector3 pos = obj.transform.position;
        pos.z -= 50f;
        pos.y = 1;
        o.transform.position = pos;
        CheckForNearBy(obj);
    }
    private void CheckForNearBy(GameObject obj) 
    {
        GameObject[] gos = new GameObject[dict.Count];
        dict.Keys.CopyTo(gos, 0);
        if (obj.activeSelf == true)
        {
            List<GameObject> listObj = new List<GameObject>();
            float range = 1.3f;
            for (int i = 0; i < gos.Length; i++)
            {
                float distance = Vector3.Distance(obj.transform.position, gos[i].transform.position);
                if (distance < range)
                {
                    if (obj == gos[i]) continue;
                    if(obj.GetComponent<TagScript>().GetTag() == gos[i].GetComponent<TagScript>().GetTag())
                        listObj.Add(gos[i]);
                }
            }

            if (listObj.Count == 0)
            {
                // if the second box is inactive, we ar emoving the box while it is a large building
                // We need to find the big box, destroy it, reactivate the small houses, 
                // remove the settings from this object and its couple object 
                if (dict[obj].activeSelf == false)
                {
                    // Get the couple object
                    GameObject o = obj.GetComponent<SecBoxScript>().GetOther();
                    // Destroy big model
                    GameObject large = obj.GetComponent<SecBoxScript>().GetLarge();
                    Destroy(large);

                    //Reactivate both objects
                    dict[obj].SetActive(true);
                    dict[o].SetActive(true);
                    // Reset all variables
                    obj.GetComponent<SecBoxScript>().SetOther(null);
                    obj.GetComponent<SecBoxScript>().SetLarge(null);
                    o.GetComponent<SecBoxScript>().SetOther(null);
                    o.GetComponent<SecBoxScript>().SetLarge(null);
                }
                return;
            }
            if (listObj.Count == 1)
            {
                // The object is already turned to big so we quit
                if (obj.GetComponent<SecBoxScript>().GetLarge() != null) return;
                
                
                // Get the vector between the two objects
                Vector3 vec = listObj[0].transform.position - obj.transform.position;
                // Get the point between the two objects and move it to the other setup
                Vector3 pos = obj.transform.position + 0.5f * vec;
                pos.z -= 50f;
                pos.y = 1f;
                // Deactivate both objects
                
                //SecBoxScript script = listObj[0].GetComponent<SecBoxScript>();
                //Destroy(script.GetObj());
                //script.SetObj(null);

                // Create a new large object
                GameObject o = (GameObject)Instantiate(buildStorage.GetLargeBulding2B());
                // Assign new position
                o.transform.position = pos;
                // Couple both object together by script
                listObj[0].GetComponent<SecBoxScript>().SetLarge(o);
                listObj[0].GetComponent<SecBoxScript>().SetOther(obj);
                obj.GetComponent<SecBoxScript>().SetLarge(o);
                obj.GetComponent<SecBoxScript>().SetOther(listObj[0]);
                // deactivate both objects.
                dict[obj].SetActive(false);
                dict[listObj[0]].SetActive(false);

                return;
            }
        }
        else 
        {
        
        }
    }
    public void ChangeModel(GameObject key, GameObject value)
    {
        GameObject o = dict[key];
        Destroy(o);
        dict[key] = value;
    }
}
