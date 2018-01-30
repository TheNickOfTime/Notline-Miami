using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] private float m_MoveSpeed = 15;
	[SerializeField] private float m_DespawnTime = 10;
	[SerializeField] private float m_Damage;
	[Space]
	[SerializeField] private GameObject m_HitParticles_Object;
	[SerializeField] private GameObject m_HitParticles_Character;

	private IEnumerator Start()
	{
		GetComponent<Rigidbody>().velocity = transform.forward * 15;

		yield return new WaitForSeconds(m_DespawnTime);
		Destroy(gameObject);
	}

	private void OnCollisionEnter(Collision other)
	{
		Character character = other.transform.GetComponent<Character>();
		if(character != null)
		{
			Instantiate(m_HitParticles_Character, transform.position, transform.rotation);
			if(character.GetIsAlive())
			{
				character.AddHealth(-m_Damage);
			}
		}
		else if(other.transform.GetComponent<Weapon>() == null)
		{
			Instantiate(m_HitParticles_Object, transform.position, transform.rotation);
		}

		Destroy(gameObject);
	}

}
