using UnityEngine;
using System.Collections;

public class ImmersiveModeEnabler : MonoBehaviour {

	AndroidJavaObject unityActivity;
	AndroidJavaObject javaObj;
	AndroidJavaClass javaClass;
	bool paused;
	static bool created;

	void Awake()
	{
		if(!Application.isEditor)
			HideNavigationBar();
		if(!created)
		{
			DontDestroyOnLoad(gameObject);
			created = true;
		}
		else
		{
			Destroy(gameObject); // duplicate will be destroyed if 'first' scene is reloaded
		}
	}
	
	void HideNavigationBar()
	{
		#if UNITY_ANDROID
		lock(this)
		{
			using(javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				unityActivity = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			
			if(unityActivity == null)
			{
				return;
			}
			
			using(javaClass = new AndroidJavaClass("com.rak24.androidimmersivemode.Main"))
			{
				if(javaClass == null)
				{
					return;
				}
				else
				{
					javaObj = javaClass.CallStatic<AndroidJavaObject>("instance");
					if(javaObj == null)
						return;
					unityActivity.Call("runOnUiThread",new AndroidJavaRunnable(() => 
					                                                           {
						javaObj.Call("EnableImmersiveMode", unityActivity);
					}));
				}
			}
		}
		#endif
	}
	
	void OnApplicationPause(bool pausedState)
	{
		paused = pausedState;
	}
	
	void OnApplicationFocus(bool hasFocus)
	{
		if(hasFocus)
		{
			if(javaObj != null && paused != true)
			{
				unityActivity.Call("runOnUiThread",new AndroidJavaRunnable(() => 
						                                                           {
							javaObj.CallStatic("ImmersiveModeFromCache", unityActivity);
						}));
			}
		}
		
	}
	
	public void PinThisApp() // Above android 5.0 - App Pinning
	{
		if(javaObj != null)
		{
			javaObj.CallStatic("EnableAppPin",unityActivity);
		}
	}
	
	public void UnPinThisApp() // Unpin the app
	{
		if(javaObj != null)
		{
			javaObj.CallStatic("DisableAppPin",unityActivity);
		}
	}

}
