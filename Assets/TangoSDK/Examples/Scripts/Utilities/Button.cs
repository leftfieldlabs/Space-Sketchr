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
/// Base class Touchable is inherited.
/// Texture based effects added in this class.
/// </summary>
public class Button : TouchableObject
{
    public Texture2D m_onTouchTexture;
    public Texture2D m_outTouchTexture;
    protected string m_debugString = string.Empty;

    /// <summary>
    /// Touch event similar to key hold.
    /// Resets texture to show effect.
    /// </summary>
	protected override void OnTouch()
    {
        renderer.material.mainTexture = m_onTouchTexture;
	}

    /// <summary>
    /// Touch event similar to key down.
    /// Resets texture to show effect.
    /// </summary>
	protected override void OutTouch()
    {
        renderer.material.mainTexture = m_outTouchTexture;
	}

    /// <summary>
    /// Touch event similar to key up.
    /// Resets texture to show effect.
    /// </summary>
	protected override void TouchUp()
	{
        renderer.material.mainTexture = m_outTouchTexture;
	}
	
    /// <summary>
    /// Called every frame.
    /// </summary>
    protected override void Update()
	{
		base.Update();
	}
}
