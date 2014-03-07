using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CubePosition))]
public class ConnectScript : MonoBehaviour {

	string st_url = null;
	public string tagQuuppa = "001830ed41a4";
	CubePosition m_cubePosition;
	private Rect r_rect;
	string st_positionX = "positionX";
	string st_positionY = "positionY";
	int offset = 3;
	int offsetValue = 5;

	string text;
	void Start () 
	{
		m_cubePosition = GetComponent<CubePosition>();
		r_rect = new Rect(0,0,200,200);

		GetInfo ();
		StartCoroutine(GetInfo());
	}

	void OnGUI()
	{
		GUI.Box (r_rect,text);
	}

	IEnumerator GetInfo()
	{
		string url = "192.168.123.124:8080/qpe/getHAIPLocation?tag=";
		st_url = url + tagQuuppa;
		while(true)
		{
#if UNITY_EDITOR
#else
			WWW www = new WWW(st_url);
			yield return www;
			if(www.error != null)
			{
				text = "Error";
			}
			int indexX = www.text.IndexOf(st_positionX) + st_positionX.Length +  offset;
			int indexY = www.text.IndexOf(st_positionY) + st_positionY.Length + offset;
			string posX = www.text.Substring(indexX, offsetValue);
			string posY = www.text.Substring(indexY, offsetValue);

			text = ("X:"+ posX + " Y:"+posY+ " " + transform.position.y);

			try
			{
				float x = float.Parse(posX);
				float y = float.Parse (posY);

				m_cubePosition.PositionCube(x, -y, 0.0f);
			}
			catch(Exception e)
			{
				text = e.Message;
			}
		}
#endif
	}
}
