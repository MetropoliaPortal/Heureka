using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{

    #region MEMBERS
    GameManager manager;
    private Rect m_leftButton, m_rightButton;
    #endregion

    #region UNITY_METHODS
    void Start () 
    {
        manager = GameObject.Find("Manager").GetComponent<GameManager>();
        float size = Screen.width / 10;
        m_leftButton = new Rect(0,Screen.height- size, size, size);
        m_rightButton = new Rect(Screen.width - size, Screen.height - size, size, size);
    }
	
	// Update is called once per frame
    void OnGUI() 
    {
        if (manager.GetState() == State.StartMenu) return;
        if (GUI.RepeatButton(m_leftButton, "Left")) 
        {
            transform.RotateAround(Vector3.zero, Vector3.up, 20 * Time.deltaTime);
        }
        if (GUI.RepeatButton(m_rightButton, "Right")) 
        {
            transform.RotateAround(Vector3.zero, Vector3.up, -20 * Time.deltaTime);
        }
    }
    #endregion
}
