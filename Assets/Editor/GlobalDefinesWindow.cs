/*
 * Copyright 2014 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using UnityEditor;

/// <summary>
/// Editor GUI for global defines.
/// </summary>
public class GlobalDefinesWindow : EditorWindow
{
    /// <summary>
    /// Tell the window to show itself.
    /// </summary>
    [MenuItem("Window/Global Defines")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GlobalDefinesWindow), false, "Global Defines");
    }

    /// <summary>
    /// Upon enable initialize the GlobalDefineManager.
    /// </summary>
    private void OnEnable()
    {
        GlobalDefineManager.GetInstance.Init();
    }

    /// <summary>
    /// When destroyed deactive the GlobalDefineManager.
    /// </summary>
    private void OnDisable()
    {
        GlobalDefineManager.GetInstance.Deactivate();
    }

    /// <summary>
    /// Force a recompilation of all scripts.
    /// </summary>
    private void ForceRecompile()
    {
        AssetDatabase.StartAssetEditing();
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in allAssetPaths)
        {
            MonoScript script = AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript)) as MonoScript;
            if (script != null)
            {
                AssetDatabase.ImportAsset(assetPath);
            }
        }
        AssetDatabase.StopAssetEditing();
    }
    
    /// <summary>
    /// Draw all global defines in the window.
    /// </summary>
    private void OnGUI()
    {
        // if the GlobalDefineManager returns true here
        // something was changed and we should force a
        // recompile
        if (GlobalDefineManager.GetInstance.OnGUI())
        { 
            ForceRecompile();
        }
    }
}