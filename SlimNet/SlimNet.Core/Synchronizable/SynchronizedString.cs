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
    public sealed class SynchronizedString : SynchronizedValue<string>
    {
        public SynchronizedString()
            : base()
        {
            Value = "";
        }

        public override int Size
        {
            get { return Value.GetNetworkByteCount(); }
        }

        protected override string UnpackValue(Network.ByteInStream stream)
        {
            return stream.ReadString();
        }

        public override void Pack(Network.ByteOutStream stream)
        {
            stream.WriteString(Value ?? "");
        }

        protected override bool ValuesEqual(string a, string b)
        {
            return a == b;
        }
    }
}
