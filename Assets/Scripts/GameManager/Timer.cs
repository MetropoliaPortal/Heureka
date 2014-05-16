using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour 
{

	public int Minutes = 0; 
	private int minutes;
	private int seconds;

	public void StartTimer()
	{
		InvokeRepeating("DecreaseTimer",1.0f,1.0f);
	}
	
	// The method is called via InvokeRepeating
	// It simply does the timer's work
	private void DecreaseTimer()
	{
		seconds--;
		if(seconds < 0)
		{
			if(minutes == 0)
			{
	
				CancelInvoke();
			}
			minutes--;
			seconds = 59;
		}
	}
}
