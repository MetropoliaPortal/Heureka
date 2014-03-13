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
    List<Transform> currentPath;
    int index = 0;
    public float speed;
    private float range;
    void Start() {
        transform = base.transform;
        cam = Camera.main.transform;
    }
    void Update() 
    {
        // Always face the camera
        transform.LookAt(-cam.position);
        // Check for orientation
        // Using some cross product

        // Move towards target
        Vector3 direction = (currentPath[index].position - transform.position).normalized;
        transform.Translate(direction * Time.deltaTime * speed);
        if (direction.sqrMagnitude < range) 
        {
            if (++index == currentPath.Count) 
            {
                // Get new Path from some path manager
                // currentPath = PathManager.GetNewPath();

                // Respawn object
                transform.position = currentPath[0].position;
            }
        }

        // Check if there is a road below
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Ground")) 
            {
                // No road under
                // from the current path find the closest road block and respawn there
                Transform closest = FindClosestRoadBlock();
                transform.position = closest.position;
                // Update index 
                index = currentPath.IndexOf(closest);
            }
        }
    }
    Transform FindClosestRoadBlock() 
    {
        Transform closest = null;
        Vector3 position = transform.position;
        float distance = Mathf.Infinity;
        foreach (Transform tr in currentPath)
        {
            Vector3 diff = tr.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = tr;
                distance = curDistance;
            }
        }
        return closest;
    }
}
