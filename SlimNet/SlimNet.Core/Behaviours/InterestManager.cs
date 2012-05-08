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

namespace SlimNet.Behaviours
{
    public class InterestManager : Behaviour
    {
        readonly static Log log = Log.GetLogger(typeof(InterestManager));

        public override Pair<string, object>[] DebugValues
        {
            get
            {
                return new Pair<string, object>[] {
                    Tuple.Create<string, object>("radius", radius),
                    Tuple.Create<string, object>("updateRate", updateRate)
                };
            }
        }

        int tick;
        int radius;
        int updateRate;

        public InterestManager(int radius)
            : this(radius, 10)
        {

        }

        public InterestManager(int radius, int updateRate)
        {
            this.radius = Math.Max(radius, 0);
            this.updateRate = Math.Max(updateRate, 0);
        }

        public override void Start()
        {
            base.Start();

            if (Actor.IsOwnedByServer)
            {
                log.Warn("InterestManager only makes sense on actors owned by players, {0} is owned by the server", Actor);
            }

            if (!Context.HasSpatialPartitioner)
            {
                log.Warn("No spatial partitioner active, InterestManager will not work.");
            }
        }

        public override void Simulate()
        {
            ++tick;

            if (tick < updateRate)
            {
                return;
            }

            tick = 0;

            if (Context.IsServer)
            {
                if (Actor.IsOwnedByServer)
                {
                    return;
                }

                if (!Context.HasSpatialPartitioner)
                {
                    return;
                }

                Player owner = Context.Server.GetActorOwner(Actor);

                if (owner == null)
                {
                    log.Warn("No owner for actor {0} found", Actor);
                    return;
                }

                // Find all close by actors
                HashSet<Actor> hits =
                    new HashSet<Actor>(
                        Context.SpatialPartitioner
                            .Overlap(new SlimMath.BoundingSphere(Transform.Position, radius))
                            .Select(x => x.Actor)
                    );

                // Remove yourself
                hits.Remove(Actor);

                // Remove old subscriptions
                foreach (Actor a in owner.SubscribedTo.ToArray())
                {
                    if (hits.Contains(a))
                    {
                        continue;
                    }

                    Context.Server.Unsubscribe(owner, a);
                    log.Trace("Unsubscribing to {0}", a);
                }

                // Add new subscriptions
                foreach (Actor a in hits)
                {
                    if (owner.SubscribedTo.Contains(a))
                    {
                        continue;
                    }

                    Context.Server.Subscribe(owner, a);
                    log.Trace("Subscribing to {0}", a);
                }
            }
        }
    }
}
