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
using System.Threading;
using Lidgren.Network;
using System;

namespace SlimNet.Network
{
    internal class LidgrenPeer : IPeer
    {
        #region Static
        readonly static Log log = Log.GetLogger(typeof(LidgrenPeer));

        public static NetPeerConfiguration CreateConfig(Configuration config)
        {
            return CreateConfig(config, 0);
        }

        public static NetPeerConfiguration CreateConfig(Configuration config, int port)
        {
            var netConfiguration = new NetPeerConfiguration(config.GameName);

#if DEBUG
            netConfiguration.SimulatedLoss = config.LidgrenSimulatedLoss;
            netConfiguration.SimulatedMinimumLatency = config.LidgrenSimulatedLatency;
            netConfiguration.SimulatedRandomLatency = config.LidgrenSimulatedRandomLatency;

            log.Debug("Network simulation settings: Packet Loss {0}%, Latency {1}ms, Jitter {2}ms", 
                (int)(config.LidgrenSimulatedLoss * 100f),
                (int)(config.LidgrenSimulatedLatency * 1000f),
                (int)(config.LidgrenSimulatedRandomLatency * 1000f)
            );
#endif

            if (port > 0)
            {
                netConfiguration.Port = port;
            }

            return netConfiguration;
        }
        #endregion

        protected readonly NetPeer Peer;
        protected readonly Peer Receiver;

        public AutoResetEvent MessageReceivedEvent
        {
            get { return Peer.MessageReceivedEvent; }
        }

        public int MTU
        {
            get { return Peer.Configuration.MaximumTransmissionUnit; }
        }

        public LidgrenPeer(NetPeer peer, Peer receiver)
        {
            Peer = peer;
            Receiver = receiver;
        }

        public bool ReceiveOne()
        {
            NetIncomingMessage message = Peer.ReadMessage();

            if (message != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        onDataMessage(message);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        onStatusChanged(message);
                        break;

                    default:
                        onInternalMessage(message);
                        break;
                }

                Peer.Recycle(message);
                return true;
            }

            return false;
        }

        void onDataMessage(NetIncomingMessage message)
        {
            if (message.SenderConnection.Tag == null)
            {
                log.Error("Received data message from a connection without a tag, unabled to process data");
                return;
            }
            ByteInStream stream = new ByteInStream(message.PeekDataBuffer(), message.LengthBytes, 0);

            stream.Connection = (IConnection)message.SenderConnection.Tag;
            stream.RemoteGameTime = stream.ReadSingle();

            Receiver.Context.Stats.AddInBytes(message.LengthBytes);
            Receiver.OnDataMessage(stream);
        }

        void onStatusChanged(NetIncomingMessage message)
        {
            log.Info("Status on connection {0} changed to {1}", message.SenderEndpoint, message.SenderConnection.Status);

            switch (message.SenderConnection.Status)
            {
                case NetConnectionStatus.Connected:
                    message.SenderConnection.Tag = new LidgrenConnection(Receiver.Context, message.SenderConnection);
                    Receiver.OnConnected((IConnection)message.SenderConnection.Tag);
                    break;

                case NetConnectionStatus.Disconnected:
                    Receiver.OnDisconnected((IConnection)message.SenderConnection.Tag);
                    break;
            }
        }

        void onInternalMessage(NetIncomingMessage message)
        {
            switch (message.MessageType)
            {
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                    log.Debug("{0}", message.ReadString());
                    break;

                case NetIncomingMessageType.WarningMessage:
                    log.Warn("{0}", message.ReadString());
                    break;

                case NetIncomingMessageType.Error:
                case NetIncomingMessageType.ErrorMessage:
                    log.Error("{0}", message.ReadString());
                    break;

                default:
                    log.Warn("Unhandled message type: {0}", message.MessageType);
                    break;
            }
        }

        public void Send(IConnection connection, Network.ByteOutStream header, Queue<Network.IStreamWriter> data, bool reliable)
        {
            if (data.Count == 0)
            {
                return;
            }

            int mtu = MTU;
            int headerSize = header.Size;
            int messageSize = mtu - headerSize;

            IStreamWriter writer = null;
            NetConnection netConnection = ((LidgrenConnection)connection).Connection;
            ByteOutStream stream = new ByteOutStream(messageSize);
            NetDeliveryMethod delivery = reliable ? NetDeliveryMethod.ReliableOrdered : NetDeliveryMethod.UnreliableSequenced;

            while (data.Count > 0)
            {
                writer = data.Dequeue();

                if (writer == null)
                {
                    log.Warn("A null value was queued on the outgoing message queue for {0}", connection);
                    continue;
                }

                if (stream.Size + writer.Size > messageSize && stream.Size > 0)
                {
                    sendStream(connection.Player, netConnection, delivery, stream, header, mtu);
                    stream.Reset();
                }

                writer.WriteToStream(connection.Player, stream);
            }

            if (stream.Size > 0)
            {
                sendStream(connection.Player, netConnection, delivery, stream, header, mtu);
            }
        }

        void sendStream(Player player, NetConnection connection, NetDeliveryMethod delivery, ByteOutStream data, ByteOutStream header, int mtu)
        {
            // Create message
            NetOutgoingMessage message = Peer.CreateMessage(Math.Max(mtu, data.Size+header.Size));

            // Write header + data
            message.WriteRaw(header.Stream.Data, 0, header.Stream.Size);
            message.WriteRaw(data.Stream.Data, 0, data.Stream.Size);

            // Record status
            player.Stats.AddOutBytes(message.LengthBytes + 2);
            player.Context.Stats.AddOutBytes(message.LengthBytes + 2);

            // Send message
            Peer.SendMessage(message, connection, delivery);
        }
    }
}
