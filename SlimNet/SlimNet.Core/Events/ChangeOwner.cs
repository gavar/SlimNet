using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlimNet.Events
{
    public class ChangeOwner : Event<Actor>
    {
        public override int DataSize
        {
            get { return 2; }
        }

        public override byte EventId
        {
            get { return HeaderBytes.EventChangeOwner; }
        }

        public override bool SynchronizeEvent
        {
            get { return false; }
        }

        public ushort NewOwnerId
        {
            get;
            set;
        }

        public ChangeOwner()
            : base(EventTargets.Owner | EventTargets.Remotes, EventSources.None)
        {

        }

        public override void Pack(Network.ByteOutStream stream)
        {
            stream.WriteUShort(NewOwnerId);
        }

        public override void Unpack(Network.ByteInStream stream)
        {
            NewOwnerId = stream.ReadUShort();
        }
    }
}
