using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class TestCode : MonoBehaviour 
{
    private Transform startPos, endPos;
    private Node startNode;
    private Node goalNode;

    public List<Node> pathArray = new List<Node>();
    public Transform[] waypoints;
    public GameObject road;
    GameObject[][] roadArray = new GameObject[7][];

    void Start() 
    {
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (Physics.Linecast(waypoints[i].position, waypoints[i + 1].position))
            {
                roadArray[i] = DrawRoadAStar(waypoints[i].position, waypoints[i + 1].position);
            }
            else
            {
                roadArray[i] = DrawStraightRoad(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        if (Physics.Linecast(waypoints[0].position, waypoints[3].position))
        {
            roadArray[5] = DrawRoadAStar(waypoints[0].position, waypoints[3].position);
        }
        else
        {
            roadArray[5] = DrawStraightRoad(waypoints[0].position, waypoints[3].position);
        }
        
        if (Physics.Linecast(waypoints[2].position, waypoints[5].position))
        {
            roadArray[6] = DrawRoadAStar(waypoints[2].position, waypoints[5].position);
        }
        else
        {
            roadArray[6] = DrawStraightRoad(waypoints[2].position, waypoints[5].position);
        }
    }
    /*void OnDrawGizmos()
    {
        if (pathArray == null)
            return;

        if (pathArray.Count > 0)
        {
            int index = 1;
            foreach (Node node in pathArray)
            {
                if (index < pathArray.Count)
                {
                    UnityEngine.Debug.DrawLine(node.position, pathArray[index].position, Color.blue, 50f);
                    index++;
                }
            };
        }
    }*/
    GameObject[] DrawRoadAStar(Vector3 start, Vector3 end) 
    {
        startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(start)));
        goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(end)));
        pathArray = AStar.FindPath(startNode, goalNode);
        List<GameObject> arrayRoad = new List<GameObject>();
        for (int k = 0; k < pathArray.Count - 1; k++)
        {
            Vector3 pos = pathArray[k].position;
            pos.y += 0.01f;
            arrayRoad.Add((GameObject)Instantiate(road, pos, Quaternion.identity));
            
            pos = pathArray[k + 1].position - pathArray[k].position;
            pos = pathArray[k].position + 0.5f * pos;
            pos.y += 0.01f;
            arrayRoad.Add((GameObject)Instantiate(road, pos, Quaternion.identity));
        }
        return arrayRoad.ToArray();
    }

    GameObject[] DrawStraightRoad(Vector3 start, Vector3 end) 
    {
        Vector3 first = start;
        List<GameObject> arrayRoad = new List<GameObject>();
        while (first != end)
        {
            Vector3 pos = first;
            pos.y += 0.01f;
            arrayRoad.Add((GameObject)Instantiate(road, pos, Quaternion.identity));
            first = Vector3.MoveTowards(first, end, 1.0f);

        }
        return arrayRoad.ToArray();
    }
}