
using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// ConnectScript.cs
/// Component is added from StartScript.cs
/// The purpose is to request from the server for the two files.
/// The parse needed information and send them to the appropriate scripts
/// 
/// Position information are sent to CubePosition.cs
/// Acceleration information are sent to CubeRotation.cs
/// </summary>
using System.Collections.Generic;


[RequireComponent(typeof(CubePosition))]
[RequireComponent(typeof(CubeRotation))]
public class QuuppaConnection : MonoBehaviour 
{

	/// <summary>
	/// The Quuppa tag
	/// Only needed to see the tag of the cube in the Inspector
	/// </summary>
	public string tagQuuppa;
	/// <summary>
	/// The frequency of the calls for the request to the server 
	/// </summary>
	public float callFrequency = 0.00f;
	
	private CubePosition m_cubePosition;		// Reference to the CubePosition script attached to that object
	private CubeRotation m_rotation;			// Reference to the CubeRotation script attached to that object
	private string st_urlPosition;				// The url request for position
	private string st_urlAccel;					// The url request for acceleration
	private Vector3 v_prevPosition = new Vector3();		// Store previous positions to discard noise

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
		m_cubePosition = GetComponent<CubePosition>();
		m_rotation = GetComponent<CubeRotation>();

        st_urlPosition = "192.168.123.124:8080/qpe/getHAIPLocation?tag=" + this.tagQuuppa;
		st_urlAccel = "192.168.123.124:8080/qpe/getTagInfo?tag=" + this.tagQuuppa;
		StartCoroutine(GetHAIPFile());
		StartCoroutine(GetTagInfoFile());
	}

	IEnumerator GetHAIPFile()
	{
		while(true)
		{
			// Control the frequency calls
			float timer  = 0f;
			while(timer < callFrequency)
			{
				timer += Time.deltaTime;
				yield return null;
			}

			//if(tagState == s_letter)
				//continue;

			// Access the url for request
			WWW www = new WWW(st_urlPosition);
			yield return www;

			if(www.error != null)
			{
				Debug.LogError("Error with HAIP file connection: " +www.error);
			}
           	else
            {
				// Try/Catch needed since some of the data come sometimes as 0 without any extra information
				// As a result, the parsed info contained a erroneous data like 0,0.2 instead of 0.000
				try
				{
					float x = GetFloatFromJson(s_positionX, www.text);
					float y = GetFloatFromJson(s_positionY, www.text);
					float z = GetFloatFromJson(s_positionZ, www.text);

					m_cubePosition.PositionCube(x, z, y);
					v_prevPosition.x = x;
					v_prevPosition.y = y;
					v_prevPosition.z = z;
				}
				catch(Exception)
				{
					// If the exception is caught, one of our value probably has a case of 0 
					// If the value was 0, the parsing has considered the following data
					// We check if a coma is contained in the string
					// If so, we used the previous values
					m_cubePosition.PositionCube(v_prevPosition.x, v_prevPosition.z, v_prevPosition.y);
				}
            }
        }
	}

	float GetFloatFromJson(string name, string file)
	{
		int index = file.IndexOf(name) + name.Length + s_offsetExtraChar;
		string pos = file.Substring(index, s_offsetGetData);

		return float.Parse(pos);
	}


	IEnumerator GetTagInfoFile()
	{
		while(true)
		{
			float timer  = 0;
			while(timer < callFrequency)
			{
				timer += Time.deltaTime;
				yield return null;
			}

			WWW wwwAccel = new WWW(st_urlAccel);
			yield return wwwAccel;


			if( wwwAccel.error != null)
			{
				Debug.LogError("Error with tagInfoFile connection: " +wwwAccel.error +", tag: " +tagQuuppa);
				errorAmount++;

				//print ("errors " +tagQuuppa +": " +errorAmount);
				//print ("successes: " +successAmount);
			}
			else
			{
				successAmount++;
				Vector3 acceleration = new Vector3();

				string accel = wwwAccel.text;

				int indexTag = accel.IndexOf (s_tagState);
				indexTag += s_tagState.Length + 3;
				tagState = accel.Substring(indexTag,1);
				// set tag state in cubeposition
				m_cubePosition.SetTagState( tagState );

				int index = accel.IndexOf(s_acceleration) + s_acceleration.Length + 4 ;

				// Isolate the acceleration data
				accel = accel.Substring(index);
				int token = accel.IndexOf(s_quotation);
				accel = accel.Substring(0, token);
				
				// Get first value
				token = accel.IndexOf (s_coma);
				string accelX = accel.Substring(0,token );
				accel = accel.Substring(token + 1);
				// Get second value
				token = accel.IndexOf (s_coma);
				string accelY = accel.Substring(0,token );
				accel = accel.Substring(token + 1);
				// Get third value
				string accelZ = accel;

				try
				{
					acceleration.x = float.Parse(accelX);
					acceleration.y = float.Parse (accelY);
					acceleration.z = float.Parse (accelZ);

					m_rotation.ProcessRotation(acceleration);
				}
				catch(Exception e)
				{
					print (e.Message);
				}
			}
		}
	}
}
