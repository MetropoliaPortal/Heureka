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
public class ConnectScript : MonoBehaviour {

	/// <summary>
	/// The Quuppa tag
	/// Only needed to see the tag of the cube in the Inspector
	/// </summary>
	public string tagQuuppa;
	/// <summary>
	/// The frequency of the calls for the request to the server 
	/// </summary>
	public float callFrequency = 0.2f;
	
	private CubePosition m_cubePosition;		// Reference to the CubePosition script attached to that object
	private CubeRotation m_rotation;			// Reference to the CubeRotation script attached to that object
	private string st_urlPosition;				// The url request for position
	private string st_urlAccel;					// The url request for acceleration
	private Vector3 v_prevPosition = new Vector3();		// Store previous positions to discard noise
	private bool b_continue = true;				// Used to define if the acceleration has changed to cancel useless calls

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


	/// <summary>
	/// Initializes this instance.
	/// The method is called from the StartScript.cs. 
	/// The other needed components are passed as parameters
	/// </summary>
	public void Init (CubePosition cubePosition, CubeRotation cubeRotation, string tagQuuppa) 
	{
		this.tagQuuppa = tagQuuppa;
		m_cubePosition = cubePosition;
		m_rotation = cubeRotation;

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
			// If acceleration has not changed, the cube was not moved, we quit this round of computation 
			if(!b_continue)continue;

			// Access the url for request
			WWW www = new WWW(st_urlPosition);
			yield return www;
            
            if (www.error == null)
            {
				// Parse data
				// "smoothedPositionX" 
                int indexX = www.text.IndexOf(s_positionX) + s_positionX.Length + s_offsetExtraChar;
				// "smoothedPositionY"
                int indexY = www.text.IndexOf(s_positionY) + s_positionY.Length + s_offsetExtraChar;
				// s_positionZ = "smoothedPositionZ"
				int indexZ = www.text.IndexOf(s_positionZ) + s_positionY.Length + s_offsetExtraChar;

				// Get 4 values precision
                string posX = www.text.Substring(indexX, s_offsetGetData);
                string posY = www.text.Substring(indexY, s_offsetGetData);
				string posZ = www.text.Substring(indexZ, s_offsetGetData);

				// Try/Catch needed since some of the data come somtimes as 0 without any extra information
				// As a result, the parsed info contained a erroneous data like 0,0.2 instead of 0.000
				try
				{
                	float x = float.Parse(posX);
                	float y = float.Parse(posY);
					float z = float.Parse(posZ);
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
	IEnumerator GetTagInfoFile()
	{
		while(true){
			float timer  = 0;
			while(timer < callFrequency)
			{
				timer += Time.deltaTime;
				yield return null;
			}

			WWW wwwAccel = new WWW(st_urlAccel);
			yield return wwwAccel;
			Vector3 acceleration = new Vector3();
			if (wwwAccel.error == null)
			{
				string accel = wwwAccel.text;

				int indexTag = accel.IndexOf (s_tagState);
				indexTag += s_tagState.Length +3;
				string tagSt = accel.Substring(indexTag,1);
				if(tagSt == s_letter){
					b_continue = false;
					continue;
				}else{
					b_continue = true;
				}

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
