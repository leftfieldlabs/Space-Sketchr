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
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Signature for functions called by command center.
/// </summary>
/// <param name="arguments"> An array of arguments passed to a command.</param>
public delegate void Command(string[] arguments);

/// <summary>
/// Command center is responsible for presenting the user
/// with a console window and keyboard to call functionality
/// from within the current application.
/// </summary>
public class CommandCenter : MonoBehaviour 
{
	private const int NUMBER_OF_TOUCHES_TO_ACTIVATE = 4;
    
    private Dictionary<string, Command> m_commands;
    private string m_currentCommand;
    private TouchScreenKeyboard m_keyboard;
    private bool m_commandWasRun;

    /// <summary>
    /// Add a new Command with a key used to call the command
    /// from the command center.
    /// </summary>
    /// <returns> Returns true, if new command was added, false otherwise.</returns>
    /// <param name="key"> Key used to call the command.</param>
    /// <param name="value"> Value, the command to be called.</param>
    public bool AddNewCommand(string key, Command value)
    {
		if (m_commands == null) 
		{
			m_commands = new Dictionary<string, Command>();
		}

        if (key != string.Empty && value != null)
        {
            if (m_commands.ContainsKey(key) == false)
            {
                m_commands.Add(key, value);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Removes a command using it's key.
    /// </summary>
    /// <returns> Returns true, if command was removed, false otherwise.</returns>
    /// <param name="key">Key used to find the command to remove.</param>
    public bool RemoveCommand(string key)
    {
        return m_commands.Remove(key);
    }

    /// <summary>
    /// Initialize members.
    /// </summary>
	private void Awake()
	{
		if (m_commands == null) 
		{
			m_commands = new Dictionary<string, Command>();
		}
        m_currentCommand = string.Empty;
        m_commandWasRun = false;
	}

    /// <summary>
    /// Perform logic associated with opening the command
    /// center, gathering keyboard input, and calling the
    /// appropriate command based on the arguments provided.
    /// </summary>
	private void Update()
    {
        if (m_keyboard == null || (m_keyboard.active == false && m_commandWasRun))
        {
            if (Input.touchCount == NUMBER_OF_TOUCHES_TO_ACTIVATE)
            {
                m_currentCommand = string.Empty;
                m_keyboard = TouchScreenKeyboard.Open(m_currentCommand, TouchScreenKeyboardType.Default, false);
                m_commandWasRun = false;
            } 
        }
        else
        {
            if (m_keyboard.done)
            {
                Command command;

                // This tells us how to split arguments
                char[] deliminators = { ' ' };

                string[] fullCommand = m_currentCommand.Split(deliminators);

                // Get the command key.
                string commandWithoutArgs = fullCommand[0];
               
                // Copy the remaining arguments into a buffer
                string[] commandArgs = null;
                int numberOfCommands = fullCommand.Length - 1;
                if (numberOfCommands > 0)
                {
                    commandArgs = new string[numberOfCommands];
                    for (int i = 0; i < numberOfCommands; ++i)
                    {
                        commandArgs[i] = fullCommand[i + 1];
                    }
				}
				DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_INFO, 
				                                   "Attempting to call command : " + commandWithoutArgs);
                // Call the delegate handler here!
                if (m_commands.TryGetValue(commandWithoutArgs, out command))
                {
                    if (command != null)
                    {
                        command(commandArgs);
                    }
                    else
                    {
                        DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_WARN, 
                                                           "Command is not valid!");
                    }
                }
                m_commandWasRun = true;
            }
            m_currentCommand = m_keyboard.text;
        }
    }
}