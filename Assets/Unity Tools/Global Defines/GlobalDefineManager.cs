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
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Global defines for different environments
// C#	                    <Project Path>/Assets/smcs.rsp
// C# - Editor Scripts	    <Project Path>/Assets/gmcs.rsp
// UnityScript	            <Project Path>/Assets/us.rsp
// Boo	                    <Project Path>/Assets/boo.rsp

/// <summary>
/// Manages Global Defines created with the
/// 'Window/Global Defines' editor window.
/// </summary>
public class GlobalDefineManager
{
    private const string DEFINES_TXT_PATH_EXT = "/Unity Tools/Global Defines/Sync Data/defines.txt";
    private const string SMCS_RSP_PATH_EXT = "/smcs.rsp";
    private const string DEFINES_DIR_EXT = "/Unity Tools/Global Defines/Sync Data";

    private const string REMOVE_TEXT = "Remove";
    private const string ADD_NEW_TEXT = "Add New";
    private const string REFRESH_TEXT = "Refresh";
    private const string ZERO_TEXT = "0";
    private const string ONE_TEXT = "1";
    private const string COLON_TEXT = ":";

    private const string SMCS_DEFINE_PREFIX = "-define:";
    private const string SMCS_DEFINE_PREFIX_NO_COLON = "-define";

    private static GlobalDefineManager instance;
    private List<GlobalDefine> m_allDefines = new List<GlobalDefine>();
    private Vector2 m_scrollPosition;
    private string m_newDefine = string.Empty;

    /// <summary>
    /// Carries all information needed to track active.
    /// global defines.
    /// </summary>
    public class GlobalDefine
    {
        public bool m_isActive;
        public string m_defineName;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="isActive"> True if this define is active.</param>
        /// <param name="defineName"> Name of the define.</param>
        public GlobalDefine(bool isActive, string defineName)
        {
            m_isActive = isActive;
            m_defineName = defineName;
        }
    }

    /// <summary>
    /// Private Constructor for singleton.
    /// </summary>
    private GlobalDefineManager()
    {
    }

