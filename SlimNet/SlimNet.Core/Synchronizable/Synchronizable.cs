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

namespace SlimNet
{
    internal class SynchronizableFullSynchronizeProxy : Network.IStreamWriter
    {
        Synchronizable synchronizable;

        internal SynchronizableFullSynchronizeProxy(Synchronizable synchronizable)
        {
            this.synchronizable = synchronizable;
        }

        public int Size
        {
            get { return synchronizable.Values.Select(x => x.Size).Sum(); }
        }

        public void WriteToStream(Player player, Network.ByteOutStream stream)
        {
            synchronizable.PackSyncData(UInt32.MaxValue, stream);
        }
    }

    internal class Synchronizable : Network.IStreamWriter
    {
        static Log log = Log.GetLogger(typeof(Synchronizable));

        internal uint DirtyIndexes = 0;
        internal SynchronizedValue[] Values = null;
        internal SynchronizableFullSynchronizeProxy FullSync = null;
        internal Dictionary<string, SynchronizedValue> ValuesNameMap = null;

        internal Actor Actor { get; private set; }
        internal Context Context { get { return Actor.Context; } }
        internal int Count { get { return Values.Length; } }

        internal bool IsDirty { get { return DirtyIndexes != 0; } }
        internal bool IsActive { get { return Actor != null && Actor.IsActive; } }

        public int Size { get { return DataSize + 7; } }

        public int DataSize
        {
            get
            {
                return
                    Values
                        .Where((x, i) => (DirtyIndexes & ((uint)1 << i)) != 0)
                        .Select(x => x.Size)
                        .Sum();
            }
        }

        internal void Init(SynchronizedValue[] values)
        {
            Values = values;
            ValuesNameMap = Values.ToDictionary(x => x.Name);

            for (int i = 0; i < values.Length; ++i)
            {
                values[i].Index = i;
                values[i].Owner = this;
            }
        }

        public Synchronizable(Actor actor)
        {
            Actor = actor;
            FullSync = new SynchronizableFullSynchronizeProxy(this);
        }

        internal void MarkDirty(SynchronizedValue value)
        {
            if (DirtyIndexes == 0)
            {
                Actor.QueueToPeers(this, true);
                Context.SynchronizationQueue.Add(this);
            }

            DirtyIndexes |= ((uint)1 << value.Index);
        }

        internal void PackSyncData(uint indexes, Network.ByteOutStream stream)
        {
            if (indexes == 0 || !IsActive)
            {
                return;
            }

            log.Debug("Sending data for {0}", Actor);

            stream.WriteByte(HeaderBytes.Synchronizable);
            stream.WriteActor(Actor);
            stream.WriteUShort((ushort)DataSize);
            stream.WriteUInt(indexes);

            for (int i = 0; i < Values.Length; ++i)
            {
                if ((indexes & ((uint)1 << i)) != 0)
                {
                    Values[i].Pack(stream);
                }
            }

            return;
        }

        internal bool UnpackSyncData(Network.ByteInStream stream)
        {
            uint indexes = stream.ReadUInt();

            if (indexes == UInt32.MaxValue)
            {
                log.Debug("Received full sync for {0}", Actor);
            }
            else
            {
                log.Debug("Received delta sync for {0}", Actor);
            }

            for (int i = 0; i < Values.Length; ++i)
            {
                if ((indexes & ((uint)1 << i)) != 0)
                {
                    Values[i].Unpack(stream);
                }
            }

            return true;
        }

        public void WriteToStream(Player player, Network.ByteOutStream stream)
        {
            PackSyncData(DirtyIndexes, stream);
        }
    }
}
