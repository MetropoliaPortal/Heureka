using UnityEngine;
using System.Collections;

public class UvSc : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Mesh theMesh;
        theMesh = GetComponent<MeshFilter>().mesh;

        // Now store a local reference for the UVs
        Vector2[] theUVs = new Vector2[theMesh.uv.Length];
        theUVs = theMesh.uv;

        // set UV co-ordinates
        // Left
        theUVs[0] = new Vector2(0.33f, 0.0f);
        theUVs[1] = new Vector2(0.66f, 0.0f);
        theUVs[2] = new Vector2(0.33f, 1f);
        theUVs[3] = new Vector2(0.66f, 1f);
                // Top
        theUVs[4] = new Vector2(0.0f, 0.0f);
        theUVs[5] = new Vector2(0.33f, 0.0f);
        theUVs[8] = new Vector2(0.0f, 1f);
        theUVs[9] = new Vector2(0.33f, 1f);
                // Right
        theUVs[16] = new Vector2(0.33f, 0.0f);
        theUVs[18] = new Vector2(0.66f, 0.0f);
        theUVs[19] = new Vector2(0.33f, 1f);
        theUVs[17] = new Vector2(0.66f, 1f);
                // Back
        theUVs[6] = new Vector2(0.66f, 0f);
        theUVs[7] = new Vector2(1f, 0f);
        theUVs[10] = new Vector2(0.66f, 1f);
        theUVs[11] = new Vector2(1f, 1f);
                // Left
        theUVs[22] = new Vector2(0.33f, 0.00f);
        theUVs[20] = new Vector2(0.66f, 0.00f);
        theUVs[23] = new Vector2(0.33f, 1f);
        theUVs[21] = new Vector2(0.66f, 1f);
        // Assign the mesh its new UVs
        theMesh.uv = theUVs;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
