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
    public partial class Server
    {
        /// <summary>
        /// Gets the proximity level of between a player and an actor
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="actor">The actor</param>
        /// <returns>The proximity level</returns>
        public ProximityLevel GetProximityLevel(Player player, Actor actor)
        {
            return player.ActorProximityLevels[actor.Id];
        }

        /// <summary>
        /// Subscribes the player to the actor with the ProximityLevel.Horizon level
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="actor">The actor</param>
        public void Subscribe(Player player, Actor actor)
        {
            Subscribe(player, actor, ProximityLevel.Horizon);
        }

        /// <summary>
        /// Subscribes the player to the actor with the proximity level
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="actor">The actor</param>
        /// <param name="proximityLevel">The proximity level</param>
        public void Subscribe(Player player, Actor actor, ProximityLevel proximityLevel)
        {
            if (Verify.Authenticated(player) && Verify.Active(actor) && !IsSubscribedTo(player, actor))
            {
                // Only add as subscriber if this player is not the owner
                if (!ReferenceEquals(player.Connection, actor.Connection))
                {
                    actor.Subscribers.Add(player);
                    player.SubscribedTo.Add(actor);
                    player.ActorProximityLevels[actor.Id] = proximityLevel;
                }

                Spawn(player, actor);
                Synchronize(player, actor);
            }
        }

        /// <summary>
        /// Subscribes the player to the actor with the ProximityLevel.Horizon level
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="actor">The actor</param>
        public void SubscribeNoSynchronize(Player player, Actor actor)
        {
            SubscribeNoSynchronize(player, actor, ProximityLevel.Horizon);
        }

        /// <summary>
        /// Subscribes the player to the actor with the proximity level
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="actor">The actor</param>
        /// <param name="proximityLevel">The proximity level</param>
        public void SubscribeNoSynchronize(Player player, Actor actor, ProximityLevel proximityLevel)
        {
            if (Verify.Authenticated(player) && Verify.Active(actor))
            {
                // Only add as subscriber if this player is not the owner
                if (!ReferenceEquals(player.Connection, actor.Connection))
                {
                    actor.Subscribers.Add(player);
                    player.SubscribedTo.Add(actor);

                    player.ActorProximityLevels[actor.Id] = proximityLevel;
                }

                Spawn(player, actor);
            }
        }

        /// <summary>
        /// Synchronizes the actor data to the player
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="actor">The actor</param>
        public void Synchronize(Player player, Actor actor)
        {
            player.Connection.Queue(actor.Synchronizable.FullSync, true);
        }

        /// <summary>
        /// Unsubscribes a player from an actor
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="actor">The actor</param>
        public void Unsubscribe(Player player, Actor actor)
        {
            if (Verify.Authenticated(player) && Verify.Active(actor))
            {
                actor.Subscribers.Remove(player);
                player.SubscribedTo.Remove(actor);

                player.ActorProximityLevels[actor.Id] = ProximityLevel.None;
            }
        }

        /// <summary>
        /// Checks if the actor is subscribed to the player
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="actor">The actor</param>
        /// <returns>True if subscribed false otherwise</returns>
        public bool IsSubscribedTo(Player player, Actor actor)
        {
            if (Verify.Authenticated(player) && Verify.Active(actor))
            {
                return actor.Subscribers.Contains(player) || ReferenceEquals(player.Connection, actor.Connection);
            }

            return false;
        }

        /// <summary>
        /// Spawns the actor on the player
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="actor">The actor</param>
        public void Spawn(Player player, Actor actor)
        {
            if (Verify.Authenticated(player) && Verify.Active(actor))
            {
                if (!player.Spawned.Contains(actor))
                {
                    // Raise event on this player
                    Events.Spawn ev = Context.PlayerEventHandler.Create<Events.Spawn>(player);

                    ev.DefinitionId = actor.Definition.Id;
                    ev.ActorId = actor.Id;
                    ev.PlayerId = actor.PlayerId;
                    ev.Position = actor.Transform.Position;
                    ev.Raise(player);

                    // Add actor as spawned on this player
                    player.Spawned.Add(actor);
                }
            }
        }

        /// <summary>
        /// Despawns the actor on the player
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="actor">The actor</param>
        public void Despawn(Player player, Actor actor)
        {
            if (Verify.Authenticated(player) && Verify.Active(actor))
            {
                if (!player.Spawned.Contains(actor))
                {
                    log.Warn("Player {0} does not have actor {1} spawned, can't send despawn", player, actor);
                    return;
                }

                // Raise despawn event on player
                Events.Despawn ev = Context.PlayerEventHandler.Create<Events.Despawn>(player);

                ev.ActorId = actor.Id;
                ev.Raise(player);

                // Remove actor as spawned on this player
                player.Spawned.Remove(actor);

                // Unsubscribe player to this actor, no point
                // in being subscribed if the actor is not spawned
                Unsubscribe(player, actor);
            }
        }

        /// <summary>
        /// Checks if the actor is spawned on the player
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="actor">The actor</param>
        /// <returns>True if spawned false otherwise</returns>
        public bool IsSpawnedOn(Player player, Actor actor)
        {
            if (Verify.Authenticated(player) && Verify.Active(actor))
            {
                return player.Spawned.Contains(actor);
            }

            return false;
        }
    }
}
