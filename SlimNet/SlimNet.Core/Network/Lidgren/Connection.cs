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

using Lidgren.Network;
using System.Collections.Generic;

namespace SlimNet.Network
{
    internal class LidgrenConnection : IConnection
    {
        Queue<Network.IStreamWriter> reliable;
        Queue<Network.IStreamWriter> unreliable;

        public NetConnection Connection { get; internal set; }
        public Player Player { get; set; }
        public Context Context { get; private set; }

        public float RoundTripTime
        {
            get { return Connection.AverageRoundtripTime; }
        }

        public System.Net.IPEndPoint RemoteEndPoint
        {
            get { return Connection.RemoteEndpoint; }
        }

        public LidgrenConnection(Context context, NetConnection connection)
        {
            Context = context;
            Connection = connection;

            reliable = new Queue<IStreamWriter>();
            unreliable = new Queue<IStreamWriter>();
        }

        public void Disconnect()
        {
            Connection.Disconnect("");
        }

        public void Send(ByteOutStream header)
        {
            Assert.NotNull(header, "header");

            Context.Peer.NetworkPeer.Send(this, header, reliable, true);
            Context.Peer.NetworkPeer.Send(this, header, unreliable, false);
        }

        public void Queue(IStreamWriter writer, bool isReliable)
        {
            Assert.NotNull(writer, "writer");

            if (isReliable)
            {
                reliable.Enqueue(writer);
            }
            else
            {
                unreliable.Enqueue(writer);
            }

            Context.NetworkQueue.Add(this);
        }
    }
}
