using UnityEngine;
using System.Collections;

public class CarSprite : MonoBehaviour 
{

	public Sprite front, back, sideLeft, sideRight; 
	private SpriteRenderer spriteRenderer;
	
	void Start () 
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Apply the corresponding srpite based on direction
	public void GetDirection( Transform[] m_path, int i_index )
	{
		Vector3 direction = (m_path[i_index].position - transform.position);

		// Get all four directions
		Vector3 [] directions = {Vector3.right, Vector3.left, Vector3.forward, Vector3.back};
		int ind = -1;
		Vector3 d = direction.normalized;
		// Check current direction against all directions to find which has dot close to 1
		for(int i = 0; i < directions.Length; i++ )
		{
			float dot =  d.x * directions[i].x + d.z * directions[i].z;
			if(dot >= 1 - 0.25f && dot <= 1 + 0.25f)
			{
				ind = i;
				break;
			}
		}
		if(ind != -1)
		{
			Vector3 dir = directions[ind];
			Vector3 targetPoint = m_path[i_index].position;
			float dot = Vector3.Dot(dir, direction);
			Vector3 a = -dot * dir;
			
			Vector3 closest = targetPoint + a;
			Vector3 v = new Vector3(directions[ind].z, directions[ind].y, -directions[ind].x);
			Vector3 b =closest + v * 0.2f;
			b.y = 0.3f;
			transform.position = b;
		}
		switch(ind)
		{
		case 0:
			spriteRenderer.sprite = sideRight;
			break;
		case 1 :
			spriteRenderer.sprite = sideLeft;
			break;
		case 2:
			spriteRenderer.sprite = back;
			break;
		case 3:
			spriteRenderer.sprite = front;
			break;
		}
	}
}
