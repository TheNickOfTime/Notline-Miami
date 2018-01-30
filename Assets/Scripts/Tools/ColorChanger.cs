using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
	[SerializeField] private Component[] m_Component;

	[SerializeField] private Color[] m_Colors;
	[SerializeField] private float m_ShiftTime = 1;

	private int m_CurrentColor;
	private int m_PrevColor;

	private void Start()
	{
		StartCoroutine(ShiftColor());
	}

	private IEnumerator ShiftColor()
	{
		NextColor();

		float timer = 0;
		while (timer < m_ShiftTime)
		{
			timer += Time.deltaTime;

			foreach (Component comp in m_Component)
			{
				if (comp is Image)
				{
					(comp as Image).color = Color.Lerp(m_Colors[m_PrevColor], m_Colors[m_CurrentColor], timer / m_ShiftTime);
				}
				else if (comp is Text)
				{
					(comp as Text).color = Color.Lerp(m_Colors[m_PrevColor], m_Colors[m_CurrentColor], timer / m_ShiftTime);
				}
				else if(comp is Camera)
				{
					(comp as Camera).backgroundColor = Color.Lerp(m_Colors[m_PrevColor], m_Colors[m_CurrentColor], timer / m_ShiftTime);
				}
			}

			yield return null;
		}

		yield return ShiftColor();
	}

	private void NextColor()
	{
		m_PrevColor = m_CurrentColor;
		m_CurrentColor += 1;

		if(m_CurrentColor >= m_Colors.Length)
		{
			m_CurrentColor = 0;
		}
	}
}
