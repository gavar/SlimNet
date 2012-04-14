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

using System.Collections.Generic;

namespace SlimNet.Collections
{
    public class UShortPool
    {
        static readonly Log log = Log.GetLogger(typeof(UShortPool));

        object sync;
        Queue<ushort> pool;
        HashSet<ushort> used;

        public UShortPool()
        {
            sync = new object();
            pool = new Queue<ushort>(ushort.MaxValue);
            used = new HashSet<ushort>();

            for (ushort i = 2; i >= 2; ++i)
            {
                pool.Enqueue(i);
            }
        }

        public bool Acquire(out ushort value)
        {
            lock (sync)
            {
                if (pool.Count > 0)
                {
                    value = pool.Dequeue();
                    used.Add(value);
                    return true;
                }
            }

            value = 0;
            return false;
        }

        public void Release(ushort value)
        {
            lock (sync)
            {
                if (used.Contains(value))
                {
                    used.Remove(value);
                    pool.Enqueue(value);
                }
                else
                {
                    log.Warn("Tried to release {0}, but it's not in use", value);
                }
            }
        }
    }
}
