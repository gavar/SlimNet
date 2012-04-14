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

using SlimNet.Utils;

namespace SlimNet
{
    public abstract partial class Event<T> : Network.IStreamWriter
        where T : class, IEventTarget
    {
        /// <summary>
        /// The size of the event data
        /// </summary>
        public abstract int DataSize { get; }

        /// <summary>
        /// The id of the event
        /// </summary>
        public abstract byte EventId { get; }

        /// <summary>
        /// If the event should be sent in the reliable channel or not
        /// </summary>
        public virtual bool Reliable { get { return true; } }

        /// <summary>
        /// The proximity level of subscribers which receive this event
        /// </summary>
        public virtual ProximityLevel ProximityLevel { get { return ProximityLevel.World; } }

        /// <summary>
        /// The log level of the event
        /// </summary>
        public virtual LogLevel LogLevel { get { return SlimNet.LogLevel.Info; } }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool SynchronizeEvent { get { return true; } }

        /// <summary>
        /// The target object of the event
        /// </summary>
        public T Target { get; internal set; }

        /// <summary>
        /// If we should stop processing the event
        /// after the current receiver has been called
        /// </summary>
        public bool StopProcessing { get; set; }

        /// <summary>
        /// The handler tied to this event
        /// </summary>
        public EventHandler<T> Handler { get; internal set; }

        /// <summary>
        /// The player the event originated from, if any
        /// </summary>
        public Player Source { get; internal set; }

        /// <summary>
        /// The source players local time (unsafe)
        /// </summary>
        public float SourceGameTime { get; internal set; }

        /// <summary>
        /// The sources this event is allowed to be received from on the server
        /// </summary>
        public EventSources AllowedSources { get; protected set; }

        /// <summary>
        /// The targets this event should be distributed to
        /// </summary>
        public EventTargets DistributionTargets { get; protected set; }

        /// <summary>
        /// True if the event was raised on this machine
        /// </summary>
        public bool IsLocal { get { return Source == null; } }

        /// <summary>
        /// True of the event was not raised on this machine
        /// </summary>
        public bool IsRemote { get { return Source != null; } }

        /// <summary>
        /// The compelete size of the event data, including all headers
        /// </summary>
        int Network.IStreamWriter.Size { get { return 2 + Handler.HeaderSize + DataSize; } }

        /// <summary>
        /// The context this event belongs to
        /// </summary>
        public Context Context { get { return Handler.Context; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="targets">The targets this event should be distributed to</param>
        /// <param name="sources">The sources this event is allowed to be raised from</param>
        protected Event(EventTargets targets, EventSources sources)
        {
            DistributionTargets = targets;
            AllowedSources = sources;
        }

        /// <summary>
        /// Raises this event
        /// </summary>
        public void Raise()
        {
            Handler.Raise(this);
        }

        /// <summary>
        /// Raises this event on a player
        /// </summary>
        /// <param name="player">The player to raise the event on</param>
        public void Raise(Player player)
        {
            Handler.Raise(this, player);
        }

        public override string ToString()
        {
            return string.Format("<{0}:{1}>", GetType().GetPrettyName(), EventId);
        }

        public abstract void Pack(Network.ByteOutStream stream);
        public abstract void Unpack(Network.ByteInStream stream);

        internal void Invoke()
        {
            Handler.Invoke(this);
        }

        void Network.IStreamWriter.WriteToStream(Player player, Network.ByteOutStream stream)
        {
            // Write event id 
            stream.WriteByte(EventId);

            // Write handler header
            Handler.WriteEventHeader(this, stream);

            // Write data
            Pack(stream);
        }
    }
}
