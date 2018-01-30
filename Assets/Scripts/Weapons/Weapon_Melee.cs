using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Melee : Weapon
{
	[SerializeField] protected float m_Damage;
	[SerializeField] protected Vector3 m_HitBox;

	protected Animation m_Anim;

	protected override void Awake()
	{
		base.Awake();

		m_Anim = GetComponent<Animation>();
	}

	public override void Use()
	{
		m_Anim.Play();
		Collider[] hitObjects = Physics.OverlapBox(transform.position + transform.forward, m_HitBox);
		foreach (Collider hitObject in hitObjects)
		{
			Character hitCharacter = hitObject.GetComponent<Character>();
			if (hitCharacter != null)
			{
				if (hitCharacter != this)
				{
					hitCharacter.AddHealth(-m_Damage);
				}
			}
		}

		StatSetter.IncrimentStat("WeaponsSwung", 1);
	}
}
