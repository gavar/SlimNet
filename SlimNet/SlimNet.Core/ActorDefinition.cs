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

using System;
using System.Collections.Generic;
using System.Linq;

namespace SlimNet
{
    public abstract class ActorDefinition
    {
        #region Static

        static ActorDefinition[] all = null;
        static Dictionary<int, ActorDefinition> byId = null;
        static Dictionary<Type, ActorDefinition> byType = null;
        static Log log = Log.GetLogger(typeof(ActorDefinition));

        public static ActorDefinition[] All
        {
            get
            {
                return all;
            }
        }

        public static bool ById(int id, out ActorDefinition definition)
        {
            return byId.TryGetValue(id, out definition);
        }

        public static bool ByType(Type type, out ActorDefinition definition)
        {
            return byType.TryGetValue(type, out definition);
        }

        internal static void Init(Context context)
        {
            if (byId == null || byType == null)
            {
                IEnumerable<ActorDefinition> instances =
                    typeof(ActorDefinition)
                        .GetSubTypes()
                        .Where(x => !x.IsAbstract && !x.IsGenericType && x.HasDefaultConstructor())
                        .Select(x => Activator.CreateInstance(x))
                        .Cast<ActorDefinition>()
                        .Select(x => { x.Context = context; return x; });

                all = instances.ToArray();
                byId = all.ToDictionary(x => x.Id);
                byType = all.ToDictionary(x => x.GetType());

                log.Info("Found {0} Actor Definitions", all.Length);
            }
        }

        #endregion

        /// <summary>
        /// The name of this actor definition
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The contex this definition belongs to
        /// </summary>
        public Context Context { get; internal set; }

        /// <summary>
        /// The identifier id for this definition
        /// </summary>
        public abstract int Id { get; }

        public virtual int SimulationOffset { get { return 2; } }
        public virtual int StateStreamUpdateRate { get { return 1; } }
        public virtual TransformSource TransformSource { get { return SlimNet.TransformSource.Engine; } }
        public virtual Behaviour[] Behaviours { get { return new Behaviour[0]; } }
        public virtual SynchronizedValue[] SynchronizedValues { get { return new SynchronizedValue[0]; } }
        public virtual IActorStateStreamer StateStreamer { get { return new DefaultStateStreamer(); } }
        public virtual Collider Collider { get { return null; } }

        public ActorDefinition()
        {
            Name = 
                GetType()
                    .Name
                    .Replace("Definition", "")
                    .Replace("Actor", "");
        }
    }
}
