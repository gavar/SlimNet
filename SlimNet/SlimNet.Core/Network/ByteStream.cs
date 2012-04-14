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

namespace SlimNet.Network
{
    public class ByteStream
    {
        public const int OverAllocateAmount = 8;

        int size;
        int position;
        byte[] data;

        public int Size { get { return size; } }
        public int Position { get { return position; } }
        public int Capacity { get { return data.Length; } }
        public byte[] Data { get { return data; } }
        public bool CanRead { get { return position < size; } }

        public ByteStream(byte[] data, int size, int position)
        {
            Set(data, size, position);
        }

        public ByteStream(byte[] data)
            : this(data, 0, 0)
        {

        }

        public ByteStream(int capacity)
            : this(new byte[capacity])
        {

        }

        public ByteStream()
            : this(OverAllocateAmount)
        {

        }

        public void Reset()
        {
            size = 0;
            position = 0;
        }

        public int Read(int count)
        {
            int index = position;
            position += count;
            return index;
        }

        public int EnsureSize(int size)
        {
            int index = this.size;

            this.size += size;

            if (data.Length < this.size)
            {
                byte[] newStorage = new byte[this.size + OverAllocateAmount];
                Buffer.BlockCopy(data, 0, newStorage, 0, data.Length);
                data = newStorage;
            }

            return index;
        }

        internal void Set(byte[] data, int size, int position)
        {
            if (data == null)
            {
                throw new NullReferenceException("storage");
            }

            if (size < 0 || size > data.Length)
            {
                throw new ArgumentOutOfRangeException("size");
            }

            if (position < 0 || position >= data.Length)
            {
                throw new ArgumentOutOfRangeException("position");
            }

            this.data = data;
            this.size = size;
            this.position = position;
        }
    }
}
