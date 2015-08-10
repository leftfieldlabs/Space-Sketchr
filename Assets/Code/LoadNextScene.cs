using UnityEngine;
using System.Collections;

public class LoadNextScene : MonoBehaviour
{
	public string sceneToLoad = "MainScene";
	public GameObject destroyOnLoad;
	public float notificationLength = 0.01f;

	// Use this for initialization
	void Start () {
		Invoke ( "GoToNextScene", notificationLength );
	}
	
	// Update is called once per frame
	void GoToNextScene ()
	{
		Application.LoadLevelAdditive( sceneToLoad );
		Destroy ( destroyOnLoad );
		Destroy ( this.gameObject );
	}
}
