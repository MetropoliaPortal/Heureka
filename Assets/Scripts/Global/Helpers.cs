using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Helpers  
{
	public static bool ComparePreviousValues(Queue<int> previousValues, int key, int ComparePreviousAmount)
	{
		if (previousValues.Count < ComparePreviousAmount) 
		{
			previousValues.Enqueue (key);
		} 
		else 
		{
			previousValues.Dequeue();
		}
		foreach (int i in previousValues) 
		{
			if( i != key)
				return false;
		}
		return true;
	}	
}
