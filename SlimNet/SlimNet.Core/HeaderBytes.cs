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
    public static class HeaderBytes
    {
        public const byte ActorStateStream = 0;
        public const byte RemoteProcedureCall = 1;
        public const byte Synchronizable = 2;
        public const byte EventHello = 3;
        public const byte EventSpawn = 4;
        public const byte EventAuthenticated = 5;
        public const byte EventDespawn = 6;
        public const byte EventLogin = 7;
        public const byte EventChangeOwner = 8;
        public const byte UserStart = 16;
    }
}
