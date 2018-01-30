using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	private AudioSource m_MusicSource;
	private AudioSource m_SFXSource;

	[System.Serializable] private struct AudioClipEntry
	{
		public string key;
		public AudioClip clip;
	}

	//Music Variables
	[SerializeField] private AudioClipEntry[] m_MusicEntries;
	private Dictionary<string, AudioClip> m_MusicDictionary;

	//SFX Variables
	[SerializeField] private AudioClipEntry[] m_SFXEntries;
	private Dictionary<string, AudioClip> m_SFXDictionary;

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

		m_MusicDictionary = new Dictionary<string, AudioClip>();//Creates Music dictionary from array
		for (int i = 0; i < m_MusicEntries.Length; i++)
		{
			m_MusicDictionary.Add(m_MusicEntries[i].key, m_MusicEntries[i].clip);
		}

		m_SFXDictionary = new Dictionary<string, AudioClip>();//Creates Music dictionary from array
		for (int i = 0; i < m_SFXEntries.Length; i++)
		{
			m_SFXDictionary.Add(m_SFXEntries[i].key, m_SFXEntries[i].clip);
		}

		//MUSIC SOURCE STUFF
		m_MusicSource = GetComponents<AudioSource>()[0];
		if (PlayerPrefs.HasKey("Volume_Music"))//Sets music volume
		{
			SetMusicVolume(PlayerPrefs.GetFloat("Volume_Music"));
		}
		else
		{
			Debug.LogWarning("'Volume_Music' does not exist in PlayerPrefs. Creating new entry...");
			PlayerPrefs.SetFloat("Volume_Music", 1);
			SetMusicVolume(PlayerPrefs.GetFloat("Volume_Music"));
		}

		//SFX SOURCE STUFF
		m_SFXSource = GetComponents<AudioSource>()[1];
		if (PlayerPrefs.HasKey("Volume_SFX"))//Sets SFX volume
		{
			SetSFXVolume(PlayerPrefs.GetFloat("Volume_SFX"));
		}
		else
		{
			Debug.LogWarning("'Volume_SFX' does not exist in PlayerPrefs. Creating new entry...");
			PlayerPrefs.SetFloat("Volume_SFX", 0.8f);
			SetSFXVolume(PlayerPrefs.GetFloat("Volume_SFX"));
		}
	}

	public void SetMusic(string clipName)//Gets clip from Music Dictionary and plays it.
	{
		if (m_MusicDictionary.ContainsKey(clipName))
		{
			m_MusicSource.clip = m_MusicDictionary[clipName];
			if (!m_MusicSource.isPlaying)
			{
				m_MusicSource.Play();
			}
		}
		else
		{
			Debug.LogWarning("Audio Clip does not exist");
		}
	}

	public void PauseMusic(bool isPaused)//Pauses music
	{
		if(isPaused)
		{
			m_MusicSource.Pause();
		}
		else
		{
			m_MusicSource.UnPause();
		}
	}

	public void PlaySFX(string clipName)//Gets clip from SFX dictionary and plays it.
	{
		if(m_SFXDictionary.ContainsKey(clipName))
		{
			m_SFXSource.clip = m_SFXDictionary[clipName];
			m_SFXSource.Play();
		}
		else
		{
			Debug.LogWarning("Audio Clip does not exist");
		}
	}

	public void SetMusicVolume(float value)//Set Music AudioSource volume (mainly used by UI)
	{
		m_MusicSource.volume = value;
	}

	public void SetSFXVolume(float value)//Set SFX AudioSource volume (mainly used by UI)
	{
		m_SFXSource.volume = value;
	}

	public AudioClip GetMusicClip(string key)//Returns a particular Music Clip
	{
		AudioClip clip = null;
		if(m_MusicDictionary.ContainsKey(key))
		{
			clip = m_MusicDictionary[key];
		}

		return clip;
	}

	public AudioClip GetSFXClip(string key)//Returns a particular SFX Clip
	{
		AudioClip clip = null;
		if (m_SFXDictionary.ContainsKey(key))
		{
			clip = m_SFXDictionary[key];
		}

		return clip;
	}

	public void SFXOneShot(string key, Vector3 pos)//Method for cleanly calling PlayClipAtPoint from another script
	{
		AudioClip clip = null;
		if (m_SFXDictionary.ContainsKey(key))
		{
			clip = m_SFXDictionary[key];
			AudioSource.PlayClipAtPoint(clip, pos, m_SFXSource.volume);
		}
	}

	public void ToggleAudioListener(bool isOn)//Turns the AudioListener on this GameObject on or off
	{
		GetComponent<AudioListener>().enabled = isOn;
	}
}
