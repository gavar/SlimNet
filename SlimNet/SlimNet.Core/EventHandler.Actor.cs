/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmström
 * 
 * This software is provided 'as-is', without any express or implied
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
 * You are free to share, copy and distribute the software in it's original, 
 * unmodified form. You are not allowed to distribute or make publicly 
 * available the software itself or it's sources in any modified manner. 
 * This notice may not be removed or altered from any source distribution.
 */

namespace SlimNet
{
    public sealed class EventHandlerActor : EventHandler<Actor>
    {
        public override int HeaderSize
        {
            get { return 2; }
        }

        internal EventHandlerActor(Context context)
            : base(context)
        {

        }

        protected override void QueueOnClient(Event<Actor> ev)
        {
            if (ev.Target.IsMine || !ev.SynchronizeEvent)
            {
                Invoke(ev);
            }
            else 
            {
                ev.Target.EventQueue.Enqueue(ev);
            }
        }

        protected override void QueueOnServer(Event<Actor> ev)
        {
            if (ev.Target.IsOwnedByServer || !ev.SynchronizeEvent)
            {
                Invoke(ev);
            }
            else
            {
                ev.Target.EventQueue.Enqueue(ev);
            }
        }

        protected override bool IsLocalTargetOwner(Event<Actor> ev)
        {
            return ev.Target != null && ev.Target.Role == ActorRole.Autonom;
        }

        protected override bool IsSourceTargetOwner(Event<Actor> ev)
        {
            return ReferenceEquals(ev.Source.Connection, ev.Target.Connection);
        }

        protected override void SendToRemotes(Event<Actor> ev)
        {
            foreach (Player player in ev.Target.Subscribers)
            {
                ProximityLevel proximity = ev.Context.Server.GetProximityLevel(player, ev.Target);

                if (proximity < ev.ProximityLevel)
                {
                    continue;
                }

                if (ReferenceEquals(ev.Source, player))
                {
                    continue;
                }

                player.Connection.Queue(ev, ev.Reliable);
            }
        }

        protected override void SendToOwner(Event<Actor> ev)
        {
            if (ev.Source == null)
            {
                ev.Target.Connection.Queue(ev, ev.Reliable);
            }
            else
            {
                if (!ReferenceEquals(ev.Source.Connection, ev.Target.Connection))
                {
                    ev.Target.Connection.Queue(ev, ev.Reliable);
                }
            }
        }

        protected override void SendToServer(Event<Actor> ev)
        {
            Context.Stats.AddOutEvent();
            ev.Target.Connection.Queue(ev, ev.Reliable);
        }

        internal override void WriteEventHeader(Event<Actor> ev, Network.ByteOutStream stream)
        {
            stream.WriteUShort(ev.Target.Id);
        }

        internal override Actor ReadEventHeader(Network.ByteInStream stream)
        {
            return Context.GetActor(stream.ReadUShort());
        }
    }
}
