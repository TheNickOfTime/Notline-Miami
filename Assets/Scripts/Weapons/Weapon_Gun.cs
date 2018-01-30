using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Gun : Weapon
{
	private UIInteractions_Canvas_Gameplay m_UI;
	private Character m_Char;

	private Transform m_FirePoint;
	private GameObject m_Bullet;

	[SerializeField] private int m_AmmoMax;
	[SerializeField] private int m_AmmoCurrent;
	[SerializeField] private bool m_IsAutomatic;

	private enum GunShotType
	{
		Single,
		Burst,
		Spread
	}
	[SerializeField] private GunShotType m_ShotType;

	protected override void Awake()
	{
		base.Awake();

		m_FirePoint = transform.Find("FirePoint");
		m_Bullet = Resources.Load("Weapons/Bullet") as GameObject;

		m_AmmoCurrent = m_AmmoMax;
	}

	private void Start()
	{
		m_UI = (UIInteractions_Canvas.instance as UIInteractions_Canvas_Gameplay);
	}

	public override void PickUp(Transform parent)
	{
		base.PickUp(parent);

		m_Char = parent.root.GetComponent<Character>();

		if (m_Char is Character_Player)
		{
			m_UseDelay = 0.2f;
		}
		else if(m_Char is Character_Enemy)
		{
			m_UseDelay = 0.5f;
		}

		(UIInteractions_Canvas.instance as UIInteractions_Canvas_Gameplay).SetAmmoText(m_AmmoCurrent, m_AmmoMax);
	}

	public override void Use()
	{
		if(m_AmmoCurrent > 0 && m_CanUse)
		{
			switch (m_ShotType)
			{
				case GunShotType.Single:
					StartCoroutine(SingleShot());
					break;
				case GunShotType.Burst:
					StartCoroutine(BurstShot());
					break;
				case GunShotType.Spread:
					StartCoroutine(ScatterShot());
					break;
			}
		}
	}

	private void SpawnBullet()
	{
		GameObject bullet = Instantiate(m_Bullet, m_FirePoint.position, m_FirePoint.rotation);
		if (m_Char is Character_Player)
		{
			bullet.layer =  LayerMask.NameToLayer("IgnorePlayer");
			m_AmmoCurrent -= 1;
		}
		else if (m_Char is Character_Enemy)
		{
			bullet.layer =  LayerMask.NameToLayer("IgnoreEnemy");
		}
	}

	private void SpawnBullet(Vector3 offset)
	{
		GameObject bullet = Instantiate(m_Bullet, m_FirePoint.position + offset, m_FirePoint.rotation);
		if (m_Char is Character_Player)
		{
			bullet.layer = LayerMask.NameToLayer("IgnorePlayer");
		}
		else if (m_Char is Character_Enemy)
		{
			bullet.layer = LayerMask.NameToLayer("IgnoreEnemy");
		}
	}

	private IEnumerator SingleShot()
	{
		SetShotsFired(1);

		m_CanUse = false;
		SpawnBullet();
		AudioManager.instance.SFXOneShot("Pistol", transform.position);
		m_UI.SetAmmoText(m_AmmoCurrent, m_AmmoMax);
		yield return new WaitForSeconds(m_UseDelay);
		m_CanUse = true;
	}

	private IEnumerator BurstShot()
	{
		m_CanUse = false;

		for (int i = 0; i < 3; i++)
		{
			SetShotsFired(1);
			SpawnBullet();
			AudioManager.instance.SFXOneShot("Rifle", transform.position);
			m_UI.SetAmmoText(m_AmmoCurrent, m_AmmoMax);
			yield return new WaitForSeconds(0.1f);
		}

		m_CanUse = true;
	}

	private IEnumerator ScatterShot()
	{
		SetShotsFired(1);

		m_CanUse = false;
		for (int i = 0; i < 7; i++)
		{
			Vector3 randomOffset = new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y) * 0.5f;
			SpawnBullet(randomOffset);
		}
		m_AmmoCurrent -= 1;
		AudioManager.instance.SFXOneShot("Shotgun", transform.position);
		m_UI.SetAmmoText(m_AmmoCurrent, m_AmmoMax);
		yield return new WaitForSeconds(m_UseDelay);
		m_CanUse = true;
	}

	private void SetShotsFired(int amount)
	{
		if (m_Char is Character_Player)
		{
			StatSetter.IncrimentStat("ShotsFired", amount);
		}
	}

	public bool GetIsAutomatic()
	{
		return m_IsAutomatic;
	}
}
