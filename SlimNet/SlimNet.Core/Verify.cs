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

namespace SlimNet
{
    public static class Verify
    {
        static readonly Log log = Log.GetLogger(typeof(Verify));

        public static bool Authenticated(Player player)
        {
            if (player == null)
            {
#if VERIFY_THROWS
                throw new RuntimeException("Player is null");
#else
                log.Error("Player is null");
                return false;
#endif
            }

            if (!player.IsAuthenticated)
            {

#if VERIFY_THROWS
                throw new RuntimeException("Player {0} is not authenticated", player);
#else
                log.Error("Player {0} is not authenticated", player);
                return false;
#endif
            }

            return true;
        }

        public static bool Active(Actor actor)
        {
            if (actor == null)
            {
#if VERIFY_THROWS
                throw new RuntimeException("Actor is null");
#else
                log.Error("Actor is null");
                return false;
#endif
            }

            if (!actor.IsActive)
            {

#if VERIFY_THROWS
                throw new RuntimeException("Actor {0} is not active", actor);
#else
                log.Error("Actor {0} is not active", actor);
                return false;
#endif
            }

            return true;
        }
    }
}
