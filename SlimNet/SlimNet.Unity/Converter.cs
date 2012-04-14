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
    public static class Converter
    {
        #region SlimNet to Unity

        public static UnityEngine.Color Convert(SlimMath.Color3 c)
        {
            return new UnityEngine.Color(c.Red, c.Green, c.Blue, 1f);
        }

        public static UnityEngine.Color Convert(SlimMath.Color4 c)
        {
            return new UnityEngine.Color(c.Red, c.Green, c.Blue, c.Alpha);
        }

        public static UnityEngine.Vector2 Convert(SlimMath.Vector2 v)
        {
            return new UnityEngine.Vector2(v.X, v.Y);
        }

        public static UnityEngine.Vector3 Convert(SlimMath.Vector3 v)
        {
            return new UnityEngine.Vector3(v.X, v.Y, v.Z);
        }

        public static UnityEngine.Vector4 Convert(SlimMath.Vector4 v)
        {
            return new UnityEngine.Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static UnityEngine.Quaternion Convert(SlimMath.Quaternion q)
        {
            return new UnityEngine.Quaternion(q.X, q.Y, q.Z, q.W);
        }

        #endregion

        #region Unity To SlimNet

        public static SlimMath.Color4 Convert(UnityEngine.Color c)
        {
            return new SlimMath.Color4(c.a, c.r, c.g, c.b);
        }

        public static SlimMath.Vector2 Convert(UnityEngine.Vector2 v)
        {
            return new SlimMath.Vector2(v.x, v.y);
        }

        public static SlimMath.Vector3 Convert(UnityEngine.Vector3 v)
        {
            return new SlimMath.Vector3(v.x, v.y, v.z);
        }

        public static SlimMath.Vector4 Convert(UnityEngine.Vector4 v)
        {
            return new SlimMath.Vector4(v.x, v.y, v.z, v.w);
        }

        public static SlimMath.Quaternion Convert(UnityEngine.Quaternion qin)
        {
            return new SlimMath.Quaternion(qin.x, qin.y, qin.z, qin.w);
        }

        #endregion
    }
}