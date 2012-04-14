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
using System.IO;
using SlimNet;
using SlimNet.Unity;
using SlimNet.Unity.Editor;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SlimNetConfigurationTab : EditorWindow
{
    static ClientConfiguration ccfg;
    static ServerConfiguration scfg;

    static SlimNetConfigurationTab()
    {
        Log.SetAdapter(new SlimNet.LogNullAdapter()); 
    }

    public static ServerConfiguration ServerConfiguratin
    {
        get
        {
            initConfiguration();
            return scfg;
        }
    }

    public static void WriteCopy(string filename, string data)
    {
        initConfiguration();

        if (ccfg != null)
        {
            SlimNetEditorSettings.Instance.VerifyServerFolderExists();
            File.WriteAllText(SlimNetEditorSettings.Instance.ServerFolder + Path.DirectorySeparatorChar + filename, data);
        }
    }

    [MenuItem("SlimNet/Configuration")]
    static void init()
    {
        SlimNetConfigurationTab window = (SlimNetConfigurationTab)EditorWindow.GetWindow(typeof(SlimNetConfigurationTab));
        window.title = "SlimNet";
    }

    static void initConfiguration()
    {
        if (ccfg == null)
        {
            ccfg = SlimNet.Unity.PeerUtils.LoadConfigurationAsset<ClientConfiguration>(SlimNet.Constants.ClientConfigName);
        }

        if(scfg == null)
        {
            scfg = SlimNet.Unity.PeerUtils.LoadConfigurationAsset<ServerConfiguration>(SlimNet.Constants.ServerConfigName);
        }
    }

    void editLogging()
    {
        SlimNet.LogLevel level = ccfg.LogLevel;

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        editLoggingFlag(ref level, SlimNet.LogLevel.Trace);
        editLoggingFlag(ref level, SlimNet.LogLevel.Info);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        editLoggingFlag(ref level, SlimNet.LogLevel.Debug);
        editLoggingFlag(ref level, SlimNet.LogLevel.Warn);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        editLoggingFlag(ref level, SlimNet.LogLevel.Error);
        editLoggingFlag(ref level, SlimNet.LogLevel.All);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();

        ccfg.LogLevel = level;
        scfg.LogLevel = level;
    }

    void editLoggingFlag(ref SlimNet.LogLevel level, SlimNet.LogLevel flag)
    {
        string name = flag.ToString();

        if ((level & flag) == flag)
        {
            if (!GUILayout.Toggle(true, name))
            {
                level ^= flag;
            }
        }
        else
        {
            if (GUILayout.Toggle(false, name))
            {
                level |= flag;
            }
        }
    }

    void OnGUI()
    {
        initConfiguration();

        // Game Settings

        SlimNetGUILayout.Label("Game", EditorStyles.boldLabel);

        ccfg.GameName = scfg.GameName = SlimNetGUILayout.TextField("Name", ccfg.GameName);
        ccfg.SimulationAccuracy = scfg.SimulationAccuracy = SlimNetGUILayout.IntSlider("Tick Rate (ms)", ccfg.SimulationAccuracy, 10, 1000);

        // Log settings

        SlimNetGUILayout.Label("Logging", EditorStyles.boldLabel);
        editLogging();

        // Network Settings

        SlimNetGUILayout.Label("Network Simulation (Debug Only)", EditorStyles.boldLabel);

        ccfg.LidgrenSimulatedLoss = scfg.LidgrenSimulatedLoss = SlimNetGUILayout.PercentSlider("Packet Loss (%)", ccfg.LidgrenSimulatedLoss);
        ccfg.LidgrenSimulatedLatency = scfg.LidgrenSimulatedLatency = SlimNetGUILayout.MillisecondSlider("Base Latency (ms)", ccfg.LidgrenSimulatedLatency);
        ccfg.LidgrenSimulatedRandomLatency = scfg.LidgrenSimulatedRandomLatency = SlimNetGUILayout.MillisecondSlider("Jitter (ms)", ccfg.LidgrenSimulatedRandomLatency);

        // Server Settings 

        SlimNetGUILayout.Label("Server", EditorStyles.boldLabel);

        scfg.ServerMode = SlimNetGUILayout.EnumPopup("Server Mode", scfg.ServerMode);

        // Send buffering for server
        scfg.SendBuffering = SlimNetGUILayout.IntSlider("Send Buffering (ms)", (int)scfg.SendBuffering, 0, 100);

        if (GUI.changed)
        {
            CallbackTimer.SetTimer("writeconfig", DateTime.Now.AddSeconds(1), () =>
            {
                string ccfgSerialized = Utils.Serialize(ccfg);
                string scfgSerialized = Utils.Serialize(scfg);

                SlimNet.Unity.Editor.Utils.WriteTextAsset(SlimNet.Unity.Constants.ClientConfigAssetPath, ccfgSerialized);
                SlimNet.Unity.Editor.Utils.WriteTextAsset(SlimNet.Unity.Constants.ServerConfigAssetPath, scfgSerialized);

                WriteCopy(SlimNet.Constants.ServerConfigNameXml, scfgSerialized);
            });
        }
    }
}