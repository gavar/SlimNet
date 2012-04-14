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

namespace SlimNet
{
    public sealed class StateStreamHandler : IPacketHandler
    {
        static Log log = Log.GetLogger(typeof(StateStreamHandler));

        bool IPacketHandler.OnPacket(byte packetId, Context context, Network.ByteInStream stream)
        {
            Actor actor;
            ushort actorId = stream.ReadUShort();
            byte size = stream.ReadByte();

            if (context.GetActor(actorId, out actor))
            {
                if (verify(context, actor, stream))
                {
                    if (actor.HasStateStream)
                    {
                        actor.LastStateStreamUpdateTime = context.Time.LocalTime;
                        actor.StateStreamer.Unpack(stream);

                        if (context.IsServer)
                        {
                            foreach (Player s in actor.Subscribers)
                            {
                                s.Connection.Queue(actor, false);
                            }
                        }

                        return true;
                    }
                    else
                    {
                        log.Error("{0} does not have a state stream, yet it received state stream packet from {1}", actor, stream.Connection.Player);
                    }
                }
                else
                {
                    log.Warn("{0} is not allowed to send transform replication to {1}, ignoring", stream.Connection.Player, actor);
                }
            }

            stream.Skip(size);
            return true;
        }

        bool verify(Context context, Actor actor, Network.ByteInStream stream)
        {
            if (context.IsServer)
            {
                return actor != null && stream.Connection.Player.OwnedActors.Contains(actor);
            }
            else
            {
                return actor != null && !actor.IsMine;
            }
        }
    }
}
