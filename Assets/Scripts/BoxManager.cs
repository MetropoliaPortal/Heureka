using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Box manager.
/// Controls all movement and alteration of the objects on screen
/// </summary>
public class BoxManager : MonoBehaviour 
{
    public BuildingStorage buildStorage;
   
   // private List<GameObject> list = new List<GameObject>();
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

	/// <summary>
	/// Sets the position of the building on screen.
	/// It is called when the cube is physically moved in space
	/// </summary>
	/// <param name="obj">Object.</param>
    public void SetPosition(GameObject obj)
    {
        GameObject o = dict[obj];
        Vector3 pos = obj.transform.position;
        pos.z -= 50f;
        pos.y = 0.5f;
        o.transform.position = pos;
        //CheckForNearBy(obj);
    }

	/// <summary>
	/// Checks for near by objects.
	/// When the cube is put to rest, the method is called.
	/// If objects are found in the surrounding, then a new bigger building is created.
	/// </summary>
	/// <param name="obj">Object.</param>
    public void CheckForNearBy(GameObject obj) 
    {
        GameObject[] gos = new GameObject[dict.Count];
        dict.Keys.CopyTo(gos, 0);
        if (obj.activeSelf == true)
        {
            List<GameObject> listObj = new List<GameObject>();
            float range = 1.7f;
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
                // if the second box is inactive, we are moving the box while it is a large building
                // We need to find the big box, destroy it, reactivate the small houses, 
                // remove the settings from this object and its couple object 
				CheckForCoupleObject(obj);
                
                return;
            }
            if (listObj.Count == 1)
            {
				SecBoxScript script = obj.GetComponent<SecBoxScript>();
                // The object is already turned to big so we quit
                if (script.GetLarge() != null) return;
                
                
                // Get the vector between the two objects
                Vector3 vec = listObj[0].transform.position - obj.transform.position;
                // Get the point between the two objects and move it to the other setup
                Vector3 pos = obj.transform.position + 0.5f * vec;
                pos.z -= 50f;
                pos.y = 0.5f;
                
				GameObject o;
                // Create a new large object
				// Object is placed above
				if( obj.transform.position.y >0.55f)
				{
					o = (GameObject)Instantiate(buildStorage.GetTallBuilding2B());
					pos.y = 1;
				}
				else // Object is aside
				{
					o = (GameObject)Instantiate(buildStorage.GetLargeBulding2B());
					// check for rotation
					float dot = Mathf.Abs (Vector3.Dot (Vector3.right, vec.normalized));
					if(dot > 0.7f)
					{
						o.transform.Rotate(Vector3.up, 90f);
					}
				}
               
                // Assign new position
                o.transform.position = pos;
                // Couple both object together by script
				script.SetLarge(o);
				script.SetOther(listObj[0]);
				script = listObj[0].GetComponent<SecBoxScript>();
                script.SetLarge(o);
                script.SetOther(obj);

                
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

	public void CheckForCoupleObject (GameObject obj)
	{
		if (dict[obj].activeSelf == false)
		{
			SecBoxScript script = obj.GetComponent<SecBoxScript>();
			// Destroy big model
			GameObject o = script.GetLarge();
			Destroy(o);
			// Get the couple object
			o = script.GetOther();
			
			//Reactivate both objects
			dict[obj].SetActive(true);
			dict[o].SetActive(true);
			// Reset all variables
			script.SetOther(null);
			script.SetLarge(null);

			script = o.GetComponent<SecBoxScript>();
			script.SetOther(null);
			script.SetLarge(null);
		}
	}
}
