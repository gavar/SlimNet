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
using System.Linq;

namespace SlimNet
{
    public abstract class SynchronizedValue
    {
        public abstract int Size { get; }

        public bool IgnoreClientValue { get; set; }
        public abstract object BoxedValue { get; set; }
        public Actor Actor { get { return Owner.Actor; } }
        public Context Context { get { return Owner.Context; } }

        public abstract Type ValueType { get; }
        public virtual bool CanHaveDefaultValue { get { return true; } }

        internal int Index;
        internal string Name;
        internal Synchronizable Owner;

        public abstract void Pack(Network.ByteOutStream stream);
        public abstract void Unpack(Network.ByteInStream stream);

        internal SynchronizedValue()
        {

        }

        public static Type[] GetConcreteClasses()
        {
            return 
                Utils.TypeUtils2.GetSubTypes(typeof(SynchronizedValue))
                    .Where(x => !x.IsAbstract && !x.IsGenericType)
                    .ToArray();
        }

        public static T Create<T>(string name, Action<T> init)
            where T : SynchronizedValue, new()
        {
            T v = Create<T>(name);

            if (init != null)
            {
                init(v);
            }

            return v;
        }

        public static T Create<T>(string name)
            where T : SynchronizedValue, new()
        {
            T v = new T();
            v.Name = name;
            return v;
        }
    }

    public abstract class SynchronizedValue<T> : SynchronizedValue
    {
        T value;
        
        public T Value
        {
            get { return value; }
            set
            {
                if (Owner != null && !ValuesEqual(this.value, value))
                {
                    Owner.MarkDirty(this);
                }

                this.value = value;
            }
        }

        public SynchronizedValue()
            : base()
        {

        }

        public override Type ValueType
        {
            get { return typeof(T); }
        }

        public override object BoxedValue
        {
            get { return Value; }
            set { Value = (T)value; }
        }

        public Func<Actor, T, T> Filter
        {
            get;
            set;
        }

        public sealed override void Unpack(Network.ByteInStream stream)
        {
            T newValue = UnpackValue(stream);

            if (Owner.Context.IsServer)
            {
                if (IgnoreClientValue)
                {
                    return;
                }

                if (Filter != null)
                {
                    newValue = Filter(Actor, newValue);
                }

                Value = newValue;
            }
            else
            {
                value = newValue;
            }

        }

        protected abstract bool ValuesEqual(T a, T b);
        protected abstract T UnpackValue(Network.ByteInStream stream);
    }

}
