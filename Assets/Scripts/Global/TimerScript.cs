using UnityEngine;
using System.Collections;
using System;

public class TimerScript : MonoBehaviour {

	private Rect r_rect;
	private Rect r_restartRect;
	private float timer;
	private bool b_showButton;
	private GameManager m_manager; 

	void Start () 
	{
		float size = Screen.width / 10f;
		float midWidth = Screen.width / 2f - size / 2f;
		r_rect = new Rect(midWidth, 0, size, size / 2f);
		r_restartRect = new Rect(midWidth, Screen.height / 2 - size/4, size, size / 2);
		timer = 5f;
		b_showButton = true;
		m_manager = GetComponent<GameManager>();
		m_manager.SetState(State.StartMenu);
	}
	
	// Update is called once per frame
	void Update () 
	{
		State current = m_manager.GetState();
		if(current == State.StartMenu)return;
		if(current == State.Postgame)return;

		timer -= Time.deltaTime;
		if(timer <=0)
		{
			b_showButton = true;
			m_manager.SetState(State.Postgame);
		}
	}
	void OnGUI()
	{
		int min = (int)(timer / 60f);
		int second = (int)(timer % 60);

		GUI.Box (r_rect, min+":"+second.ToString("00"));
		if(b_showButton)
		{
			State current = m_manager.GetState();
			if(current == State.Postgame)
			{
				if(GUI.Button (r_restartRect, "RESTART"))
				{
					GC.Collect ();
					timer = 5f;
					m_manager.SetState(State.StartMenu);
					GetComponent<BackgroundTextureScript>().SetValue();
				}
			}
			else if(current == State.StartMenu)
			{
				if(GUI.Button (r_restartRect, "START"))
				{
					m_manager.SetState(State.Running);
				}
			}
		}
	}
}
