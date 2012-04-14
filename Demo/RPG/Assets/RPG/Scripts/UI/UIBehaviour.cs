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

public class UIBehaviour : MonoBehaviour
{
    void OnGUI()
    {
        // We only wanna show UIs if we're connected
        if (SlimNet.Unity.Client.Instance != null && SlimNet.Unity.Client.Instance.Connected)
        {
            GUI.matrix = UIMatrix;
            DrawGUI();
        }
    }

    public static Matrix4x4 UIMatrix
    {
        get
        {
            return Matrix4x4.TRS(
                Vector3.zero, 
                Quaternion.identity, 
                new Vector3(
                    Screen.width / GameSettings.UIWidth, 
                    Screen.height / GameSettings.UIHeight, 
                    1f
                )
            );
        }
    }

    protected virtual void DrawGUI()
    {

    }
}
