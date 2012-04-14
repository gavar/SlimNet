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

namespace SlimNet
{
    public partial class Player : IEventTarget
    {
        /// <summary>
        /// The unique player id that identifiers the server
        /// </summary>
        public const int ServerPlayerId = 1;

        /// <summary>
        /// Identifies the player across the network
        /// </summary>
        public ushort Id { get; internal set; }

        /// <summary>
        /// The context this player belongs to
        /// </summary>
        public Context Context { get; private set; }

        /// <summary>
        /// If the player is authenticated or not
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// If the player is the local player
        /// </summary>
        public bool IsLocal { get { return Context.IsClient && ReferenceEquals(this, Context.Client.Player); } }

        /// <summary>
        /// Tag that can contain any user data
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Stats Stats { get; private set; }

        internal Network.IConnection Connection { get; private set; }
        internal HashSet<Actor> OwnedActors { get; private set; }
        internal HashSet<Actor> SubscribedTo { get; private set; }
        internal HashSet<Actor> Spawned { get; private set; }
        internal ProximityLevel[] ActorProximityLevels { get; private set; }

        internal Player(Context context, Network.IConnection connection, ushort playerId)
        {
            Assert.NotNull(context, "context");
            Assert.NotNull(connection, "connection");

            Context = context;
            Id = playerId;
            Connection = connection;
            Connection.Player = this;

            OwnedActors = new HashSet<Actor>();
            SubscribedTo = new HashSet<Actor>();
            Spawned = new HashSet<Actor>();

            ActorProximityLevels = new ProximityLevel[UInt16.MaxValue];
            Stats = new Stats(context);
        }
            
        /// <summary>
        /// Raises an event on this player
        /// </summary>
        /// <typeparam name="TEvent">Type of the event</typeparam>
        /// <param name="initializer">The event initializer or null</param>
        public void RaiseEvent<TEvent>(Action<TEvent> initializer)
            where TEvent : Event<Player>, new()
        {
            Context.PlayerEventHandler.Raise<TEvent>(this, initializer);
        }

        public bool IsOwner(Actor actor)
        {
            return actor != null && OwnedActors.Contains(actor);
        }

        public void Disconnect()
        {
            Connection.Disconnect();
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override string ToString()
        {
            return String.Format("<Player:{0}:{1}>", Id, Connection.RemoteEndPoint);
        }
    }
}
