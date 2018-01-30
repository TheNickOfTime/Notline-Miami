using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[System.Serializable] private struct CharacterEntry
	{
		public string key;
		public GameObject character;
	}
	private Dictionary<string, GameObject> m_CharacterDictionary;

	[Header("Setup")]
	[SerializeField] private string m_StartMusic;
	[SerializeField] private string m_LevelName;
	[SerializeField] private CharacterEntry[] m_CharacterEntries;
	[Tooltip("Level Unlocks: 'Level[num]Complete'" + "\n" + "Mask Unlocks: 'Mask[num]Unlocked'")]
	[SerializeField] private string[] m_CompletionUnlocks;

	[Header("Stats")]
	[SerializeField] private Character_Player m_Player;
	[SerializeField] private Character_Enemy[] m_Enemies;
	private int m_InitalEnemyCount;
	private bool m_IsPaused = false;
	private bool m_CanPause = false;
	private bool m_LevelComplete = false;

	private void Awake()
	{
		//Ensures game is running full speed
		Time.timeScale = 1;

		//singleton
		if(instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}

		//Initialize dictionary and populates with entries
		m_CharacterDictionary = new Dictionary<string, GameObject>();
		for (int i = 0; i < m_CharacterEntries.Length; i++)
		{
			m_CharacterDictionary.Add(m_CharacterEntries[i].key, m_CharacterEntries[i].character);
		}

		//Initialized enemy array and temporarily disables them
		UpdateEnemies();
		m_InitalEnemyCount = m_Enemies.Length;
		for (int i = 0; i < m_Enemies.Length; i++)
		{
			m_Enemies[i].enabled = false;
		}
	}

	private void Start()
	{
		//Plays specified music
		AudioManager.instance.SetMusic(m_StartMusic);
	}

	private void Update()
	{
		CheckLevelComplete();
		UpdateEnemies();

		if (Input.GetKeyDown(KeyCode.Escape))//Toggles pause
		{
			PauseGame(!m_IsPaused);
		}
	}

	public void PauseGame(bool pausedState)//Sets state to paused, as well as bring up the pause menu
	{
		if(m_CanPause)//Game can only be paused if m_CanPause == true
		{
			m_IsPaused = pausedState;

			if (m_IsPaused)
			{
				UIInteractions_Canvas.instance.SetActivePanel("Panel_Pause");
				UIInteractions_Canvas.instance.SetOverlay("Effect_VHS");
				Time.timeScale = 0;
			}
			else
			{
				UIInteractions_Canvas.instance.SetActivePanel("Panel_HUD");
				UIInteractions_Canvas.instance.SetOverlay("");
				Time.timeScale = 1;
			}
		}
		else
		{
			Debug.LogWarning("Game cannot be paused at this time.");
		}
	}

	public void SelectCharacter(string characterKey)
	{
		//Spawns player
		if(m_CharacterDictionary.ContainsKey(characterKey))
		{
			GameObject characterToSpawn = m_CharacterDictionary[characterKey];
			GameObject player = Instantiate(characterToSpawn, PlayerSpawn.instance.transform.position, Quaternion.identity);
			m_Player = Character_Player.instance;
		}

		//Enables enemies
		foreach (Character_Enemy enemy in m_Enemies)
		{
			enemy.enabled = true;
		}

		//Enables camera
		CameraFollower.instance.enabled = true;
	}

	public void SetCanPause(bool canPause)
	{
		m_CanPause = canPause;
	}

	private void CheckLevelComplete()//Checks how many enemies are alive, if it is 0, then the player has won the game
	{
		int enemiesAlive = 0;
		for (int i = 0; i < m_Enemies.Length; i++)
		{
			if(m_Enemies[i] != null)
			{
				enemiesAlive += 1;
			}
		}

		if(enemiesAlive <= 0)
		{
			//Makes it clear to the player that they should return to their car to complete the level
			if(!m_LevelComplete)
			{
				AudioManager.instance.SetMusic("Static");
				m_LevelComplete = true;
			}

			(UIInteractions_Canvas.instance as UIInteractions_Canvas_Gameplay).SetScoreText("Level Complete!");
			(UIInteractions_Canvas.instance as UIInteractions_Canvas_Gameplay).SetAmmoTextVisibility(true);
			(UIInteractions_Canvas.instance as UIInteractions_Canvas_Gameplay).SetAmmoText("Get In Your Car!");
		}
	}

	public void LevelComplete()//Brings up Win screen and sets the specified unlocks.
	{
		AudioManager.instance.SetMusic("Miami");
		UIInteractions_Canvas.instance.SetActivePanel("Panel_Win");
		(UIInteractions_Canvas.instance as UIInteractions_Canvas_Gameplay).SetFinalScoreText(m_Player.GetScore());

		foreach (string unlock in m_CompletionUnlocks)
		{
			PlayerPrefs.SetInt(unlock, 0);
		}

		Time.timeScale = 0;
	}

	public void UpdateEnemies()//Gets how many enemies are alive and updates UI
	{
		int count = m_Enemies.Length;
		m_Enemies = FindObjectsOfType<Character_Enemy>();
		if(m_Enemies.Length < count)
		{
			(UIInteractions_Canvas.instance as UIInteractions_Canvas_Gameplay).UpdateEnemyIcons();
		}
	}

	public Character_Enemy[] GetEnemies()
	{
		return m_Enemies;
	}

	public int GetInitalEnemyCount()
	{
		return m_InitalEnemyCount;
	}

	public string GetLevelName()
	{
		return m_LevelName;
	}

	public bool GetLevelComplete()
	{
		return m_LevelComplete;
	}
}
