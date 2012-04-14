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

using SlimNet;
using UnityEngine;

public class UIPortrait : UIBehaviour
{
    SynchronizedValue<string> playerName;

    [SerializeField]
    Texture image;

    protected override void DrawGUI()
    {
        if (GameSettings.PlayerActor != null)
        {
            if (playerName == null)
            {
                playerName = GameSettings.PlayerActor.GetValue<string>("Name");
            }

            GUIStyle nameplateStyle = new GUIStyle("box");
            nameplateStyle.normal.textColor = Color.green;

            GUI.Box(new Rect(10, 10, 128, 25), playerName.Value, nameplateStyle);
            GUI.Box(new Rect(10, 40, 131, 131), "");

            if (image != null)
            {
                GUI.DrawTexture(new Rect(11, 41, 128, 128), image);
            }
        }
    }
}