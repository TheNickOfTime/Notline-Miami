using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
	//Components & References
	protected UIInteractions_Canvas_Gameplay m_UI;
	protected CharacterController m_CharacterController;
	protected Animator m_Anim;
	protected GameObject m_Blood;
	[SerializeField] protected Transform m_WeaponAnchor;
	[SerializeField] protected Weapon m_Weapon;
	[SerializeField] protected Slider m_HealthSlider;

	[Header("Character Config")]
	[SerializeField] protected float m_MoveSpeed = 1;
	[SerializeField] protected int m_HealthMax = 10;
	[SerializeField] protected int m_PunchDamage;

	[Header("Stats")]
	[SerializeField] protected float m_HealthCurrent;
	[SerializeField] protected bool m_IsAlive = true;

	protected virtual void Awake()
	{
		m_CharacterController = GetComponent<CharacterController>();
		m_Anim = GetComponent<Animator>();
		m_Blood = Resources.Load("BulletHit_Character") as GameObject;
	}

	protected virtual void Start()
	{
		m_UI = (UIInteractions_Canvas.instance as UIInteractions_Canvas_Gameplay);
		SetHealth(m_HealthMax);
	}

	protected virtual void Update()
	{
		//Character State Machine
	}

	protected virtual void Idle()
	{
		//Idle Actions
	}

	protected virtual void Move()
	{
		//Moves character
	}

	protected virtual void Dead()
	{
		//GAME OVER
	}

	protected virtual void UseWeapon()
	{
		if(m_Weapon != null)//If the character has a weapon, use it
		{
			m_Weapon.Use();
		}
		else//Otherwise punch
		{
			Punch();
		}
	}

	protected virtual void Punch()//Gets all enemies within the specified hitbox, punches them if there is no obstacle between
	{
		if(Time.timeScale > 0)
		{
			m_Anim.SetTrigger("Punch");

			bool didHit = false;

			Collider[] hitObjects = Physics.OverlapBox(transform.position + transform.forward, new Vector3(1.5f, 1.5f, 2));
			foreach (Collider hitObject in hitObjects)
			{
				Character hitCharacter = hitObject.GetComponent<Character>();
				if (hitCharacter != null)//Checks if hitObject is Character
				{
					if (hitCharacter != this)
					{
						if(!Physics.Linecast(transform.position, hitObject.transform.position))//Checks for obstacles (so punching through walls cannot happen)
						{
							hitCharacter.AddHealth(-m_PunchDamage);
							didHit = true;
							Instantiate(m_Blood, hitCharacter.transform.position, transform.rotation);
						}
					}
				}
			}

			if (didHit)//If the player has hit anything punchable, play clip with contact noise in it, otherwise it is just the swing
			{
				AudioManager.instance.SFXOneShot("Punch_Hit", transform.position);
			}
			else
			{
				AudioManager.instance.SFXOneShot("Punch_Swing", transform.position);
			}
		}
	}

	public virtual void SetHealth(float value)//Sets health to specified value, if less than zero state is set to dead
	{
		m_HealthCurrent = value;
		
		if(m_HealthCurrent > m_HealthMax)
		{
			m_HealthCurrent = m_HealthMax;
		}
		if (m_HealthCurrent <= 0)
		{
			m_HealthCurrent = 0;
			Dead(); //GAME OVER
		}

		//Update UI
	}

	public virtual void AddHealth(float amount)//Adds health by specified amount
	{
		SetHealth(m_HealthCurrent + amount);
	}

	public bool GetIsAlive()
	{
		return m_IsAlive;
	}
}
