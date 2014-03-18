using UnityEngine;
using System.Collections.Generic;

public class StackPool
{
    GameObject gameObject;
    Stack<GameObject> m_stack = null;
    Transform parentTr;

    public StackPool(GameObject road, Transform parentTr)
    {
        m_stack = new Stack<GameObject>();
        this.gameObject = road;
        this.parentTr = parentTr;
    }

	public StackPool()
	{
		m_stack = new Stack<GameObject>();
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
            GameObject o = (GameObject)MonoBehaviour.Instantiate(gameObject);
            o.tag = "Road";
            o.transform.parent = parentTr;
            return o;
        }
    }

	/// <summary>
	/// Tries to pop from stack and does not instantiate new gameobject when popping is not possible.
	/// </summary>
	/// <returns>GameObject</returns>
	public GameObject PopLimited() 
	{
		if (m_stack.Count > 0)
		{
			GameObject o = m_stack.Pop();
			o.SetActive(true);
			return o;
		} else 
			return null;
	}
}
