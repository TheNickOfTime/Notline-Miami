using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
	private static Texture2D[] cursorTextures;

	private void Awake()
	{
		cursorTextures = new Texture2D[System.Enum.GetValues(typeof(CursorType)).Length];
		cursorTextures[0] = null;
		cursorTextures[1] = Resources.Load("Crosshair") as Texture2D;
	}

	public static void ChangeCursorType(CursorType cursorType)
	{
		Cursor.SetCursor(cursorTextures[1], Vector2.zero, CursorMode.Auto);
	}
}
