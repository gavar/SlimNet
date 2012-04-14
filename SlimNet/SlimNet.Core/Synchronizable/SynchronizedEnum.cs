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

using System;

namespace SlimNet
{
    public class SynchronizedEnum<TEnum> : SynchronizedValue<TEnum>
        where TEnum : struct
    {
        public override int Size
        {
            get { return sizeof(int); }
        }

        protected override TEnum UnpackValue(Network.ByteInStream stream)
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), stream.ReadInt());
        }

        public override void Pack(Network.ByteOutStream stream)
        {
            stream.WriteInt((int)(ValueType)Value);
        }

        protected override bool ValuesEqual(TEnum a, TEnum b)
        {
            return a.Equals(b);
        }
    }
}
