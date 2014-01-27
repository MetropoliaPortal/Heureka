using UnityEngine;
using System.Collections;

public class ConnectionScript : MonoBehaviour {
	
	public SecBoxScript script;
    private Vector3 m_prevPosition;
    private Transform m_transform;
	void Start () 
	{
        m_transform = GetComponent<Transform>();
	    m_prevPosition = m_transform.position;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (CheckPos() == true)
        {
            script.SetPosition(m_transform.position);
        }
	}
	public void SendMessageToSecCube()
	{
        script.CheckNearItem();
	}
    private bool CheckPos()
    {
        if (m_prevPosition.x != m_transform.position.x || m_prevPosition.z != m_transform.position.z) 
        {
            return true;
        }
        return false; 
    }
}
