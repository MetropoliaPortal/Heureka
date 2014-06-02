using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class TagMaintenanceGUI : QuuppaGUIStateManager
{

	private Rect r_rect;
	private Rect r_restartRect;
	private string[] GUIStrings = {"Resume","Add","Remove All","Remove All","Quit"};
	private string addQuuppaIdInput = "";
	private string addRefIdInput = "";
	private Vector2 scrollPosition = Vector2.zero;
	private List<string> toRemove = new List<string> ();

	private TagManager tagManager;
	
	#region UNITY_METHODS
	void Awake()
	{
		e_state = State.Running;
	}
	void Start () 
	{
		tagManager = GetComponent<TagManager>();

		float size = Screen.width / 10f;
		float midWidth = Screen.width / 2f - size / 2f;
		r_rect = new Rect(midWidth, 0, size, size / 2f);
		r_restartRect = new Rect(midWidth, Screen.height / 2 - size/4, size, size / 2);
	}

	void Update () 
	{
		if(Input.GetKeyDown (KeyCode.M)) e_state = State.GUIMenu;

		//TODO:directx thing
		if(Input.GetKeyDown (KeyCode.F))
			Screen.fullScreen = !Screen.fullScreen;
	}

	void OnGUI()
	{
		if(e_state == State.GUIMenu)
		{
			GUIMainBoard();
		}
	}
	#endregion
	
	private void GUIMainBoard()
	{
		float halfWidth = Screen.width / 2;
		float halfHeight = Screen.height / 2;
		float sizeBox = halfWidth / 2;

		float widthMargin = 0;//Screen.width / 10;
		float heightMargin = 0;//Screen.height / 10;
		Rect rect = new Rect(  widthMargin, 
		                     heightMargin, 
		                     Screen.width - 2 * widthMargin, 
		                     Screen.height - 2 * heightMargin );

		GUILayout.BeginArea(rect);

		// Resume
		if(GUILayout.Button (GUIStrings[0]))
		{
			e_state = State.Running;
		}

		// Add
		GUILayout.BeginHorizontal ();
		addRefIdInput = GUILayout.TextField ( addRefIdInput );
		addQuuppaIdInput = GUILayout.TextField ( addQuuppaIdInput );
		if(GUILayout.Button (GUIStrings[1]))
		{
			if ( CheckProperInput() )
			{
				tagManager.AddNewTag(addRefIdInput, addQuuppaIdInput);
			}
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		scrollPosition = GUILayout.BeginScrollView (scrollPosition);
		CreateTagList ();
		GUILayout.EndScrollView ();
		GUILayout.EndHorizontal ();

		//Remove all
		if(GUILayout.Button (GUIStrings[3]))
		{
			tagManager.RemoveAll();
		}
		//Quit
		if(GUILayout.Button (GUIStrings[4]))
		{
			Application.Quit ();
		}

		GUILayout.EndArea ();
	}

	private bool CheckProperInput()
	{
		return addRefIdInput.Length == 2 && addQuuppaIdInput.Length == 12;
	}

	private void CreateTagList()
	{
		Dictionary<string,TagData> tagDatas = tagManager.TagDataDictionary;
		Dictionary<string,QuuppaData> quuppaDatas = tagManager.QuuppaDataDictionary;

		GUILayout.BeginHorizontal();
		
		GUILayout.Label( "Ref ID" );
		GUILayout.Label( "Quuppa ID" );
		GUILayout.Label( "Battery Voltage" );
		GUILayout.Label( "Position" );
		GUILayout.Label( "Acceleration" );
		GUILayout.Label( "Building Type" );
		GUILayout.Label( "" );
		GUILayout.Label( "" );
		
		GUILayout.EndHorizontal();

		foreach( KeyValuePair<string, TagData> tag in tagDatas)
		{
			GUILayout.BeginHorizontal();

			if( quuppaDatas.ContainsKey( tag.Key ) )
			{
				if( GUILayout.Button( tag.Key ))
				{
					tagManager.HighlightCube( tag.Key );
				}
				GUILayout.Label( tag.Value.QuuppaId );
				GUILayout.Label( quuppaDatas[tag.Key].BatteryVoltage.ToString() );
				GUILayout.Label( quuppaDatas[tag.Key].Position.ToString() );
				GUILayout.Label( quuppaDatas[tag.Key].Acceleration.ToString() );
				if( GUILayout.Button( tag.Value.BuildingType.ToString() ) )
				{
					tagManager.ChangeBuildingType( tag.Key );
				}
				if( GUILayout.Button( "Configure") ) 
				{
					tagManager.ConfigureTag( tag.Key );
				}
				if( GUILayout.Button( "Remove") ) 
				{
					toRemove.Add( tag.Key );
				}
			}
			else
			{
				GUILayout.Label( "Offline ");
				GUILayout.Label( tag.Key );
				GUILayout.Label( "" );
				GUILayout.Label( "" );
				GUILayout.Label( "" );
				if( GUILayout.Button( tag.Value.BuildingType.ToString() ) )
				{
					tagManager.ChangeBuildingType( tag.Key );
				}
				GUILayout.Label( "" );
				if( GUILayout.Button( "Remove") ) 
				{
					toRemove.Add( tag.Key );
				}
			}
			GUILayout.EndHorizontal();
		}
		if(toRemove.Count != 0)
			tagManager.RemoveTags( toRemove );
	}

	// If the game is over, display commands to restart and start the game
	private string[] st_startRestart = {"RESTART", "START"};
	private void GUIStartGame()
	{
		if(e_state == State.Postgame)
		{
			if(GUI.Button (r_restartRect, st_startRestart[0]))
			{
				GC.Collect ();
				e_state = State.StartMenu;
				GameObject.Find("Screens").GetComponent<ScreenTexture>().SetValue();
			}
		}
		else if(e_state == State.StartMenu)
		{
			if(GUI.Button (r_restartRect, st_startRestart[1]))
			{
				e_state = State.Running;

			}
		}
	}

	private string[]s_errorMessages = {"Id must be 12 characters", "Tag GUID does not exist"};
	private int i_errorIndex = -1;
	private TagState e_prevState;
	private void GUITagError()
	{
		float halfWidth = Screen.width / 2f;
		float sizeBox = halfWidth / 2;
		float height = 50f; 
		if(GUI.Button (new Rect(0, sizeBox / 2 - 25, sizeBox, height), s_errorMessages[i_errorIndex]))
		{
		}
	}

	private string[]st_confirmMessages = {"Are you sure \nyou want to add the tag?", "Are you sure \nyou want to remove the tag?"};
	private ConfirmState e_confirmState = ConfirmState.None;
	
	// Look for the tag in file, remove tag if existing and remove from the tag list

	//private void RemoveTag( /*string tagID*/)
		/*
	{
		//string url = @"..\tagFile.txt";
		string url = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		url += @"\tagFile.txt";	

		string file= File.ReadAllText(url); 
		string endtoken = ";";
		int startIndex = file.IndexOf (st_id); 	// Find index of the tag
		startIndex -= 3;						// remove the "id:" part
		string temp = file.Substring(startIndex);// remove all parts before tag and place it in temp
		int endIndex = temp.IndexOf(endtoken);	
		file = file.Remove(startIndex , endIndex + 1);
		file = Regex.Replace(file, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
		File.WriteAllText(url,file);
	}
	*/

	#region ENUM
	enum TagState
	{
		Front,
		Add,
		Remove,
		Error,
		Confirm,
		Deficient
	}
	enum ConfirmState
	{
		None, Add, Remove
	}
	#endregion
}


