using UnityEngine;
using System.Collections;

public class GizmosDrawing : MonoBehaviour {
	public Color color;
	public float size = 1f;
	void OnDrawGizmosSelected() {
		Gizmos.color = color;
		Gizmos.DrawSphere(transform.position, size);
	}
}
