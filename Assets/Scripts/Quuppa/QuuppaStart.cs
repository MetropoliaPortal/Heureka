using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System;


public enum BuildingType
{
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
	// DEBUG
	// This is just when running in debug mode without Quuppa system
	void Start()
	{
		GetFileBuilding();
	}
/*
 * this part is when using Quuppa System
	IEnumerator Start () 
	{
		string url = "192.168.123.124:8080/qpe/getHAIPLocation";                // url for the server, all tags are requested
		WWW www = new WWW(url);                                                 // GET request
		yield return www;
		if(www.error != null)print (www.error);
		string [] arrGUID = FindAllGUID(www.text);                              // All GUID are stored in array
	 	Dictionary<string, BuildingType>dict;

		/////////////////////////////////////////////
		/// 
		/// Here the file for building type attribution is accessed
		/// 
		/////////////////////////////////////////////
	 	dict = GetFileBuilding();

		UvSc uvScript = gameObject.AddComponent<UvSc>();						// Add UvSc.cs to that object
		for (int i = 0 ; i < arrGUID.Length; i++)                               // Using how many GUID were found to create as many cubes
		{
     		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);  	// Create a new cube
			obj.transform.localScale = new Vector3(2,2,2);						// The cube is scaled up to 2

			CubePosition cubePosition = obj.AddComponent<CubePosition>();       // Add components (could be replaced by prefab)
			CubeRotation cubeRotation = obj.AddComponent<CubeRotation>();		// Add CubeRotation to that object
			ConnectScript connectScript = obj.AddComponent<ConnectScript>();	// Add ConnectScript to that object

			connectScript.Init(cubePosition, cubeRotation,arrGUID[i]); 
			uvScript.Init(obj);													// Initialize the UvSc for the current cube object

			cubeRotation.Init(arrGUID[i], dict[arrGUID[i]]);					// initialize the CubeRotation on that object passing id and building type
		}
		// Remove the objects as there are no longer useful
		Destroy (uvScript);
		Destroy (this);
	}
*/	

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
		// Dictionary for Key-tag / Value- Building type
		Dictionary<string, BuildingType>dict = new Dictionary<string, BuildingType>();
		// Data needed for parsing
		string id = "id";	string token = ",";		int offset = 1;		int tagLength = 12;

		// Get the file from location
		try
		{
			// The url needs to be changed while on build mode
			// url should be tagFile.txt only.
			string url = @"tagFile.txt";
			//string url = @"C:\Users\Lucas\Desktop\tagFile.txt";		// This one when in Debug mode with no QuuppaSystem
																	// The file text is to be kept on the desktop, could be in Resources folder
			string urlBuild = @"..\tagFile.txt";					// This is when building the project
																	// The file is kept in the same folder as the build exe.
			using (StreamReader sr = new StreamReader(urlBuild))
			{
				string file = sr.ReadToEnd();
				while(true)
				{
					// Read each line of the file
					// Each line contains the Quuppa tag followed by the corresponding building type
					// id:01234567ac81,Official

					int index = file.IndexOf(id);				// Get the index of "id"
					if(index < 0) break;						// if "id" not found break loop			
					index += id.Length + offset;				// move the index to the starting of the id
					string tagQuuppa = file.Substring(index, tagLength);	// get the id string
					file = file.Substring(index +tagLength + 1);				// Remove the used part of the file including the middle coma 
					index = file.IndexOf(token);				// get the index of the end of line coma 
					string type = file.Substring(0,index);		// isolate the string containing the type
					file = file.Substring(index + 1);			// remove the used part of the file including the final coma

					BuildingType bt = (BuildingType)System.Enum.Parse(typeof(BuildingType), type);// Parse the type 

					dict.Add (tagQuuppa, bt);					// Add to the dictionary
				}
			}
		}
		catch (Exception e)
		{
			print(e.Message);
		}
		// Return the dictionary with all matching id and type
		return dict;
	}
}
