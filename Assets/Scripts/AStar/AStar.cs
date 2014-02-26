using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar
{
    #region List fields

    public static PriorityQueue closedList, openList;

    #endregion

    /// <summary>
    /// Calculate the final path in the path finding
    /// </summary>
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

    /// <summary>
    /// Find the path between start node and goal node using AStar Algorithm
    /// </summary>
    public static List<Node> FindPath(Node start, Node goal)
    {
        //Start Finding the path
        openList = new PriorityQueue();
        openList.Push(start);
        start.nodeTotalCost = 0.0f;
        //start.estimatedCost = HeuristicEstimateCost(start, goal);
        start.estimatedCost = (start.position - goal.position).sqrMagnitude;
        closedList = new PriorityQueue();
        Node node = null;

		//
		Node previousNode = null;

        while (openList.Length != 0)
        {
            node = openList.First();

            if (node.position == goal.position)
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

                if (!closedList.Contains(neighbourNode))
                {					
					//Cost from current node to this neighbour node
	                //float cost = HeuristicEstimateCost(node, neighbourNode);
                    float cost = (node.position - neighbourNode.position).sqrMagnitude;
					//Total Cost So Far from start to this neighbour node
	                float totalCost = node.nodeTotalCost + cost;
					
					//Estimated cost for neighbour node to the goal
					//Does not take goal position Z into account when calculating the estimated cost to prevent zig-zag roads
					float neighbourNodeEstCost = (neighbourNode.position - new Vector3(goal.position.x, 0, neighbourNode.position.z)).sqrMagnitude;	

					//Assign new cost and parent only if totalCost is smaller than neighbours total cost
					if(neighbourNode.nodeTotalCost == 1.0f || totalCost < neighbourNode.nodeTotalCost){
						//Assign neighbour node properties
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
