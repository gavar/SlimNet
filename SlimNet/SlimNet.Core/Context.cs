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

using System.Collections.Generic;
using System.Linq;
using SlimNet.Utils;
using System;

namespace SlimNet
{
    public partial class Context
    {
        static readonly Log log = Log.GetLogger(typeof(Context));

        readonly IPacketHandler[] packetHandlers = new IPacketHandler[256];
        readonly Dictionary<ushort, Actor> actors = new Dictionary<ushort, Actor>();
        readonly Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();

        public readonly Peer Peer;
        public readonly Server Server;
        public readonly Client Client;
        public readonly bool IsClient;
        public readonly bool IsServer;

        public readonly Stats Stats;
        public readonly TimeManager Time;
        public readonly RPCDispatcher RPC;
        public readonly Scheduler Scheduler;
        public readonly EventHandlerActor ActorEventHandler;
        public readonly EventHandlerPlayer PlayerEventHandler;
        public readonly ISpatialPartitioner SpatialPartitioner;

        public IEnumerable<Actor> Actors { get { return actors.Values.ToArray(); } }
        public IEnumerable<Player> Players { get { return players.Values.ToArray(); } }
        public IEnumerable<Player> AuthenticatedPlayers { get { return Players.Where(x => x.IsAuthenticated).ToArray(); } }
        public bool HasSpatialPartitioner { get { return SpatialPartitioner != null; } }

        internal readonly HashSet<Network.IConnection> NetworkQueue;
        internal readonly HashSet<Synchronizable> SynchronizationQueue;
        internal readonly StateStreamHandler StateStreamHandler;

        internal Context(Client client)
            : this(client as Peer)
        {

        }

        internal Context(Server server)
            : this(server as Peer)
        {

        }

        Context(Peer peer)
        {
            Assert.NotNull(peer, "peer");

            Client = peer as Client;
            Server = peer as Server;
            IsServer = peer is Server;
            IsClient = peer is Client;

            Peer = peer;
            Peer.ContextPlugin.Context = this;

            Scheduler = new Scheduler(this);
            NetworkQueue = new HashSet<Network.IConnection>();
            SynchronizationQueue = new HashSet<Synchronizable>();

            // 
            Time = new TimeManager(peer);

            //
            Stats = new Stats(this);

            // Register packet handlers
            RegisterPacketHandler(new SynchronizableHandler(), HeaderBytes.Synchronizable);
            RegisterPacketHandler(RPC = new RPCDispatcher(this), HeaderBytes.RemoteProcedureCall);
            RegisterPacketHandler(StateStreamHandler = new StateStreamHandler(), HeaderBytes.ActorStateStream);

            // Register player events
            registerEventsForHandler<Actor>(ActorEventHandler = new EventHandlerActor(this));
            registerEventsForHandler<Player>(PlayerEventHandler = new EventHandlerPlayer(this));

            // Create spatial partitioner
            SpatialPartitioner = createSpatialPartitioner();

            // Plugin callback
            Peer.ContextPlugin.ContextStarted();

            // Init actor definitions
            ActorDefinition.Init(this);
        }

        public void Render()
        {
            // Update time
            Time.Update();

            //
            Scheduler.RunExpired();

            // Trigger callback
            Peer.ContextPlugin.BeforeRenderUpdate();

            // Store current actors in local array so
            // we can protect ourselves against modifications
            // of the actors dictionary
            Actor[] currentActors = actors.Values.ToArray();

            // Invoke events and position actors localy
            foreach (Actor actor in currentActors)
            {
                actor.InvokeEvents();

                if (IsServer)
                {
                    if (!actor.IsOwnedByServer)
                    {
                        actor.StateStreamer.SetTransform(actor.SimulationTime);
                    }
                }
                else
                {
                    if (actor.Role == ActorRole.Simulated)
                    {
                        actor.StateStreamer.SetTransform(actor.SimulationTime);
                    }
                }
            }

            // Trigger callback
            Peer.ContextPlugin.AfterRenderUpdate();

            // Update time
            Time.Update();

            //
            Scheduler.RunExpired();
        }

