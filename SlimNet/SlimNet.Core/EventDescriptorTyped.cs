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

namespace SlimNet
{
    internal class EventDescriptorTyped<TEvent, TTarget> : EventDescriptor<TTarget>
        where TTarget : class, IEventTarget
        where TEvent : Event<TTarget>, new()
    {
        List<Action<TEvent>> receivers = new List<Action<TEvent>>();
        List<Action<TEvent>>[] targetReceivers = new List<Action<TEvent>>[UInt16.MaxValue];

        internal override Event<TTarget> Create(TTarget target)
        {
            return Handler.Create<TEvent>(target);
        }

        internal override void Invoke(Event<TTarget> ev)
        {
            TEvent casted = (TEvent)ev;

            runEventOn(casted, receivers);
            runEventOn(casted, targetReceivers[ev.Target.Id]);
        }

        internal override void RemoveReceiver(Delegate receiver)
        {
            receivers.Remove((Action<TEvent>)receiver);
        }

        internal override void RemoveReceiver(Delegate receiver, TTarget target)
        {
            if (targetReceivers[target.Id] != null)
            {
                targetReceivers[target.Id].Remove((Action<TEvent>)receiver);
            }
        }

        internal override void RegisterReceiver(Delegate receiver)
        {
            receivers.Add((Action<TEvent>)receiver);
        }

        internal override void RegisterReceiver(Delegate receiver, TTarget target)
        {
            if (targetReceivers[target.Id] == null)
            {
                targetReceivers[target.Id] = new List<Action<TEvent>>();
            }

            targetReceivers[target.Id].Add((Action<TEvent>)receiver);
        }

        internal override void ClearReceivers(TTarget target)
        {
            Assert.NotNull(target, "target");

            if (targetReceivers[target.Id] != null)
            {
                targetReceivers[target.Id].Clear();
            }
        }

        internal override List<Delegate> GetTargetReceivers(TTarget target)
        {
            if (targetReceivers[target.Id] != null)
            {
                return targetReceivers[target.Id].Cast<Delegate>().ToList();
            }

            return new List<Delegate>();
        }

        void runEventOn(TEvent ev, List<Action<TEvent>> list)
        {
            if (list != null)
            {
                for (var i = 0; i < list.Count; ++i)
                {
                    if (ev.StopProcessing)
                    {
                        return;
                    }

                    list[i](ev);
                }
            }
        }
    }
}
