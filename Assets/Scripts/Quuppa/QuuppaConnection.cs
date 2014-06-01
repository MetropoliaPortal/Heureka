using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class QuuppaConnection : MonoBehaviour 
{
	/// <summary>
	/// The frequency of the calls for the request to the server 
	/// </summary>
	public float callFrequency = 0.00f;
	public ThreadPriority connectionThreadPriority;

	private string tagQuuppa;
	private QuuppaData tagInfo;

	private string haipFileUrl;	
	private string tagFileUrl;

	//for parsing
	private const string s_positionX = "smoothedPositionX";
	private const string s_positionY = "smoothedPositionY";
	private const string s_positionZ = "smoothedPositionZ";
	private const string s_acceleration = "acceleration";
	private const string s_tagState = "tagState\"";
	private const string s_positionAccuray = "positionAccuracy";
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

	private TagManager tagManager;

	//quuppa debug
	private int errorAmount = 0;
	private int successAmount = 0;

	void Start()
	{

	}
	
	public void Initialize (string tag) 
	{
		tagQuuppa = tag;
		tagInfo = GetComponent<QuuppaData>();
		tagManager = GetComponent<TagManager> ();

        haipFileUrl = "192.168.123.124:8080/qpe/getHAIPLocation?tag=" + tagQuuppa;
		tagFileUrl = "192.168.123.124:8080/qpe/getTagInfo?tag=" + tagQuuppa;
		StartCoroutine(GetHAIPFile());
		StartCoroutine(GetTagInfoFile());
	}

	//initialization for single file
	/*
	public void Initialize () 
	{
		tagQuuppa = tag;
		tagInfo = GetComponent<TagInfo>();
		
		haipFileUrl = "192.168.123.124:8080/qpe/getHAIPLocation";
		tagFileUrl = "192.168.123.124:8080/qpe/getTagInfo";
		StartCoroutine(GetHAIPFile());
		StartCoroutine(GetTagInfoFile());
	}
	*/

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

			WWW haipFileAccess = new WWW(haipFileUrl);
			haipFileAccess.threadPriority = connectionThreadPriority;

			yield return haipFileAccess;

			if(haipFileAccess.error != null)
			{
				Debug.LogError("Error with HAIP file connection: " +haipFileAccess.error);
			}
           	else
            {
				SolvePosition( haipFileAccess.text );
				/*
				int idx = 0;
				while(true)
				{
					string txt = SolveNextJsonEntity();
					if( idx == 0 ) break;
					TagInfo nextTagInfo = SolveNextTagInfo();
					SolvePosition( haipFileAccess.text );	
					SolvePositionAccuracy();
				}
				*/
            }
        }
	}

	private string SolveNextJsonEntity()
	{
		//todo
		return null;
	}

	private QuuppaData SolveNextTagInfo()
	{
		//todo
		return null;
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

			tagInfo.Position = new Vector3(x, z, y);
			tagInfo.HeightQuuppa = z;
		}
		catch(Exception e)
		{
			Debug.LogError(e.Message);
		}
	}

	private void SolvePositionAccuracy()
	{
		//todo
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
			tagFileAccess.threadPriority = connectionThreadPriority;

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

			tagInfo.Acceleration = acceleration;
		}
		catch(Exception e)
		{
			print (e.Message);
		}
	}

	private void SolveBatteryVoltage()
	{
		//todo
	}
}
