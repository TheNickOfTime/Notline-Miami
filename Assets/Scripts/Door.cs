using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
	private Rigidbody m_Rig;
	private HingeJoint m_Joint;

	private void Awake()
	{
		m_Rig = GetComponent<Rigidbody>();
		m_Joint = GetComponent<HingeJoint>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(m_Joint.velocity > 100 || m_Joint.velocity < -100)
		{
			if (collision.transform.root.GetComponent<Character_Enemy>() != null)
			{
				AudioManager.instance.PlaySFX("Punch_Hit");
				collision.transform.root.GetComponent<Character_Enemy>().SetState(EnemyState.Unconscious);
			}
		}
	}
}
