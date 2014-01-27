using UnityEngine;
using System.Collections;

public class InputControl : MonoBehaviour
{
    #region MEMBERS
    private bool b_cubeClicked = false;
	private Transform m_selectedCube;
    #endregion
    #region UNITY_METHODS
    // Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
    {
		if (Input.GetMouseButtonDown(0) && !b_cubeClicked)
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.tag == "Cube")
                {
					GameObject obj = hit.collider.gameObject;
					m_selectedCube = obj.transform.parent;
                    Vector3 pos = m_selectedCube.transform.position;
                    pos.y += 2f;
                    m_selectedCube.transform.position = pos;
                    b_cubeClicked = true;

					obj.rigidbody.isKinematic = true;
					obj.layer = LayerMask.NameToLayer("Ignore Raycast");
					b_cubeClicked = true;
                }
            }
        }
		else if(Input.GetMouseButtonDown(0) && b_cubeClicked)
		{
			b_cubeClicked = false;
			m_selectedCube.gameObject.layer = LayerMask.NameToLayer("Default");
			m_selectedCube.transform.Find ("Cube").gameObject.layer =  LayerMask.NameToLayer("Default");
			m_selectedCube.transform.Find ("Cube").rigidbody.isKinematic = false;
			m_selectedCube.GetComponent<ConnectionScript>().SendMessageToSecCube();
		}

        if (b_cubeClicked == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
				if(hit.collider.tag == "Floor" || hit.collider.tag == "Cube")
				{
					Vector3 vec = hit.point;
					vec.y +=2f;
					m_selectedCube.position = vec;
				}
            }
        }
    }
    #endregion
}
