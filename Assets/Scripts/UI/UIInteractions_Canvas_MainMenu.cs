using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInteractions_Canvas_MainMenu : UIInteractions_Canvas
{
	[Header("UI Components")]
	[SerializeField] private Toggle m_TutorialToggle;
	[SerializeField] private Button[] m_LevelButtons;

	[Header("Stats Components")]
	[SerializeField] private StatEntry[] m_StatEntries;
	[System.Serializable] private struct StatEntry
	{
		public string m_Key;
		public Text m_Text;
	}


	protected override void Awake()
	{
		base.Awake();

		//Tutorial toggle stuff
		if(PlayerPrefs.HasKey("Tutorial"))
		{
			SetTutorialToggle(GetTutorialPref());
		}
		else
		{
			Debug.LogWarning("'Tutorial' key does not exist in PlayerPrefs, creating new entry...");
			PlayerPrefs.SetInt("Tutorial", 0);
		}

		//Level button stuff
		for (int i = 1; i < m_LevelButtons.Length; i++)
		{
			int levelNum = i;
			string key = "Level" + levelNum + "Complete";

			if (PlayerPrefs.HasKey(key))
			{
				if (PlayerPrefs.GetInt(key) == 0)
				{
					m_LevelButtons[i].interactable = true;
				}
				else if (PlayerPrefs.GetInt(key) == 1)
				{
					m_LevelButtons[i].interactable = false;
				}
			}
			else
			{
				Debug.LogWarning("'" + key + "' key does not exist in PlayerPrefs, creating new entry...");
				PlayerPrefs.SetInt(key, 1);
				m_LevelButtons[i].interactable = false;
			}
		}
	}

	private void Start()
	{
		AudioManager.instance.SetMusic("Horse Steppin");
		UpdateStats();
	}

	public void SetTutorial(bool isOn)
	{
		SetTutorialPref(isOn);
	}

	private void SetTutorialToggle(bool isOn)
	{
		m_TutorialToggle.isOn = isOn;
	}

	private bool GetTutorialPref()
	{
		bool intToBool = false;
		int isOn = PlayerPrefs.GetInt("Tutorial");

		if (isOn == 0)
		{
			intToBool = true;
		}
		else if (isOn == 1)
		{
			intToBool = false;
		}
		else
		{
			Debug.LogWarning("'Tutorial' PlayerPrefs entry is fucked up, resetting to true...");
		}

		return intToBool;
	}

	private void SetTutorialPref(bool isOn)
	{
		if (isOn)
		{
			PlayerPrefs.SetInt("Tutorial", 0);
		}
		else
		{
			PlayerPrefs.SetInt("Tutorial", 1);
		}
	}

	private void UpdateStats()
	{
		for (int i = 0; i < m_StatEntries.Length; i++)
		{
			if(PlayerPrefs.HasKey(m_StatEntries[i].m_Key))
			{
				m_StatEntries[i].m_Text.text = PlayerPrefs.GetInt(m_StatEntries[i].m_Key).ToString("N0");
			}
			else
			{
				Debug.LogWarning(m_StatEntries[i].m_Key + " does not exist in PlayerPrefs, creating new entry...");
				PlayerPrefs.SetInt(m_StatEntries[i].m_Key, 0);
				m_StatEntries[i].m_Text.text = PlayerPrefs.GetInt(m_StatEntries[i].m_Key).ToString("N0");
			}
		}
	}

	public void ResetData()
	{
		PlayerPrefs.DeleteAll();
	}
}
