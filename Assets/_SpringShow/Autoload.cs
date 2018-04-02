using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Autoload : MonoBehaviour
{
	private AsyncOperation m_LoadingLevel = null;

	private IEnumerator Start()
	{
		Time.timeScale = 1;
		yield return null;

		m_LoadingLevel = SceneManager.LoadSceneAsync(1);
		m_LoadingLevel.allowSceneActivation = false;

		yield return new WaitForSecondsRealtime(5);
		yield return new WaitUntil(() => m_LoadingLevel.progress >= 0.9f);

		m_LoadingLevel.allowSceneActivation = true;
	}
}
