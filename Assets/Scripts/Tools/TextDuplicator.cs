using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDuplicator : MonoBehaviour
{
	private Text m_ThisText;
	private Text m_NewText;

	[SerializeField] private Color m_Color = Color.white;
	private Color m_NewColor;
	[SerializeField] private Vector3 m_Offset;

	private void Awake()
	{
		if(transform.parent.GetComponent<TextDuplicator>() != null)
		{
			Destroy(this);
		}
		if(transform.parent.GetComponent<Animation>() != null)
		{
			Destroy(GetComponent<Animation>());
		}

		m_NewColor = GetComponent<Text>().color;
	}

	private void Start()
	{
		m_ThisText = GetComponent<Text>();
		m_NewText = Instantiate(m_ThisText, transform);
		m_NewText.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
		m_NewText.rectTransform.pivot = m_ThisText.rectTransform.pivot;
		m_NewText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
		m_NewText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
		m_NewText.rectTransform.anchoredPosition3D = m_Offset;

		m_ThisText.color = m_Color;
		m_NewText.color = m_NewColor;
	}

	private void Update()
	{
		m_NewText.text = m_ThisText.text;
		m_NewText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_ThisText.rectTransform.rect.width);
		m_NewText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_ThisText.rectTransform.rect.height);
	}
}
