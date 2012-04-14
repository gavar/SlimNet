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
    public class Scheduler
    {
        Context context;
        Collections.BinaryHeap<float, Action> heap;

        internal Scheduler(Context context)
        {
            Assert.NotNull(context, "context");

            this.context = context;
            this.heap = new Collections.BinaryHeap<float, Action>();
        }

        internal void RunExpired()
        {
            while (heap.Count > 0)
            {
                if (heap.PeekKey() < context.Time.LocalTime)
                {
                    heap.Remove()();
                }
                else
                {
                    return;
                }
            }
        }

        public void RunDelayed(float seconds, Action action)
        {
            if (action != null && seconds >= 0f)
            {
                heap.Add(context.Time.LocalTime + seconds, action);
            }
        }

        public void RunRepeating(float seconds, Func<int, bool> action)
        {
            if (action != null && seconds >= 0f)
            {
                runRepeatingInternal(0, seconds, action);
            }
        }

        void runRepeatingInternal(int n, float seconds, Func<int, bool> action)
        {
            heap.Add(context.Time.LocalTime + seconds, () =>
            {
                if (action(n))
                {
                    runRepeatingInternal(n + 1, seconds, action);
                }
            });
        }
    }
}
