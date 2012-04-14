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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using SlimNet;
using SlimNet.Unity;
using UnityEngine;

using Events = SlimNet.Events;
using Network = SlimNet.Network;

public class UIChat : UIBehaviour
{
    string log = "";
    string message = "";
    LinkedList<string> messages = new LinkedList<string>();

    void Start()
    {
        SlimNet.Unity.Client.OnInit((client) =>
        {
            client.Context.ActorEventHandler.RegisterReceiver<ChatMessage>(onChatMessage);
        });
    }

    void onChatMessage(ChatMessage msg)
    {
        string text = msg.Message;

        if (msg.IsLocal)
        {
            text = "You: " + text;
        }

        messages.AddLast(text);

        while (messages.Count > 16)
        {
            messages.RemoveFirst();
        }

        log = string.Join("\n", messages.ToArray());
    }

    protected override void DrawGUI()
    {
        if (GameSettings.PlayerActor != null)
        {
            GUIStyle inputStyle = new GUIStyle("textfield");
            inputStyle.normal.background = null;

            GUIStyle logStyle = new GUIStyle("textarea");
            logStyle.normal.background = null;
            logStyle.focused.background = null;
            logStyle.active.background = null;
            logStyle.hover.background = null;

            float textHeight = logStyle.CalcHeight(new GUIContent(log), 300);

            GUI.Box(new Rect(10, GameSettings.UIHeight - 245, 300, 200), "");
            GUI.BeginScrollView(new Rect(10, GameSettings.UIHeight - 245, 300, 200), new Vector2(0, float.MaxValue), new Rect(10, GameSettings.UIHeight - 245, 300, textHeight));
            GUI.TextArea(new Rect(10, GameSettings.UIHeight - 245, 300, textHeight), log, logStyle);
            GUI.EndScrollView();

            if (Event.current.Equals(Event.KeyboardEvent("return")))
            {
                message = message.Trim();

                if (message.Length > 0)
                {
                    GameSettings.PlayerActor.RaiseEvent<ChatMessage>((ev) =>
                    {
                        ev.Message = message;
                    });
                }

                message = "";
            }

            GUI.Box(new Rect(10, GameSettings.UIHeight - 35, 300, 25), "");
            message = GUI.TextField(new Rect(10, GameSettings.UIHeight - 35, 300, 25), message, inputStyle);
        }
    }
}