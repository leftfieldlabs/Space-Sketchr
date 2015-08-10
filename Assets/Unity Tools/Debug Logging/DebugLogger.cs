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
#pragma warning disable 0414
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To wrap the Debug.Log functionality of Unit and add support for different
/// levels of information. This allows levels of debugging to be toggled to 
/// prevent spamming of the console. The defines for the different levels of 
/// debug information can be found in the 'smcs.rsp' file in the root of the 
/// Assets folder. After adding/removing defines from the smcs.rsp file you
/// must save the scene for the changes to take effect.
/// </summary>
public class DebugLogger : MonoBehaviour
{
    private const string WARN_MESSAGE = "Warning : ";
    private const string ERROR_MESSAGE = "Error : ";
    private const string CRITICAL_MESSAGE = "Critical : ";
    private const string INFO_MESSAGE = "Information : ";

    private static DebugLogger instance;
    private List<LogDescriptor> m_currentLogs;

    /// <summary>
    /// Object that describes anything logged to the screen.
    /// </summary>
    public class LogDescriptor
    {
        private string output;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="exceptionText"> Text for this descriptor.</param>
        public LogDescriptor(string exceptionText)
        {
            output = exceptionText;
        }

        /// <summary>
        /// Draws the descriptor at the position provided.
        /// </summary>
        /// <param name="screenPosition"> Position to draw the descriptor.</param>
        public void OnGUI(Rect screenPosition)
        {
            GUI.Label(screenPosition, output);
        }
    }

    /// <summary>
    /// Each enumerated value should be a power of two. These
    /// represent the different debug levels being logged.
    /// </summary>
    public enum EDebugLevel
    {
        DEBUG_WARN = 0x1,
        DEBUG_ERROR = 0x2,
        DEBUG_CRITICAL = 0x4,
        DEBUG_INFO = 0x8
    }

    /// <summary>
    /// Get the singleton instance.
    /// </summary>
    /// <value> Read only DebugLogger.</value>
    public static DebugLogger GetInstance
    {
        get
        {
            if (instance == null)
            {
                GameObject newSingleton = new GameObject("DebugLogger");
                newSingleton.AddComponent<DebugLogger>();

                DontDestroyOnLoad(newSingleton);
            }

            return instance;
        }
    }

    /// <summary>
    /// First call to this object (constructor).
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Initialization.
    /// </summary>
    private void Start()
    {
        m_currentLogs = new List<LogDescriptor>();
    }

    /// <summary>
    /// Function responsible for writing to the debug log.
    /// </summary>
    /// <param name="debugLevel"> Debug level of this message, from enum EDebugLevel.</param>
    /// <param name="incomingMessage"> The message to write.</param>
    public void WriteToLog(EDebugLevel debugLevel, object incomingMessage)
    {
#if DEBUG_LEVEL_WARN
        if ((debugLevel & EDebugLevel.DEBUG_WARN) != 0)
        {
            Debug.Log(WARN_MESSAGE + incomingMessage);
            LogDescriptor newException = new LogDescriptor(WARN_MESSAGE + incomingMessage);
            if (m_currentLogs != null)
            {
                m_currentLogs.Add(newException);
            }
        }
#endif //DEBUG_LEVEL_WARN

#if DEBUG_LEVEL_ERROR
        if ((debugLevel & EDebugLevel.DEBUG_ERROR) != 0)
        {
            Debug.Log(ERROR_MESSAGE + incomingMessage);
            LogDescriptor newException = new LogDescriptor(ERROR_MESSAGE + incomingMessage);
            if (m_currentLogs != null)
            {
                m_currentLogs.Add(newException);
            }
        }
#endif //DEBUG_LEVEL_ERROR

#if DEBUG_LEVEL_CRITICAL
        if ((debugLevel & EDebugLevel.DEBUG_CRITICAL) != 0)
        {
            Debug.Log(CRITICAL_MESSAGE + incomingMessage);
            LogDescriptor newException = new LogDescriptor(CRITICAL_MESSAGE + incomingMessage);
            if (m_currentLogs != null)
            {
                m_currentLogs.Add(newException);
            }
        }
#endif //DEBUG_LEVEL_CRITICAL

#if DEBUG_LEVEL_INFO
        if ((debugLevel & EDebugLevel.DEBUG_INFO) != 0)
        {
            Debug.Log(INFO_MESSAGE + incomingMessage);
            LogDescriptor newException = new LogDescriptor(INFO_MESSAGE + incomingMessage);
            if (m_currentLogs != null)
            {
                m_currentLogs.Add(newException);
            }
        }
#endif //DEBUG_LEVEL_INFO
    }

    /// <summary>
    /// Destructor.
    /// </summary>
    private void OnDisable()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Draws logs to the screen.
    /// </summary>
    private void OnGUI()
    {
#if DRAW_LOGS_TO_SCREEN
        Rect drawOffset = new Rect(25, 600, 200, 50);

        if (m_currentLogs != null)
        {
            int numEntries = m_currentLogs.Count;
            int numDrawn = 0;
            for (int i = numEntries - 1; i >= 0; --i)
            {
                m_currentLogs[i].OnGUI(drawOffset);
                drawOffset.y -= 50;

                if (drawOffset.y < 100)
                {
                    break;
                }
                else
                {
                    numDrawn++;
                }
            }

            if (numDrawn < numEntries)
            {
                m_currentLogs.RemoveRange(0, numEntries - numDrawn);
            }
        }
#endif
    }
}
