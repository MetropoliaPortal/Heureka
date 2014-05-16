using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System;

public class QuuppaStart : MonoBehaviour 
{
	private TagManager tagManager;

	public IEnumerator Start () 
	{


		string url = "192.168.123.124:8080/qpe/getHAIPLocation";
		WWW www = new WWW(url);                                                
		yield return www;
		if (www.error != null) 
		{
			print (www.error);
		}

		tagManager = GetComponent<TagManager> ();

		// All GUID are stored in array
		string [] arrGUID = FindAllGUID(www.text);                              

		/*
		foreach (KeyValuePair<string, BuildingType> kp in tagBuildingTypes) 
		{
			Debug.Log(kp.Value);
		}
		*/			
		string tagInfo = "192.168.123.124:8080/qpe/getTagInfo?tag=";
		string token = "batteryAlarm\": \"";
		// Reference to the QuuppaDeficientTag script
		// The component is used to remove deficient tag and cubes from the game
		// The reference is set to null, if no tag is deficient the reference is not used
		TagMaintenanceGUI manager = null;		

		// Using how many GUID were found to create as many cubes
		for (int i = 0 ; i < arrGUID.Length; i++)                               
		{
			string urlInfo = tagInfo + arrGUID[i];
			WWW info = new WWW(urlInfo);
			yield return info;					
			if(info.error != null)
			{
				Debug.LogError("error connecting server");
				continue;					// an error occured, we skip the creation of the cube as the tag is probably faulty or old
			}

			tagManager.AddTag( arrGUID[i] );

			//NOT WORKING
			/*
			string str = info.text;			
			int index = str.IndexOf(token);
			index += token.Length;
			char c = urlInfo[index + 1];
			if(c == 'l') // Battery is low, inform user
			{
				// Access manager and pas GameObject and string 
				if(manager == null)manager = GetComponent<Manager>();
				manager.AddDeficientTag(arrGUID[i], obj);
			}
			// The qdt reference is not null so we have some low battery tag
			// The coroutine to remove tags is started
			if(!manager.IsDeficientTagEmpty())
			{
				StartCoroutine(manager.QuuppaDeficientRemoval());
			}
			*/
		}
	}

	/// <summary>
	/// The whole json files are given to the method
	/// The "id" tag is searched and the GUID is given to the list which is returned as array
	/// </summary>
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
		return list.ToArray ();
	}
}
