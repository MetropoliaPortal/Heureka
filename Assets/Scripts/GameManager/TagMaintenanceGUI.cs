using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Manager class. 
/// Is attached to the Scripts object 
/// It controlled most of the GUI (Start, Quit, Timer, file maintenance)
/// </summary>
public class TagMaintenanceGUI : QuuppaGUIStateManager
{

	private Rect r_rect;
	private Rect r_restartRect;
	private List<string> m_tagList = new List<string>();
	private string[] GUIStrings = {"Resume","Add","Remove All","Remove","Quit"};
	private string addIdFieldText;
	private Vector2 scrollPosition = Vector2.zero;
	private string st_id;

	private TagManager tagManager;
	
	#region UNITY_METHODS
	void Awake()
	{
		e_state = State.Running;
		//Screen.SetResolution(3840, 1200, false);
	}
	void Start () 
	{
		float size = Screen.width / 10f;
		float midWidth = Screen.width / 2f - size / 2f;
		r_rect = new Rect(midWidth, 0, size, size / 2f);
		r_restartRect = new Rect(midWidth, Screen.height / 2 - size/4, size, size / 2);
	}

	void Update () 
	{
		if(Input.GetKeyDown (KeyCode.M)) e_state = State.GUIMenu;

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
		Rect rect = new Rect(halfWidth - sizeBox / 2, halfHeight - sizeBox / 2, sizeBox, sizeBox);

		GUILayout.BeginArea(rect);

		// Resume
		if(GUILayout.Button (GUIStrings[0]))
		{
			//todo
		}

		// Add
		GUILayout.BeginHorizontal ();
		addIdFieldText = GUILayout.TextField ( addIdFieldText );
		if(GUILayout.Button (GUIStrings[1]))
		{
			//todo
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.BeginScrollView (scrollPosition);
		CreateTagList ();
		GUILayout.EndScrollView ();
		GUILayout.EndHorizontal ();

		//Remove all
		if(GUILayout.Button (GUIStrings[3]))
		{
			//todo
		}

		//Quit
		if(GUILayout.Button (GUIStrings[4]))
		{
			Application.Quit ();
		}

		GUILayout.EndArea ();
	}

	private void CreateTagList()
	{

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
	
	#region GUI_MAINTENANCE_TAG
	private void GUIRemoveDeficientTag()
	{
		GUI.BeginGroup(new Rect());
		GUI.Box (new Rect(), "Do you wish to remove the tag "+st_id+"?");
		if(GUI.Button (new Rect(), "Yes"))
		{
			i_removeTag = 0;
		}
		if(GUI.Button (new Rect(), "No"))
		{
			i_removeTag = 1;
		}
		GUI.EndGroup();
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
	#endregion

	#region METHODS
	private Dictionary<string, GameObject>m_deficientTag = null;
	/// <summary>
	/// Adds the deficient tag to the list.
	/// </summary>
	public void AddDeficientTag(string quuppaTag,GameObject obj)
	{
		if(m_deficientTag == null)m_deficientTag = new Dictionary<string,GameObject>();
		m_deficientTag.Add (quuppaTag, obj);
	}
	/// <summary>
	/// Determines whether deficient tags have been found
	/// </summary>
	/// <returns><c>true</c> if the list is empty <c>false</c> if the list contains deficient tag(s)</returns>
	public bool IsDeficientTagEmpty()
	{
		return m_deficientTag.Count == 0;
	}
	
	private int i_removeTag = -1;
	public IEnumerator QuuppaDeficientRemoval()
	{
		
		foreach(KeyValuePair<string , GameObject> kvp in m_deficientTag)
		{
			// for each deficient tag
			// Get the cube
			GameObject obj = kvp.Value;
			st_id = kvp.Key;
			// Change it to red
			obj.renderer.material.color = Color.red;
			while(i_removeTag == -1)yield return null;
			// Remove the tag and cube
			if(i_removeTag == 0)
			{
				Destroy (obj);
				m_tagList.Remove(st_id);
			}
			i_removeTag = -1;
		}
	} 
	// Look for the tag in file, remove tag if existing and remove from the tag list
	private void RemoveTag( /*string tagID*/)
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

	private void AddTag()
	{
		//string url = @"..\tagFile.txt";	
		string url = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		url += @"\tagFile.txt";	

		using (StreamWriter sw = File.AppendText(url))
		{
			//string newTag = "id:"+st_id+","+selectionStrings[selectionGridInt]+";";
			//sw.WriteLine(newTag); 
			//AddToTagList(newTag);
		}
	}

	public void AddToTagList(string quuppaTag)
	{
		m_tagList.Add (quuppaTag);
	}
	public bool ListContainsTag(string quuppaTag)
	{
		return m_tagList.Contains (quuppaTag);
	}
	public void SetTagRemoval(string quuppaTag)
	{
		st_id = quuppaTag;
	}
	#endregion

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


