using UnityEngine;
using System.Collections;

public class Manager : GameManager
{

    #region MEMBERS
    private float m_timer = 120;
    private Rect m_timerRect;
	private Rect m_overRect;
	private Rect m_restartRect;
	private bool b_over = false;
    #endregion

    #region UNITY_METHODS
    void Start () 
    {
        e_state = State.StartMenu;
        float size = Screen.width / 10f;
        float halfWidth = Screen.width / 2f;
        m_timerRect = new Rect(halfWidth - size/ 2f, 0, size, size);
		m_overRect = new Rect(Screen.width / 2 - size / 2, Screen.height / 2 - size - 2 , size, size);
		float sizeButton = size / 2;
		m_restartRect = new Rect(Screen.width / 2 - sizeButton / 2, Screen.height / 2 + size / 2 , sizeButton, sizeButton);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (e_state != State.Running) return;

        m_timer -= Time.deltaTime;
		if(m_timer <= 0.0f)
		{
			m_timer = 0.0f;
			b_over = true;

		}
    }
    void OnGUI() 
    {
        int minutes = (int)(m_timer / 60);
        int seconds = (int)(m_timer % 60);
        GUI.Box(m_timerRect,minutes.ToString() + ":"+seconds.ToString("00"));
		if(b_over)
		{
			GUI.Box(m_overRect,"GameOver");
			if(GUI.Button (m_restartRect, "Restart"))
			{
				Application.LoadLevel(Application.loadedLevelName);
			}
		}
    }
    #endregion

    #region METHODS

    #endregion
}


