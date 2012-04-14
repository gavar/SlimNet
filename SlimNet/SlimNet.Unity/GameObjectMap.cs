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

using System.Collections.Generic;
using UnityEngine;
using System;

namespace SlimNet.Unity
{
    internal static class GameObjectMap
    {
        static GameObject[] gameObjectMap;

        static GameObjectMap()
        {
            gameObjectMap = new GameObject[UInt16.MaxValue];
        }

        internal static void Register(Actor actor, GameObject gameObject)
        {
            gameObjectMap[actor.Id] = gameObject;
        }

        internal static void Remove(Actor actor)
        {
            gameObjectMap[actor.Id] = null;
        }

        internal static GameObject Retrieve(Actor actor)
        {
            return gameObjectMap[actor.Id];
        }
    }
}
