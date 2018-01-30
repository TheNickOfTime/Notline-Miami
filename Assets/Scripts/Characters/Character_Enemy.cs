using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class Character_Enemy : Character
{
	//Components & References
	private NavMeshAgent m_Nav;
	private Character_Player m_Player;

	[Header("Enemy States")]
	[SerializeField] private EnemyState m_DefaultState;
	[SerializeField] private EnemyState m_State = EnemyState.Moving;
	[SerializeField] private EnemyAlertState m_AlertState = EnemyAlertState.Passive;

	[Header("Enemy Config")]
	[SerializeField] private Transform m_WaypointParent;
	[Space]
	[SerializeField] private float m_WalkSpeed;
	[SerializeField] private float m_RunSpeed;
	[Space]
	[SerializeField] private int m_FOV;
	[SerializeField] private float m_ViewDistance;
	[SerializeField] private float m_ActionDistance;
	[Space]
	[SerializeField] private float m_UseTime;
	[SerializeField] private float m_UnconciousTime;

	//Stats & Values
	private Vector3 m_StartPos;
	private Vector3[] m_Waypoints;
	private int m_WaypointIndex;
	private float m_UseTimer;
	private float m_UnconsciousTimer;
	private Vector3 m_LastPlayerPos;
	private Vector3 m_LastHitDirection;
	private Weapon m_ClosestWeapon;

	protected override void Awake()
	{
		base.Awake();

		m_Nav = GetComponent<NavMeshAgent>();
		m_StartPos = transform.position;
		if(m_WaypointParent != null)//Set waypoints if there are any
		{
			m_Waypoints = new Vector3[m_WaypointParent.childCount];
			for (int i = 0; i < m_Waypoints.Length; i++)
			{
				m_Waypoints[i] = m_WaypointParent.GetChild(i).position;
			}
		}
	}

	protected override void Start()
	{
		base.Start();
		m_Player = Character_Player.instance;
		SetState(m_DefaultState);
		SetAlertState(EnemyAlertState.Passive);
	}

	protected override void Update()
	{
		m_UseTimer += Time.deltaTime;

		if (m_IsAlive)
		{
			//Forces enemy to search for weapon if it has none, as long as it is not already seeking or unconscious
			if (m_Weapon == null && m_State != EnemyState.Seeking && m_State != EnemyState.Unconscious)
			{
				SetState(EnemyState.Seeking);
			}
		}

		switch (m_State)//Primary Behavior State Machine
		{
			case EnemyState.Idle:
				Idle();
				break;
			case EnemyState.Moving:
				Move();
				break;
			case EnemyState.Seeking:
				Seek();
				break;
			case EnemyState.Unconscious:
				Unconscious();
				break;
			case EnemyState.Dead:
				Dead();
				break;
		}
	}

	protected override void Idle()//Controls behavior while in Idle state
	{
		switch(m_AlertState)
		{
			case EnemyAlertState.Passive:
				if (CheckForPlayer())//If enemy sees player, set AlertState to alert
				{
					SetAlertState(EnemyAlertState.Alert);
				}
				break;

			case EnemyAlertState.Alert://If Idle and Alert, set state to moving
				SetState(EnemyState.Moving);
				break;
		}
	}

	protected override void Move()//Controls behavior while in Move state
	{
		switch (m_AlertState)
		{
			case EnemyAlertState.Passive://Moving actions if Enemy is in Passive State
				m_Nav.SetDestination(GetWaypoint());//Moves player from waypoint to waypoint
				CheckForNextWaypoint();
				if (CheckForPlayer() && CheckPlayerLineOfSight())//If enemy sees player, set AlertState to alert
				{
					SetAlertState(EnemyAlertState.Alert);
				}
				break;

			case EnemyAlertState.Alert://Moving actions if Enemy is in Alert State
				m_Nav.SetDestination(m_LastPlayerPos);//Moves enemy towards player
				m_WeaponAnchor.transform.LookAt(new Vector3(m_Player.transform.position.x, m_WeaponAnchor.transform.position.y, m_Player.transform.position.z));//points weapon at player
				if(CheckPlayerLineOfSight())//If Player is in sight, and within action distance, use held weapon/punch
				{
					if (CheckForPlayer())
					{
						if (GetDistanceFromPlayer() <= m_ActionDistance)
						{
							transform.LookAt(new Vector3(m_Player.transform.position.x, transform.position.y, m_Player.transform.position.z));
							UseWeapon();
						}
					}
				}
				else//If player is not in sight, keep moving towards last known position, if player cannot be found, revert AlertState to passive
				{
					if(m_Nav.remainingDistance <= 2)
					{
						SetAlertState(EnemyAlertState.Passive);
						SetState(m_DefaultState);
					}
				}
				break;
		}
	}

	protected virtual void Seek()//Has enemy search for closest weapon
	{
		if(m_Weapon == null)//if the enemy has not found the weapon yet, move towards it and pick it up
		{
			m_Nav.SetDestination(m_ClosestWeapon.transform.position);
			if(Vector3.Distance(transform.position, m_ClosestWeapon.transform.position) < 1.0f)
			{
				PickupWeapon();
			}
		}
		else//once the enemy has picked up the weapon, return to default state
		{
			switch (m_DefaultState)
			{
				case EnemyState.Idle:
					m_Nav.SetDestination(m_StartPos);
					if(m_Nav.remainingDistance <= 1)
					{
						SetState(m_DefaultState);
					}
					break;
				case EnemyState.Moving:
					m_Nav.SetDestination(m_Waypoints[m_WaypointIndex]);
					if (m_Nav.remainingDistance <= 1)
					{
						SetState(m_DefaultState);
					}
					break;
			}
		}
	}

	protected virtual void Unconscious()//Controls behavior while in Unconscious state
	{
		m_UnconsciousTimer += Time.deltaTime;
		if(m_UnconsciousTimer > m_UnconciousTime)//After unconscious period, return to default state
		{
			m_Anim.applyRootMotion = true;
			m_Anim.SetBool("IsUnconscious", false);
			SetState(m_DefaultState);
		}
	}

	public void SetState(EnemyState state)//Set state, as well as one shot actions for a given state change
	{
		m_State = state;

		switch (state)
		{
			case EnemyState.Idle:
				m_Anim.SetBool("IsMoving", false);
				break;

			case EnemyState.Moving:
				m_Nav.isStopped = false;
				m_Anim.SetBool("IsMoving", true);
				break;

			case EnemyState.Seeking:
				if(FindClosestWeapon() != null)
				{
					m_ClosestWeapon = FindClosestWeapon();
					m_Anim.SetBool("IsMoving", true);
				}
				else
				{
					SetState(m_DefaultState);
				}
				break;

			case EnemyState.Unconscious:
				m_UnconsciousTimer = 0;
				m_Nav.isStopped = true;
				DropWeapon();
				m_Anim.applyRootMotion = false;
				m_Anim.SetBool("IsUnconscious", true);
				break;

			case EnemyState.Dead:
				m_IsAlive = false;
				m_Nav.isStopped = true;
				m_Anim.applyRootMotion = false;
				m_Anim.SetBool("IsUnconscious", true);
				StatSetter.IncrimentStat("EnemiesKilled", 1);
				Destroy(this);
				break;
		}

		//Debug.Log(name + " is " + m_State);
	}

	public void SetAlertState(EnemyAlertState state)//Sets alert state and any one shot actions for a given state change
	{
		m_AlertState = state;

		switch (state)
		{
			case EnemyAlertState.Passive:
				SetMoveSpeed(m_WalkSpeed);
				break;
			case EnemyAlertState.Alert:
				SetMoveSpeed(m_RunSpeed);
				break;
		}
	}

	public override void SetHealth(float value)//Sets health to specified value, also updates UI
	{
		float startHealth = m_HealthCurrent;
		m_HealthCurrent = value;

		if (startHealth > m_HealthCurrent)//Checks if enemy has lost health, if so knocks unconscious
		{
			SetState(EnemyState.Unconscious);
		}

		if (m_HealthCurrent > m_HealthMax)
		{
			m_HealthCurrent = m_HealthMax;
		}
		if (m_HealthCurrent <= 0)
		{
			m_HealthCurrent = 0;
			SetState(EnemyState.Dead);
			m_Player.AddScore(150);
		}

		m_HealthSlider.value = m_HealthCurrent / m_HealthMax;
	}

	public override void AddHealth(float amount)
	{
		base.AddHealth(amount);

		m_Player.AddScore(50);
	}

	protected virtual bool CheckForPlayer()//Checks if player is within range and within FOV of enemy
	{
		bool inSight = false;

		Vector3 direction = m_Player.transform.position - transform.position;
		float angle = Vector3.Angle(direction, transform.forward);

		if (angle < m_FOV * 0.5f)
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position + Vector3.up, direction.normalized, out hit, m_ViewDistance, ~(1 << LayerMask.NameToLayer("IgnoreWeapon")  | 1 << LayerMask.NameToLayer("Enemy"))))
			{
				if (hit.transform.root == m_Player.transform)
				{
					inSight = true;
				}
			}
		}

		return inSight;
	}

	protected virtual bool CheckPlayerLineOfSight()//Checks if enemy has line of sight to the player
	{
		bool hitPlayer = false;

		Vector3 direction = m_Player.transform.position - transform.position;
		RaycastHit hit;
		Physics.Raycast(transform.position + Vector3.up, direction.normalized, out hit, m_ViewDistance, ~(1 << LayerMask.NameToLayer("IgnoreWeapon") | 1 << LayerMask.NameToLayer("Enemy")));
		if (hit.transform != null)
		{
			if (hit.transform == m_Player.transform)
			{
				hitPlayer = true;
				m_LastPlayerPos = m_Player.transform.position;
			}
		}
		return hitPlayer;
	}

	private float GetDistanceFromPlayer()
	{
		return Vector3.Distance(transform.position, m_Player.transform.position);
	}

	private void CheckForNextWaypoint()//Checks if player is within range of setting next waypoint
	{
		if(Vector3.Distance(transform.position, GetWaypoint()) < 0.6f)
		{
			SetWaypoint(m_WaypointIndex += 1);
		}
	}

	private Vector3 GetWaypoint()
	{
		return m_Waypoints[m_WaypointIndex];
	}

	private void SetWaypoint(int index)
	{
		m_WaypointIndex = index;
		if(index >= m_Waypoints.Length)
		{
			m_WaypointIndex = 0;
		}
		else if(index < 0)
		{
			m_WaypointIndex = m_Waypoints.Length - 1;
		}
	}

	private void SetMoveSpeed(float speed)
	{
		m_MoveSpeed = speed;
		m_Nav.speed = speed;
	}

	private void SetActionDistance()//Sets action distance based off of type of weapon
	{
		if(m_Weapon == null || m_Weapon != null && m_Weapon is Weapon_Melee)
		{
			m_ActionDistance = 2;
		}
		else if(m_Weapon is Weapon_Gun)
		{
			m_ActionDistance = 10;
		}
	}

	protected override void UseWeapon()
	{
		if (m_Weapon != null)//If the character has a weapon, use it
		{
			m_Weapon.Use();
		}
		else//Otherwise punch
		{
			Punch();
		}

		//m_UseTimer = 0;
	}

	public void DropWeapon()//Drops weapon
	{
		if(m_Weapon != null)
		{
			m_Weapon.Drop(Vector3.zero);
			m_Weapon = null;
		}
	}

	public void PickupWeapon()//Picks up weapon
	{
		if(m_Weapon == null)
		{
			m_Weapon = m_ClosestWeapon;
			m_Weapon.PickUp(m_WeaponAnchor);
			SetActionDistance();
		}
	}

	public void PickupWeapon(Weapon weapon)//Overload for picking up a specified weapon
	{
		m_Weapon = weapon;
		m_Weapon.PickUp(m_WeaponAnchor);
		SetActionDistance();
	}

	private Weapon FindClosestWeapon()//Gets closest weapon in range, checks if enemy can see it or if it it held by another character
	{
		float closestDistance = 1000;

		Weapon weapon = null;

		Weapon[] weapons = FindObjectsOfType<Weapon>();
		for (int i = 0; i < weapons.Length; i++)
		{
			if(!Physics.Linecast(transform.position + Vector3.one, weapons[i].transform.position + Vector3.one))
			{
				if (weapons[i].transform.parent == null)
				{
					float distance = Vector3.Distance(transform.position, weapons[i].transform.position);
					if (distance < closestDistance)
					{
						closestDistance = distance;
						weapon = weapons[i];
					}
				}
			}
		}

		//Debug.Log(weapons[weaponToUse].name);
		return weapon;
	}
}
