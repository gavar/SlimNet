﻿/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmström
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

namespace SlimNet.Unity
{
    public class LogAdapter : SlimNet.ILogAdapter
    {
        public void Log(SlimNet.Log.LogEvent @event)
        {
            string message = string.Format("{0} {1} - {2}", @event.Level.ToString().ToUpper(), @event.Log.Name, @event.Message);

            switch (@event.Level)
            {
                case SlimNet.LogLevel.Trace:
                case SlimNet.LogLevel.Info:
                case SlimNet.LogLevel.Debug:
                    Debug.Log(message);
                    break;

                case SlimNet.LogLevel.Warn:
                    Debug.LogWarning(message);
                    break;

                case SlimNet.LogLevel.Error:
                    Debug.LogError(message);
                    break;
            }
        }
    }
}