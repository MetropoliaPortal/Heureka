//#define DEBUGMODE
using UnityEngine;
using System.Collections.Generic;

public class CubePosition : MonoBehaviour
{
    #region MEMBERS
    // Cache the transform for faster performance
    private new Transform transform;

	private Queue<Vector3> m_queue = new Queue<Vector3>();
    // Cache the grid
    private Vector3 prevPos;
    private int row;
    private int column;
    public delegate void EventMove();
    public static event EventMove OnMove = new EventMove(() => { });
	public static event EventMove OnMoveSecond = new EventMove(() => { });
    private bool b_fireEvent = false;
    #endregion

    public void Start () 
    {
        transform = base.transform;
        GameObject obj = GameObject.Find("GridManager");
        GridManager gm = obj.GetComponent<GridManager>();
        row = gm.numOfRows;
        column = gm.numOfColumns;
        GridManager.obstacleList.Add(gameObject);
        PositionCube(0,0,0);
        
#if DEBUGMODE
		InvokeRepeating("CheckPosition", 0.001f, 0.01f);
#endif
	}
    
    /// <summary>
    /// Position the cube on the center of the four squares it occupies
    /// Constrain the cube within the boundaries of the game
    /// </summary>
    public void PositionCube(float x, float y , float z)
    {
        // Drop the decimal part
		m_queue.Enqueue(new Vector3(x,y,z));
		Vector3 v = CheckValue();
		v.x *= (row) / 3.43f;
		v.y *= (column) / 3.43f;
		v.x = Mathf.Round(v.x);
		v.y = Mathf.Round(v.y);
		//z = Mathf.Round(z);
		if(v.z <= 0.40)v.z = 1;
		else if (v.z > 0.40 && v.z <= 0.80)v.z = 3;
		else if (v.z > 0.80) v.z = 5;
        // Store the value
		prevPos = transform.position;
		Vector3 pos = new Vector3(Mathf.Clamp(v.x,2,row - 2), v.z ,Mathf.Clamp(v.y, 2, column - 2));	
		//m_queue.Enqueue(pos);
		transform.position = pos;
		CheckPosition();
    }

	void CheckPosition() 
    {
        // Check if Cube has moved
        if (prevPos != transform.position)
        {
            //PositionCube();
            b_fireEvent = true;
        }
        else if (prevPos == transform.position && b_fireEvent == true)
        {
            b_fireEvent = false;
            OnMove();
			OnMoveSecond();
        }  
    }
	private Vector3 CheckValue()
	{
		if(m_queue.Count < 10) return prevPos;
		while(m_queue.Count > 10)
		{
			m_queue.Dequeue();
		}
		List<Vector3> list = new List<Vector3>(m_queue);

		float totalX = 0f;
		float totalY = 0f;
		float totalZ = 0f;
		int count = list.Count;
		// Get average for each component
		for (int i = 0; i < count; i++)
		{
			totalX += list[i].x;
			totalY += list[i].y;
			totalZ += list[i].z;
		}
		Vector3 vec = new Vector3();
		float c = (float)count;
		vec.x = totalX / c;
		vec.y = totalY / c;
		vec.z = totalZ / c;



		return vec;
	}
}
