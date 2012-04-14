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
using SlimNet;
using UnityEngine;

public class SlimNetClientProxy : UnityEngine.MonoBehaviour
{
    static readonly Log log = Log.GetLogger(typeof(SlimNetClientProxy));

    [SerializeField]
    string host = "127.0.0.1";

    [SerializeField]
    int port = 14000;

    [SerializeField]
    bool connectOnStart = false;

    [SerializeField]
    bool saveBetweenScenes = true;

    [SerializeField]
    bool showStats = true;

    [SerializeField]
    bool showSpatialPartitions = false;

    public string Host { get { return host; } }
    public int Port { get { return port; } }
    public SlimNet.Unity.Client Instance { get; private set; }

    void Start()
    {
        if (saveBetweenScenes)
        {
            DontDestroyOnLoad(gameObject);
        }

        // Initialize log adapter
        Log.SetAdapter(new SlimNetConsole.LogAdapter());

        // Create client
        Instance = SlimNet.Unity.Client.Create();

        if (connectOnStart)
        {
            Connect(Host, Port);
        }
    }

    void Update()
    {
        if (Instance != null)
        {
            try
            {
                Instance.Update(Time.deltaTime);
            }
            catch (Exception exn)
            {
                
                exn = exn.GetBaseException();

                log.Debug(exn.Message);
                log.Debug(exn.StackTrace);
            }
        }
    }

    void OnApplicationQuit()
    {
        if (Instance != null)
        {
            Instance.Disconnect();
        }
    }

    void OnGUI()
    {
        if (Instance != null && showStats)
        {
            GUI.Box(new Rect(Screen.width - 310, Screen.height - 85, 300, 75), "");
            GUI.Window(1024, new Rect(Screen.width - 310, Screen.height - 85, 300, 75), window, "", GUIStyle.none);
        }
    }

    void OnDrawGizmos()
    {
        if (showSpatialPartitions && Instance != null && Instance.Context.HasSpatialPartitioner)
        {
            Instance.Context.SpatialPartitioner.Draw((center, size, color) =>
            {
                Gizmos.color = SlimNet.Unity.Converter.Convert(color);
                Gizmos.DrawWireCube(SlimNet.Unity.Converter.Convert(center), SlimNet.Unity.Converter.Convert(size));
            });
        }
    }

    void window(int _)
    {
        GUILayout.Label("SlimNet (Ping: " + Instance.Ping + "ms)");

        float inKbs = (float)Math.Round(Instance.Context.Stats.BytesInPerSecond / 1024f, 2);
        float outKbs = (float)Math.Round(Instance.Context.Stats.BytesOutPerSecond / 1024f, 2);

        float inEvs = (float)Math.Round(Instance.Context.Stats.EventsInPerSecond, 2);
        float outEvs = (float)Math.Round(Instance.Context.Stats.EventsOutPerSecond, 2);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Bytes");
        GUILayout.Label(" In: " + inKbs + " kb/s");
        GUILayout.Label(" Out: " + outKbs + " kb/s");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Events");
        GUILayout.Label(" In: " + inEvs + " ev/s");
        GUILayout.Label(" Out: " + outEvs + " ev/s");
        GUILayout.EndHorizontal();
    }

    public void Connect(string host, int port)
    {
        if (Instance != null)
        {
            this.host = host;
            this.port = port;

            Instance.Connect(host, port);
        }
    }

    public static void Enable()
    {
        ((SlimNetClientProxy)FindObjectOfType(typeof(SlimNetClientProxy))).enabled = true;
    }
}
