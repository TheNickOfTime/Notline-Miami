using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatSetter : MonoBehaviour
{
	public static void CreateStat(string key, int defaultValue)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			Debug.LogWarning("'" + key + "' does not exist in PlayerPrefs. Creating new entry...");
			PlayerPrefs.SetInt(key, defaultValue);
		}
		else
		{
			Debug.LogWarning("'" + key + "' Already exists in PlayerPrefs. Use 'SetStat()' to set this value.");
		}
	}

	public static int GetStat(string key)
	{
		int value = 666;

		if (!PlayerPrefs.HasKey(key))
		{
			Debug.LogWarning("'" + key + "' does not exist in PlayerPrefs. Use CreateStat() to initialize this key.");
		}
		else
		{
			value = PlayerPrefs.GetInt(key);
		}

		return value;
	}

	public static void SetStat(string key, int value)
	{
		if(PlayerPrefs.HasKey(key))
		{
			PlayerPrefs.SetInt(key, value);
		}
		else
		{
			Debug.LogWarning("'" + key + "' does not exist in PlayerPrefs. Key has either not been initialized, or has been entered incorrectly");
		}
	}

	public static void IncrimentStat(string key, int amount)
	{
		SetStat(key, GetStat(key) + amount);
	}

	public static bool CheckForKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}
}
