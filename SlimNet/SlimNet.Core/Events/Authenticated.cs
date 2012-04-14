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

using System.Text;

namespace SlimNet.Events
{
    public sealed class Authenticated : Event<Player>
    {
        public override byte EventId { get { return HeaderBytes.EventAuthenticated; } }
        public override int DataSize { get { return sizeof(bool) + Error.GetNetworkByteCount(); } }

        public string Error { get; set; }

        public bool IsAuthenticated
        {
            get { return Target.IsAuthenticated; }
            set { Target.IsAuthenticated = value; }
        }

        public Authenticated()
            : base(EventTargets.Server | EventTargets.Owner, EventSources.None)
        {

        }

        public override void Pack(Network.ByteOutStream stream)
        {
            stream.WriteBool(IsAuthenticated);
            stream.WriteString(Error);
        }

        public override void Unpack(Network.ByteInStream reader)
        {
            IsAuthenticated = reader.ReadBool();
            Error = reader.ReadString();
        }
    }
}
