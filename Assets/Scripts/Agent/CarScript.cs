using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// //////////////////////////////////////////////
// This is all done without any test so it may not totally work but still
// I would guess the logic is there
/// ///////////////////////////////////////////////
public class CarScript : MonoBehaviour 
{
    private new Transform transform;
    private Transform cam;
    List<Node> currentPath;
    int index;
    public float speed;
    private float range;

    void Start() {
        transform = base.transform;
        cam = Camera.main.transform;
		range = 0.1f;
		speed = 1.0f;
		index = 0;

		//Checking for changed is done temporarily only here
		PathManager.CheckPathsChanged();
		currentPath = PathManager.GetNewPath();
		transform.position = currentPath[0].position;
    }

    void Update() 
    {
		// this is not this straight forward
        // Always face the camera
        //transform.LookAt(-cam.position);
        // Check for orientation
        // Using some cross product

        // Move towards target
		Vector3 direction = (currentPath[index].position - transform.position);
		transform.Translate(direction.normalized * Time.deltaTime * speed);

		if (direction.sqrMagnitude < range) 
		{
			if (++index == currentPath.Count) 
			{
				// Get new Path from some path manager
				currentPath = PathManager.GetNewPath();
				
				// Respawn object
				index = 0;
				transform.position = currentPath[0].position;
			}
		}


		//Some problems with this too

        // Check if there is a road below
		/*
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Ground")) 
            {
                // No road under
                // from the current path find the closest road block and respawn there
                Node closest = FindClosestRoadBlock();
                transform.position = closest.position;
                // Update index 
                index = currentPath.IndexOf(closest);
            }
        }
		*/
        
    }

	Node FindClosestRoadBlock() 
    {
        Node closest = null;
        Vector3 position = transform.position;
        float distance = Mathf.Infinity;
		foreach (Node node in currentPath)
        {
            Vector3 diff = node.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = node;
                distance = curDistance;
            }
        }
        return closest;
    }
}
