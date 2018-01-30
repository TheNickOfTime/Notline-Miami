using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInteractions_Settings : MonoBehaviour
{
	[SerializeField] private Slider m_Slider_Music;
	[SerializeField] private Slider m_Slider_SFX;

	private void Start()
	{
		m_Slider_Music.value = PlayerPrefs.GetFloat("Volume_Music") * m_Slider_Music.maxValue;
		m_Slider_SFX.value = PlayerPrefs.GetFloat("Volume_SFX") * m_Slider_SFX.maxValue;
	}

	public void SetMusicVolume()
	{
		float value = m_Slider_Music.value / m_Slider_Music.maxValue;

		if (AudioManager.instance != null)
		{
			AudioManager.instance.SetMusicVolume(value);
			PlayerPrefs.SetFloat("Volume_Music", value);
		}
	}

	public void SetSFXVolume()
	{
		float value = m_Slider_SFX.value / m_Slider_SFX.maxValue;

		if (AudioManager.instance != null)
		{
			AudioManager.instance.SetSFXVolume(value);
			PlayerPrefs.SetFloat("Volume_SFX", value);
		}
	}
}
