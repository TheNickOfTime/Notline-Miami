using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Character_Player : Character
{
	public static Character_Player instance;

	[Header("Player Config")]
	[SerializeField] protected float m_HealthRegenSpeed = 1;
	[SerializeField] protected float m_IdleTimer_Max = 0;
	//States
	protected PlayerState m_PlayerState;
	[SerializeField] protected PlayerState m_DefaultState;

	[Header("Player Stats")]
	[SerializeField] protected int m_Score;
	protected float m_IdleTimer_Current = 0;
	protected Vector3 m_Direction;
	protected bool m_CanShoot = true;

	protected override void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}

		base.Awake();
	}

	protected override void Start()
	{
		m_UI = (UIInteractions_Canvas.instance as UIInteractions_Canvas_Gameplay);
		m_UI.AssignPlayerHealthSlider(m_HealthSlider);
		SetHealth(m_HealthMax);

		CursorManager.ChangeCursorType(CursorType.crosshair);
		AudioManager.instance.ToggleAudioListener(false);
	}

	protected override void Update()
	{
		PlayerInput();
		LookAtCursor();

		//Player State Machine
		switch (m_PlayerState)
		{
			case PlayerState.Idle:
				Idle();
				break;
			case PlayerState.Moving:
				Move();
				break;
			case PlayerState.Dead:
				Dead();
				break;
		}
	}

	protected void LateUpdate()
	{
		transform.position = new Vector3(transform.position.x, 0, transform.position.z);
	}

	public void SetState(PlayerState state)//Set state, as well as one shot actions for a given state change
	{
		m_PlayerState = state;

		switch (state)
		{
			case PlayerState.Idle:
				m_Anim.SetBool("IsMoving", false);
				break;
			case PlayerState.Moving:
				m_Anim.SetBool("IsMoving", true);
				break;
			case PlayerState.Dead:
				AudioManager.instance.ToggleAudioListener(true);
				m_IsAlive = false;
				m_Anim.applyRootMotion = false;
				m_Anim.SetBool("IsUnconscious", true);
				StatSetter.IncrimentStat("TimesDied", 1);
				m_UI.SetActivePanel("Panel_Lose");
				break;
		}
	}

	protected void PlayerInput()
	{
		m_Direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		if(m_Direction.magnitude > 0)//If the player has begun moving
		{
			if(m_PlayerState == PlayerState.Idle)//And the player's current state is Idle
			{
				SetState(PlayerState.Moving);//Set state to moving
			}
		}
		else//If player is not moving
		{
			if (m_PlayerState == PlayerState.Moving)//And the player's state is moving
			{
				SetState(PlayerState.Idle);//Set state to Idle
			}
		}

		if(Input.GetButton("Fire1"))
		{
			if(m_CanShoot)
			{
				UseWeapon();
			}
		}

		if (Input.GetButtonDown("Fire1"))
		{
			if(m_Weapon != null )
			{
				if(m_Weapon is Weapon_Gun)
				{
					if(!(m_Weapon as Weapon_Gun).GetIsAutomatic())
					{
						m_CanShoot = false;
					}
				}
			}
		}

		if (Input.GetButtonUp("Fire1"))
		{
			if (m_Weapon != null)
			{
				if (m_Weapon is Weapon_Gun)
				{
					if (!(m_Weapon as Weapon_Gun).GetIsAutomatic())
					{
						m_CanShoot = true;
					}
				}
			}
		}

		if (Input.GetButtonDown("Fire2"))
		{
			if (m_Weapon != null)//If the player has a weapon, drop it
			{
				m_Weapon.Drop(transform.forward);
				m_Weapon = null;
				(UIInteractions_Canvas.instance as UIInteractions_Canvas_Gameplay).SetAmmoTextVisibility(false);
			}
			else//Otherwise check if there are any weapons in range of the player, and pick it up
			{
				Collider[] weapons = Physics.OverlapSphere(new Vector3(transform.position.x, m_CharacterController.bounds.extents.y, transform.position.z), 1.25f, 1 << LayerMask.NameToLayer("Weapons"));
				if(weapons.Length > 0)
				{
					m_Weapon = weapons[0].GetComponent<Weapon>();
					m_Weapon.PickUp(m_WeaponAnchor);
					(UIInteractions_Canvas.instance as UIInteractions_Canvas_Gameplay).SetAmmoTextVisibility(true);
				}
			}
		}
	}

	protected void LookAtCursor()
	{
		transform.LookAt(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 15 - transform.position.y)));
		m_WeaponAnchor.transform.LookAt(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 15 - m_WeaponAnchor.position.y)));
	}

	protected override void UseWeapon()
	{
		if (m_Weapon != null)//If the character has a weapon, use it
		{
			m_Weapon.Use();
		}
		else if(Input.GetButtonDown("Fire1"))//Otherwise punch
		{
			Punch();
		}
	}

	protected override void Punch()
	{
		base.Punch();

		StatSetter.IncrimentStat("PunchesThrown", 1);
	}

	protected override void Idle()
	{
		m_IdleTimer_Current += Time.deltaTime;//Adds to IdleTimer

		if(m_IdleTimer_Current >= m_IdleTimer_Max)//If the player has been idle long enough
		{
			if(m_HealthCurrent < m_HealthMax)//And the players health is less than the max
			{
				AddHealth(Time.deltaTime * m_HealthRegenSpeed);//Regen slowly;
			}
		}
	}

	protected override void Move()
	{
		m_CharacterController.Move(m_Direction * Time.deltaTime * m_MoveSpeed);
	}

	protected override void Dead()
	{
		//Do dead stuff
	}

	public int GetScore()
	{
		return m_Score;
	}

	public void SetScore(int value)
	{
		m_Score = value;

		if(m_Score < 0)
		{
			m_Score = 0;
		}

		m_UI.SetScoreText(m_Score);
	}

	public void AddScore(int amount)
	{
		SetScore(m_Score + amount);

		int totalScore = PlayerPrefs.GetInt("TotalScore");
		totalScore += amount;
		PlayerPrefs.SetInt("TotalScore", totalScore);
	}

	public override void SetHealth(float value)
	{
		m_HealthCurrent = value;

		if (m_HealthCurrent > m_HealthMax)
		{
			m_HealthCurrent = m_HealthMax;
		}
		if (m_HealthCurrent <= 0)
		{
			m_HealthCurrent = 0;
			SetState(PlayerState.Dead);
		}

		m_UI.SetPlayerHealth(m_HealthCurrent, m_HealthMax);
	}
}
