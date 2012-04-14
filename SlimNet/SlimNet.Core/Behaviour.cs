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

namespace SlimNet
{
    [Serializable]
    public abstract partial class Behaviour
    {
        static Log log = Log.GetLogger(typeof(Behaviour));

        /// <summary>
        /// The actor this behaviour belongs to
        /// </summary>
        public Actor Actor { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasActor { get { return Actor != null; } }

        /// <summary>
        /// The type this behaviour can be located under
        /// </summary>
        public virtual Type BaseType { get { return GetType(); } }

        /// <summary>
        /// Debug values for this behaviour
        /// </summary>
        public virtual Pair<string, object>[] DebugValues { get { return new Pair<string,object>[0]; } }

        /// <summary>
        /// The context this behaviour belongs to
        /// </summary>
        public Context Context { get { return HasActor ? Actor.Context : null; } }

        /// <summary>
        /// The transform of this actor
        /// </summary>
        public Transform Transform { get { return HasActor ? Actor.Transform : null; } }

        internal void InternalStart(Actor actor)
        {
            Actor = actor;
            Start();
        }

        internal void InternalSimulate()
        {
            Simulate();
        }

        internal void InternalDestroy()
        {
            Destroy();
            Actor = null;
        }

        public override string ToString()
        {
            return String.Format("<{0}@{1}>", this.GetTypeName(), Actor);
        }

        /// <summary>
        /// Called when the actor starts
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// Called on every simulation step on the client and server
        /// </summary>
        public virtual void Simulate() { }

        /// <summary>
        /// Called when the actor is destroyed
        /// </summary>
        public virtual void Destroy() { }
    }
}
