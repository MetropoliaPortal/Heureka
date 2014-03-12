using UnityEngine;
using System.Collections.Generic;

public class StackPool
{
    GameObject road;
    Stack<GameObject> m_stack = null;
    Transform parentTr;

    public StackPool(GameObject road, Transform parentTr)
    {
        m_stack = new Stack<GameObject>();
        this.road = road;
        this.parentTr = parentTr;
    }

    public void Push(GameObject obj) 
    {
        m_stack.Push(obj);
        obj.SetActive(false);
    }
    public GameObject Pop() 
    {
        if (m_stack.Count > 0)
        {
            GameObject o = m_stack.Pop();
            o.SetActive(true);
            return o;
        }
        else 
        {
            GameObject o = (GameObject)MonoBehaviour.Instantiate(road);
            o.tag = "Road";
            o.transform.parent = parentTr;
            return o;
        }
    }
}
