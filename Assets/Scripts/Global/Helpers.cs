using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Helpers  
{
	public static bool ComparePreviousValues(Queue<int> previousValues, int key, int comparePreviousAmount)
	{
		if (previousValues.Count < comparePreviousAmount) 
		{
			previousValues.Enqueue (key);
		} 
		else 
		{
			previousValues.Dequeue();
			/*
			while( previousValues.Count > comparePreviousAmount )
			{
				previousValues.Dequeue();
			}
			*/
		}

		foreach (int i in previousValues) 
		{
			if( i != key)
			{
				return false;
			}
		}
		return true;
	}	

	public static float SolveAverage(Queue<float> previousValues, float key, int comparePreviousAmount)
	{
		if (previousValues.Count < comparePreviousAmount) 
		{
			previousValues.Enqueue (key);
		} 
		else 
		{
			previousValues.Dequeue();
			/*
			while( previousValues.Count > comparePreviousAmount )
			{
				previousValues.Dequeue();
			}
			*/
		}
		float sum = 0;
		float[] values = previousValues.ToArray ();

		for(int i = 0; i < values.Length; i++)
		{
			sum += values[i];
		}

		return sum / previousValues.Count;
	}

	public static int SolveMode(Queue<int> previousValues, int key, int comparePreviousAmount)
	{
		if (previousValues.Count < comparePreviousAmount) 
		{
			previousValues.Enqueue (key);
		} 
		else 
		{
			while( previousValues.Count > comparePreviousAmount )
			{
				previousValues.Dequeue();
			}
		}

		int highestIdx = 0;
		int highestAmount = 0;
		int amount;
		int[] valueArray = previousValues.ToArray ();

		for (int i = 0; i < valueArray.Length; i++) 
		{
			amount = 0;
			for (int j = 0; j < valueArray.Length; j++) 
			{
				if( valueArray[i] == valueArray[j] )
				{
					amount++;
					if( amount > highestAmount)
					{
						highestIdx = i;
						highestAmount = amount;
					}
				}
			}
		}
		return valueArray[highestIdx];
	}
}