        public void Simulate()
        {
            Time.Update();

            while (Time.ShouldStep)
            {
                // Update time
                Time.Step();
                Time.UpdateDelta();

                // 
                Scheduler.RunExpired();

                // Trigger callbacks
                Peer.BeforeSimulate();
                Peer.ContextPlugin.BeforeSimulate();

                // Store current actors in local array so
                // we can protect ourselves against modifications
                // of the actors dictionary
                Actor[] currentActors = actors.Values.ToArray();

                // Call simulate on all actors
                foreach (Actor actor in currentActors)
                {
                    actor.InternalSimulate();
                }

                // Update actor colliders
                foreach (Actor actor in currentActors)
                {
                    actor.UpdateCollider();
                }

                // Trigger callbacks
                Peer.ContextPlugin.AfterSimulate();
                Peer.AfterSimulate();

                // Update time
                Time.Update();

                //
                Scheduler.RunExpired();

                // If this is the last simulation update, queue tranform replication
                if (!Time.ShouldStep)
                {

#if DEBUG || RECORD_STATS
                    Stats.Update();

                    foreach (Player player in players.Values.ToArray())
                    {
                        player.Stats.Update();
                    }
#endif

                    // Trigger callback
                    Peer.ContextPlugin.BeforeQueueTransformReplication();

                    foreach (Actor actor in currentActors)
                    {
                        actor.QueueStateStream();
                    }

                    // Trigger callback
                    Peer.ContextPlugin.AfterQueueTransformReplication();
                }
                else
                {
                    log.Warn("Not running in real time: {0}", Time.ElapsedMilliseconds);
                }
            }
        }

        public bool Send()
        {
            if (NetworkQueue.Count > 0)
            {
                Peer.ContextPlugin.BeforeSend();

                // Copy dirty connections
                HashSet<Network.IConnection> connections = new HashSet<Network.IConnection>(NetworkQueue);
                NetworkQueue.Clear();

                // Make sure we have an up-to-date time
                Time.Update();

                //
                Scheduler.RunExpired();

                // Create message header
                Network.ByteOutStream header = new Network.ByteOutStream(4);
                header.WriteSingle(Time.GameTime);

                // Send
                foreach (Network.IConnection connection in connections)
                {
                    connection.Send(header);
                }

                foreach (Synchronizable sync in SynchronizationQueue)
                {
                    sync.DirtyIndexes = 0;
                }

                SynchronizationQueue.Clear();
                Peer.ContextPlugin.AfterSend();

                return true;
            }

            return false;
        }

        internal void OnNetworkStream(Network.ByteInStream stream)
        {
#if !DEBUG
            try
            {
#endif
            while (stream.Stream.CanRead)
            {
                byte handlerId = stream.ReadByte();
                IPacketHandler handler = packetHandlers[handlerId];

                if (handler == null)
                {
                    log.Error("No packet handler for id #{0} found, can't process byte stream", handlerId);
                    return;
                }

                if (!handler.OnPacket(handlerId, this, stream))
                {
                    log.Error("Handler '{0}' failed processing byte stream", handler.GetType().GetPrettyName());
                    return;
                }
            }
#if !DEBUG
            }
            catch (Exception exn)
            {
                log.Error(exn);
            }
#endif
        }

        internal void RegisterPacketHandler(IPacketHandler handler, byte id)
        {
            Assert.NotNull(handler, "handler");
            Assert.IsNull(packetHandlers[id], "packetHandlers[" + id + "]");

            packetHandlers[id] = handler;
        }

        void registerEventsForHandler<T>(EventHandler<T> handler)
            where T : class, IEventTarget
        {
            Event<T>[] events =
                TypeUtils2.GetSubTypes(typeof(Event<T>))
                    .Where(x => !x.IsAbstract)
                    .Select<System.Type, System.Object>(TypeUtils2.CreateInstance)
                    .Cast<Event<T>>()
                    .ToArray();

            foreach (Event<T> ev in events)
            {
                handler.Register(ev.GetType());
                RegisterPacketHandler(handler, ev.EventId);
            }
        }

        ISpatialPartitioner createSpatialPartitioner()
        {
            ISpatialPartitioner spatialPartitioner = Peer.ContextPlugin.CreateSpatialPartitioner();

            if (spatialPartitioner != null)
            {
                log.Info("Spatial Partitioner: {0}", spatialPartitioner.GetTypeName());
            }
            else
            {
                log.Warn("No spatial partitioner detected, you will not be able to do any raycasts or overlaps");
            }

            return spatialPartitioner;
        }
    }
}
