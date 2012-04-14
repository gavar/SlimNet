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

using SlimMath;

namespace SlimNet
{
    public partial class Context
    {
        public bool GetActor(ushort actorId, out Actor actor)
        {
            return actors.TryGetValue(actorId, out actor) && Verify.Active(actor);
        }

        public Actor GetActor(ushort actorId)
        {
            Actor actor;

            if (GetActor(actorId, out actor))
            {
                return actor;
            }

            return null;
        }

        internal void DestroyActor(Actor actor)
        {
            if (Verify.Active(actor))
            {
                log.Info("Destroying {0}", actor);

                //TODO: The call order here might need to be moved around
                // as it can leave the actor in an a weird state for the user
                Peer.ContextPlugin.ActorDespawning(actor);
                Peer.BeforeActorDestroy(actor);

                // Call internal destroy
                actor.InternalDestroy();

                // Remove collider from spatial partition
                if (actor.HasCollider)
                {
                    actor.Collider.RemoveFromPartition();
                }

                // Remove actor connection
                if (actor.Connection != null)
                {
                    actor.Connection.Player.OwnedActors.Remove(actor);
                    actor.Connection = null;
                }

                // Unregister all receivers
                ActorEventHandler.RemoveReceivers(actor.RegisteredEventIds, actor);

                // Remove actor from storage
                actors.Remove(actor.Id);

                // Clear some values
                actor.Id = 0;
                actor.Name = "";
                actor.PlayerId = 0;
                actor.StateStreamRate = 0;
                actor.SimulationOffset = 0;
            }
        }

        internal void InstantiateActor(Network.IConnection connection, int prefabId, ushort actorId, ushort playerId, Vector3 position)
        {
            Actor actor;
            InstantiateActor(connection, prefabId, actorId, playerId, position, out actor);
        }

        internal bool InstantiateActor(Network.IConnection connection, int prefabId, ushort actorId, ushort playerId, Vector3 position, out Actor instance)
        {
            ActorDefinition definition;

            // On client, verify we always have a connection
            if (IsClient)
            {
                Assert.NotNull(connection, "connection");
            }

            // On server, if the connection is null, playerId MUST equal Player.ServerPlayerId
            if (IsServer && connection == null && playerId != Player.ServerPlayerId)
            {
                log.Error("Connection was null and playerId was not Player.ServerPlayerId");
                instance = null;
                return false;
            }

            // Make sure this id is not in use
            if (actors.ContainsKey(actorId))
            {
                log.Error("An actor with id #{0} already exists", actorId);
                instance = null;
                return false;
            }

            if (!ActorDefinition.ById(prefabId, out definition))
            {
                log.Error("Could not find actor definition with id #{0}", prefabId);
                instance = null;
                return false;
            }

            // Create instance
            instance = Peer.CreateActor();

            // Set public properties
            instance.Role = Peer.ResolveActorRole(playerId);
            instance.Id = actorId;
            instance.PlayerId = playerId;
            instance.Connection = connection;
            instance.Transform.Position = position;
            instance.Context = this;
            instance.Definition = definition;
            instance.StateStreamer = definition.StateStreamer;

            // Some actors dont have a state streamer
            if (instance.StateStreamer != null)
            {
                instance.StateStreamer.Actor = instance;
            }

            // Copy definition valeus
            instance.Name = definition.Name;
            instance.StateStreamRate = definition.StateStreamUpdateRate;
            instance.TransformSource = definition.TransformSource;
            instance.SimulationOffset = definition.SimulationOffset;
            instance.InitBehaviours(definition.Behaviours);
            instance.Synchronizable.Init(definition.SynchronizedValues);

            // Only copy collider if we have a spatial partitioner
            if (HasSpatialPartitioner)
            {
                instance.Collider = definition.Collider;

                // Of we got a collider from the definition
                if (instance.HasCollider)
                {
                    // Set actor on collider
                    instance.Collider.Actor = instance;

                    // Update collider (null safe)
                    instance.UpdateCollider();   

                    // Insert into spatial partitioner
                    SpatialPartitioner.Insert(instance.Collider);
                }
            }

            // Add actor to actor index
            actors.Add(instance.Id, instance);

            // Allow the peer to modify the actor before it's started
            Peer.BeforeActorStart(instance);

            // Start actor
            instance.InternalStart();

            // Call plugin
            Peer.ContextPlugin.ActorSpawned(instance);

            // Log
            log.Info("Instantiated {0} at {1}", instance, position);

            return true;
        }
    }
}
