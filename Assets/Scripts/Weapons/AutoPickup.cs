using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPickup : MonoBehaviour
{
	[SerializeField] private Weapon m_Weapon;
	private Character_Enemy m_Enemy;

	private void Start()
	{
		m_Enemy = GetComponent<Character_Enemy>();

		if(m_Weapon == null)
		{
			GameObject[] weapons = new GameObject[]
			{
				Resources.Load("Weapons/Weapon_Pistol") as GameObject,
				Resources.Load("Weapons/Weapon_Rifle") as GameObject,
				Resources.Load("Weapons/Weapon_ShotGun") as GameObject,
			};
			int randomNum = Random.Range(0, weapons.Length);

			m_Weapon = Instantiate(weapons[randomNum]).GetComponent<Weapon>();
		}

		m_Enemy.PickupWeapon(m_Weapon);
	}
}
