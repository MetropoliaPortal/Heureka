using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CubePosition))]
public class ConnectScript : MonoBehaviour {

	public string tagQuuppa;// = "001830ed41a4";
	CubePosition m_cubePosition;
	string st_positionX = "positionX";
	string st_positionY = "positionY";
	int offset = 3;
	int offsetValue = 5;
    string url; 
	void Start () 
	{
		m_cubePosition = GetComponent<CubePosition>();

        url = "192.168.123.124:8080/qpe/getHAIPLocation?tag=" + tagQuuppa;
		GetInfo ();
		StartCoroutine(GetInfo());
	}

	IEnumerator GetInfo()
	{
		while(true)
		{
			WWW www = new WWW(url);
			yield return www;
            
            if (www.error == null)
            {
                int indexX = www.text.IndexOf(st_positionX) + st_positionX.Length + offset;
                int indexY = www.text.IndexOf(st_positionY) + st_positionY.Length + offset;
                string posX = www.text.Substring(indexX, offsetValue);
                string posY = www.text.Substring(indexY, offsetValue);
               
                float x = float.Parse(posX);
                float y = float.Parse(posY);
                m_cubePosition.PositionCube(x, -y, 0.0f);
            }
        }
	}
}
