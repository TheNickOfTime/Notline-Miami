using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInteractions_Canvas : MonoBehaviour
{
	public static UIInteractions_Canvas instance;

	[Header("Config")]
	[SerializeField] protected GameObject canvas;

	[Header("Stats")]
	[SerializeField] protected GameObject currentPanel;
	[SerializeField] protected GameObject currentOverlay;
	[SerializeField] protected List<GameObject> previousPanels;

	protected virtual void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		instance = this;

		currentPanel = canvas.transform.GetChild(0).gameObject;
	}

	public void SetActivePanel(string panelName)
	{
		GameObject newPanel = canvas.transform.Find(panelName).gameObject;
		GameObject oldPanel = currentPanel;

		oldPanel.SetActive(false);
		newPanel.SetActive(true);

		currentPanel = newPanel;
		previousPanels.Insert(0, oldPanel);
	}

	public void SetActivePanelAdditive(string panelName)
	{
		GameObject newPanel = canvas.transform.Find(panelName).gameObject;
		GameObject oldPanel = currentPanel;

		newPanel.SetActive(true);

		currentPanel = newPanel;
		previousPanels.Insert(0, oldPanel);
	}

	public void SetActivePanelPrevious()
	{
		GameObject newPanel = previousPanels[0];
		GameObject oldPanel = currentPanel;

		newPanel.SetActive(true);
		oldPanel.SetActive(false);

		currentPanel = newPanel;
		previousPanels.RemoveAt(0);
	}

	public void SetOverlay(string overlayName)
	{
		if(overlayName != "")
		{
			GameObject newOverlay = canvas.transform.Find(overlayName).gameObject;
			GameObject oldOverlay = currentOverlay;

			newOverlay.SetActive(true);
			if(oldOverlay != null)
			{
				oldOverlay.SetActive(false);
			}

			currentOverlay = newOverlay;
		}
		else
		{
			currentOverlay.SetActive(false);
			currentOverlay = null;
		}
	}
}
