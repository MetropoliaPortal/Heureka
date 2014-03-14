using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class UvScript : MonoBehaviour {

    public Texture2D[] textures;
    void Start() 
    {
        // Get the mesh
        Mesh theMesh;
        theMesh = this.transform.GetComponent<MeshFilter>().mesh;
 
        // Now store a local reference for the UVs
        Vector2[] theUVs   = new Vector2[theMesh.uv.Length];
        theUVs = theMesh.uv;
 
        // set UV co-ordinates
        // Left
        theUVs[0] = new Vector2(0.0f, 0.33f);
        theUVs[1] = new Vector2(0.25f, 0.33f);
        theUVs[2] = new Vector2(0.0f, 0.66f);
        theUVs[3] = new Vector2(0.25f, 0.66f);
        // Top
        theUVs[4] = new Vector2(0.25f, 0.0f);
        theUVs[5] = new Vector2(0.5f, 0.0f);
        theUVs[8] = new Vector2(0.25f, 0.33f);
        theUVs[9] = new Vector2(0.5f, 0.33f);
        // Right
        theUVs[16] = new Vector2(0.25f,0.33f);
        theUVs[18] = new Vector2(0.5f, 0.33f);
        theUVs[19] = new Vector2(0.25f, 0.66f);
        theUVs[17] = new Vector2(0.5f, 0.66f);
        // Assign the mesh its new UVs
        theMesh.uv = theUVs;
    }
    public void ChangeTexture(int index) 
    {
        if (index >= textures.Length)
        {
            Debug.LogError("Wrong index passed to the method");
            return;
        }
        renderer.material.mainTexture = textures[index];
    }
}
