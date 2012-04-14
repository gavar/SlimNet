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
    public partial class Context
    {
        public bool GetPlayer(ushort playerId, out Player player)
        {
            if(players.TryGetValue(playerId, out player))
            {
                Assert.NotNull(player, "player");
                return true;
            }

            return false;
        }

        internal Player CreatePlayer(ushort playerId, Network.IConnection connection)
        {
            Assert.NotNull(connection, "connection");

            if (players.ContainsKey(playerId))
            {
                log.Error("A player with id #{0} already exists", playerId);
                return null;
            }

            Player player = new Player(this, connection, playerId);
            players.Add(playerId, player);

            Peer.PlayerJoined(player);
            Peer.ContextPlugin.PlayerJoined(player);

            return player;
        }

        internal void RemovePlayer(Player player)
        {
            Assert.NotNull(player, "player");

            if (!players.ContainsKey(player.Id))
            {
                log.Error("No player with id #{0} exists", player.Id);
                return;
            }

            Peer.ContextPlugin.PlayerLeaving(player);
            Peer.PlayerLeaving(player);

            players.Remove(player.Id);
        }
    }
}
