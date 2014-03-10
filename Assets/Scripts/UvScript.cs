using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class UvScript : MonoBehaviour {

    void Start() 
    {
        // Get the mesh
        Mesh theMesh;
        theMesh = this.transform.GetComponent<MeshFilter>().mesh;
 
        // Now store a local reference for the UVs
        Vector2[] theUVs   = new Vector2[theMesh.uv.Length];
        theUVs = theMesh.uv;
 
        // set UV co-ordinates
        // Front
        theUVs[0] = new Vector2(0.0f, 0.35f);
        theUVs[1] = new Vector2(0.25f, 0.35f);
        theUVs[2] = new Vector2(0.0f, 0.66f);
        theUVs[3] = new Vector2(0.25f, 0.66f);
        // Top
        theUVs[4] = new Vector2(0.25f, 0.0f);
        theUVs[5] = new Vector2(0.5f, 0.0f);
        theUVs[8] = new Vector2(0.25f, 0.35f);
        theUVs[9] = new Vector2(0.5f, 0.35f);
        // Right
        theUVs[16] = new Vector2(0.25f,0.35f);
        theUVs[18] = new Vector2(0.5f, 0.35f);
        theUVs[19] = new Vector2(0.25f, 0.66f);
        theUVs[17] = new Vector2(0.5f, 0.66f);
        // Back
        theUVs[6] = new Vector2(0.75f, 0.35f);
        theUVs[7] = new Vector2(0.5f, 0.35f);
        theUVs[10] = new Vector2(0.75f, 0.66f);
        theUVs[11] = new Vector2(0.5f, 0.66f);       
        // Left
        theUVs[22] = new Vector2(1f, 0.35f);
        theUVs[20] = new Vector2(0.75f, 0.35f);
        theUVs[23] = new Vector2(0.75f, 0.66f);
        theUVs[21] = new Vector2(1f, 0.66f);
        // Assign the mesh its new UVs
        theMesh.uv = theUVs;
    }
}
