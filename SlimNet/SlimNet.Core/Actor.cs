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
using System.Collections.Generic;
using System.Linq;

namespace SlimNet
{
    public partial class Actor : Network.IStreamWriter, IEventTarget
    {
        static readonly Log log = Log.GetLogger(typeof(Actor));

        internal int Ticks;
        internal Behaviour[] Behaviours;
        internal Synchronizable Synchronizable;
        internal Network.IConnection Connection;
        internal Dictionary<Type, Behaviour> BehaviourTypeMap;

        internal readonly HashSet<Player> Subscribers = new HashSet<Player>();
        internal readonly Queue<Event<Actor>> EventQueue = new Queue<Event<Actor>>();
        internal readonly HashSet<byte> RegisteredEventIds = new HashSet<byte>();

        /// <summary>
        /// The name of the actor
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The rate that the transform is replicated across the network, in ticks
        /// </summary>
        public int StateStreamRate { get; internal set; }

        /// <summary>
        /// Simulation offset in ticks
        /// </summary>
        public int SimulationOffset { get; internal set; }

        /// <summary>
        /// The source of the tranform for this actor
        /// </summary>
        public TransformSource TransformSource { get; internal set; }

        /// <summary>
        /// The actor definition that created this actor
        /// </summary>
        public ActorDefinition Definition { get; internal set; }

        /// <summary>
        /// Identifies the actor across the network
        /// </summary>
        public ushort Id { get; internal set; }

        /// <summary>
        /// The role of the actor locally
        /// </summary>
        public ActorRole Role { get; internal set; }

        /// <summary>
        /// The id of the player that owns this actor
        /// </summary>
        public ushort PlayerId { get; internal set; }

        /// <summary>
        /// The context the actor belongs to
        /// </summary>
        public Context Context { get; internal set; }

