using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar
{
    private static PriorityQueue closedList, openList;
	
    private static List<Node> CalculatePath(Node node)
    {
        List<Node> list = new List<Node>();
        while (node != null)
        {
            list.Add(node);
            node = node.parent;

        }
        list.Reverse();
        return list;
    }
	
    public static List<Node> FindPath(Node start, Node goal, bool pathForRoad)
    {

		//Reset estimated costs from previous iteration
		GridManager.instance.ResetEstimatedCosts ();

        //Start Finding the path
        openList = new PriorityQueue();
        openList.Push(start);
        start.nodeTotalCost = 0.0f;
        start.estimatedCost = (start.position - goal.position).sqrMagnitude;
        closedList = new PriorityQueue();
        Node node = null;

		//Determine where starting point is and discard one of the goal.positions components to achieve 
		//more natural looking roads
		bool discardX = false, discardZ = false;
		if ( start.position.x < 0.5f || start.position.x > (GridManager.instance.numOfColumns - 1) ) {
			discardZ = true;
		}else if ( start.position.z < 0.5f || start.position.z > (GridManager.instance.numOfRows - 1) ) {
			discardX = true;
		}
		
        while (openList.Length != 0)
        {
            node = openList.First();

			if (node.position.x == goal.position.x && node.position.z == goal.position.z)
            {
                return CalculatePath(node);
            }

            List<Node> neighbours = new List<Node>();
            GridManager.instance.GetNeighbours(node, neighbours);
            #region CheckNeighbours

            //Get the Neighbours
            for (int i = 0; i < neighbours.Count; i++)
            {
                //Cost between neighbour nodes
                Node neighbourNode = neighbours[i];

				//With this code the function can be used with paths for roads and cars
				//Break the loop iteration if needed
				if( pathForRoad && neighbourNode.bObstacle )
					continue;
				if ( !pathForRoad && !neighbourNode.isRoad )
					continue;


                if (!closedList.Contains(neighbourNode))
                {					
					//Cost from current node to this neighbour node
                    float cost = (node.position - neighbourNode.position).sqrMagnitude;
					//Total Cost So Far from start to this neighbour node
	                float totalCost = node.nodeTotalCost + cost;
					
					//Estimated cost for neighbour node to the goal
					//Remove one of the goals position components. for preventing zig-zag roads
					float neighbourNodeEstCost;

					//Trading one of the goal.positions coordinates if needed

					if(discardZ){
						neighbourNodeEstCost = (neighbourNode.position - new Vector3(goal.position.x, 0, neighbourNode.position.z)).sqrMagnitude;
					}
					else if(discardX){
						neighbourNodeEstCost = (neighbourNode.position - new Vector3(neighbourNode.position.x, 0, goal.position.z)).sqrMagnitude;
					}
					else
						neighbourNodeEstCost = (neighbourNode.position - goal.position).sqrMagnitude;

					//Assign new neighbour node properties only if totalCost is smaller than neighbours current total cost
					if(neighbourNode.nodeTotalCost == 0.0f || totalCost < neighbourNode.nodeTotalCost){
						neighbourNode.nodeTotalCost = totalCost;
						neighbourNode.parent = node;
						neighbourNode.estimatedCost = totalCost + neighbourNodeEstCost;
					}
	
	                //Add the neighbour node to the list if not already existed in the list
	                if (!openList.Contains(neighbourNode))
	                {
	                    openList.Push(neighbourNode);
	                }
                }
            }
			
            #endregion
            
            closedList.Push(node);
            openList.Remove(node);
        }

        //If finished looping and cannot find the goal then return null
        if (node.position != goal.position)
        {
            Debug.LogError("Goal Not Found");
            return null;
        }

        //Calculate the path based on the final node
        return CalculatePath(node);
    }
}
