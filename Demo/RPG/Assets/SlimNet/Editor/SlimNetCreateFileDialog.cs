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

using UnityEditor;
using UnityEngine;

public class SlimNetCreateFileDialog : EditorWindow
{
    string filename = "";
    System.Action<string> onCreate;

    public static void Open(System.Action<string> onCreate)
    {
        if (onCreate != null)
        {
            SlimNetCreateFileDialog dialog = ScriptableObject.CreateInstance<SlimNetCreateFileDialog>();
            dialog.title = "Create File";
            dialog.maxSize = dialog.minSize = new Vector2(400, 35);
            dialog.onCreate = onCreate;
            dialog.ShowUtility();
        }
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        GUI.SetNextControlName("SlimNetCreateFileDialogInput");
        filename = GUILayout.TextField(filename, GUILayout.Width(310));
        GUI.FocusControl("SlimNetCreateFileDialogInput");

        bool enterPressed = (Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.Return);
        if ((GUILayout.Button("Create", GUILayout.Width(75)) || enterPressed) && !string.IsNullOrEmpty(filename))
        {
            onCreate(filename);
            onCreate = null;
        }

        EditorGUILayout.EndHorizontal();

        if (onCreate == null)
        {
            Close();
        }
    }
}