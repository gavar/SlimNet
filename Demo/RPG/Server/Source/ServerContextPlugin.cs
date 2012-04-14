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

using SlimNet;
using SlimNet.Events;

[ServerContextPlugin]
public class ServerContextPlugin : SlimNet.DefaultContextPlugin
{
    static ServerContextPlugin self;
    static Log log = Log.GetLogger(typeof(ServerContextPlugin));

    Database db;

    public override ISpatialPartitioner CreateSpatialPartitioner()
    {
        return new SlimNet.Collections.QuadTree();
    }

    public override void AfterSimulate()
    {

    }

    public override void ContextStarted()
    {
        self = this;

        db = new Database("localhost", "slimnet", "root", "");
        db.LogoutAllUsers();

        Context.ActorEventHandler.RegisterReceiver<ChatMessage>(onChatMessage);
        Context.PlayerEventHandler.RegisterReceiver<Authenticated>(onAuthenticated);
    }

    void onAuthenticated(Authenticated ev)
    {
        if (ev.IsAuthenticated)
        {
            string accountName = (ev.Target.Tag as PlayerData).AccountName;

            Actor actor = Context.Server.SpawnActor<PlayerActorDefinition>(ev.Target, new SlimMath.Vector3(0, 10, 0));
            actor.GetValue<string>("Name").Value = accountName;

            base.PlayerJoined(ev.Target);

            actor.RaiseEvent<ChatMessage>((chat) =>
            {
                chat.Message = string.Format("<{0} joined the game>", accountName);
            });
        }
        else
        {
            if ((ev.Target.Tag as PlayerData).LoginAttempts++ == 3)
            {
                ev.Target.Disconnect();
            }
        }
    }

    void onChatMessage(ChatMessage ev)
    {
        if (ev.IsRemote)
        {
            ev.Message = ev.Target.GetValue<string>("Name").Value + ": " + ev.Message;
        }
    }

    public override void PlayerJoined(SlimNet.Player player)
    {
        player.Tag = new PlayerData();
    }

    [SlimNet.RPC(typeof(RPC))]
    public static string CreateAccount(string account, string password, RPCInfo info)
    {
        return self.db.CreateAccount(account, password);
    }

    [SlimNet.RPC(typeof(RPC))]
    public static void Login(string account, string password, RPCInfo info)
    {
        self.Context.PlayerEventHandler.Raise<SlimNet.Events.Authenticated>(info.Caller,
            (ev) =>
            {
                ev.Error = self.db.Login(account, password, info.Caller);
                ev.IsAuthenticated = ev.Error == "";

                if (ev.IsAuthenticated)
                {
                    (info.Caller.Tag as PlayerData).AccountName = account;
                }
            }
        );
    }
}