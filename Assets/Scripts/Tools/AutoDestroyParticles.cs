using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticles : MonoBehaviour
{
	private ParticleSystem m_Particles;

	private void Start()
	{
		m_Particles = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if(!m_Particles.isPlaying)
		{
			Destroy(gameObject);
		}
	}
}
