using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum TextureType { }
public class UvScript : MonoBehaviour 
{

    private Dictionary<TextureType, Texture2D[]> dict = new Dictionary<TextureType, Texture2D[]>();
    void Start() 
    {
        string path = "Textures/";
        foreach (TextureType type in Enum.GetValues(typeof(TextureType)))
        {
            string pathType = path + type.ToString();
            Texture2D[] textures = Resources.LoadAll<Texture2D>(pathType);
            dict.Add(type, textures);
        }     
    }

    public Texture2D[] SetTextures(TextureType type) 
    {
        if (!dict.ContainsKey(type)) 
        {
            Debug.LogError("No key found in the dictionary");
            return null;
        }
        return dict[type];
    }
    /// <summary>
    /// Method takes the object as parameter and returns an array of Vector2
    /// This is to be placed in the mesh.uv of the object
    /// </summary>
    /// <param name="obj"></param>
    public Vector2[] SetUVs(GameObject obj) 
    {
        // Get the mesh
        Mesh theMesh;
        theMesh = obj.GetComponent<MeshFilter>().mesh;
 
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
        return theUVs;
    }
    public void ChangeTexture(TextureType type, int index) 
    {
        Texture2D[] textures = dict[type];
        if (index >= textures.Length)
        {
            Debug.LogError("Wrong index passed to the method");
            return;
        }
        renderer.material.mainTexture = textures[index];
    }
}
