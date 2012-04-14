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

using System.Linq;

namespace SlimNet
{
    public partial class Server
    {
        Actor spawnActor(Player owner, ushort playerId, int prefabId, SlimMath.Vector3 position)
        {
            Actor actor;
            ushort actorId;
            bool hasOwner = owner != null;

            if (!actorIdPool.Acquire(out actorId))
            {
                log.Error("Can't spawn actor, failed to acquire a new actor id");
                return null;
            }

            if (hasOwner && owner.Connection == null)
            {
                log.Error("Owner supplied, but it has no connection object");
                return null;
            }

            Network.IConnection connection = hasOwner ? owner.Connection : null;

            if (Context.InstantiateActor(connection, prefabId, actorId, playerId, position, out actor))
            {
                if (hasOwner)
                {
                    owner.OwnedActors.Add(actor);
                }

                return actor;
            }

            return null;
        }

        bool changeOwner(Actor actor, ushort ownerId)
        {
            if (Verify.Active(actor))
            {
                if (actor.PlayerId == ownerId)
                {
                    return false;
                }

                actor.PlayerId = ownerId;
                actor.RaiseEvent<Events.ChangeOwner>(ev => ev.NewOwnerId = ownerId);

                return true;
            }

            return false;
        }

        public Actor SpawnActor<T>(Player player, SlimMath.Vector3 position)
            where T : ActorDefinition
        {
            if (Verify.Authenticated(player))
            {
                ActorDefinition definition;

                if (ActorDefinition.ByType(typeof(T), out definition))
                {
                    return spawnActor(player, player.Id, definition.Id, position);
                }
                else
                {
                    log.Warn("No actor definition of type {0} found", typeof(T));
                    return null;
                }
            }

            return null;
        }

        public bool ChangeOwner(Actor actor, Player player)
        {
            if (Verify.Authenticated(player))
            {
                return changeOwner(actor, player.Id);
            }

            return false;
        }

        public bool TakeOwnership(Actor actor)
        {
            return changeOwner(actor, Player.ServerPlayerId);
        }

        public Actor SpawnActor<T>(SlimMath.Vector3 position)
            where T : ActorDefinition
        {
            ActorDefinition definition;

            if (ActorDefinition.ByType(typeof(T), out definition))
            {
                return spawnActor(null, Player.ServerPlayerId, definition.Id, position);
            }
            else
            {
                log.Warn("No actor definition of type {0} found", typeof(T));
                return null;
            }
        }

        public void Despawn(Actor actor)
        {
            ushort actorId = actor.Id;

            // Despawn actor on all subscribers
            foreach (Player player in actor.Subscribers.ToArray())
            {
                Despawn(player, actor);
            }

            // Despawn actor on owner
            if (actor.Connection != null)
            {
                Despawn(actor.Connection.Player, actor);
            }

            // Destroy actor
            Context.DestroyActor(actor);

            // Release actor id for re-use
            actorIdPool.Release(actorId);
        }

        public Player GetActorOwner(Actor actor)
        {
            if (Verify.Active(actor))
            {
                Player owner;

                if (Context.GetPlayer(actor.PlayerId, out owner))
                {
                    return owner;
                }
            }

            return null;
        }
    }
}
