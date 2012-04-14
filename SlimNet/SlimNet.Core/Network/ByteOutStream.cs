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

using System;
using System.Text;
using SlimMath;

namespace SlimNet.Network
{
    public class ByteOutStream : IStreamWriter
    {
        Utils.ByteConverter converter;

        internal ByteStream Stream { get; private set; }
        public int Size { get { return Stream.Size; } }

        public ByteOutStream(int capacity)
        {
            Stream = new ByteStream(capacity);
        }

        public void WriteToStream(Player player, Network.ByteOutStream stream)
        {
            stream.WriteRaw(Stream.Data, 0, Stream.Size);
        }

        public void Reset()
        {
            Stream.Reset();
        }

        internal int Skip(int bytes)
        {
            return Stream.EnsureSize(bytes);
        }

        public void WriteByte(byte value)
        {
            WriteByte(value, Stream.EnsureSize(1));
        }

        public void WriteByte(byte value, int index)
        {
            Stream.Data[index] = value;
        }

        public void WriteByteArray(byte[] data)
        {
            if (data == null)
            {
                data = new byte[0];
            }

            WriteInt(data.Length);

            int index = Stream.EnsureSize(data.Length);

            for (int i = 0; i < data.Length; ++i)
            {
                Stream.Data[index + i] = data[i];
            }
        }

        public void WriteSByte(sbyte value)
        {
            WriteByte((byte)value);
        }

        public void WriteBool(bool value)
        {
            WriteByte(value ? (byte)1 : (byte)0);
        }

        public void WriteBoolArray(bool[] value)
        {
            if (value == null)
            {
                value = new bool[0];
            }

            WriteInt(value.Length);

            for (var i = 0; i < value.Length; ++i)
            {
                WriteBool(value[i]);
            }
        }

        public void WriteUShort(ushort value)
        {
            WriteUShort(value, Stream.EnsureSize(2));
        }

        public void WriteUShort(ushort value, int index)
        {
            converter.UShort = value;

            Stream.Data[index] = converter.B0;
            Stream.Data[index + 1] = converter.B1;
        }

        public void WriteUShortArray(ushort[] value)
        {
            if (value == null)
            {
                value = new ushort[0];
            }

            WriteInt(value.Length);

            for (var i = 0; i < value.Length; ++i)
            {
                WriteUShort(value[i]);
            }
        }

        public void WriteShort(short value)
        {
            WriteUShort((ushort)value);
        }

        public void WriteShortArray(short[] value)
        {
            if (value == null)
            {
                value = new short[0];
            }

            WriteInt(value.Length);

            for (var i = 0; i < value.Length; ++i)
            {
                WriteShort(value[i]);
            }
        }

        public void WriteChar(char value)
        {
            WriteUShort((ushort)value);
        }

        public void WriteCharArray(char[] value)
        {
            if (value == null)
            {
                value = new char[0];
            }

            WriteInt(value.Length);

            for (var i = 0; i < value.Length; ++i)
            {
                WriteChar(value[i]);
            }
        }

        public void WriteUInt(uint value)
        {
            WriteUInt(value, Stream.EnsureSize(4));
        }

        public void WriteUInt(uint value, int index)
        {
            converter.UInt = value;

            Stream.Data[index] = converter.B0;
            Stream.Data[index + 1] = converter.B1;
            Stream.Data[index + 2] = converter.B2;
            Stream.Data[index + 3] = converter.B3;
        }

        public void WriteUIntArray(uint[] value)
        {
            if (value == null)
            {
                value = new uint[0];
            }

            WriteInt(value.Length);

            for (var i = 0; i < value.Length; ++i)
            {
                WriteUInt(value[i]);
            }
        }

        public void WriteInt(int value)
        {
            WriteUInt((uint)value);
        }

        public void WriteIntArray(int[] value)
        {
            if (value == null)
            {
                value = new int[0];
            }

            WriteInt(value.Length);

            for (var i = 0; i < value.Length; ++i)
            {
                WriteInt(value[i]);
            }
        }

        public void WriteULong(ulong value)
        {
            WriteULong(value, Stream.EnsureSize(8));
        }

        public void WriteULong(ulong value, int index)
        {
            converter.ULong = value;

            Stream.Data[index] = converter.B0;
            Stream.Data[index + 1] = converter.B1;
            Stream.Data[index + 2] = converter.B2;
            Stream.Data[index + 3] = converter.B3;
            Stream.Data[index + 4] = converter.B4;
            Stream.Data[index + 5] = converter.B5;
            Stream.Data[index + 6] = converter.B6;
            Stream.Data[index + 7] = converter.B7;
        }

