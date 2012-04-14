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
using UnityEditor;
using UnityEngine;

using TimerDict = System.Collections.Generic.Dictionary<string, SlimNet.Pair<System.DateTime, System.Action>>;
using TimerKVP = System.Collections.Generic.KeyValuePair<string, SlimNet.Pair<System.DateTime, System.Action>>;

namespace SlimNet.Unity.Editor
{
    [InitializeOnLoad]
    public static class CallbackTimer
    {
        static DateTime lastCheck;
        static TimerDict timerActions;

        static CallbackTimer()
        {
            lastCheck = DateTime.Now;
            timerActions = new TimerDict();
            EditorApplication.update += timer;

            SlimNet.Log.NoAdapterSet += () =>
            {
                if (Application.isPlaying)
                {
                    SlimNet.Log.SetAdapter(new SlimNetConsole.LogAdapter());
                }
                else
                {
                    SlimNet.Log.SetAdapter(new SlimNet.Unity.LogAdapter());
                }
            };
        }

        static void timer()
        {
            if (lastCheck.AddSeconds(1) < DateTime.Now)
            {
                TimerDict copy = null;

                lock (timerActions)
                {
                    copy = new TimerDict(timerActions);
                }

                foreach (TimerKVP kvp in copy)
                {
                    if (kvp.Value.First < DateTime.Now)
                    {
                        try
                        {
                            kvp.Value.Second();
                        }
                        finally
                        {
                            lock (timerActions)
                            {
                                try
                                {
                                    timerActions.Remove(kvp.Key);
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }

                lastCheck = DateTime.Now;
            }
        }

        public static void SetTimer(string name, DateTime time, Action callback)
        {
            lock (timerActions)
            {
                timerActions.Remove(name);
                timerActions.Add(name, SlimNet.Tuple.Create(time, callback));
            }
        }

        public static void ClearTimer(string name)
        {
            lock (timerActions)
            {
                timerActions.Remove(name);
            }
        }
    }
}