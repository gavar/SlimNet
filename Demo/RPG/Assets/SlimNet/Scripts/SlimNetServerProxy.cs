/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmstr�m
 * 
 * This software is provided 'as-is', without any express or implied
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
 * You are free to share, copy and distribute the software in it's original, 
 * unmodified form. You are not allowed to distribute or make publicly 
 * available the software itself or it's sources in any modified manner. 
 * This notice may not be removed or altered from any source distribution.
 */

using UnityEngine;

public class SlimNetServerProxy : MonoBehaviour
{
    [SerializeField]
    int port = 14000;

    [SerializeField]
    bool listenOnStart = false;

    [SerializeField]
    bool saveBetweenScenes = true;

    /// <summary>
    /// Server instance
    /// </summary>
    public SlimNet.Unity.Server Instance { get; private set; }

    void Start()
    {
        if (saveBetweenScenes)
        {
            DontDestroyOnLoad(gameObject);
        }
        
        // Initialize log4net adapter
        SlimNet.Log.SetAdapter(new SlimNetConsole.LogAdapter());

        // Create server
        Instance = SlimNet.Unity.Server.Create(port);

        if (listenOnStart)
        {
            Instance.Start();
        }
    }

    void Update()
    {
        if (Instance != null)
        {
            Instance.Update(Time.deltaTime);
        }
    }

    void OnApplicationQuit()
    {

    }
}