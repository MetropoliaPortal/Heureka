using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Node : IComparable
{
    #region Fields
    public float nodeTotalCost;         //Total cost so far for the node
    public float estimatedCost;         //Estimated cost from this node to the goal node
    public bool bObstacle;              //Does the node is an obstacle or not
	public bool isRoad;					//Marks roads for path managerer
    public Node parent;                 //Parent of the node in the linked list
    public Vector3 position;            //Position of the node
    #endregion

    /// <summary>
    //Default Constructor
    /// </summary>
    public Node()
    {
        this.estimatedCost = 0.0f;
        this.nodeTotalCost = 1.0f;
        this.bObstacle = false;
		this.isRoad = false;	
        this.parent = null;
    }

    /// <summary>
    //Constructor with adding position to the node creation
    /// </summary>
    public Node(Vector3 pos)
    {
        this.estimatedCost = 0.0f;
        this.nodeTotalCost = 1.0f;
        this.bObstacle = false;
		this.isRoad = false;
        this.parent = null;
        this.position = pos;
    }

    /// <summary>
    // This CompareTo methods affect on Sort method
    // It applies when calling the Sort method from ArrayList
    // Compare using the estimated total cost between two nodes
    /// </summary>
    public int CompareTo(object obj)
    {
        Node node = (Node)obj;
        if (this.estimatedCost < node.estimatedCost)
            return -1;
        if (this.estimatedCost > node.estimatedCost)
            return 1;

        return 0;
    }
}

[System.Serializable]
public class CrossNode 
{
    public Node[] node = new Node[4];
    public Vector3 position;
    public CrossNode(Vector3 position, Node[,]nodeArray, Dictionary<int, CrossNode>dict) 
    {
		// Assign position of the cross node
		this.position = position;

		// Get the position of the 4 squares around the node
		Vector3[] pos = {position, position, position, position};
		pos[0].x -= 0.5f;
		pos[0].z -= 0.5f;
		pos[1].x -= 0.5f;
		pos[1].z += 0.5f;
		pos[2].x += 0.5f;
		pos[2].z -= 0.5f;
		pos[3].x += 0.5f;
		pos[3].z += 0.5f;

		// Find the squares(nodes) from the list of squares 
		// Place them in the array of square(nodes)
		int row = nodeArray.GetLength(0);
		int col = nodeArray.GetLength(1);
		for (int i = 0; i < 4 ; i++)
		{
			for(int j = 0; j < row ; j++)
			{
				for(int k = 0; k < col ; k++)
				{
					if(nodeArray[k,j].position == pos[i])
					{
						node[i] = nodeArray[k,j];
						break;
					}
				}
			}
		}

		int  key = (int)position.x * 100 + (int)position.z;
		dict.Add(key, this);
    }
}


