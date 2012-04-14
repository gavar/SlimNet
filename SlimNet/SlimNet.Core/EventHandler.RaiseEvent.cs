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
    public partial class EventHandler<T>
		where T : class, IEventTarget
    {
        public void Raise(Event<T> ev)
        {
            if (verifyEvent(ev))
            {
                ev.SourceGameTime = Context.Time.GameTime;

                if (Context.IsServer)
                {
                    raiseEventOnServer(ev);
                }
                else
                {
                    raiseEventOnClient(ev);
                }
            }
        }

        public void Raise(Event<T> ev, Player player)
        {
            Assert.NotNull(player, "player");

            if (verifyEvent(ev))
            {
                player.Connection.Queue(ev, true);
            }
        }

        public void Raise<TEvent>(T target)
            where TEvent : Event<T>, new()
        {
            Raise<TEvent>(target, (Action<TEvent>)null);
        }

        public void Raise<TEvent>(T target, Player player)
            where TEvent : Event<T>, new()
        {
            Raise<TEvent>(target, player, null);
        }

        public void Raise<TEvent>(T target, Action<TEvent> initializer)
            where TEvent : Event<T>, new()
        {
            TEvent ev = Create<TEvent>(target);

            if (initializer != null)
            {
                initializer(ev);
            }

            Raise(ev);
        }

        public void Raise<TEvent>(T target, Player player, Action<TEvent> initializer)
            where TEvent : Event<T>, new()
        {
            TEvent ev = Create<TEvent>(target);

            if (initializer != null)
            {
                initializer(ev);
            }

            Raise(ev, player);
        }
    }
}
