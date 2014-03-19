using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// Script is attached to an empty game object
/// Purpose is to fetch from server how many tags are active
/// This avoids manipulation of the software if a tag or a cube goes broken.
/// </summary>
public class StartScript : MonoBehaviour 
{
    UvSc textureScript;
	void Start () 
	{
        textureScript = GetComponent<UvSc>();
		/*string url = "192.168.123.124:8080/qpe/getHAIPLocation";                // url for the server
		WWW www = new WWW(url);                                                 // GET request
		yield return www;
		string [] arrGUID = FindAllGUID(www.text);                              // All GUID are stored in array
		for (int i = 0 ; i < arrGUID.Length; i++)                               // Using how many GUID were found to create as many cubes
		{
                GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);  // Create a new cube
                o.AddComponent<CubePosition>();               // Add components (could be replaced by prefab)
                ConnectScript cs = o.AddComponent<ConnectScript>();
                cs.tagQuuppa = arrGUID[i];                                      // tagQuuppa is given the GUID
                o.renderer.material
		}*/
	}
	
	/// <summary>
	/// The whole json files are given to the method
    /// The "id" tag is searched and the GUID is given to the list which is returned as array
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	private string[] FindAllGUID (string text) 
	{
		List<string> list = new List<string>();     // The list to store the GUID
		string id = "id";                           // The token id
		string str = text;                          // Storing the text file into a local string

		int offset = 6; int length = 12;            // offset values for string manipulation

		while(true)
		{
			int index = str.IndexOf(id);            // Looking for the token id
			if(index < 0) break;                    // if not found break the loop
			int start = index + offset;             // use offset to get the starting of the GUID
			list.Add (str.Substring(start,length)); // Isolate the GUID
			str = str.Substring(start + length);    // Cut the string for next search
		}
		return list.ToArray ();                     // Return the list
	}
}
