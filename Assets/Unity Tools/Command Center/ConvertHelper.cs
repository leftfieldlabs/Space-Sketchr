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
using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Helper class for common conversions.
/// </summary>
public class ConvertHelper
{
	public static bool GetIntFromString(string text, out int value)
	{
		bool wasSuccessful = false;
		int intValue = 0;

		try
		{
			intValue = Convert.ToInt32(text);
		}
		catch (FormatException e)
		{
			Debug.Log("ConvertHelper.GetIntFromString() FormatException : " + e.Message);
		}
		catch (OverflowException e)
		{
			Debug.Log("ConvertHelper.GetIntFromString() OverflowException : " + e.Message);
		}
		finally
		{
			wasSuccessful = true;
			value = intValue;
		}

		return wasSuccessful;
	}
}
