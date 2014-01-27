using UnityEngine;
using System.Collections;

public class SecBoxScript : MonoBehaviour {

	public Transform m_boxTransform;
	private Transform m_transform;
	void Start () 
	{
		m_transform = GetComponent<Transform>();
		Vector3 pos = m_boxTransform.position;
		pos.z -= 50f;
		m_transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 pos = m_boxTransform.position;
		pos.z -= 50f;
		m_transform.position = pos;
	}
}
