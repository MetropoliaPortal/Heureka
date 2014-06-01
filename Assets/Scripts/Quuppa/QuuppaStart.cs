using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System;

public class QuuppaStart : MonoBehaviour 
{
	private TagManager tagManager;
	private QuuppaConnectAll quuppaConnect;

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
		quuppaConnect = GetComponent<QuuppaConnectAll> ();

		// All GUID are stored in array
		string [] arrGUID = FindAllQuuppaId(www.text);                              

		string tagInfo = "192.168.123.124:8080/qpe/getTagInfo?tag=";
		string token = "batteryAlarm\": \"";

		for (int i = 0 ; i < arrGUID.Length; i++)                               
		{
			tagManager.TryCreateCube( arrGUID[i] );
		}

		quuppaConnect.Initialize ();
	}
	
	private string[] FindAllQuuppaId (string text) 
	{
		List<string> connectedTags = new List<string>(); 
		// The token id
		string id = "id";    
		// Storing the text file into a local string
		string str = text;                          
		// offset values for string manipulation
		int offset = 6; 
		int length = 12;            
		
		while(true)
		{
			int index = str.IndexOf(id);            // Looking for the token id
			if(index < 0) break;                    // if not found break the loop
			int start = index + offset;             // use offset to get the starting of the GUID
			connectedTags.Add (str.Substring(start,length)); // Isolate the GUID
			str = str.Substring(start + length);    // Cut the string for next search
		}
		tagManager.ConnectedTags = connectedTags;

		return connectedTags.ToArray ();
	}
}
