/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmström
 * 
 * This software is provided 'as-is', without any express or implied
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
 * You are free to share, copy and distribute the software in it's original, 
 * unmodified form. You are not allowed to distribute or make publicly 
 * available the software itself or it's sources in any modified manner. 
 * This notice may not be removed or altered from any source distribution.
 */

using System;
using System.Collections.Generic;
using SlimNet.Utils;

namespace SlimNet
{
    public abstract partial class EventHandler<T> : IPacketHandler
        where T : class, IEventTarget
    {
        static protected readonly Log log = Log.GetLogger(typeof(EventHandler<T>));

        readonly EventDescriptor<T>[] descriptors = new EventDescriptor<T>[256];
        readonly Dictionary<Type, byte> eventIdMap = new Dictionary<Type, byte>(256);

        /// <summary>
        /// The context the event handler belongs to
        /// </summary>
        public Context Context { get; private set; }

        /// <summary>
        /// Size of the handler specific header
        /// </summary>
        public abstract int HeaderSize { get; }

        protected abstract void SendToRemotes(Event<T> ev);
        protected abstract void SendToOwner(Event<T> ev);
        protected abstract void SendToServer(Event<T> ev);

        protected abstract void QueueOnClient(Event<T> ev);
        protected abstract void QueueOnServer(Event<T> ev);

        protected abstract bool IsLocalTargetOwner(Event<T> ev);
        protected abstract bool IsSourceTargetOwner(Event<T> ev);

        internal abstract T ReadEventHeader(Network.ByteInStream stream);
        internal abstract void WriteEventHeader(Event<T> ev, Network.ByteOutStream stream);

        protected internal EventHandler(Context context)
        {
            Assert.NotNull(context, "context");
            Context = context;
        }

        /// <summary>
        /// Create a new event
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="target">The target this event should be bound to</param>
        /// <returns>The created event</returns>
        public TEvent Create<TEvent>(T target)
            where TEvent : Event<T>, new()
        {
            Assert.NotNull(target, "target");

            TEvent ev = new TEvent();

            ev.Target = target;
            ev.Handler = this;

            return ev;
        }

        /// <summary>
        /// Register a global receiver for one event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="receiver">The receiver to be invoked on this event</param>
        public void RegisterReceiver<TEvent>(Action<TEvent> receiver) 
            where TEvent : Event<T>, new()
        {
            EventDescriptor<T> descriptor;

            if (getEventDescriptor(typeof(TEvent), out descriptor))
            {
                descriptor.RegisterReceiver(receiver);
            }
        }

        /// <summary>
        /// Removes a generic receiver from an event
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="receiver">The receiver delegate</param>
        public void RemoveReceiver<TEvent>(Action<TEvent> receiver)
        {
            EventDescriptor<T> descriptor;

            if (getEventDescriptor(typeof(TEvent), out descriptor))
            {
                descriptor.RemoveReceiver(receiver);
            }
        }

        /// <summary>
        /// Remote a target receiver from an event
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="receiver">The receiver delegate</param>
        /// <param name="target">The target</param>
        public void RemoveReceiver<TEvent>(Action<TEvent> receiver, T target)
        {
            EventDescriptor<T> descriptor;

            if (getEventDescriptor(typeof(TEvent), out descriptor))
            {
                descriptor.RemoveReceiver(receiver, target);
            }
        }

        internal void RegisterReceiver<TEvent>(Action<TEvent> receiver, T target, HashSet<byte> set)
            where TEvent : Event<T>, new()
        {
            EventDescriptor<T> descriptor;

            if (getEventDescriptor(typeof(TEvent), out descriptor))
            {
                set.Add(descriptor.EventId);
                descriptor.RegisterReceiver(receiver, target);
            }
        }

        internal void Register(Type type)
        {
            byte eventId;

            if (!type.IsSubclassOf(typeof(Event<T>)))
            {
                log.Error(
                    "Type '{0}' is not a subclass of '{1}'",
                    type.GetPrettyName(),
                    typeof(Event<T>).GetPrettyName()
                );

                return;
            }

            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                log.Error("Event type '{0}' does not have a parameterless public constructor", type.GetPrettyName());
                return;
            }

            if (!GetEventId(type, out eventId))
            {
                return;
            }

            if (descriptors[eventId] != null)
            {
                log.Error(
                    "Tried to register event '{0}' with id #{1} but '{2}' is already registered with that id",
                    type.GetPrettyName(), eventId, descriptors[eventId].Type.GetPrettyName()
                );

                return;
            }

            Type descriptorType = typeof(EventDescriptorTyped<,>).MakeGenericType(type, typeof(T));
            EventDescriptor<T> descriptor = (EventDescriptor<T>)Activator.CreateInstance(descriptorType);

            descriptor.Handler = this;
            descriptor.Type = type;
            descriptor.EventId = eventId;

            // Save event type under this id
            descriptors[eventId] = descriptor;
        }

