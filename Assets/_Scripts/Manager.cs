using UnityEngine;
using System.Collections;

public class Manager : GameManager
{

    #region MEMBERS
    private float m_timer = 120;
    private Rect m_timerRect;
    #endregion

    #region UNITY_METHODS
    void Start () 
    {
        e_state = State.StartMenu;
        float size = Screen.width / 10f;
        float halfWidth = Screen.width / 2f;
        m_timerRect = new Rect(halfWidth - size/ 2f, 0, size, size);
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (e_state != State.Running) return;

        m_timer -= Time.deltaTime;
    }
    void OnGUI() 
    {
        int minutes = (int)(m_timer / 60);
        int seconds = (int)(m_timer % 60);
        GUI.Box(m_timerRect,minutes.ToString() + ":"+seconds.ToString("00"));
    }
    #endregion

    #region METHODS

    #endregion
}


