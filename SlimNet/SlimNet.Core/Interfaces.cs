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
using SlimMath;

namespace SlimNet
{
    public interface ILogAdapter
    {
        void Log(Log.LogEvent @event);
    }

    public interface IPacketHandler
    {
        bool OnPacket(byte packetId, Context context, Network.ByteInStream stream);
    }

    public interface IActorStateStreamer
    {
        byte Size { get; }
        Actor Actor { get; set; }

        void Pack(Network.ByteOutStream stream);
        void Unpack(Network.ByteInStream stream);
        void SetTransform(float time);
    }

    public interface IEventTarget
    {
        ushort Id { get; }
    }

    public interface ISpatialPartition
    {
        void Update(Collider c);
        void Remove(Collider c);
    }

    public interface ISpatialPartitioner
    {
        void Draw(System.Action<Vector3, Vector3, Color4> draw);
        void Insert(Collider c);
        List<Collider> Overlap(BoundingBox box);
        List<Collider> Overlap(BoundingSphere sphere);
        List<Collider> Raycast(Ray ray);
    }

}
