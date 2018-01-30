using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if(other.GetComponent<Character_Player>() != null)
		{
			if(GameManager.instance.GetLevelComplete())
			{
				GameManager.instance.LevelComplete();
			}
		}
	}
}
