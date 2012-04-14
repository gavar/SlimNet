﻿/*
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

namespace SlimNet
{
    public class Transform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        internal Transform()
        {
            Reset();
        }

        public Vector3 Up { get { return Vector3.Transform(Vector3.Up, Rotation); } }
        public Vector3 Down { get { return Vector3.Transform(Vector3.Down, Rotation); } }
        public Vector3 Left { get { return Vector3.Transform(Vector3.Left, Rotation); } }
        public Vector3 Right { get { return Vector3.Transform(Vector3.Right, Rotation); } }
        public Vector3 Forward { get { return Vector3.Transform(Vector3.Forward, Rotation); } }
        public Vector3 Backward { get { return Vector3.Transform(Vector3.Backward, Rotation); } }

        public void Translate(Vector3 vector)
        {

        }

        internal void Reset()
        {
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
        }
    }
}
