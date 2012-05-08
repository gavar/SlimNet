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

namespace SlimNet
{
    public abstract partial class Client : Peer
    {
        static readonly Log log = Log.GetLogger(typeof(Client));

        readonly Network.IClient networkClient;

        public static event Action<Client> OnClientHello;
        public static event Action<Client> OnClientConnected;
        public static event Action<Client> OnClientDisconnected;

        public Player Player { get; private set; }
        public bool Connected { get { return Player != null; } }
        public int Ping { get { return Connected ? (int)(Player.Connection.RoundTripTime * 1000f) : 0; } }
        public ClientConfiguration ClientConfiguration { get { return (ClientConfiguration)Configuration; } }

        protected Client(ClientConfiguration configuration)
        {
            // Verify and set configuraiton
            Assert.NotNull(configuration, "configuration");
                
            Configuration = configuration;

            // Set log level
            Log.SetLevel(configuration.LogLevel);

            // Create context and plugin
            ContextPlugin = CreateContextPlugin(typeof(ClientContextPluginAttribute));
            Context = new Context(this);

            // Network
            NetworkPeer = networkClient = new Network.LidgrenClient(this);

            // Event handlers
            Context.PlayerEventHandler.RegisterReceiver<Events.Hello>(onHello);
            Context.PlayerEventHandler.RegisterReceiver<Events.Spawn>(onSpawn);
            Context.PlayerEventHandler.RegisterReceiver<Events.Despawn>(onDespawn);
            Context.ActorEventHandler.RegisterReceiver<Events.ChangeOwner>(onChangeOwner);
        }

        public void Update(float deltaTime)
        {
            // Receive all network input
            // TODO: This needs to be limited.
            while (networkClient.ReceiveOne())
            {

            }

            if (Connected)
            {
                Context.Render();
                Context.Simulate();
                Context.Send();
            }
        }

        public void Connect(string host, int port)
        {
            log.Info("Connecting to {0}:{1}", host, port);
            networkClient.Connect(host, port);
        }

        public void Disconnect()
        {
            disconnect(true);
        }

        public override Actor CreateActor()
        {
            return new Actor();
        }

        public override ActorRole ResolveActorRole(ushort playerId)
        {
            if (Player != null)
            {
                return Player.Id == playerId ? ActorRole.Autonom : ActorRole.Simulated;
            }

            log.Warn("Trying to resolve actor role without being connected, defaulting to Autonom");
            return ActorRole.Autonom;
        }

        public override void OnDataMessage(Network.ByteInStream stream)
        {
            Context.Time.UpdateOffset(stream.RemoteGameTime);
            Context.OnNetworkStream(stream);
        }

        public override void OnConnected(Network.IConnection connection)
        {
            Context.Time.Start();

            connection.Player = Player = Context.CreatePlayer(0, connection);

            log.Info("Connected");

            if (OnClientConnected != null)
            {
                OnClientConnected(this);
            }
        }

        public override void OnDisconnected(Network.IConnection connection)
        {
            disconnect(false);
        }

        void disconnect(bool disconnectNetwork)
        {
            if (Connected)
            {
                if (disconnectNetwork)
                {
                    Player.Connection.Disconnect();
                }

                // Stop context timer
                Context.Time.Stop();

                // Clear network connection
                Player = null;

                log.Info("Disconnected");

                if (OnClientDisconnected != null)
                {
                    OnClientDisconnected(this);
                }
            }
        }

        void onHello(Events.Hello ev)
        {
            Player.Id = ev.PlayerId;

            if (OnClientHello != null)
            {
                OnClientHello(this);
            }
        }

        void onSpawn(Events.Spawn ev)
        {
            Actor actor;

            if (Context.InstantiateActor(Player.Connection, ev.DefinitionId, ev.ActorId, ev.PlayerId, ev.Position, out actor))
            {
                ev.Actor = actor;

                if (actor.IsMine)
                {
                    Player.OwnedActors.Add(actor);
                }
            }
        }

        void onDespawn(Events.Despawn ev)
        {
            if (ev.Actor == null)
            {
                ev.StopProcessing = true;
                return;
            }

            Context.DestroyActor(ev.Actor);
        }

        void onChangeOwner(Events.ChangeOwner ev)
        {
            ev.Target.PlayerId = ev.NewOwnerId;
            ev.Target.Role = ev.NewOwnerId == Player.Id ? ActorRole.Autonom : ActorRole.Simulated;
        }
    }
}
