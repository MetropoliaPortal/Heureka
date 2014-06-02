using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System;

public enum BuildingType
{
	Official, Residential, Leisure, Shop
}

public class TagManager : MonoBehaviour 
{
	public Dictionary<string, QuuppaData> QuuppaDataDictionary{ get; private set;}
	public Dictionary<string, TagData> TagDataDictionary{ get; private set;}
	public List<string> ConnectedTags{ get; set;}
	private CubeCreator _cubeCreator;

	void Awake()
	{
		QuuppaDataDictionary = new Dictionary<string, QuuppaData> ();
		TagDataDictionary = new Dictionary<string, TagData> ();
		ConnectedTags = new List<string> ();
		_cubeCreator = GetComponent<CubeCreator> ();
		ParseTagFile();
	}

	public void TryCreateCube(string quuppaId)
	{
		if (TagDataDictionary.ContainsKey (quuppaId)) 
		{
			QuuppaData newQuuppaData = _cubeCreator.CreateCube( quuppaId );
			//QuuppaDataDictionary.Remove( quuppaId );
			QuuppaDataDictionary.Add( quuppaId, newQuuppaData );
		}
		else
		{
			Debug.LogError("Tag not found from TagFile: " +quuppaId);
		}
	}

	private void ParseTagFile()
	{
		string key_referenceId = "refId:";
		string key_quuppaId = "quuppaId:";
		//string token = ","; 
		string endToken = ";";			
		int refIdValueLength = 2;
		int quuppaIdValueLength = 12;

		try
		{
			string url = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			url += @"\tagFile.txt";	
			
			using (StreamReader sr = new StreamReader(url))
			{
				string file = sr.ReadToEnd();
				if(file.Length == 0)
				{
					Debug.LogError("tagFile length is 0");
				}

				while(true)
				{
					
					// Get the index of "id"
					//int index = file.IndexOf(quuppaId);
					int index = file.IndexOf(key_referenceId);
					// if "id" not found break loop	
					if(index < 0) break;								

					string refId = "";
					string quuppaId = "";
					string buildingType = "";

					index += key_referenceId.Length;
					refId = file.Substring(index, refIdValueLength);
					file = file.Substring(index + refIdValueLength + 1);

					// move the index to the starting of the id	
					index = 0;
					index += key_quuppaId.Length;	
					// get the id string
					quuppaId = file.Substring(index, quuppaIdValueLength);	
					// Remove the used part of the file including the middle coma 
					file = file.Substring(index + quuppaIdValueLength + 1);
					// get the index of the end of line coma 
					index = file.IndexOf(endToken);
					// isolate the string containing the type
					buildingType = file.Substring(0,index);
					// remove the used part of the file including the final coma
					file = file.Substring(index + 1);

					print("id: " +refId +", quuppa: " +quuppaId +", bt: " +buildingType);

					TagData newTagData = new TagData( refId, quuppaId );
					try
					{
						BuildingType bt = (BuildingType)System.Enum.Parse(typeof(BuildingType), buildingType);
						newTagData.BuildingType = bt;
					}
					catch(System.Exception e)
					{
						Debug.Log("Error parsing building type");
					}

					//TODO: default acceleration from tagfile
					newTagData.DefaultAcceleration = Vector3.zero;
					TagDataDictionary.Add( quuppaId, newTagData );
					//QuuppaDataDictionary.Add( quuppaId, null );
				}
			}
		}
		catch (Exception e)
		{
			print(e.Message);
		}
	}
	
	public void AddNewTag(string refId, string quuppaId)
	{
		TagData newTagData = new TagData( refId, quuppaId );
		newTagData.BuildingType = BuildingType.Leisure;
		newTagData.DefaultAcceleration = Vector3.zero;
		TagDataDictionary.Add( quuppaId, newTagData );
		QuuppaDataDictionary.Add( quuppaId, null );
		if(ConnectedTags.Contains(quuppaId))
			TryCreateCube (quuppaId);
		TagFileHandler.UpdateTagFile ( TagDataDictionary );
	}

	//TODO: get mean of multiple values for default 
	public void ConfigureTag(string quuppaId)
	{
		TagDataDictionary [quuppaId].DefaultAcceleration = QuuppaDataDictionary [quuppaId].Acceleration;
		//StartCoroutine (GetDefaultAcceleration ());
	}

	public void ChangeBuildingType(string quuppaId)
	{
		if( (int)(TagDataDictionary[quuppaId].BuildingType) < 3 )
		{
			TagDataDictionary[quuppaId].BuildingType++;
		}
		else
		{
			TagDataDictionary[quuppaId].BuildingType = 0;
		}
		UpdateTagFile ();
	}

	public void RemoveTags(List<string> tagsToRemove)
	{
		foreach( string tag in tagsToRemove)
		{
			if( QuuppaDataDictionary[ tag ] != null )
			{
				Destroy ( QuuppaDataDictionary[ tag ].gameObject);
			}
			QuuppaDataDictionary.Remove( tag );
			TagDataDictionary.Remove( tag );
		}

		tagsToRemove.Clear ();
		UpdateTagFile ();
	}
	
	public void RemoveAll()
	{
		foreach(KeyValuePair<string, QuuppaData> tag in QuuppaDataDictionary)
		{
			if( tag.Value != null )
			{
				Destroy ( tag.Value.gameObject);
			}
		}

		QuuppaDataDictionary.Clear();
		TagDataDictionary.Clear();
		UpdateTagFile ();
	}

	public void HighlightCube(string quuppaId)
	{
		QuuppaDataDictionary[quuppaId].gameObject.GetComponent<CubeHighlight>().ChangeHighlight();
	}

	public void UpdateTagFile()
	{
		TagFileHandler.UpdateTagFile ( TagDataDictionary );
	}
}
