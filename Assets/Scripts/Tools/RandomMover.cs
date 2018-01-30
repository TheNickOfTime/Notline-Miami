using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMover : MonoBehaviour
{
	[SerializeField] private Vector3 m_Direction;
	[SerializeField] private float m_SpeedMin;
	[SerializeField] private float m_SpeedMax;
	[SerializeField] private float m_LoopAt;

	private float m_Speed;
	private Vector3 m_StartPos;

	private void Awake()
	{
		m_Speed = Random.Range(m_SpeedMin, m_SpeedMax);
	}

	private void Update()
	{
		transform.position += m_Direction * m_Speed;

		if(Vector3.Distance(Vector3.zero, transform.position) > m_LoopAt)
		{
			transform.position = Vector3.zero;
		}
	}
}
