using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;


public enum BuildingType{
	Official, Residential,Leisure, Shop
}
/// <summary>
/// Script is attached to an empty game object
/// Purpose is to fetch from server how many tags are active
/// This avoids manipulation of the software if a tag or a cube goes broken.
/// The object is used on start of the program and destroy at the end of the Start
/// 
/// 	StartScript.cs -------------|
/// 		|						|->UvSc.cs modifies the Material uv map of the Cube object
///    	Cube object-----------------|
/// 		|
/// 		|-> ConnectScript.cs
/// 		|		|
/// 		|		|-> Sends info to CubePosition -> process and modify Transform of the Cube object
/// 		|		|
/// 		|		|-> Sends info to CubeRotation -> process and modify the texture on the Cube object
/// 		|
/// 		|-> UvSc.cs -> modifies the uv of the cube
/// 
/// 
/// 
/// 
/// </summary>
public class StartScript : MonoBehaviour 
{
#if DEBUG
#elif PROD
	IEnumerator Start () 
	{
		string url = "192.168.123.124:8080/qpe/getHAIPLocation";                // url for the server, all tags are requested
		WWW www = new WWW(url);                                                 // GET request
		yield return www;
		if(www.error != null)print (www.error);
		string [] arrGUID = FindAllGUID(www.text);                              // All GUID are stored in array
		// Dictionary<string, BuildingType>dict;

		/////////////////////////////////////////////
		/// 
		/// Here the file for building type attribution is accessed
		/// 
		/////////////////////////////////////////////
		// dict = GetFileBuilding();

		UvSc uvScript = gameObject.AddComponent<UvSc>();						// Add UvSc.cs to that object
		for (int i = 0 ; i < arrGUID.Length; i++)                               // Using how many GUID were found to create as many cubes
		{
     		GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);  	// Create a new cube
			o.transform.localScale = new Vector3(2,2,2);						// The cube is scaled up to 2

			CubePosition cp = o.AddComponent<CubePosition>();               	// Add components (could be replaced by prefab)
			CubeRotation cr = o.AddComponent<CubeRotation>();					// Add CubeRotation to that object
			ConnectScript cs = o.AddComponent<ConnectScript>();					// Add ConnectScript to that object

			cs.Init(cp, cr,arrGUID[i]); 
			uvScript.Init(o);													// Initialize the UvSc for the current cube object
			                                  
			/////////////////////////////////////////////
			/// 
			/// Here the file for building type attribution is read
			/// BuildingType type = GetBuildingTypeFromTag(cs.tagQuuppa);
			/// 
			/////////////////////////////////////////////
			cr.Init(arrGUID[i]/*, dict[arrGUID[i]]*/);												// initialize the CubeRotation on that object
		}
		// Remove the objects as there are no longer useful
		Destroy (uvScript);
		Destroy (this);
	}
#endif
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
	Dictionary<string, BuildingType> GetFileBuilding()
	{
		string file = null;
		// Dictionary for Key-tag / Value- Building type
		Dictionary<string, BuildingType>dict = new Dictionary<string, BuildingType>();
		// Get the file from location
		// while not the end of the file
			// Read each line of the file
			// Each line contains the Quuppa tag followed by the corresponding building type
			// "id":"01234567ac81", "type":"Official"
			// Parse the id
			// Parse the type 
			// BuildingType bt = (BuildingType)System.Enum.Parse(typeof(Buildingtype), type);
			// Add to the dictionary

		return dict;
	}
}
