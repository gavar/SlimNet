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

using SlimMath;
using System;

namespace SlimNet
{
    public sealed class DefaultStateStreamer : IActorStateStreamer
    {
        struct State
        {
            public Vector3 Position;
            public Quaternion Rotation;
        }

        Vector3 position;
        Quaternion rotation;
        StateBuffer<State> buffer = new StateBuffer<State>(20);

        float yaw;
        byte size;
        bool yawOnly;
        bool compressPosition;

        public byte Size { get { return size; } }
        public float Time { get; set; }
        public Actor Actor { get; set; }

        public DefaultStateStreamer()
            : this(true, false)
        {

        }

        public DefaultStateStreamer(bool compressPosition, bool yawOnly)
        {
            this.yawOnly = yawOnly;
            this.compressPosition = compressPosition;

            size = 0;
            size += compressPosition ? (byte)6 : (byte)12;
            size += yawOnly ? (byte)1 : (byte)7;
        }

        public void Pack(Network.ByteOutStream stream)
        {
            Vector3 p;
            Quaternion r;
            byte angle = 0;

            if (Actor.Context.IsServer)
            {
                p = position;
                r = rotation;

                if (yawOnly)
                {
                    angle = (byte)(yaw * 0.71111f);
                }
            }
            else
            {
                p = Actor.Transform.Position;
                r = Actor.Transform.Rotation;

                if (yawOnly)
                {
                    angle = (byte)(Vector3.SignedAngle(Vector3.Forward, Actor.Transform.Forward, Vector3.Up) * 0.71111f);
                }
            }

            if (compressPosition)
            {
                stream.WriteUShort(HalfUtilities.Pack(p.X));
                stream.WriteUShort(HalfUtilities.Pack(p.Y));
                stream.WriteUShort(HalfUtilities.Pack(p.Z));
            }
            else
            {
                stream.WriteVector3(p);
            }

            if (yawOnly)
            {
                stream.WriteByte(angle);
            }
            else
            {
                p = r.Axis;
                stream.WriteUShort(HalfUtilities.Pack(p.X));
                stream.WriteUShort(HalfUtilities.Pack(p.Y));
                stream.WriteUShort(HalfUtilities.Pack(p.Z));
                stream.WriteByte((byte)(r.Angle * SlimMath.Single.Rad2Deg * 0.71111f));
            }
        }

        public void Unpack(Network.ByteInStream stream)
        {
            if (compressPosition)
            {
                position = new Vector3(
                    HalfUtilities.Unpack(stream.ReadUShort()),
                    HalfUtilities.Unpack(stream.ReadUShort()),
                    HalfUtilities.Unpack(stream.ReadUShort())
                );
            }
            else
            {
                position = stream.ReadVector3();
            }

            if (yawOnly)
            {
                yaw = stream.ReadByte() * 1.40625f;
                rotation = Quaternion.RotationAxis(Vector3.Up, yaw * SlimMath.Single.Deg2Rad);
            }
            else
            {
                rotation = Quaternion.RotationAxis(
                    new Vector3(
                        HalfUtilities.Unpack(stream.ReadUShort()),
                        HalfUtilities.Unpack(stream.ReadUShort()),
                        HalfUtilities.Unpack(stream.ReadUShort())
                    ),
                    (stream.ReadByte() * 1.40625f * SlimMath.Single.Deg2Rad)
                );
            }

            buffer.Push(Actor.Context.Time.GameTime, 
                new State { Position = position, Rotation = rotation }
            );
        }

        public void SetTransform(float time)
        {
            State earlier;
            float earlierTime;

            State later;
            float laterTime;

            if (buffer.GetStates(time, out earlier, out later, out earlierTime, out laterTime))
            {
                var t = 0.0f;
                var length = laterTime - earlierTime;

                if (length > 0.0001f)
                {
                    t = (float)((time - earlierTime) / length);
                }

                t = SlimMath.Single.Clamp01(t);

                Actor.Transform.Position = Vector3.Lerp(earlier.Position, later.Position, t);
                Actor.Transform.Rotation = Quaternion.Lerp(earlier.Rotation, later.Rotation, t);
            }
        }
    }
}