        internal void RemoveReceivers(IEnumerable<byte> eventIds, T target)
        {
            foreach (byte eventId in eventIds)
            {
                if (descriptors[eventId] != null)
                {
                    descriptors[eventId].ClearReceivers(target);
                }
            }
        }

        internal EventDescriptor<T> GetEventDescriptor(byte eventId)
        {
            return descriptors[eventId];
        }

        internal bool GetEventId(Type type, out byte id)
        {
            if (!eventIdMap.TryGetValue(type, out id))
            {
                Event<T> ev = (Event<T>)Activator.CreateInstance(type);
                eventIdMap[type] = id = ev.EventId;
                return true;
            }
            else
            {
                return true;
            }
        }

        internal void Invoke(Event<T> ev)
        {
            EventDescriptor<T> descriptor = descriptors[ev.EventId];

            if (descriptor != null)
            {
                // Log event invoke
                log.Message(ev.LogLevel, "Invoking event of type {0}", ev.GetTypeName());

                descriptor.Invoke(ev);
            }
        }

        bool IPacketHandler.OnPacket(byte eventId, Context context, Network.ByteInStream stream)
        {
            EventDescriptor<T> descriptor = descriptors[eventId];
            T target = ReadEventHeader(stream);

            // Verify event target object

            if (target == null)
            {
                log.Error("Failed to resolve target for event #{0}", eventId);
                return false;
            }

            // Unpack event
            Event<T> ev = descriptor.Create(target);

            ev.Source = stream.Player;
            ev.SourceGameTime = stream.RemoteGameTime;
            ev.Unpack(stream);

            // Increase stats counter
            Context.Stats.AddInEvent();

            // Special logic depending on 
            // if we're a server or client
            if (context.IsServer)
            {
                EventSources s = ev.AllowedSources;

                bool raisedByOwner = IsSourceTargetOwner(ev);
                bool isOwnerValid = raisedByOwner && (s & EventSources.Owner) == EventSources.Owner;
                bool isRemoteValid = !raisedByOwner && (s & EventSources.Remotes) == EventSources.Remotes;

                if (isOwnerValid || isRemoteValid)
                {
                    raiseEventOnServer(ev);
                }
                else
                {
                    log.Warn("Event {0} was remotely raised by {1}, but that player is not allowed to raise events of that type on target {2}", ev, ev.Source, ev.Target);
                }
            }
            else
            {
                QueueOnClient(ev);
            }

            // Everythings fine, return true
            return true;
        }

        bool getEventDescriptor(Type type, out EventDescriptor<T> descriptor)
        {
            byte eventId;

            if (GetEventId(type, out eventId))
            {
                return (descriptor = descriptors[eventId]) != null;
            }

            log.Error("No descriptor for event {0} existed on handler {1}", type.GetPrettyName(), this);
            descriptor = null;
            return false;
        }

        bool verifyEvent(Event<T> ev)
        {
            Assert.NotNull(ev, "ev");
            Assert.NotNull(ev.Target, "ev.Target");

            if (!ReferenceEquals(this, ev.Handler))
            {
                log.Error("Event '{0}' does not belong to handler '{1}'", ev, this);
                return false;
            }

            return true;
        }

        void raiseEventOnServer(Event<T> ev)
        {
            EventTargets t = ev.DistributionTargets;

            bool toOwner = (t & EventTargets.Owner) == EventTargets.Owner;
            bool toRemotes = (t & EventTargets.Remotes) == EventTargets.Remotes;
            bool toServer = (t & EventTargets.Server) == EventTargets.Server;

            if (toServer)
            {
                QueueOnServer(ev);
            }

            if (toRemotes)
            {
                SendToRemotes(ev);
            }

            if (toOwner)
            {
                SendToOwner(ev);
            }

            // Track stats
            if (toOwner || toRemotes)
            {
                Context.Stats.AddOutEvent();
            }
        }

        void raiseEventOnClient(Event<T> ev)
        {
            EventTargets t = ev.DistributionTargets;
            EventSources s = ev.AllowedSources;
            
            bool toOwner = (t & EventTargets.Owner) == EventTargets.Owner;
            bool toRemotes = (t & EventTargets.Remotes) == EventTargets.Remotes;
            bool toServer = (t & EventTargets.Server) == EventTargets.Server;

            bool allowedOwner = (s & EventSources.Owner) == EventSources.Owner;
            bool allowedRemotes = (s & EventSources.Remotes) == EventSources.Remotes;

            if (IsLocalTargetOwner(ev))
            {
                if (toOwner)
                {
                    QueueOnClient(ev);
                }

                if ((toServer || toRemotes) && allowedOwner)
                {
                    SendToServer(ev);
                }
            }
            else
            {
                if (toRemotes)
                {
                    QueueOnClient(ev);
                }

                if ((toServer || toOwner) && allowedRemotes)
                {
                    SendToServer(ev);
                }
            }
        }
    }
}