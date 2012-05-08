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
using System.IO;
using System.Reflection;
using System.Threading;
using System.Linq;
using SlimNet.Utils;

namespace SlimNet
{
    public abstract partial class Server : Peer
    {
        static readonly Log log = Log.GetLogger(typeof(Server));

        Collections.UShortPool actorIdPool;
        Collections.UShortPool playerIdPool;

        protected readonly Network.IServer NetworkServer;

        public ServerConfiguration ServerConfiguration
        {
            get { return (ServerConfiguration)Configuration; }
        }

        protected Server(ServerConfiguration configuration)
        {
            // Set configuration
            Assert.NotNull(configuration, "configuration");

            Configuration = configuration;

            // Set log level
            Log.SetLevel(configuration.LogLevel);

            // Log some generic data
            log.Info("Running server in {0} mode", configuration.ServerMode);
            log.Info("Using {0} ms of send buffering", configuration.SendBuffering);

            // Setup id pools
            actorIdPool = new Collections.UShortPool();
            playerIdPool = new Collections.UShortPool();

            //
            LoadAssemblies();

            // Setup context
            ContextPlugin = CreateContextPlugin(typeof(ServerContextPluginAttribute));
            Context = new Context(this);

            // Create server
            NetworkPeer = NetworkServer = new Network.LidgrenServer(this);
        }

        public override Actor CreateActor()
        {
            return new Actor();
        }

        public override ActorRole ResolveActorRole(ushort playerId)
        {
            return ActorRole.Authority;
        }

        public override void OnDataMessage(Network.ByteInStream stream)
        {
            Context.Time.Update();
            Context.OnNetworkStream(stream);
        }

        public override void PlayerJoined(Player player)
        {
            Events.Hello ev;

            ev = Context.PlayerEventHandler.Create<Events.Hello>(player);
            ev.PlayerId = player.Id;
            ev.Raise(player);
        }

        public override void OnConnected(Network.IConnection connection)
        {
            Assert.NotNull(connection, "connection");

            ushort playerId;

            if (playerIdPool.Acquire(out playerId))
            {
                log.Info("Connected {0}", Context.CreatePlayer(playerId, connection));
            }
            else
            {
                connection.Disconnect();
            }
        }

        public override void OnDisconnected(Network.IConnection connection)
        {
            if (connection != null && connection.Player != null)
            {
                log.Info("Disconnected {0}", connection.Player);

                playerIdPool.Release(connection.Player.Id);
                Context.RemovePlayer(connection.Player);
            }
        }

        public abstract void Start();
        protected abstract void LoadAssemblies();
    }
}
