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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SlimNet.Unity
{
    public static class MonoBehaviourExtensions
    {
        public static Actor GetSlimNetActor(this MonoBehaviour behaviour)
        {
            ActorProxy proxy = behaviour.GetComponent<ActorProxy>();

            if (proxy != null)
            {
                return proxy.Actor;
            }

            return null;
        }

        public static Context GetSlimNetContext(this MonoBehaviour behaviour)
        {
            ActorProxy proxy = behaviour.GetComponent<ActorProxy>();

            if (proxy != null)
            {
                return proxy.Actor.Context;
            }

            return null;
        }
    }
}
