using UnityEngine;
using System.Collections;

public class TagScript : MonoBehaviour {

	public BuildingType buildingType;
    private bool b_tag;
    private Rect[] r_rect = new Rect[3];
    public SecBoxScript script;
    private BoxManager manager;
    void Start() 
    {
        manager = GameObject.Find("Manager").GetComponent<BoxManager>();
        float screenHalfWidth = Screen.width / 2f;
        float screenHalfHeight = Screen.height / 2f;
        float size = 100f;
        r_rect[0] = new Rect(screenHalfWidth - size, screenHalfHeight - size / 2f, size, size);
        r_rect[1] = new Rect(screenHalfWidth , screenHalfHeight - size / 2f, size, size);
        r_rect[2] = new Rect(screenHalfWidth - size / 4, screenHalfHeight + size / 2f, size / 2f, size / 2f);
    }
	public BuildingType GetTag()
	{
		return buildingType;
	}
	public void SetTag(BuildingType bt)
	{
		manager.CheckForCoupleObject (gameObject);
	 	buildingType = bt;
        GameObject o = script.SetModel();
        manager.ChangeModel(gameObject, o);
	}
    public void SetTagGui(bool value) 
    {
        b_tag = true;
    }
    void OnGUI()
    {
        if (b_tag)
        { 
            if(GUI.Button(r_rect[0],"House"))
            {
                SetTag(BuildingType.House);
            }
            if(GUI.Button(r_rect[1],"Environment"))
            {
                SetTag(BuildingType.Environment);
            }
            if (GUI.Button(r_rect[2], "Ok"))
            {
                b_tag = false;
            }
        }
    }
}

public enum BuildingType
{ 
	House, Environment
}
