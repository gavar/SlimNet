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
using System.Collections;
using System.Collections.Generic;
using SlimNet;
using SlimMath;

using Network = SlimNet.Network;
using Events = SlimNet.Events;

public  partial class PlayerActorDefinition : ActorDefinition
{
    public override int Id
    {
        get { return 0; }
    }

    public override IActorStateStreamer StateStreamer
    {
        get
        {
            return new DefaultStateStreamer(true, true);
        }
    }

    public override SynchronizedValue[] SynchronizedValues
    {
        get
        {
            return new SynchronizedValue[]
            {
                SynchronizedValue.Create<SynchronizedString>("Name", (v) => v.IgnoreClientValue = true),
                SynchronizedValue.Create<SynchronizedActor>("Target")
            };
        }
    }

    public override Behaviour[] Behaviours
    {
        get
        {
            return new Behaviour[]
            {
                new SlimNet.Behaviours.InterestManager(25)
            };
        }
    }
}