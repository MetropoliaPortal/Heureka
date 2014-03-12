using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class StartScript : MonoBehaviour {

	string []textG;
	GameObject obj = null;
	IEnumerator Start () 
	{
		string url = "192.168.123.124:8080/qpe/getHAIPLocation";
		WWW www = new WWW(url);
		yield return www;
		textG = FindAllTag(www.text);
		for (int i = 0 ; i < textG.Length; i++)
		{
			GameObject  o = GameObject.CreatePrimitive(PrimitiveType.Cube);
			obj = o;
			ConnectScript cs = o.AddComponent<ConnectScript>();
			cs.tagQuuppa = textG[i];
			o.AddComponent<CubePosition>();
		}
	}
	
	// Update is called once per frame
	private string[] FindAllTag (string text) 
	{
		List<string> list = new List<string>();
		string id = "id";
		string str = text;

		int offset = 6; int length = 11;

		while(true)
		{
			int index = str.IndexOf(id);
			if(index < 0) break;
			int start = index + offset;
			list.Add (str.Substring(start,length));
			str = str.Substring(start + length);
		}
		return list.ToArray ();
	}
	void OnGUI()
	{
		GUI.Box (new Rect(0,100,200,100),obj.transform.position.ToString ());
	}
}
