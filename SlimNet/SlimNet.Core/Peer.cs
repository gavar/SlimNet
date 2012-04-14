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
using System.Linq;
using SlimNet.Network;
using SlimNet.Utils;

namespace SlimNet
{
    public abstract partial class Peer
    {
        static Log log = Log.GetLogger(typeof(Peer));

        public Context Context { get; protected set; }
        public ContextPlugin ContextPlugin { get; protected set; }
        public Configuration Configuration { get; protected set; }
        public Network.IPeer NetworkPeer { get; protected set; }

        abstract public Actor CreateActor();
        abstract public ActorRole ResolveActorRole(ushort playerId);

        abstract public void OnDataMessage(ByteInStream stream);
        abstract public void OnConnected(IConnection connection);
        abstract public void OnDisconnected(IConnection connection);

        internal protected ContextPlugin CreateContextPlugin(Type attributeType)
        {
            Type contextPluginType =
                TypeUtils2.GetSubTypesWithAttribute(
                    typeof(ContextPlugin),
                    true,
                    attributeType
                ).FirstOrDefault();

            if (contextPluginType == null)
            {
                log.Warn("Found no ContextPlugin class with the {0} attribute, using DefaultContextPlugin", attributeType.Name);
                contextPluginType = typeof(DefaultContextPlugin);
            }

            return (ContextPlugin)TypeUtils2.CreateInstance(contextPluginType);
        }

        virtual public void PlayerJoined(Player player) { }
        virtual public void PlayerLeaving(Player player) { }

        virtual public void BeforeActorStart(Actor actor) { }
        virtual public void BeforeActorDestroy(Actor actor) { }

        virtual public void BeforeSimulate() { }
        virtual public void AfterSimulate() { }
    }
}
