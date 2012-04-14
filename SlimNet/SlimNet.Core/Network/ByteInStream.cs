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

using System.Text;
using SlimMath;

namespace SlimNet.Network
{
    public class ByteInStream
    {
        static Log log = Log.GetLogger(typeof(ByteInStream));

        int markPosition = 0;
        Utils.ByteConverter converter;

        public ByteStream Stream { get; private set; }
        public float RemoteGameTime { get; internal set; }
        public Player Player { get { return Connection.Player; } }

        internal Network.IConnection Connection { get; set; }

        public ByteInStream(byte[] data, int size, int position)
        {
            Stream = new ByteStream(data, size, position);
        }

        public ByteInStream(byte[] data, int size)
            : this(data, size, 0)
        {

        }

        public ByteInStream(byte[] data)
            : this(data, data.Length)
        {

        }

        internal void Mark()
        {
            markPosition = Stream.Position;
        }

        internal bool CompareMark(int expectedBytes)
        {
            if (markPosition == -1)
            {
                return true;
            }

            int realBytes = Stream.Position - markPosition;
            markPosition = -1;

            if (realBytes != expectedBytes)
            {
                log.Error("Expected to read {1} bytes, but {2} were read", expectedBytes, realBytes);
                return false;
            }

            return true;
        }

        internal void Reset()
        {
            Stream.Reset();
        }

        internal void Skip(int bytes)
        {
            Stream.Read(bytes);
        }

        public byte ReadByte()
        {
            return Stream.Data[Stream.Read(1)];
        }

        public byte[] ReadByteArray()
        {
            int size = ReadInt();
            byte[] value = new byte[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadByte();
            }

            return value;
        }

        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }

        public sbyte[] ReadSByteArray()
        {
            int size = ReadInt();
            sbyte[] value = new sbyte[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadSByte();
            }

            return value;
        }

        public bool ReadBool()
        {
            return ReadByte() == 1;
        }

        public bool[] ReadBoolArray()
        {
            int size = ReadInt();
            bool[] value = new bool[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadBool();
            }

            return value;
        }

        public ushort ReadUShort()
        {
            int index = Stream.Read(2);

            converter.B0 = Stream.Data[index];
            converter.B1 = Stream.Data[index + 1];

            return converter.UShort;
        }

        public ushort[] ReadUShortArray()
        {
            int size = ReadInt();
            ushort[] value = new ushort[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadUShort();
            }

            return value;
        }

        public short ReadShort()
        {
            return (short)ReadUShort();
        }

        public short[] ReadShortArray()
        {
            int size = ReadInt();
            short[] value = new short[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadShort();
            }

            return value;
        }

        public uint ReadUInt()
        {
            int index = Stream.Read(4);

            converter.B0 = Stream.Data[index];
            converter.B1 = Stream.Data[index + 1];
            converter.B2 = Stream.Data[index + 2];
            converter.B3 = Stream.Data[index + 3];

            return converter.UInt;
        }

        public uint[] ReadUIntArray()
        {
            int size = ReadInt();
            uint[] value = new uint[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadUInt();
            }

            return value;
        }

        public int ReadInt()
        {
            return (int)ReadUInt();
        }

        public int[] ReadIntArray()
        {
            int size = ReadInt();
            int[] value = new int[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadInt();
            }

            return value;
        }

        public ulong ReadULong()
        {
            int index = Stream.Read(8);

            converter.B0 = Stream.Data[index];
            converter.B1 = Stream.Data[index + 1];
            converter.B2 = Stream.Data[index + 2];
            converter.B3 = Stream.Data[index + 3];
            converter.B4 = Stream.Data[index + 4];
            converter.B5 = Stream.Data[index + 5];
            converter.B6 = Stream.Data[index + 6];
            converter.B7 = Stream.Data[index + 7];

            return converter.ULong;
        }

        public ulong[] ReadULongArray()
        {
            int size = ReadInt();
            ulong[] value = new ulong[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadULong();
            }

            return value;
        }

        public long ReadLong()
        {
            return (long)ReadULong();
        }

        public long[] ReadLongArray()
        {
            int size = ReadInt();
            long[] value = new long[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadLong();
            }

            return value;
        }

        public float ReadSingle()
        {
            int index = Stream.Read(4);

            converter.B0 = Stream.Data[index];
            converter.B1 = Stream.Data[index + 1];
            converter.B2 = Stream.Data[index + 2];
            converter.B3 = Stream.Data[index + 3];

            return converter.Single;
        }

        public float[] ReadSingleArray()
        {
            int size = ReadInt();
            float[] value = new float[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadSingle();
            }

            return value;
        }

        public double ReadDouble()
        {
            int index = Stream.Read(8);

            converter.B0 = Stream.Data[index];
            converter.B1 = Stream.Data[index + 1];
            converter.B2 = Stream.Data[index + 2];
            converter.B3 = Stream.Data[index + 3];
            converter.B4 = Stream.Data[index + 4];
            converter.B5 = Stream.Data[index + 5];
            converter.B6 = Stream.Data[index + 6];
            converter.B7 = Stream.Data[index + 7];

            return converter.Double;
        }

        public double[] ReadDoubleArray()
        {
            int size = ReadInt();
            double[] value = new double[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadDouble();
            }

            return value;
        }

        /*
        public byte[] ReadByteArray()
        {
            int size = ReadInt();
            byte[] data = new byte[size];

            for (int i = 0; i < size; ++i)
            {
                data[i] = Stream.Data[Stream.Position + i];
            }

            Stream.Read(size);
            return data;
        }
        */

        public string ReadString(Encoding encoding)
        {
            byte[] data = ReadByteArray();
            return encoding.GetString(data);
        }

        public string ReadString()
        {
            return ReadString(Encoding.UTF8);
        }

        public string[] ReadStringArray(Encoding encoding)
        {
            int size = ReadInt();
            string[] value = new string[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadString(encoding);
            }

            return value;
        }

        public string[] ReadStringArray()
        {
            return ReadStringArray(Encoding.UTF8);
        }

        public byte ReadFlags(out bool f0, out bool f1, out bool f2, out bool f3, out bool f4, out bool f5, out bool f6, out bool f7)
        {
            byte b = ReadByte();

            f0 = (b & 1) == 1;
            f1 = (b & 2) == 2;
            f2 = (b & 4) == 4;
            f3 = (b & 8) == 8;
            f4 = (b & 16) == 16;
            f5 = (b & 32) == 32;
            f6 = (b & 64) == 64;
            f7 = (b & 128) == 128;

            return b;
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Vector3[] ReadVector3Array()
        {
            int size = ReadInt();
            Vector3[] value = new Vector3[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadVector3();
            }

            return value;
        }

        public Quaternion ReadQuaternion()
        {
            return new Quaternion(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Quaternion[] ReadQuaternionArray()
        {
            int size = ReadInt();
            Quaternion[] value = new Quaternion[size];

            for (int i = 0; i < size; ++i)
            {
                value[i] = ReadQuaternion();
            }

            return value;
        }

        public Actor ReadActor()
        {
            return Player.Context.GetActor(ReadUShort());
        }
    }
}
