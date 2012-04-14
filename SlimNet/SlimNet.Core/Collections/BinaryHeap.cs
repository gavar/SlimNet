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
using System.Collections.Generic;

namespace SlimNet.Collections
{
    public class BinaryHeap<K, V> : IEnumerable<V>
        where K : IComparable<K>
    {
        struct Pair
        {
            public K Key;
            public V Value;
        }

        int count;
        Pair[] heap;

        public int Count
        {
            get { return count; }
        }

        public BinaryHeap(int capacity)
        {
            heap = new Pair[Math.Max(capacity, 2)];
            count = 0;
        }

        public BinaryHeap()
            : this(16)
        {

        }

        public K PeekKey()
        {
            ensureNotEmpty();
            return heap[0].Key;
        }

        public V PeekValue()
        {
            ensureNotEmpty();
            return heap[0].Value;
        }

        public void Add(K key, V value)
        {
            if (count >= heap.Length)
            {
                Pair[] newHeap = new Pair[heap.Length * 2];
                Array.Copy(heap, newHeap, heap.Length);
                heap = newHeap;
            }

            heap[count].Key = key;
            heap[count].Value = value;

            Pair tmp;
            int index = count;
            int parent = 0;

            while (index != 0)
            {
                parent = (index - 1) / 2;

                if (heap[index].Key.CompareTo(heap[parent].Key) == -1)
                {
                    tmp = heap[parent];
                    heap[parent] = heap[index];
                    heap[index] = tmp;
                    index = parent;
                }
                else
                {
                    break;
                }
            }

            ++count;
        }

        public V Remove()
        {
            ensureNotEmpty();

            V result = heap[0].Value;
            count -= 1;

            if (count > 0)
            {
                heap[0] = heap[count];

                // Must clear the last value so we don't leak memory
                heap[count] = default(Pair);

                // Lets trickle the item downwards
                Pair tmp;
                int swap = 0;
                int index = 0;
                
                do
                {
                    index = swap;

                    int left = (index * 2) + 1;
                    int right = (index * 2) + 2;

                    if (right < count)
                    {
                        if(heap[index].Key.CompareTo(heap[left].Key) != -1)
                        {
                            swap = left;
                        }

                        if (heap[swap].Key.CompareTo(heap[right].Key) != -1)
                        {
                            swap = right;
                        }
                    }
                    else if (left < count)
                    {
                        if (heap[index].Key.CompareTo(heap[left].Key) != -1)
                        {
                            swap = left;
                        }
                    }

                    if (index != swap)
                    {
                        tmp = heap[index];
                        heap[index] = heap[swap];
                        heap[swap] = tmp;
                    }

                } while(index != swap);
            }
            else
            {
                heap[0] = default(Pair);
            }

            return result;
        }

        void ensureNotEmpty()
        {
            if (count < 1)
            {
                throw new InvalidOperationException("The heap is empty");
            }
        }

        public IEnumerator<V> GetEnumerator()
        {
            for (int i = 0; i < count; ++i)
            {
                yield return heap[i].Value;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