        /// <summary>
        /// The transform of the actor
        /// </summary>
        public Transform Transform { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Collider Collider { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public IActorStateStreamer StateStreamer { get; internal set; }

        /// <summary>
        /// Tag that can contain any user data
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float LastStateStreamUpdateTime { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasCollider { get { return Collider != null; } }

        /// <summary>
        /// The types of behaviours attached
        /// </summary>
        public IEnumerable<Type> AttachedBehaviourTypes { get { return BehaviourTypeMap.Keys.ToList(); } }

        /// <summary>
        /// If the actor is active or not
        /// </summary>
        public bool IsActive
        {
            get { return Id != 0; }
        }

        /// <summary>
        /// If the actor belongs to the server
        /// </summary>
        public bool IsOwnedByServer
        {
            get { return PlayerId == Player.ServerPlayerId; }
        }

        /// <summary>
        /// If the actor belongs to the local client
        /// </summary>
        public bool IsMine
        {
            get
            {
                if(Context == null)
                {
                    return false;
                }

                return (Context.IsServer && IsOwnedByServer) || (Context.IsClient && Role == ActorRole.Autonom);
            }
        }

        /// <summary>
        /// If the actors connection is idle
        /// </summary>
        public bool IsIdle
        {
            get
            {
                if (Context == null)
                {
                    return true;
                }

                if (IsMine)
                {
                    return false;
                }

                return (Context.Time.LocalTime - LastStateStreamUpdateTime) > (4f * StateStreamTime);
            }
        }

        public bool HasStateStream
        {
            get
            {
                return StateStreamer != null;
            }
        }

        /// <summary>
        /// If the actor transform should be copied
        /// from slimnet to the hosting engine (Unity, XNA, etc.)
        /// </summary>
        public bool CopyTransformToEngine
        {
            get
            {
                if (Context == null)
                {
                    return TransformSource == TransformSource.SlimNet;
                }

                if (Context.IsClient)
                {
                    if (IsMine)
                    {
                        return TransformSource == TransformSource.SlimNet;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (IsOwnedByServer)
                    {
                        return TransformSource == TransformSource.SlimNet;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// If the transform from the hosting engine (Unity, XNA, etc.)
        /// should be copied into slimnet actor
        /// </summary>
        public bool CopyTransformToSlimNet
        {
            get
            {
                return !CopyTransformToEngine;
            }
        }
        
        /// <summary>
        /// How often (in seconds) outgoing state updates are sent
        /// </summary>
        public float StateStreamTime
        {
            get
            {
                if (Context == null)
                    return 0;

                return ((float)(Context.Time.SimulationStepSize * StateStreamRate)) / 1000f;
            }
        }

        /// <summary>
        /// Offset in seconds that the simulation for this actor runs at.
        /// </summary>
        public float SimulationTimeOffset
        {
            get
            {
                return StateStreamTime * (float)SimulationOffset;
            }
        }

        /// <summary>
        /// The simulation time of this actor.
        /// </summary>
        public float SimulationTime
        {
            get
            {
                if (SimulationOffset < 1)
                {
                    return Context.Time.GameTime;
                }

                return Context.Time.GameTime - SimulationTimeOffset;
            }
        }

        /// <summary>
        /// The amount of subscribers to this actor
        /// </summary>
        public int SubscriberCount
        {
            get
            {
                return Subscribers.Count;
            }
        }

        /// <summary>
        /// The synchronized values for this actor, paired with name
        /// </summary>
        public KeyValuePair<string, SynchronizedValue>[] SynchronizedValues
        {
            get
            {
                return Synchronizable.ValuesNameMap.Select(x => x).ToArray();
            }
        }

        /// <summary>
        /// List of event types and their attached delegates for this actor
        /// </summary>
        public List<Pair<Type, List<Delegate>>> ActiveEventHandlers
        {
            get
            {
                return
                    RegisteredEventIds
                        .Select(x => Context.ActorEventHandler.GetEventDescriptor(x))
                        .Where(x => x != null)
                        .Select(x => Tuple.Create(x.Type, x.GetTargetReceivers(this)))
                        .ToList();
            }
        }

        /// <summary>
        /// Network size of this actor
        /// </summary>
        int Network.IStreamWriter.Size
        {
            get { return HasStateStream ? 0 : sizeof(byte) + sizeof(ushort) + sizeof(byte) + StateStreamer.Size; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        internal Actor()
        {
            Transform = new SlimNet.Transform();
            Synchronizable = new Synchronizable(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateCollider()
        {
            if (HasCollider)
            {
                Collider.Update(Transform.Position);
            }
        }

        /// <summary>
        /// Register a receiver for an event on this actor
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="receiver">The receiver delegate</param>
        public void RegisterEventReceiver<TEvent>(Action<TEvent> receiver)
            where TEvent : Event<Actor>, new()
        {
            Assert.NotNull(receiver, "receiver");
            Context.ActorEventHandler.RegisterReceiver<TEvent>(receiver, this, RegisteredEventIds);
        }

        /// <summary>
        /// Raises an event on this actor
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="initializer">The event initialization delegate</param>
        public void RaiseEvent<TEvent>(Action<TEvent> initializer)
            where TEvent : Event<Actor>, new()
        {
            Context.ActorEventHandler.Raise<TEvent>(this, initializer);
        }

        /// <summary>
        /// Raises an event without parameters on this actor
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        public void RaiseEvent<TEvent>()
            where TEvent : Event<Actor>, new()
        {
            Context.ActorEventHandler.Raise<TEvent>(this, (Action<TEvent>)null);
        }

        /// <summary>
        /// Gets a synchronized value for this actor
        /// </summary>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <param name="name">The name of the value</param>
        /// <returns>The value if found, or null if not</returns>
        public SynchronizedValue<TValue> GetValue<TValue>(string name)
        {
            name = name ?? "";

            SynchronizedValue value;

            if (Synchronizable.ValuesNameMap.TryGetValue(name, out value))
            {
                if (value is SynchronizedValue<TValue>)
                {
                    return (SynchronizedValue<TValue>)value;
                }
            }

            log.Warn("No synchronized value of type '{2}' with name '{0}' found on {1}", name, this, typeof(TValue));
            return null;
        }

        public SynchronizedValue<TValue> GetValue<TValue>(int index)
        {
            if (index < Synchronizable.Values.Length)
            {
                if(Synchronizable.Values[index] is SynchronizedValue<TValue>)
                {
                    return (SynchronizedValue<TValue>)Synchronizable.Values[index];
                }
            }

            log.Warn("No synchronized value of type '{2}' with index '{0}' found on {1}", index, this, typeof(TValue));
            return null;
        }

        /// <summary>
        /// Gets a behaviour based on type from this actor
        /// </summary>
        /// <typeparam name="T">The type of the behaviour</typeparam>
        /// <returns>The found behaviour, or null</returns>
        public T GetBehaviour<T>()
            where T : Behaviour
        {
            return (T)GetBehaviour(typeof(T));
        }

        /// <summary>
        /// Gets a behaviour based on type from this actor
        /// </summary>
        /// <param name="type">The type of the behaviour</param>
        /// <returns>The found behaviour, or null</returns>
        public Behaviour GetBehaviour(Type type)
        {
            if (BehaviourTypeMap.ContainsKey(type))
            {
                return BehaviourTypeMap[type];
            }

            return null;
        }

        public override string ToString()
        {
            return String.Format("<Actor:{0}:{1}:{2}>", Name, Id, Role);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        internal void InternalStart()
        {
            for (int i = 0; i < Behaviours.Length; ++i)
            {
                Behaviours[i].InternalStart(this);
            }
        }

        internal void InternalSimulate()
        {
            if (HasStateStream && !IsMine)
            {
                StateStreamer.SetTransform(SimulationTime);
            }

            InvokeEvents();

            for (int i = 0; i < Behaviours.Length; ++i)
            {
                Behaviours[i].InternalSimulate();
            }
        }

        internal void InternalDestroy()
        {
            for (int i = 0; i < Behaviours.Length; ++i)
            {
                Behaviours[i].InternalDestroy();
            }
        }

        internal void QueueStateStream()
        {
            if (IsMine && HasStateStream)
            {
                ++Ticks;

                if (Ticks != StateStreamRate)
                {
                    return;
                }

                Ticks = 0;

                Context.Peer.ContextPlugin.BeforeStateStreamQueue(this);
                QueueToPeers(this, false);
                Context.Peer.ContextPlugin.AfterStateStreamQueue(this);
            }
        }

        internal void QueueToPeers(Network.IStreamWriter writer, bool isReliable)
        {
            if (Context.IsServer)
            {
                foreach (Player player in Subscribers)
                {
                    player.Connection.Queue(writer, isReliable);
                }
            }

            if (Connection != null)
            {
                Connection.Queue(writer, isReliable);
            }
        }

        internal void InitBehaviours(Behaviour[] behaviours)
        {
            Behaviours = behaviours;
            BehaviourTypeMap = behaviours.ToDictionary(x => x.BaseType);

            for (var i = 0; i < behaviours.Length; ++i)
            {
                behaviours[i].Actor = this;
            }
        }

        internal void InvokeEvents()
        {
            while (EventQueue.Count > 0 && EventQueue.Peek().SourceGameTime <= SimulationTime)
            {
                EventQueue.Dequeue().Invoke();
            }
        }

        void Network.IStreamWriter.WriteToStream(Player player, Network.ByteOutStream stream)
        {
            stream.WriteByte(HeaderBytes.ActorStateStream);
            stream.WriteUShort(Id);
            stream.WriteByte(StateStreamer.Size);

            StateStreamer.Pack(stream);
        }
    }
}
