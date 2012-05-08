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

namespace SlimNet
{
    public sealed class EventHandlerPlayer : EventHandler<Player>
    {
        public override int HeaderSize
        {
            get { return 0; }
        }

        internal EventHandlerPlayer(Context context)
            : base(context)
        {

        }

        protected override void QueueOnClient(Event<Player> ev)
        {
            Invoke(ev);
        }

        protected override void QueueOnServer(Event<Player> ev)
        {
            Invoke(ev);
        }

        protected override void SendToRemotes(Event<Player> ev)
        {
            log.Warn("Player events can't be sent to remotes");
        }

        protected override void SendToOwner(Event<Player> ev)
        {
            if (!ReferenceEquals(ev.Target, ev.Source))
            {
                ev.Target.Connection.Queue(ev, ev.Reliable);
            }
        }

        protected override void SendToServer(Event<Player> ev)
        {
            Context.Stats.AddOutEvent();
            ev.Target.Connection.Queue(ev, ev.Reliable);
        }

        protected override bool IsLocalTargetOwner(Event<Player> ev)
        {
            return ev.Target != null && ev.Target.IsLocal;
        }

        protected override bool IsSourceTargetOwner(Event<Player> ev)
        {
            return true;
        }

        internal override void WriteEventHeader(Event<Player> ev, Network.ByteOutStream stream)
        {

        }

        internal override Player ReadEventHeader(Network.ByteInStream stream)
        {
            return stream.Player;
        }
    }
}
