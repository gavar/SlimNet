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