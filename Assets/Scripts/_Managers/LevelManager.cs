using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public static LevelManager instance;

	private AsyncOperation m_LoadingLevel = null;

	[SerializeField] private float m_MinimumLoadTime = 3;

	private void Awake()
	{
		if(instance != null)//Singleton
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public void LoadLevel(int index)//Loads level by string
	{
		StartCoroutine(WaitForAsync(index));
	}

	public void LoadLevel(string name)//Loads level by build index
	{
		StartCoroutine(WaitForAsync(name));
	}

	private IEnumerator WaitForAsync(int index)//Brings up loading screen while loading level in the background.
	{
		SceneManager.LoadScene(2);
		Time.timeScale = 1;
		yield return null;

		m_LoadingLevel = SceneManager.LoadSceneAsync(index);
		m_LoadingLevel.allowSceneActivation = false;

		yield return new WaitForSecondsRealtime(m_MinimumLoadTime);
		yield return new WaitUntil(() => m_LoadingLevel.progress >= 0.9f);

		m_LoadingLevel.allowSceneActivation = true;
	}

	private IEnumerator WaitForAsync(string name)
	{
		SceneManager.LoadScene(2);
		Time.timeScale = 1;
		yield return null;

		m_LoadingLevel = SceneManager.LoadSceneAsync(name);
		m_LoadingLevel.allowSceneActivation = false;

		yield return new WaitForSecondsRealtime(m_MinimumLoadTime);
		yield return new WaitUntil(() => m_LoadingLevel.progress >= 0.9f);

		m_LoadingLevel.allowSceneActivation = true;
	}
}
