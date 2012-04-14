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