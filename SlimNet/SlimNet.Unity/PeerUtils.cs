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
using System.Xml.Serialization;
using System.IO;

namespace SlimNet.Unity
{
    public static class PeerUtils
    {
        static readonly Log log = Log.GetLogger(typeof(PeerUtils));

        internal static void CopyTransformsToSlimNet(Context context)
        {
            foreach (Actor actor in context.Actors)
            {
                if (actor.CopyTransformToSlimNet)
                {
                    GameObject gameObject = actor.GetGameObject();
                    actor.Transform.Position = Converter.Convert(gameObject.transform.position);
                    actor.Transform.Rotation = Converter.Convert(gameObject.transform.rotation);
                }
            }
        }

        public static T LoadConfigurationAsset<T>(string resourcePath)
            where T : SlimNet.Configuration, new()
        {
            T config = default(T);

            try
            {
                TextAsset configAsset = Resources.Load(resourcePath) as TextAsset;

                if (configAsset == null)
                {
                    log.Warn("Could not load {0}, using default configuration", resourcePath);
                    config = new T();
                }
                else
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    config = (T)serializer.Deserialize(new StringReader(configAsset.text));
                }
            }
            catch (Exception exn)
            {
                log.Error(exn);
                log.Warn("Exception throw while loading {0}, using default configuration", resourcePath);

                config = new T();
            }

            return config;
        }

        internal static GameObject CreateGameObject(Actor actor)
        {
            ActorProxy actorProxy = null;
            GameObject unityInstance = null;
            GameObject unityPrefab = GlobalSettings.PrefabLocator(actor);

            if (unityPrefab == null)
            {
                log.Error("Could not load unity prefab for {0}", actor);
                return null;
            }

            unityInstance = (GameObject)GameObject.Instantiate(unityPrefab);
            actorProxy = unityInstance.AddComponent<ActorProxy>();

            log.Info("Instantiated game object {0}", unityInstance);

            // Store actor 
            actorProxy.Actor = actor;

            // Register actor with gameobject so we have a link that goes both ways
            GameObjectMap.Register(actor, unityInstance);

            // Return game object
            return unityInstance;
        }
    }
}
