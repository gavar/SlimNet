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
    public class SynchronizableHandler : IPacketHandler
    {
        static Log log = Log.GetLogger(typeof(SynchronizableHandler));

        bool IPacketHandler.OnPacket(byte packetId, Context context, Network.ByteInStream stream)
        {
            Actor actor = stream.ReadActor();
            ushort size = stream.ReadUShort();

            if (actor != null)
            {
                if (context.IsServer)
                {
                    if (stream.Player.OwnedActors.Contains(actor))
                    {
                        return actor.Synchronizable.UnpackSyncData(stream);
                    }
                    else
                    {
                        log.Warn("Got synchronized data from {0} for {1}, but that player does not own that actor", stream.Player, actor);
                    }
                }
                else
                {
                    return actor.Synchronizable.UnpackSyncData(stream);
                }
            }

            stream.Skip(4);
            stream.Skip(size);
            return true;
        }
    }
}
