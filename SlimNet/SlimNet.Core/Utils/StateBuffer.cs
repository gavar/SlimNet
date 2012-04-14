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

namespace SlimNet
{
    public class StateBuffer<T> where T : struct
    {
        readonly T[] buffer;
        readonly float[] timestamps;

        public readonly int Size;
        public int Count { get; private set; }

        public StateBuffer(int size)
        {
            Size = size;
            Count = 0;
            buffer = new T[Size];
            timestamps = new float[Size];
        }

        public void Push(float time, T state)
        {
            freeFirstSlot();

            buffer[0] = state;
            timestamps[0] = time;
        }

        public bool GetStates(float time, out T earlier, out T later, out float earlierTime, out float laterTime)
        {
            for (var i = 0; i < Count; ++i)
            {
                if (timestamps[i] <= time || i == Count - 1)
                {
                    later = buffer[Math.Max(i - 1, 0)];
                    laterTime = timestamps[Math.Max(i - 1, 0)];

                    earlier = buffer[i];
                    earlierTime = timestamps[i];

                    return true;
                }
            }

            later = default(T);
            laterTime = -1f;

            earlier = default(T);
            earlierTime = -1f;

            return false;
        }

        void freeFirstSlot()
        {
            for (var i = (Size - 2); i >= 0; --i)
            {
                timestamps[i + 1] = timestamps[i];
                buffer[i + 1] = buffer[i];
            }

            Count = Math.Min(Count + 1, Size);
        }
    }
}