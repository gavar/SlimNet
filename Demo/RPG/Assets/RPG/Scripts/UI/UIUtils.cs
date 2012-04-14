/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmström
 * 
 * This notice may not be removed or altered.
 * 
 * This software is provided 'as-is', without any expressed or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software. 
 * 
 * Attribution
 * The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. For any works using this 
 * software, reasonable acknowledgment is required.
 * 
 * Noncommercial
 * You may not use this software for commercial purposes.
 * 
 * Distribution
 * You are not allowed to distribute or make publicly available the software 
 * itself or its source code in original or modified form.
 */

using UnityEngine;
using System.Collections;

public static class UIUtils
{
    public static Rect Center(float width, float height, float widthOffset, float heightOffset)
    {
        return new Rect(
            (GameSettings.UIHalfWidth - (width / 2f)) + widthOffset,
            (GameSettings.UIHalfHeight - (height / 2f)) + heightOffset,
            width,
            height
        );
    }

    public static GUIStyle ErrorLabel
    {
        get
        {
            GUIStyle style = new GUIStyle("Label");
            style.normal.textColor = Color.red;
            return style;
        }
    }
}