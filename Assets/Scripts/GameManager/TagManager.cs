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
	public List<TagInfo> tagInfos{get;set;}
	private CubeCreator cubeCreator;
	private Dictionary<string, BuildingType> tagBuildingTypes;
	private Dictionary<string, int> tagReferenceIds;

	void Awake()
	{
		tagInfos = new List<TagInfo>();
		cubeCreator = GetComponent<CubeCreator> ();
		GetBuildingTypes();
	}

	public void AddTag(string quuppaId )
	{
		try
		{
			BuildingType type = tagBuildingTypes [quuppaId];
			TagInfo newTagInfo = cubeCreator.CreateCube( quuppaId, type );
			InitializeTagInfoValues( newTagInfo, quuppaId, type);
			tagInfos.Add( newTagInfo );
		}
		catch(KeyNotFoundException e)
		{
			Debug.LogError(e.Message);
			Debug.LogError("Can not add cube, Missing key: " +quuppaId);
			//add leisure if not found...
			tagInfos.Add( cubeCreator.CreateCube( quuppaId, BuildingType.Leisure ) );
		}
	}

	private void InitializeTagInfoValues(TagInfo newTagInfo, string quuppaId, BuildingType type)
	{
		newTagInfo.QuuppaId = quuppaId;
		newTagInfo.BuildingType = type;
	}

	private void GetBuildingTypes()
	{
		tagBuildingTypes = new Dictionary<string, BuildingType>();
		string id = "id";	
		//string token = ","; 
		string endToken = ";";		
		int offset = 1;		
		int tagLength = 12;

		try
		{
			//string url = @"..\tagFile.txt";	
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
					// Read each line of the file
					// Each line contains the Quuppa tag followed by the corresponding building type
					// id:01234567ac81,Official;
					
					// Get the index of "id"
					int index = file.IndexOf(id);
					// if "id" not found break loop	
					if(index < 0) break;								
					
					string tagQuuppa = "";
					string type = "";
					
					// move the index to the starting of the id	
					index += id.Length + offset;	
					// get the id string
					tagQuuppa = file.Substring(index, tagLength);	
					// Remove the used part of the file including the middle coma 
					file = file.Substring(index +tagLength + 1);
					// get the index of the end of line coma 
					index = file.IndexOf(endToken);
					// isolate the string containing the type
					type = file.Substring(0,index);
					// remove the used part of the file including the final coma
					file = file.Substring(index + 1);			
					
					try
					{
						BuildingType bt = (BuildingType)System.Enum.Parse(typeof(BuildingType), type);
						tagBuildingTypes.Add (tagQuuppa, bt);
					}
					catch(System.Exception e)
					{
						Debug.Log("Error parsing building type");
					}
					
					Debug.Log("tag added: " +tagQuuppa);
				}
			}
		}
		catch (Exception e)
		{
			print(e.Message);
		}
	}

	private void GetReferenceId()
	{
		//todo
	}

	public void ChangeBuildingType(int tagIndex)
	{
		tagInfos[ tagIndex ].BuildingType++;
		if( (int)tagInfos[ tagIndex ].BuildingType >= 4 )
		{
			tagInfos[ tagIndex ].BuildingType = 0;
		}
	}
}
