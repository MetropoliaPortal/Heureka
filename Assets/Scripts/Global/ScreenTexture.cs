using UnityEngine;
using System.Collections;

public class ScreenTexture : MonoBehaviour 
{

	public Texture[] textures;
	public Material mat1;
	public Material mat2;
	static int value = 0;
	void Start () 
	{
		mat1 = GameObject.Find ("Screen1").renderer.material;
		mat2 = GameObject.Find ("Screen2").renderer.material;
		SetValue ();
	}
	
	// Update is called once per frame
	void OnGUI () 
	{
		GUI.Box (new Rect(0,0,100,50), value.ToString());
	}
	public void SetValue()
	{
		if(++value == textures.Length)value = 0;
		System.IO.File.WriteAllText(@"C:\Documents\Info.txt",value.ToString());
		mat1.mainTexture = textures[value];
		mat2.mainTexture = textures[value];
	}
}
