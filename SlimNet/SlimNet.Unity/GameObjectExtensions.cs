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
using UnityEngine;

namespace SlimNet.Unity
{
    public static class GameObjectExtensions
    {
        public static ActorProxy GetActorProxyRecursive(this GameObject go)
        {
            while (go != null)
            {
                ActorProxy ap = go.GetComponent<ActorProxy>();

                if (ap != null)
                {
                    return ap;
                }

                go = go.transform.parent.gameObject;
            }

            return null;
        }

        public static ActorProxy GetActorProxy(this GameObject go)
        {
            return go.GetComponent<ActorProxy>();
        }

        public static Actor GetActor(this GameObject go)
        {
            return go.GetActorProxy().Actor;
        }

        public static Actor GetActorRecursive(this GameObject go)
        {
            return go.GetActorProxyRecursive().Actor;
        }

        public static Behaviour GetBehaviour(this GameObject go, Type type)
        {
            return go.GetActor().GetBehaviour(type);
        }

        public static T GetBehaviour<T>(this GameObject go) where T : Behaviour
        {
            return (T)GetBehaviour(go, typeof(T));
        }
    }
}
