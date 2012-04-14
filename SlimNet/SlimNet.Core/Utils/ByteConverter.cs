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

using System.Runtime.InteropServices;

namespace SlimNet.Utils
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ByteConverter
    {
        [FieldOffset(0)] public uint UInt;
        [FieldOffset(0)] public ushort UShort;
        [FieldOffset(0)] public float Single;
        [FieldOffset(0)] public double Double;
        [FieldOffset(0)] public ulong ULong;

        [FieldOffset(0)] public byte B0;
        [FieldOffset(1)] public byte B1;
        [FieldOffset(2)] public byte B2;
        [FieldOffset(3)] public byte B3;
        [FieldOffset(4)] public byte B4;
        [FieldOffset(5)] public byte B5;
        [FieldOffset(6)] public byte B6;
        [FieldOffset(7)] public byte B7;
    }
}
