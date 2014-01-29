using UnityEngine;
using System.Collections;

public class InputControl : MonoBehaviour
{
    #region MEMBERS
    private bool b_cubeClicked = false;
	private Transform m_selectedCube;
    private BoxManager m_boxManager;
    #endregion
    #region UNITY_METHODS
    void Start() 
    {
        m_boxManager = GetComponent<BoxManager>();
    }
	// Update is called once per frame
	void Update () 
    {
        CheckInput();
        ChangeTag();
        if (b_cubeClicked == true)
        {
            PositionCube();
        }
    }
    #endregion
    #region METHODS
    private void CheckInput() 
    {
        if (Input.GetMouseButtonDown(0) && !b_cubeClicked)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Cube")
                {
                    // Get the cube
                    GameObject obj = hit.collider.gameObject;
                    // Get the parent object, obj is the main object
                    m_selectedCube = obj.transform.parent;
                    // Store parent position (same as cube but in world)
                    // Then lift it up and assign it back
                    Vector3 pos = m_selectedCube.transform.position;
                    pos.y += 2f;
                    m_selectedCube.transform.position = pos;

                    // Change layer mask of the cube to avoid raycast and activate green plane
                    m_selectedCube.transform.Find("Cube").gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                    m_selectedCube.transform.Find("Plane").gameObject.SetActive(true);

                    b_cubeClicked = true;
                }
            }
        }
        else if (Input.GetMouseButtonDown(0) && b_cubeClicked)
        {
            // Change layer mask of the cube to receive raycast and deactivate green plane
            m_selectedCube.transform.Find("Cube").gameObject.layer = LayerMask.NameToLayer("Default");
            m_selectedCube.transform.Find("Plane").gameObject.SetActive(false);

            // Get the raycast and set the position of the cube at hit point
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            Vector3 pos = hit.point;
            pos.y += 0.5f;
            m_selectedCube.transform.position = pos;
            b_cubeClicked = false;
        }
    }

    private void PositionCube() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Floor" || hit.collider.tag == "Cube")
            {
                Vector3 vec = hit.point;
                vec.y += 2f;
                m_selectedCube.position = vec;
                m_boxManager.SetPosition(m_selectedCube.gameObject);
            }
        }
    }

    private void ChangeTag() 
    {
        if (Input.GetMouseButtonDown(1) && !b_cubeClicked)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                hit.transform.parent.GetComponent<TagScript>().SetTagGui(true);
            }
        }
    }
    #endregion
}
