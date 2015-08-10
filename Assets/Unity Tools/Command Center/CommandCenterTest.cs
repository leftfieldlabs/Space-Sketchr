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
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CommandCenter))]

/// <summary>
/// This is an example of how to use command
/// center to handle console commands.
/// </summary>
public class CommandCenterTest : MonoBehaviour
{
    private CommandCenter m_commandCenter;

	/// <summary>
	/// Exits the application.
	/// </summary>
	/// <param name="arguments"> Contains any arguments supplied
	/// by the command center.</param>
	public void ExitApplication(string[] arguments) 
	{
			Application.Quit();
	}

	/// <summary>
	/// Resets the application.
	/// </summary>
	/// <param name="arguments"> Contains any arguments supplied
	/// by the command center.</param>
	public void ResetApplication(string[] arguments)
	{
			Application.LoadLevel(Application.loadedLevelName);
	}

	/// <summary>
	/// Get a reference to the command center.
	/// </summary>
	private void Awake()
	{
		m_commandCenter = GetComponent<CommandCenter>();
	}

    /// <summary>
    /// Add commands when enabled.
    /// </summary>
	private void OnEnable()
    {
        if (m_commandCenter != null)
        {
            m_commandCenter.AddNewCommand("exit", ExitApplication);
            m_commandCenter.AddNewCommand("reset", ResetApplication);
        }
	}

    /// <summary>
    /// Remove commands when disabled.
    /// </summary>
    private void OnDisable()
    {
        if (m_commandCenter != null)
        {
            m_commandCenter.RemoveCommand("exit");
            m_commandCenter.RemoveCommand("reset");
        }
    }
}
