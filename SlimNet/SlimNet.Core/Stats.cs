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
using System.Linq;
using System.Text;

namespace SlimNet
{
    public class Counter
    {
        TimeManager timer;
        LinkedList<MutablePair<int, int>> list;

        public float Value
        {
            get
            {
                // Make sure we have any values
                if (list.Count > 1)
                {
                    // Accumulated value
                    int value = 0;

                    // Start at the first node
                    LinkedListNode<MutablePair<int, int>> current = list.First;

                    // Never include the current seconds values
                    while (current != null && current.Next != null)
                    {
                        value += current.Value.First;
                        current = current.Next;
                    }

                    // Value per second
                    return value / (float)(list.Count - 1);
                }

                return 0;
            }
        }

        public Counter(TimeManager t)
        {
            timer = t;
            list = new LinkedList<MutablePair<int, int>>();
        }

        public void AddValue(int n)
        {
            // Current second
            int s = (int)timer.LocalTime;

            // Check if we have no nodes and if then create first 
            if (list.Count == 0)
            {
                list.AddLast(new MutablePair<int, int>(0, s));
            }

            // Get the last node
            LinkedListNode<MutablePair<int, int>> current = list.Last;

            if (s > current.Value.Second)
            {
                // Add a new last node
                list.AddLast(new MutablePair<int, int>(0, s));

                // If this was the 12th one, remove the 1st one
                if (list.Count >= 12)
                {
                    list.RemoveFirst();
                }

                // Get the current last (11th node)
                current = list.Last;
            }

            // Add our va
            current.Value.First += n;
        }
    }

    public class Stats
    {
#if DEBUG || RECORD_STATS
        Counter inBytes;
        Counter outBytes;
        Counter inEvents;
        Counter outEvents;
#endif

        public Stats(Context context)
        {
#if DEBUG || RECORD_STATS
            inBytes = new Counter(context.Time);
            outBytes = new Counter(context.Time);
            inEvents = new Counter(context.Time);
            outEvents = new Counter(context.Time);
#endif
        }

        public void AddInBytes(int n)
        {
#if DEBUG || RECORD_STATS
            inBytes.AddValue(n);
#endif
        }

        public void AddOutBytes(int n)
        {
#if DEBUG || RECORD_STATS
            outBytes.AddValue(n);
#endif
        }

        public void AddInEvent()
        {
#if DEBUG || RECORD_STATS
            inEvents.AddValue(1);
#endif
        }

        public void AddOutEvent()
        {
#if DEBUG || RECORD_STATS
            outEvents.AddValue(1);
#endif
        }

        public float EventsInPerSecond
        {
            get
            {
#if DEBUG || RECORD_STATS
                return inEvents.Value;
#else
                return -1f;
#endif
            }
        }

        public float EventsOutPerSecond
        {
            get
            {
#if DEBUG || RECORD_STATS
                return outEvents.Value;
#else
                return -1f;
#endif
            }
        }

        public float BytesInPerSecond
        {
            get
            {
#if DEBUG || RECORD_STATS
                return inBytes.Value;
#else
                return -1f;
#endif
            }
        }

        public float BytesOutPerSecond
        {
            get
            {
#if DEBUG || RECORD_STATS
                return outBytes.Value;
#else
                return -1f;
#endif
            }
        }

        internal void Update()
        {
#if DEBUG || RECORD_STATS
            inBytes.AddValue(0);
            outBytes.AddValue(0);
            inEvents.AddValue(0);
            outEvents.AddValue(0);
#endif
        }
    }
}