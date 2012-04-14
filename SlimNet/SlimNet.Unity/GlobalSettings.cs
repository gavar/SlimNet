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