    /// <summary>
    /// Get a reference to the GlobalDefineManager.
    /// </summary>
    /// <value> Get a reference.</value>
    public static GlobalDefineManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                instance = new GlobalDefineManager();
            }

            return instance;
        }
    }

    /// <summary>
    /// Initialize the Define Manager and syncronize all defines
    /// across all files.
    /// </summary>
    public void Init()
    {
        Debug.Log("Init");
        List<string> smcsDefines = null;

        // does a smcs.rsp file already exist?
        if (!DoesSMCSExist())
        {
            // create a global define file!
            FileStream fs = File.Create(Application.dataPath + SMCS_RSP_PATH_EXT);
            fs.Close();
        }
        else
        {
            // Get all declared defines from the file
            smcsDefines = GetDefinesFromSMCS();
        }

        if (!DoGlobalDefinesExist())
        {
            // create a global define file!
            Directory.CreateDirectory(Application.dataPath + DEFINES_DIR_EXT);
            FileStream fs = File.Create(Application.dataPath + DEFINES_TXT_PATH_EXT);
            fs.Close();
        }
        else
        {
            PopulateGlobalsFromDefinesTXT();
        }

        // Sync smcs.rsp with defines.txt
        SyncWithSMCS(smcsDefines);

        // Populate the smcs.rsp file with active defines
        Refresh();
    }

    /// <summary>
    /// Makes the Define Manager inactive.
    /// </summary>
    public void Deactivate()
    {
        m_allDefines.Clear();
    }

    /// <summary>
    /// Responsible for drawing the Global defines window.
    /// </summary>
    /// <returns> Return true if all a compilation should be forced.</returns>
    public bool OnGUI()
    {
        GUILayout.BeginVertical();
        {
            // List all current defines
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);
            {
                if (m_allDefines != null & m_allDefines.Count > 0)
                {
                    foreach (GlobalDefine define in m_allDefines)
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.BeginHorizontal();
                            {
                                // Make a button to remove a define
                                if (GUILayout.Button(REMOVE_TEXT, GUILayout.Width(75)))
                                {
                                    // delete this guy!
                                    RemoveDefine(define);
                                    Refresh();
                                    return true;
                                }

                                // Make the defines toggle off/on with a button click
                                Color previousColor = GUI.color;
                                GUI.color = define.m_isActive ? Color.green : Color.red;
                                if (GUILayout.Button(define.m_defineName))
                                {
                                    // toggle this define off and refresh
                                    ToggleDefine(define, !define.m_isActive);
                                    Refresh();
                                    return true;
                                }
                                GUI.color = previousColor;
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                }
            }
            GUILayout.EndScrollView();

            // Add ability to add new defines to the list
            GUILayout.BeginHorizontal();
            {
                m_newDefine = GUILayout.TextField(m_newDefine, GUILayout.Width(150));

                if (GUILayout.Button(ADD_NEW_TEXT))
                {
                    // add new define
                    AddDefine(m_newDefine, true);

                    m_newDefine = string.Empty;
                    Refresh();
                    return true;
                }
                if (GUILayout.Button(REFRESH_TEXT))
                {
                    Refresh();
                    return true;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        return false;
    }

    /// <summary>
    /// Refreshes the list of defines.
    /// </summary>
    private void Refresh()
    {
        AddDefinesToSMCS();
        WriteDefinesToFile();
    }

    /// <summary>
    /// Return all defines from the smcs.rsp file.
    /// </summary>
    /// <returns> List of strings containing all defines from the 
    /// smcs.rsp file.</returns>
    private List<string> GetDefinesFromSMCS()
    {
        List<string> allDefines = new List<string>();

        // Parse the smcs.rsp file for declared defines
        using (StreamReader sr = File.OpenText(Application.dataPath + SMCS_RSP_PATH_EXT))
        {
            string line = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {
                string[] defineText = line.Split(':');
                if (defineText[0] == SMCS_DEFINE_PREFIX_NO_COLON)
                {
                    allDefines.Add(defineText[1]);
                }
            }
        }

        return allDefines;
    }

    /// <summary>
    /// Writes all defines to the defines.txt file.
    /// </summary>
    private void WriteDefinesToFile()
    {
        if (m_allDefines != null)
        {
            using (StreamWriter sw = new StreamWriter(Application.dataPath + DEFINES_TXT_PATH_EXT, false))
            {
                foreach (GlobalDefine define in m_allDefines)
                {
                    string line = (define.m_isActive ? ONE_TEXT : ZERO_TEXT) + COLON_TEXT + define.m_defineName;
                    sw.WriteLine(line);
                }
            }
        }
    }

    /// <summary>
    /// Adds all defines from the smcs.rsp file to the 
    /// list of defines as active defines.
    /// </summary>
    /// <param name="smcsDefines"> List of strings containing
    /// all defines from the smcs.rsp file.</param>
    private void SyncWithSMCS(List<string> smcsDefines)
    {
        if (smcsDefines != null)
        {
            // copy into
            foreach (string define in smcsDefines)
            {
                AddDefine(define, true);
            }
        }
    }

    /// <summary>
    /// Returns all active defines from the defines.txt file.
    /// </summary>
    /// <returns> List of strings containing all active
    /// defines from the defines.txt file.</returns>
    private List<string> GetActiveDefinesFromGlobalDefines()
    {
        List<string> globalDefines = new List<string>();

        using (StreamReader sr = File.OpenText(Application.dataPath + DEFINES_TXT_PATH_EXT))
        {
            string line = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {
                string[] defineText = line.Split(':');
                if (defineText[0] == ONE_TEXT)
                {
                    globalDefines.Add(defineText[1]);
                }
            }
        }

        return globalDefines;
    }

    /// <summary>
    /// Populate the list of defines from the defines.txt file.
    /// </summary>
    private void PopulateGlobalsFromDefinesTXT()
    {
        StreamReader sr = File.OpenText(Application.dataPath + DEFINES_TXT_PATH_EXT);
        string line = string.Empty;
        while ((line = sr.ReadLine()) != null)
        {
            string[] defineText = line.Split(':');
            if (defineText[0] == ONE_TEXT)
            {
                AddDefine(defineText[1], true);
            }
            else if (defineText[0] == ZERO_TEXT)
            {
                AddDefine(defineText[1], false);
            }
        }

        sr.Close();
    }

    /// <summary>
    /// Write out all active defines to the smcs.rsp file.
    /// </summary>
    private void AddDefinesToSMCS()
    {
        if (m_allDefines != null)
        {
            using (StreamWriter sw = new StreamWriter(Application.dataPath + SMCS_RSP_PATH_EXT, false))
            {
                // add support for unsafe code 
                sw.WriteLine("-unsafe");

                foreach (GlobalDefine define in m_allDefines)
                {
                    if (define.m_isActive)
                    {
                        string activeDefine = SMCS_DEFINE_PREFIX + define.m_defineName;
                        sw.WriteLine(activeDefine);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Turn a define off or on.
    /// </summary>
    /// <param name="define"> The define to toggle.</param>
    /// <param name="isActive"> The state to toggle to.</param>
    private void ToggleDefine(GlobalDefine define, bool isActive)
    {
        define.m_isActive = isActive;
    }

    /// <summary>
    /// Add a define to the list of defines.
    /// </summary>
    /// <param name="defineText"> Define text.</param>
    /// <param name="isActive"> True if it is active.</param>
    private void AddDefine(string defineText, bool isActive)
    {
        if (m_allDefines.Exists(x => x.m_defineName == defineText) == false)
        {
            GlobalDefine newDefine = new GlobalDefine(isActive, defineText);
            m_allDefines.Add(newDefine);
        }
    }

    /// <summary>
    /// Remove a define from the list of defines.
    /// </summary>
    /// <param name="define"> The define to remove.</param>
    private void RemoveDefine(GlobalDefine define)
    {
        m_allDefines.Remove(define);
    }

    /// <summary>
    /// Checks to see if the Unity file for handling
    /// custom global defines exists in this project.
    /// </summary>
    /// <returns> True if the file exists. </returns>
    private bool DoesSMCSExist()
    {
        return File.Exists(Application.dataPath + SMCS_RSP_PATH_EXT);
    }

    /// <summary>
    /// Checks to see if our file for handling
    /// global defines exists in this project.
    /// </summary>
    /// <returns> True if defines.txt already exists.</returns>
    private bool DoGlobalDefinesExist()
    {
        return File.Exists(Application.dataPath + DEFINES_TXT_PATH_EXT);
    }
}