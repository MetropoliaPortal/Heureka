using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// //////////////////////////////////////////////
// This is all done without any test so it may not totally work but still
// I would guess the logic is there
/// ///////////////////////////////////////////////
public class CarScript : MonoBehaviour 
{
    //private new Transform transform;
    private Transform cam;
	int currentPathId;
    List<Node> currentPath;
    int index;
    public float speed;
    private float range;
	private bool isMovingOppositeDirection;

    void Start() {
		//transform = base.transform;
        cam = Camera.main.transform;
		range = 0.1f;
		speed = 1.0f;
		index = 0;
    }

	/// <summary>
	/// Called from RoadManager when getting a car from stack.
	/// Randomly decides which direction the car is moving
	/// currentPathId is stored for getting the possibly updated path.
	/// </summary>
	public void Initialize(){

		isMovingOppositeDirection = ( Random.Range(0,2) == 1) ? true : false;
		
		currentPathId = PathManager.GetNewPathId();
		currentPath = PathManager.GetNewPath( currentPathId );

		if( isMovingOppositeDirection )
			index = currentPath.Count-1;
		else
			index = 0;

		transform.position = currentPath[ index ].position;
	}

    void Update() 
    {
		currentPath = PathManager.GetUpdatedPath( currentPathId );
        // Always face the camera
        transform.LookAt(-cam.position);
        // Check for orientation
        // Using some cross product

        // Move towards target
		Vector3 direction = (currentPath[index].position - transform.position);

		//This is affected by transform.lookAt() function
		//transform.Translate(direction.normalized * Time.deltaTime * speed);

		//Move transform without getting affected by lookAt()
		transform.position = Vector3.MoveTowards (transform.position, currentPath[index].position, 0.05f);

		if (direction.sqrMagnitude < range) 
		{
			if ( isMovingOppositeDirection )
				MoveToNextOpposite();
			else
				MoveToNext();
		}

		CheckRoadUnder();    
    }

	void MoveToNextOpposite(){
		if (--index == 0)
			Initialize();
	}

	void MoveToNext(){
		if (++index == currentPath.Count) 
			Initialize();
	}
	
	/// <summary>
	/// Uses linecast from default layer(cars) to road layer(roads) to check if road is below
	/// </summary>
	void CheckRoadUnder(){
		int _layerMask = 1 << 8;

		Vector3 castStart = transform.position;
		castStart.y += 0.5f;
		
		Vector3 castEnd = transform.position;
		castEnd.y -= 0.5f;
		
		if ( !Physics.Linecast (castStart, castEnd, _layerMask) ){
			Node closest = FindClosestRoadBlock();
			transform.position = closest.position;
			// Update index 
			index = currentPath.IndexOf(closest);
		}	
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
