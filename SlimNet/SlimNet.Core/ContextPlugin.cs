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
    public abstract class ContextPlugin
    {
        public Context Context { get; internal set; }

        public abstract void ContextStarted();
        public abstract ISpatialPartitioner CreateSpatialPartitioner();

        public abstract void ActorSpawned(Actor actor);
        public abstract void ActorDespawning(Actor actor);

        public abstract void PlayerJoined(Player player);
        public abstract void PlayerLeaving(Player player);

        public abstract void BeforeStateStreamQueue(Actor actor);
        public abstract void AfterStateStreamQueue(Actor actor);

        public abstract void BeforeQueueTransformReplication();
        public abstract void AfterQueueTransformReplication();
        public abstract void BeforeSimulate();
        public abstract void AfterSimulate();
        public abstract void BeforeSend();
        public abstract void AfterSend();
        public abstract void AfterRenderUpdate();
        public abstract void BeforeRenderUpdate();
    }
}
