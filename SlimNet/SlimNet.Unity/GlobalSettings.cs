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
    public static class GlobalSettings
    {
        static Func<Actor, GameObject> prefabLocator;
        static readonly Log log = Log.GetLogger(typeof(GlobalSettings));

        public static Func<Actor, GameObject> PrefabLocator
        {
            get { return prefabLocator; }
            set
            {
                if (value == null)
                {
                    throw new RuntimeException("Can't set GlobalSettings.PrefabLocator to null");
                }

                prefabLocator = value;
            }
        }

        static GlobalSettings()
        {
            // Default prefab locator looks for a resource named after the definition
            PrefabLocator = new Func<Actor, GameObject>(defaultPrefabLocator);
        }

        static GameObject defaultPrefabLocator(Actor actor)
        {
            GameObject gameObject = Resources.Load(actor.Definition.Name) as GameObject;

            if (gameObject == null)
            {
                log.Error("Failed loading prefab resource with name {0}", actor.Definition.Name);
            }

            return gameObject;
        }
    }
}
