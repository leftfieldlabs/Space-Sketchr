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

/// <summary>
/// This should serve as an example of how to use the
/// tools provided in this project.
/// </summary>
public class ToolExample : MonoBehaviour
{
    /// <summary>
    /// Demonstrate the functionality of the global defines extension
    /// and the improved debug logging.
    /// </summary>
    void Update ()
    {
        // If you would like to draw logs to the screen of your device
        // make sure DRAW_LOGS_TO_SCREEN is enabled. It will still show
        // logs in the Console window, even with this enabled.

        // by toggling the defines for each debug priority you can
        // enable/disable logging for a specific type

        //DEBUG_LEVEL_INFO
        DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_INFO, "I am info!");

        //DEBUG_LEVEL_ERROR
        DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_ERROR, "I am an error!");

        //DEBUG_LEVEL_WARN
        DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_WARN, "I am a warning!");

        //DEBUG_LEVEL_CRITICAL
        DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_CRITICAL, "I am CRITICAL!");
    }

    private void OnGUI()
    {
        // This is just a demonstration of how to use the global defines
        // anywhere, just like you would normally use a define in a single file.
        // To show this message just make this define in the global define editor window!
#if GLOBAL_DEFINES_YAY
        GUI.Label(new Rect(Screen.width - 250, 100, 200, 50), "Yay for global defines!");
#endif //GLOBAL_DEFINES_YAY
    }
}
