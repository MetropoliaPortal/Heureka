using UnityEngine;
using System.Collections;

public class WelcomeScript : MonoBehaviour
{

    #region MEMBERS

    private GameManager manager;
    private Rect m_welcomeGroup;
    private Rect m_welcomeBox;
    private Rect m_buttonNext;
    private bool b_intro = true;
    private string [] st_strWelcome = {"Welcome to the Dream-City.\nThis tutorial will explain \nhow to use the Dream-city builder",
                                          "In this environment, \nyou have 2 minutes to develop your city.\nThe counter show on the top of the screen",
                                      "To begin with,\n we will start with interaction.\n",
                                        "To grab a cub, simply left click on it."};
    private int m_index = 0;
    #endregion
    #region UNITY_METHODS
	void Start () 
    {
        manager = GetComponent<GameManager>();
        
        float size = Screen.width / 20f;
        float halfWidth = Screen.width / 2f;
        float halfHeight = Screen.height / 2f;
        float sizeWelcomeWidth = size * 6f;
        float sizeWelcomeHeight = size * 3f;
        m_welcomeGroup = new Rect(halfWidth - sizeWelcomeWidth / 2, halfHeight - sizeWelcomeHeight /2 ,sizeWelcomeWidth,sizeWelcomeHeight);
        m_welcomeBox = new Rect(0,0,sizeWelcomeWidth,sizeWelcomeHeight);
        m_buttonNext = new Rect(sizeWelcomeWidth / 2f - size/2, sizeWelcomeHeight - size,size,size );
	}
	
	// Update is called once per frame
	void OnGUI () 
    {
 
        GUI.BeginGroup(m_welcomeGroup);
        GUI.Box(m_welcomeBox, st_strWelcome[m_index]);

        if (GUI.Button(m_buttonNext, "Next"))
        { 
            if (++m_index == st_strWelcome.Length)
            {
                this.enabled = false;
            }
            if (m_index == 3)
            {
                m_welcomeGroup.x = Screen.width - m_welcomeGroup.width;
                manager.SetState(State.Tutorial);
            }
        }
        GUI.EndGroup();
    }
    #endregion
}
