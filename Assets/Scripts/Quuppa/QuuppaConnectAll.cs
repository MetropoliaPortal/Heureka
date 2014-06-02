using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class QuuppaConnectAll : MonoBehaviour 
{
	/// <summary>
	/// The frequency of the calls for the request to the server 
	/// </summary>
	public float callFrequency = 0.00f;
	public ThreadPriority connectionThreadPriority;
	
	private string tagQuuppa;
	private QuuppaData tagInfo;
	
	private string haipFileUrl = "192.168.123.124:8080/qpe/getHAIPLocation";	
	private string tagFileUrl = "192.168.123.124:8080/qpe/getTagInfo";
	
	//for parsing
	//haip file
	private const string s_positionX = "smoothedPositionX";
	private const string s_positionY = "smoothedPositionY";
	private const string s_positionZ = "smoothedPositionZ";
	private const string s_id = "id\"";
	private const string s_positionAccuray = "positionAccuracy";

	//tag file
	private const string s_acceleration = "acceleration";
	private const string s_tagState = "tagState\"";
	private const string s_batteryVoltage = "batteryVoltage\"";
	private const int s_offsetExtraChar = 3;				// int for offsetting index when getting data
	// This removes the extra ": " characters placed before the values
	// smoothedPositionX": "0.10",
	private const int s_offsetGetData = 4;  				// int to define precision of the value
	// smoothedPositionX": "0.25252526532" only 4 digits are considered
	private const string s_quotation = "\"";					// string for quotation to avoid creation of new string each round
	private const string s_coma = ",";
	private const string s_letter = "d";					// string for d letter to avoid creation of new string each round
	
	private TagManager tagManager;

	public void Initialize () 
	{
		tagManager = GetComponent<TagManager> ();
		StartCoroutine(GetHAIPFile());
		StartCoroutine(GetTagInfoFile());
	}
	
	private IEnumerator GetHAIPFile()
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
				string txt = haipFileAccess.text;

				while(true)
				{
					int index = txt.IndexOf(s_id);
					if(index < 0) break;
					index = txt.IndexOf(s_id) + s_id.Length + 3;
					string quuppaId = txt.Substring(index, 12);
					//tarviiko try catch?
					Vector3 pos = ParsePosition( txt );

					float posAccuracy = ParsePositionAccuracy( txt );
					//print ( id +", posacc: " +posAccuracy);

					try
					{
						QuuppaData quuppaData = tagManager.QuuppaDataDictionary[quuppaId];
						quuppaData.Position = pos;
						quuppaData.HeightQuuppa = pos.y;
						quuppaData.PositionAccuracy = posAccuracy;
					}
					catch( Exception e)
					{
						Debug.LogError( "Quuppa tag not registered: " +quuppaId);
						Debug.LogError( e.Message );
					}
					int start = index + 6;            
					txt = txt.Substring(start + 12);
				}
				//print ("solved////////////////////////////");
			}
		}
	}
	
	private QuuppaData ParseNextTagInfo( string jsonTxt )
	{
		int index = jsonTxt.IndexOf(s_id) + s_id.Length + 3;
		string id = jsonTxt.Substring(index, 12);

		return null;
	}
	
	// Try/Catch needed since some of the data come sometimes as 0 without any extra information
	// As a result, the parsed info contained a erroneous data like 0,0.2 instead of 0.000
	private Vector3 ParsePosition( string jsonTxt )
	{
		try
		{
			float x = GetFloatFromJson(s_positionX, jsonTxt);
			float y = GetFloatFromJson(s_positionY, jsonTxt);
			float z = GetFloatFromJson(s_positionZ, jsonTxt);

			return new Vector3(x, z, y);
		}
		catch(Exception e)
		{
			Debug.LogError(e.Message);
		}
		return Vector3.zero;
	}
	
	private float ParsePositionAccuracy( string jsonTxt )
	{
		float posAccuracy = 0.0f;
		try
		{
			posAccuracy = GetFloatFromJson(s_positionAccuray, jsonTxt);
		}
		catch(Exception e)
		{
			Debug.LogError(e.Message);
		}
		return posAccuracy;
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
			}
			else
			{
				string txt = tagFileAccess.text;

				while(true)
				{
					int index = txt.IndexOf(": {");
					if(index < 0) break;

					index = txt.IndexOf(": {") - 12 - 1;
					string quuppaId = txt.Substring(index, 12);

					if( tagManager.QuuppaDataDictionary.ContainsKey( quuppaId ) )
					{
						string batteryAlarm = ParseBatteryAlarm( txt );
						Vector3 acceleration = ParseAcceleration( txt );
						string tagState = ParseTagState( txt );
						float batteryVoltage = ParseBatteryVoltage( txt );

						QuuppaData quuppaData = null;
						try
						{
							quuppaData = tagManager.QuuppaDataDictionary[ quuppaId ];
							quuppaData.Acceleration = acceleration;
							quuppaData.TagState = tagState;
							quuppaData.BatteryVoltage = batteryVoltage;
						}
						catch( Exception e)
						{
							Debug.LogError( e.Message );
							print (quuppaData);
							Debug.LogError( "missing key: " +quuppaId);
						}
					}
					int start = txt.IndexOf("}");            
					txt = txt.Substring(start + 1);
				}
			}
		}
	}
	
	private string ParseTagState(string jsonTxt)
	{
		int indexTag = jsonTxt.IndexOf (s_tagState);
		indexTag += s_tagState.Length + 3;
		return jsonTxt.Substring(indexTag,1);
	}
	
	private Vector3 ParseAcceleration(string jsonTxt)
	{
		string txt = jsonTxt;
		Vector3 acceleration = new Vector3();

		try
		{
			int index = txt.IndexOf(s_acceleration) + s_acceleration.Length + 4 ;
			// Isolate the acceleration data
			txt = txt.Substring(index);
			//print (txt);
			
			int token = txt.IndexOf(s_quotation);
			txt = txt.Substring(0, token);
			
			//print (txt);
			
			// Get first value
			token = txt.IndexOf (s_coma);
			string accelX = txt.Substring(0,token );
			txt = txt.Substring(token + 1);
			// Get second value
			token = txt.IndexOf (s_coma);
			string accelY = txt.Substring(0,token );
			txt = txt.Substring(token + 1);
			// Get third value
			string accelZ = txt;


			acceleration.x = float.Parse(accelX);
			acceleration.y = float.Parse (accelY);
			acceleration.z = float.Parse (accelZ);
		}
		catch(Exception e)
		{
			Debug.LogError (e.Message);
		}

		return acceleration;
	}
	
	private float ParseBatteryVoltage( string txt )
	{
		float batteryVoltage = 0.0f;
		try
		{
			batteryVoltage = GetFloatFromJson(s_batteryVoltage, txt);
		}
		catch(Exception e)
		{
			Debug.LogError(e.Message);
		}
		return batteryVoltage;
	}

	private string ParseBatteryAlarm( string txt )
	{
		return null;
	}

	private float GetFloatFromJson(string name, string file)
	{
		int index = file.IndexOf(name) + name.Length + s_offsetExtraChar;
		string pos = file.Substring(index, s_offsetGetData);
		
		return float.Parse(pos);
	}
}

