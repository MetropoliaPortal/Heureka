using UnityEngine;
using System.Collections;

/// <summary>
/// UvSc.cs
/// The component is created from the StartScript.cs
/// It only contains one method to modify the UV map of the material
/// once all cubes are taken care, the UvSc object is removed from the game
/// </summary>
public class UvSc : MonoBehaviour {

	/// <summary>
	/// Initialize the specified cubeObject.
	/// The method is called from the StartScript.cs
	/// The Cube object to be taken care is passed as parameter
	/// </summary>
	/// <param name="cubeObject">Cube object.</param>
	public void Initialize (GameObject cubeObject) {
        Mesh theMesh;
		theMesh = cubeObject.GetComponent<MeshFilter>().mesh;

        // Now store a local reference for the UVs
        Vector2[] theUVs = new Vector2[theMesh.uv.Length];
        theUVs = theMesh.uv;

        // set UV co-ordinates

		// Top
		theUVs[8] = new Vector2(0.0f, 0.0f);
		theUVs[9] = new Vector2(0.33f, 0.0f);
		theUVs[4] = new Vector2(0.0f, 1f);
		theUVs[5] = new Vector2(0.33f, 1f);
      
		// Back
        theUVs[1] = new Vector2(0.33f, 0.0f);
        theUVs[0] = new Vector2(0.66f, 0.0f);
        theUVs[3] = new Vector2(0.33f, 1f);
        theUVs[2] = new Vector2(0.66f, 1f);
        // Right
		theUVs[20] = new Vector2(0.66f, 0f);
		theUVs[22] = new Vector2(1f, 0.00f);
		theUVs[23] = new Vector2(0.66f, 1f);
		theUVs[21] = new Vector2(1f, 1f);
        // Left
		theUVs[7] = new Vector2(0.33f, 0.0f);
		theUVs[6] = new Vector2(0.66f, 0.0f);
		theUVs[11] = new Vector2(0.33f, 1f);
		theUVs[10] = new Vector2(0.66f, 1f);
        // Front
		theUVs[18] = new Vector2(0.66f, 0f);
        theUVs[16] = new Vector2(1f, 0.00f);
        theUVs[17] = new Vector2(0.66f, 1f);
        theUVs[19] = new Vector2(1f, 1f);
        // Assign the mesh its new UVs
        theMesh.uv = theUVs;
	}
}
