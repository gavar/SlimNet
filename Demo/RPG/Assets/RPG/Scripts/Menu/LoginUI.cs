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

public class LoginUI : UIBehaviour
{
    string error = "";
    string message = "";
    string username = "";
    string password = "";

    void Start()
    {
        SlimNet.Unity.Client.OnInit((client) =>
        {
            client.Context.PlayerEventHandler.RegisterReceiver<SlimNet.Events.Authenticated>(onLoginResult);
        });
    }

    protected override void DrawGUI()
    {
        GUI.Window(0, UIUtils.Center(300, 400, 0, 0), window, "Login");
    }

    void onLoginResult(SlimNet.Events.Authenticated ev)
    {
        if (ev.IsAuthenticated)
        {
            Application.LoadLevel(1);
        }
        else
        {
            error = ev.Error;
            message = "";
        }
    }

    void onAccountCreateResult(string result)
    {
        error = result;
        message = error == "" ? "Account created" : "";
    }

    void window(int id)
    {
        GUILayout.Label("Account");
        username = GUILayout.TextField(username);

        GUILayout.Label("Password");
        password = GUILayout.PasswordField(password, '*');

        if (GUILayout.Button("Login") && username != "" && password != "")
        {
            RPC.Login.Invoke(username, password);
        }

        if (GUILayout.Button("Create New Account") && username != "" && password != "")
        {
            RPC.CreateAccount.InvokeOnServer(username, password).OnComplete += onAccountCreateResult;
        }

        if (error != "")
        {
            GUILayout.Label(error, UIUtils.ErrorLabel);
        }
        else
        {
            GUILayout.Label(message);
        }
    }
}