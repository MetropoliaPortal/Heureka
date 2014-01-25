using UnityEngine;
using System.Collections;

public class InputControl : MonoBehaviour
{
    #region MEMBERS
    private bool b_cubeClicked = false;
    #endregion
    #region UNITY_METHODS
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(0))
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Cube")
                {
                    hit.rigidbody.isKinematic = true;
                    Vector3 pos = hit.transform.position;
                    pos.y += 2f;
                    hit.transform.position = pos;
                    b_cubeClicked = true;
                }
            }
        }
        if (b_cubeClicked == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

            }
        }
    }
    #endregion
}
