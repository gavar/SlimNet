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

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SlimNetConsole : MonoBehaviour
{
    public class LogAdapter : SlimNet.ILogAdapter
    {
        SlimNetConsole console = (SlimNetConsole)Behaviour.FindObjectOfType(typeof(SlimNetConsole));

        public void Log(SlimNet.Log.LogEvent @event)
        {
            if (console == null)
            {
                Debug.LogError("Could not find any SlimNetConsole script");
                return;
            }

            console.WriteLine(@event.Message);
        }
    }

    [SerializeField]
    int maxLines = 250;

    [SerializeField]
    bool visible = true;

    [SerializeField]
    Font font;

    [SerializeField]
    int fontSize = 11;

    [SerializeField]
    float consoleHeight = 0.5f;

    [SerializeField]
    KeyCode toggleKey = KeyCode.Tab;

    [SerializeField]
    bool autoFocus = true;

    [SerializeField]
    Texture2D backgroundTexture;

    [SerializeField]
    Color textColor = new Color(114, 208, 255);

    int screenWidth;
    float totalHeight = 0f;

    GUIStyle textStyle;
    GUIStyle inputStyle;
    GUIStyle boxStyle;

    string text = "";
    string input = "";

    Vector2 scrollPos = Vector2.zero;

    LinkedList<string> lines = new LinkedList<string>();
    StringBuilder buffer = new StringBuilder(1024 * 64);
    Dictionary<string, Action<string>> commands = new Dictionary<string, Action<string>>();

    public void WriteLine(string text, params object[] args)
    {
        lines.AddLast(String.Format(text, args));

        if (lines.Count > maxLines)
        {
            lines.RemoveFirst();
        }

        updateBuffer();
    }

    public void RegisterCommand(string command, Action<string> act)
    {
        commands.Add(command, act);
    }

    void updateBuffer()
    {
        Start();

        buffer.Remove(0, buffer.Length);

        foreach (string line in lines)
        {
            buffer.AppendLine(line);
        }

        text = buffer.ToString().Trim();
        totalHeight = textStyle.CalcHeight(new GUIContent(text), Screen.width - 25);
        screenWidth = Screen.width;

        scrollPos = new Vector2(0, totalHeight);
    }

    void Start()
    {
        if (textStyle == null)
        {
            consoleHeight = Mathf.Clamp01(consoleHeight);

            textStyle = new GUIStyle();
            textStyle.fontSize = fontSize;
            textStyle.font = font;
            textStyle.wordWrap = true;
            textStyle.normal.textColor = textColor;

            inputStyle = new GUIStyle();
            inputStyle.fontSize = textStyle.fontSize;
            inputStyle.font = textStyle.font;
            inputStyle.normal.textColor = textStyle.normal.textColor;
        }
    }

    void evalCommand(string command)
    {
        command = (command ?? "").Trim();

        if (command.Length > 0)
        {
            if (command[0] != '/')
            {
                WriteLine("Invalid command '{0}'", command);
                return;
            }
        }
    }

    void OnGUI()
    {
        if (boxStyle == null)
        {
            boxStyle = new GUIStyle("box");

            if (backgroundTexture != null)
            {
                boxStyle.padding = new RectOffset(0, 0, 0, 0);
                boxStyle.fixedHeight = 0;
                boxStyle.fixedWidth = 0;
                boxStyle.stretchHeight = true;
                boxStyle.stretchWidth = true;
                boxStyle.normal.background = backgroundTexture;
            }
        }

        if (Event.current.Equals(Event.KeyboardEvent(toggleKey.ToString())))
        {
            visible = !visible;
        }

        if (screenWidth != Screen.width)
        {
            updateBuffer();
        }

        if (visible)
        {
            int height = (int)((float)Screen.height * consoleHeight);

            GUI.Box(new Rect(0, 0, Screen.width, height), "", boxStyle);

            scrollPos = GUI.BeginScrollView(new Rect(0, 0, Screen.width, height - 25), scrollPos, new Rect(0, 0, Screen.width - 25, totalHeight));
            GUI.TextArea(new Rect(5, 5, screenWidth - 5, totalHeight), text, textStyle);
            GUI.EndScrollView();
            
            if (Event.current.Equals(Event.KeyboardEvent("return")))
            {
                if (input.Length > 0)
                {
                    evalCommand(input);
                }

                input = "";
            }

            GUI.Label(new Rect(5, height - 20, 10, 25), ">", inputStyle);

            if (autoFocus)
            {
                GUI.SetNextControlName("SlimNetConsoleInput");
            }

            input = GUI.TextField(new Rect(15, height - 20, screenWidth - 15, 25), input, inputStyle);

            if (autoFocus)
            {
                GUI.FocusControl("SlimNetConsoleInput");
            }
        }
    }
}
