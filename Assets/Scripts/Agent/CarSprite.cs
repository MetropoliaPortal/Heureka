using UnityEngine;
using System.Collections;

public class CarSprite : MonoBehaviour 
{
	public Sprite leftDown, rightUp, leftUp, rightDown; 
	private SpriteRenderer spriteRenderer;
	private CarMovement carMovement;
	
	void Start () 
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		carMovement = transform.parent.gameObject.GetComponent<CarMovement>();
	}

	// Apply the corresponding srpite based on direction
	public void GetDirection( Transform[] m_path, int i_index )
	{
		Vector3 pathPoint = m_path[i_index].position;
		Vector3 dir = (pathPoint - transform.parent.position);
		dir.Normalize();

		Vector3 [] directions = {Vector3.right, Vector3.left, Vector3.forward, Vector3.back};
		int correspondingDir = -1;
		float smallestDotOffset = 1;

		// Check current direction against all directions to find which has dot close to 1
		for(int i = 0; i < directions.Length; i++ )
		{
			float dot =  Vector3.Dot(dir, directions[i]);

			if( Mathf.Abs( 1 - dot ) < smallestDotOffset )
			{
				correspondingDir = i;
				smallestDotOffset = Mathf.Abs( 1 - dot );
			}
		}

		switch(correspondingDir)
		{
		case 0:
			if(spriteRenderer.sprite != rightDown)
			{
				//Vector3 adjustedPos = Vector3.zero;
				//adjustedPos.z -= 0.4f;
				//transform.localPosition = adjustedPos;
				spriteRenderer.sprite = rightDown;
			}
			break;
		case 1 :
			if(spriteRenderer.sprite != leftUp)
			{
				//Vector3 adjustedPos = Vector3.zero;
				//adjustedPos.z -= 0.4f;
				//transform.localPosition = adjustedPos;
				spriteRenderer.sprite = leftUp;
			}
			break;
		case 2:
			if(spriteRenderer.sprite != rightUp)
			{
				//Vector3 adjustedPos = Vector3.zero;
				//adjustedPos.x += 0.2f;
				//transform.localPosition = adjustedPos;
				spriteRenderer.sprite = rightUp;
			}
			break;
		case 3:
			if(spriteRenderer.sprite != leftDown)
			{
				//Vector3 adjustedPos = Vector3.zero;
				//adjustedPos.x -= 0.5f;
				//transform.localPosition = adjustedPos;
				spriteRenderer.sprite = leftDown;
			}
			break;
		}
	}
}
