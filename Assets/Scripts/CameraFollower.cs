using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
	public static CameraFollower instance;

	[SerializeField] private float m_FollowSpeed = 1;

	private Character_Player m_Player;

	private Vector3 target;

	private void Awake()
	{
		//Singleton
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}

		enabled = false;
	}

	private void Start()
	{
		m_Player = Character_Player.instance;
	}

	private void Update()
	{
		if (!Input.GetKey(KeyCode.LeftShift))
		{
			target = new Vector3(m_Player.transform.position.x, transform.position.y, m_Player.transform.position.z);
			transform.position = target;
		}
		else
		{
			target = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, transform.position.y, Camera.main.ScreenToWorldPoint(Input.mousePosition).z);
			if (Vector3.Distance(new Vector3(m_Player.transform.position.x, 0, m_Player.transform.position.z), new Vector3(target.x, 0, target.z)) < 10)
			{
				transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime);
			}
		}
	}
}
