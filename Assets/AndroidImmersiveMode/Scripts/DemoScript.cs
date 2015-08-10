using UnityEngine;
using System.Collections;

public class DemoScript : MonoBehaviour {
	
	public GUISkin guiSkin;
	
	void OnGUI()
	{
		GUI.TextArea(new Rect(Screen.width/2 - 100, 50, 200,30), "Click Here", guiSkin.textArea);
	}
}
