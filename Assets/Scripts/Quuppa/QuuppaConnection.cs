using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


[RequireComponent(typeof(CubePosition))]
[RequireComponent(typeof(CubeRotation))]
public class QuuppaConnection : MonoBehaviour 
{
	

	/// <summary>
	/// The frequency of the calls for the request to the server 
	/// </summary>
	public float callFrequency = 0.00f;

	private string tagQuuppa;
	private TagInfo tagInfo;

	private string haipFileUrl;	
	private string tagFileUrl;

	// All data are marked as const to make them immutable and static 
	private const string s_positionX = "smoothedPositionX";	// string for X position parsing
	private const string s_positionY = "smoothedPositionY";	// string for Y position parsing
	private const string s_positionZ = "smoothedPositionZ";	// string for Z position parsing
	private const string s_acceleration = "acceleration"; 	// string for acceleration parsing
	private const string s_tagState = "tagState\"";			// string for tag state parsing
	private const int s_offsetExtraChar = 3;				// int for offsetting index when getting data
															// This removes the extra ": " characters placed before the values
	           												// smoothedPositionX": "0.10",
	private const int s_offsetGetData = 4;  				// int to define precision of the value
															// smoothedPositionX": "0.25252526532" only 4 digits are considered
	private const string s_quotation = "\"";					// string for quotation to avoid creation of new string each round
	private const string s_coma = ",";
	private const string s_letter = "d";					// string for d letter to avoid creation of new string each round
															// defining whether the tag state is default or triggered
	private string tagState;

	//quuppa debug
	private int errorAmount = 0;
	private int successAmount = 0;
	
	public void Initialize (string tag) 
	{
		tagQuuppa = tag;
		tagInfo = GetComponent<TagInfo>();

        haipFileUrl = "192.168.123.124:8080/qpe/getHAIPLocation?tag=" + tagQuuppa;
		tagFileUrl = "192.168.123.124:8080/qpe/getTagInfo?tag=" + tagQuuppa;
		StartCoroutine(GetHAIPFile());
		StartCoroutine(GetTagInfoFile());
	}

	IEnumerator GetHAIPFile()
	{
		while(true)
		{
			// Control the frequency calls
			float timer = 0f;
			while(timer < callFrequency)
			{
				timer += Time.deltaTime;
				yield return null;
			}

			//if(tagState == s_letter)
				//continue;

			// Access the url for request
			WWW haipFileAccess = new WWW(haipFileUrl);
			yield return haipFileAccess;

			if(haipFileAccess.error != null)
			{
				Debug.LogError("Error with HAIP file connection: " +haipFileAccess.error);
			}
           	else
            {
				SolvePosition( haipFileAccess.text );
            }
        }
	}

	// Try/Catch needed since some of the data come sometimes as 0 without any extra information
	// As a result, the parsed info contained a erroneous data like 0,0.2 instead of 0.000
	private void SolvePosition( string jsonTxt)
	{
		try
		{
			float x = GetFloatFromJson(s_positionX, jsonTxt);
			float y = GetFloatFromJson(s_positionY, jsonTxt);
			float z = GetFloatFromJson(s_positionZ, jsonTxt);
			
			//cubePosition.SolvePosition(x, z, y);
			tagInfo.Position = new Vector3(x, y, z);
			tagInfo.HeightQuuppa = y;
		}
		catch(Exception e)
		{
			Debug.LogError(e.Message);
		}
	}

	private float GetFloatFromJson(string name, string file)
	{
		int index = file.IndexOf(name) + name.Length + s_offsetExtraChar;
		string pos = file.Substring(index, s_offsetGetData);

		return float.Parse(pos);
	}
	
	private IEnumerator GetTagInfoFile()
	{
		while(true)
		{
			float timer  = 0;
			while(timer < callFrequency)
			{
				timer += Time.deltaTime;
				yield return null;
			}

			WWW tagFileAccess = new WWW(tagFileUrl);
			yield return tagFileAccess;

			if( tagFileAccess.error != null)
			{
				Debug.LogError("Error with tagInfoFile connection: " +tagFileAccess.error +", tag: " +tagQuuppa);
				errorAmount++;

				//print ("errors " +tagQuuppa +": " +errorAmount);
				//print ("successes: " +successAmount);
			}
			else
			{
				string jsonText = tagFileAccess.text;
				successAmount++;

				SolveTagState( jsonText );
				SolveAcceleration( jsonText );

			}
		}
	}

	private void SolveTagState(string jsonTxt)
	{
		int indexTag = jsonTxt.IndexOf (s_tagState);
		indexTag += s_tagState.Length + 3;
		tagState = jsonTxt.Substring(indexTag,1);

		//cubePosition.SetTagState( tagState );
		tagInfo.TagState = tagState;
	}

	private void SolveAcceleration(string jsonTxt)
	{
		Vector3 acceleration = new Vector3();

		int index = jsonTxt.IndexOf(s_acceleration) + s_acceleration.Length + 4 ;
		
		// Isolate the acceleration data
		jsonTxt = jsonTxt.Substring(index);
		int token = jsonTxt.IndexOf(s_quotation);
		jsonTxt = jsonTxt.Substring(0, token);
		
		// Get first value
		token = jsonTxt.IndexOf (s_coma);
		string accelX = jsonTxt.Substring(0,token );
		jsonTxt = jsonTxt.Substring(token + 1);
		// Get second value
		token = jsonTxt.IndexOf (s_coma);
		string accelY = jsonTxt.Substring(0,token );
		jsonTxt = jsonTxt.Substring(token + 1);
		// Get third value
		string accelZ = jsonTxt;
		
		try
		{
			acceleration.x = float.Parse(accelX);
			acceleration.y = float.Parse (accelY);
			acceleration.z = float.Parse (accelZ);
			
			//cubeRotation.ProcessRotation(acceleration);
			tagInfo.Acceleration = acceleration;
		}
		catch(Exception e)
		{
			print (e.Message);
		}
	}
}
