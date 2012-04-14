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

public static class RPGControllerUtils
{
    public static float SignedAngle(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
    }

    public static bool GetButtonSafe(string name, bool @default)
    {
        try
        {
            return Input.GetButton(name);
        }
        catch
        {
            Debug.LogError("The button '" + name + "' isn't defined in the input manager");
            return @default;
        }
    }

    public static bool GetButtonDownSafe(string name, bool @default)
    {
        try
        {
            return Input.GetButtonDown(name);
        }
        catch
        {
            Debug.LogError("The button '" + name + "' isn't defined in the input manager");
            return @default;
        }
    }

    public static float GetAxisRawSafe(string name, float @default)
    {
        try
        {
            return Input.GetAxisRaw(name);
        }
        catch
        {
            Debug.LogError("The axis '" + name + "' isn't defined in the input manager");
            return @default;
        }
    }
}
