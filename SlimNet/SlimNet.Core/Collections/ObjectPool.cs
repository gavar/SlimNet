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
    public abstract class ObjectPool<T>
        where T : class
    {
        int maxPooled;
        Queue<T> queue;

        public ObjectPool(int maxPooledItems)
        {
            queue = new Queue<T>();
            maxPooled = maxPooledItems;
        }

        public T Acquire()
        {
            T item;

            if (queue.Count > 0)
            {
                item = queue.Dequeue();
            }
            else
            {
                item = Create();
            }

            Acquired(item);
            return item;
        }

        public void Release(T item)
        {
            Released(item);

            if (queue.Count < maxPooled)
            {
                queue.Enqueue(item);
            }
        }

        protected abstract T Create();
        protected abstract void Acquired(T item);
        protected abstract void Released(T item);
    }
}