        public void WriteULongArray(ulong[] value)
        {
            if (value == null)
            {
                value = new ulong[0];
            }

            WriteInt(value.Length);

            for (var i = 0; i < value.Length; ++i)
            {
                WriteULong(value[i]);
            }
        }

        public void WriteLong(long value)
        {
            WriteULong((ulong)value);
        }

        public void WriteLongArray(long[] value)
        {
            if (value == null)
            {
                value = new long[0];
            }

            WriteInt(value.Length);

            for (var i = 0; i < value.Length; ++i)
            {
                WriteLong(value[i]);
            }
        }

        public void WriteSingle(float value)
        {
            int index = Stream.EnsureSize(4);

            converter.Single = value;

            Stream.Data[index] = converter.B0;
            Stream.Data[index + 1] = converter.B1;
            Stream.Data[index + 2] = converter.B2;
            Stream.Data[index + 3] = converter.B3;
        }

        public void WriteSingleArray(float[] value)
        {
            if (value == null)
            {
                value = new float[0];
            }

            WriteInt(value.Length);

            for (var i = 0; i < value.Length; ++i)
            {
                WriteSingle(value[i]);
            }
        }

        public void WriteDouble(double value)
        {
            int index = Stream.EnsureSize(8);

            converter.Double = value;

            Stream.Data[index] = converter.B0;
            Stream.Data[index + 1] = converter.B1;
            Stream.Data[index + 2] = converter.B2;
            Stream.Data[index + 3] = converter.B3;
            Stream.Data[index + 4] = converter.B4;
            Stream.Data[index + 5] = converter.B5;
            Stream.Data[index + 6] = converter.B6;
            Stream.Data[index + 7] = converter.B7;
        }

        public void WriteDoubleArray(double[] value)
        {
            if (value == null)
            {
                value = new double[0];
            }

            WriteInt(value.Length);

            for (var i = 0; i < value.Length; ++i)
            {
                WriteDouble(value[i]);
            }
        }

        public void WriteRaw(byte[] data, int offset, int length)
        {
            int index = Stream.EnsureSize(length);
            Buffer.BlockCopy(data, offset, Stream.Data, index, length);
        }

        public void WriteFlags(bool f0, bool f1, bool f2, bool f3, bool f4, bool f5, bool f6, bool f7)
        {
            byte b = 0;

            b |= (f0 ? (byte)1 : (byte)0);
            b |= (f1 ? (byte)2 : (byte)0);
            b |= (f2 ? (byte)4 : (byte)0);
            b |= (f3 ? (byte)8 : (byte)0);
            b |= (f4 ? (byte)16 : (byte)0);
            b |= (f5 ? (byte)32 : (byte)0);
            b |= (f6 ? (byte)64 : (byte)0);
            b |= (f7 ? (byte)128 : (byte)0);

            WriteByte(b);
        }

        public void WriteString(string data)
        {
            WriteString(data, Encoding.UTF8);
        }

        public void WriteString(string data, Encoding encoding)
        {
            WriteByteArray(encoding.GetBytes(data ?? ""));
        }

        public void WriteStringArray(string[] value, Encoding encoding)
        {
            if (value == null)
            {
                value = new string[0];
            }

            WriteInt(value.Length);

            for (var i = 0; i < value.Length; ++i)
            {
                WriteString(value[i]);
            }
        }

        public void WriteStringArray(string[] value)
        {
            WriteStringArray(value, Encoding.UTF8);
        }

        public void WriteVector3(Vector3 vector)
        {
            WriteSingle(vector.X);
            WriteSingle(vector.Y);
            WriteSingle(vector.Z);
        }

        public void WriteVector3Array(Vector3[] value)
        {
            if (value == null)
            {
                value = new Vector3[0];
            }

            WriteInt(value.Length);

            for (var i = 0; i < value.Length; ++i)
            {
                WriteVector3(value[i]);
            }
        }

        public void WriteQuaternion(Quaternion quaternion)
        {
            WriteSingle(quaternion.X);
            WriteSingle(quaternion.Y);
            WriteSingle(quaternion.Z);
            WriteSingle(quaternion.W);
        }

        public void WriteQuaternionArray(Quaternion[] value)
        {
            if (value == null)
            {
                value = new Quaternion[0];
            }

            WriteInt(value.Length);

            for (var i = 0; i < value.Length; ++i)
            {
                WriteQuaternion(value[i]);
            }
        }

        public void WriteActor(Actor actor)
        {
            if (Verify.Active(actor))
            {
                WriteUShort(actor.Id);
            }
        }
    }
}
