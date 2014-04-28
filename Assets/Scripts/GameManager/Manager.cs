using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// Manager class. 
/// Is attached to the Scripts object 
/// It controlled most of the GUI (Start, Quit, Timer, file maintenance)
/// </summary>
using System.IO;


public class Manager : GameManager
{
	public int minutes = 0; 
	private int i_minute, i_second;
	private Rect r_rect;
	private Rect r_restartRect;
	private GameManager m_manager; 
	//private string urlDEBUG = @"C:\Users\Lucas\Desktop\tagFile.txt";
	private string urlDEBUG = @"tagFile.txt";

	void Awake()
	{
		e_state = State.GUIMenu;
		//string url = @"..\tagFile.txt";

		if(!File.Exists (urlDEBUG))
		{
			File.Create(urlDEBUG);
		}
	}
	void Start () 
	{
		i_minute = minutes;
		i_second = 0;
		float size = Screen.width / 10f;
		float midWidth = Screen.width / 2f - size / 2f;
		r_rect = new Rect(midWidth, 0, size, size / 2f);
		r_restartRect = new Rect(midWidth, Screen.height / 2 - size/4, size, size / 2);
	}


	// Update is called once per frame
	void Update () 
	{
		// Press G to bring up GUI
		// Mainly for quitting the game
		if(Input.GetKeyDown (KeyCode.G)) e_state = State.GUIMenu;
	}

	void OnGUI()
	{
		if(e_state == State.GUIMenu)
		{
			GUIMainBoard();
		}

		// Display the Timer
		else if(e_state == State.Running)
		{
			GUI.Box (r_rect, i_minute+":"+i_second.ToString("00"));
		}

		else if((e_state & (State.Postgame | State.StartMenu)) > 0 )
		{
			GUICommand ();
		}
		else if(e_state == State.AddTag)
		{
			GUIAddTag();
		}
	}
	#region GUI_METHODS
	// The main GUI for resuming or quitting the game
	private void GUIMainBoard()
	{
		float halfWidth = Screen.width / 2;
		float halfHeight = Screen.height / 2;
		int amountButton = 3;
		float sizeBox = halfWidth / 2;
		Rect rect = new Rect(halfWidth - sizeBox / 2, halfHeight - sizeBox / 2, sizeBox, sizeBox);

		GUI.BeginGroup(rect);
		// Resume button
		if(GUI.Button (new Rect(0,0,sizeBox , sizeBox / amountButton),"Resume"))
		{
			e_state = State.StartMenu;
		}
		// Quit button
		if(GUI.Button (new Rect(0, sizeBox / amountButton,sizeBox , sizeBox / amountButton),"Add Tag"))
		{
			e_state = State.AddTag;
		}
		if(GUI.Button (new Rect(0,sizeBox / amountButton * 2,sizeBox, sizeBox/ amountButton),"Quit"))
		{
			Application.Quit ();
		}
		GUI.EndGroup ();
	}

	// If the game is over, display commands to restart and start the game
	private void GUICommand()
	{
		if(e_state == State.Postgame)
		{
			if(GUI.Button (r_restartRect, "RESTART"))
			{
				GC.Collect ();
				i_minute = minutes;
				i_second = 0;
				e_state = State.StartMenu;
				GameObject.Find("Screens").GetComponent<ScreenTexture>().SetValue();
			}
		}
		else if(e_state == State.StartMenu)
		{
			if(GUI.Button (r_restartRect, "START"))
			{
				e_state = State.Running;
				InvokeRepeating("DecreaseTimer",1.0f,1.0f);
			}
		}
	}

	private TagState e_tagState = TagState.Front;
	string st_id = "Tag Id";
	private int selectionGridInt = 0;
	private string[] selectionStrings = {"Official", "Leisure", "Shop", "Sport"};
	private bool b_show = false;
	private string st_Add = "Add Tag";
	private string st_Remove = "Remove Tag";
	private string st_Cancel = "Cancel";
	private void GUIAddTag()
	{
		float halfWidth = Screen.width / 2f;
		float halfHeight = Screen.height / 2f;
		float sizeBox = halfWidth / 2;
		Rect rect = new Rect(halfWidth - sizeBox / 2, halfHeight - sizeBox / 2, sizeBox, sizeBox);
		
		GUI.BeginGroup(rect);
		switch(e_tagState)
		{
		case TagState.Error:
			GUITagError ();
			break;
		case TagState.Front:
			// Button Add
			if(GUI.Button (new Rect(0,0,sizeBox, sizeBox / 2),st_Add))
			{
				e_tagState = TagState.Add;
			}
			// Button Remove
			if(GUI.Button (new Rect(0,sizeBox / 2,sizeBox, sizeBox / 2),st_Remove))
			{
				e_tagState = TagState.Remove;
			}
			break;
		case TagState.Add:
			// TextField id,
			st_id = GUI.TextField(new Rect(0,0,sizeBox / 2,20),st_id);
			if (GUI.Button (new Rect(sizeBox / 2,0,sizeBox / 2,20),selectionStrings[selectionGridInt]))
			{
				b_show = !b_show;
			}
			if(b_show)
				selectionGridInt = GUI.SelectionGrid (new Rect (sizeBox / 2, 25, 100,80 ), selectionGridInt, selectionStrings, 1);
			if(GUI.Button (new Rect(0,sizeBox - sizeBox / 2, sizeBox / 2, sizeBox / 2),st_Add))
			{
				// Check if tag is valid
				// Write on file
				if(st_id.Length != 12)
					e_tagState = TagState.Error;
				else 
				{
					using (StreamWriter sw = File.AppendText(urlDEBUG))
					{
						string newTag = "id:"+st_id+","+selectionStrings[selectionGridInt]+",";
						sw.WriteLine(newTag); 
					}
					e_tagState = TagState.Front;
				}
			}
			if(GUI.Button(new Rect(sizeBox / 2,sizeBox - sizeBox / 2, sizeBox / 2, sizeBox / 2),st_Cancel))
			{
				e_tagState = TagState.Front;
			}
			break;
		case TagState.Remove:
			st_id = GUI.TextField(new Rect(),st_id);
			if(GUI.Button (new Rect(),st_Add))
			{
				// Check if tag is valid
				// Remove from file and rewrite file
				e_tagState = TagState.Front;
			}
			if(GUI.Button(new Rect(),st_Cancel))
			{
				e_tagState = TagState.Front;
			}
			break;	
		}

		GUI.EndGroup();
	}

	private void GUITagError()
	{
		float halfWidth = Screen.width / 2f;
		float sizeBox = halfWidth / 2;
		float height = 50f; 
		if(GUI.Button (new Rect(0, sizeBox / 2 - 25, sizeBox, height), "Id must be 12 characters"))
		{
			e_tagState = TagState.Add;
		}
	}
	#endregion


	// The method is called via InvokeRepeating
	// It simply does the timer's work
	private void DecreaseTimer()
	{
		i_second--;
		if(i_second < 0)
		{
			if(i_minute == 0)
			{
				// We have no minute nor second left
				// State to PostGame
				// Stop the invoke
				e_state = State.Postgame;
				CancelInvoke();
			}
			i_minute--;
			i_second = 59;
		}
	}

	enum TagState
	{
		Front,
		Add,
		Remove,
		Error
	}
}


