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
using System.Net;
using System.Threading;

namespace SlimNet.Network
{
    public interface IPeer
    {
        int MTU { get; }
        AutoResetEvent MessageReceivedEvent { get; }
        bool ReceiveOne();
        void Send(IConnection connection, Network.ByteOutStream header, Queue<Network.IStreamWriter> data, bool reliable);
    }

    public interface IClient : IPeer
    {
        void Connect(string host, int port);
    }

    public interface IServer : Network.IPeer
    {
        void Listen();
        void Shutdown();
    }

    public interface IConnection
    {
        Player Player { get; set; }
        float RoundTripTime { get; }
        IPEndPoint RemoteEndPoint { get; }
        void Disconnect();
        void Send(ByteOutStream header);
        void Queue(IStreamWriter writer, bool reliabe);
    }

    public interface IRawStream
    {
        void WriteRaw(byte[] bytes, int offset, int length);
    }

    public interface IStreamWriter
    {
        int Size { get; }
        void WriteToStream(Player player, ByteOutStream stream);
    }
}
