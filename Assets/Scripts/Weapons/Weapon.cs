using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Weapon : MonoBehaviour
{
	//Components & References
	protected Rigidbody m_Rig;

	[SerializeField] protected float m_UseDelay;
	protected bool m_CanUse = true;

	private enum WeaponState
	{
		Static,
		Held,
		Thrown
	}
	private WeaponState m_State;

	protected virtual void Awake()
	{
		m_Rig = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		switch(m_State)
		{
			case WeaponState.Static:
				break;
			case WeaponState.Held:
				break;
			case WeaponState.Thrown:
				if(m_Rig.velocity == Vector3.zero)
				{
					m_State = WeaponState.Static;
				}
				break;
		}
	}

	public virtual void PickUp(Transform parent)
	{
		m_Rig.useGravity = false;
		m_Rig.isKinematic = true;
		transform.parent = parent;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;

		m_State = WeaponState.Held;
	}

	public virtual void Drop(Vector3 throwForce)
	{
		transform.parent = null;
		m_Rig.useGravity = true;
		m_Rig.isKinematic = false;
		m_Rig.AddForce(throwForce * 15, ForceMode.Impulse);

		m_State = WeaponState.Thrown;
	}

	public virtual void Use()
	{
		Debug.Log("Used");
	}

	private void OnCollisionEnter(Collision other)
	{
		Character_Enemy enemy = other.transform.GetComponent<Character_Enemy>();
		if (enemy != null)
		{
			enemy.AddHealth(-3);
		}
	}
}
