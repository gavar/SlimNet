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
    public partial class Client : SlimNet.Client
    {
        #region Static

        static Log log = Log.GetLogger(typeof(Client));
        static event Action<Client> onInit;

        public static Client Instance
        {
            get;
            private set;
        }

        public static void OnInit(Action<Client> callback)
        {
            if (Instance == null)
            {
                onInit += callback;
            }
            else
            {
                callback(Instance);
            }
        }

        public static Client Create()
        {
            return new Client(PeerUtils.LoadConfigurationAsset<ClientConfiguration>(SlimNet.Constants.ClientConfigName));
        }

        #endregion

        Client(ClientConfiguration configuration)
            : base(configuration)
        {
            Instance = this;

            if (onInit != null)
            {
                onInit(this);

                // Remove all delegates from invocation list
                foreach (Delegate d in onInit.GetInvocationList())
                {
                    onInit -= (Action<Client>)d;
                }
            }
        }

        public override void BeforeActorStart(Actor actor)
        {
            PeerUtils.CreateGameObject(actor);

            GameObject go = GameObjectMap.Retrieve(actor);
            go.transform.position = Converter.Convert(actor.Transform.Position);
            go.transform.rotation = Converter.Convert(actor.Transform.Rotation);

            GameObject.DontDestroyOnLoad(go);
        }

        public override void BeforeActorDestroy(Actor actor)
        {
            GameObject.Destroy(actor.GetGameObject());
            GameObjectMap.Remove(actor);
        }

        public override void BeforeSimulate()
        {
            PeerUtils.CopyTransformsToSlimNet(Context);
        }
    }
}
