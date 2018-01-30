using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIInteractions_Canvas_Gameplay : UIInteractions_Canvas
{
	[Header("UI Components")]
	[SerializeField] private Text m_ScoreText;
	[SerializeField] private Text m_HighScoreText;
	[SerializeField] private Text m_FinalScoreText;
	[SerializeField] private Text m_AmmoText;
	[Space]
	[SerializeField] private Slider m_PlayerHealthSlider;
	[Space]
	[SerializeField] private Button[] m_MaskButtons;
	[Space]
	[SerializeField] private Image m_EnemyIcon;
	private Image[] m_EnemyIcons;
	[SerializeField] private Color m_EnemyDeadColor;

	protected override void Awake()
	{
		base.Awake();

		if(PlayerPrefs.GetInt("Tutorial") == 1)
		{
			SetActivePanel("Panel_CharacterSelect");
		}



		for (int i = 1; i < m_MaskButtons.Length; i++)
		{
			int maskNum = i + 1;
			string key = "Mask" + maskNum + "Unlocked";
			

			if (PlayerPrefs.HasKey(key))
			{
				if(PlayerPrefs.GetInt(key) == 0)
				{
					m_MaskButtons[i].interactable = true;
				}
				else if(PlayerPrefs.GetInt(key) == 1)
				{
					m_MaskButtons[i].interactable = false;
				}
			}
			else
			{
				Debug.LogWarning("'" + key + "'" + " does not exist in PlayerPrefs, creating new entry...");
				PlayerPrefs.SetInt(key, 1);
				m_MaskButtons[i].interactable = false;
			}
		}
	}

	private void Start()
	{
		if(GameManager.instance.GetEnemies().Length > 0)
		{
			m_EnemyIcons = new Image[GameManager.instance.GetEnemies().Length];
			for (int i = 0; i < m_EnemyIcons.Length; i++)
			{
				if(i != 0)
				{
					m_EnemyIcons[i] = Instantiate(m_EnemyIcon, m_EnemyIcon.transform.parent);
				}
				else
				{
					m_EnemyIcons[i] = m_EnemyIcon;
				}
			}
		}
		else
		{
			m_EnemyIcon.transform.parent.gameObject.SetActive(false);
		}

		SetScoreText(0);
		SetAmmoTextVisibility(false);
	}

	public void SetScoreText(int score)
	{
		m_ScoreText.text = score + "pts";
		SetHighScoreText(score);
	}

	public void SetScoreText(string text)
	{
		m_ScoreText.text = text;
	}

	public void SetHighScoreText(int score)
	{
		string key = GameManager.instance.GetLevelName() + "Score";

		if (!PlayerPrefs.HasKey(key))
		{
			Debug.LogWarning("'" + key + "' does not exist in PlayerPrefs, creating new entry...");
			PlayerPrefs.SetInt(key, 0);
		}

		int highScore = PlayerPrefs.GetInt(key);
		if(score > highScore)
		{
			highScore = score;
			PlayerPrefs.SetInt(key, highScore);
		}
		m_HighScoreText.text = "High Score: " + highScore.ToString("N0");
	}

	public void SetFinalScoreText(int score)
	{
		m_FinalScoreText.text = score.ToString("N0");
	}

	public void SetAmmoText(int current, int max)
	{
		m_AmmoText.text = current + "/" + max + "rnds";
	}

	public void SetAmmoText(string text)
	{
		m_AmmoText.text = text;
	}

	public void SetAmmoTextVisibility(bool isVisible)
	{
		m_AmmoText.transform.parent.gameObject.SetActive(isVisible);
	}

	public void SetPlayerHealth(float current, int max)
	{
		m_PlayerHealthSlider.value = current / max;
	}

	public void UpdateEnemyIcons()
	{
		GameManager.instance.UpdateEnemies();
		for (int i = GameManager.instance.GetInitalEnemyCount() - 1; i > GameManager.instance.GetEnemies().Length - 1; i--)
		{
			m_EnemyIcons[i].color = m_EnemyDeadColor;
		}
	}

	public void AssignPlayerHealthSlider(Slider slider)
	{
		m_PlayerHealthSlider = slider;
	}

	public void ReloadScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
