using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class TagFileHandler : MonoBehaviour 
{
	public static void UpdateTagFile( Dictionary<string, TagData> tagDatas)
	{
		string url = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		url += @"\tagFile.txt";	

		using (StreamWriter sw = File.CreateText(url))
		{
			foreach(KeyValuePair<string, TagData> tagInfo in tagDatas)
			{
				string tag = "refId:" +tagInfo.Value.RefId +",quuppaId:" +tagInfo.Value.QuuppaId +"," +tagInfo.Value.BuildingType.ToString() +";";
				sw.WriteLine( tag ); 
			}
		}
	}
}
