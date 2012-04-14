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
    public sealed class Login : Event<Player>
    {
        public override byte EventId { get { return HeaderBytes.EventLogin; } }
        public override int DataSize { get { return Username.GetNetworkByteCount() + Password.GetNetworkByteCount(); } }

        public string Username { get; set; }
        public string Password { get; set; }

        public Login()
            : base(EventTargets.Server, EventSources.Owner)
        {

        }

        public override void Pack(Network.ByteOutStream stream)
        {
            stream.WriteString(Username);
            stream.WriteString(Password);
        }

        public override void Unpack(Network.ByteInStream reader)
        {
            Username = reader.ReadString();
            Password = reader.ReadString();
        }
    }
}
