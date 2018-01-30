using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageDuplicator : MonoBehaviour
{
	private Image m_ThisImage;
	private Image m_NewImage;

	[SerializeField] private Color m_Color = Color.white;
	private Color m_NewColor;
	[SerializeField] private Vector3 m_Offset;

	private void Awake()
	{
		if(transform.parent.GetComponent<ImageDuplicator>() != null)
		{
			Destroy(this);
		}
		if(transform.parent.GetComponent<Animation>() != null)
		{
			Destroy(GetComponent<Animation>());
		}

		m_NewColor = GetComponent<Image>().color;
	}

	private void Start()
	{
		m_ThisImage = GetComponent<Image>();
		m_NewImage = Instantiate(m_ThisImage, transform);

		if (m_NewImage.transform.childCount > 0)
		{
			foreach (Transform child in m_NewImage.transform)
			{
				Destroy(child.gameObject);
			}
		}

		m_NewImage.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
		m_NewImage.rectTransform.pivot = m_ThisImage.rectTransform.pivot;
		m_NewImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
		m_NewImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
		m_NewImage.rectTransform.anchoredPosition3D = m_Offset;

		m_ThisImage.color = m_Color;
		m_NewImage.color = m_NewColor;
	}

	private void Update()
	{
		m_NewImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_ThisImage.rectTransform.rect.width);
		m_NewImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_ThisImage.rectTransform.rect.height);
	}
}
