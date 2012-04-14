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

namespace SlimNet.Unity
{
    public static class ByteStreamExtensions
    {
        public static void WriteUnityVector3(this Network.ByteOutStream stream, UnityEngine.Vector3 v)
        {
            stream.WriteSingle(v.x);
            stream.WriteSingle(v.y);
            stream.WriteSingle(v.z);
        }

        public static void WriteUnityQuaternion(this Network.ByteOutStream stream, UnityEngine.Quaternion q)
        {
            stream.WriteSingle(q.x);
            stream.WriteSingle(q.y);
            stream.WriteSingle(q.z);
            stream.WriteSingle(q.w);
        }

        public static UnityEngine.Vector3 ReadUnityVector3(this Network.ByteInStream stream)
        {
            return 
                new UnityEngine.Vector3(
                    stream.ReadSingle(),
                    stream.ReadSingle(),
                    stream.ReadSingle()
                );
        }

        public static UnityEngine.Quaternion ReadUnityQuaternion(this Network.ByteInStream stream)
        {
            return
                new UnityEngine.Quaternion(
                    stream.ReadSingle(),
                    stream.ReadSingle(),
                    stream.ReadSingle(),
                    stream.ReadSingle()
                );
        }

    }
}
