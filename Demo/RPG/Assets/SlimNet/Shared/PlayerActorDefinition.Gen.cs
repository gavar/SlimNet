using System;
using System.Collections;
using System.Collections.Generic;
using SlimNet;
using SlimMath;

using Network = SlimNet.Network;
using Events = SlimNet.Events;

public partial class PlayerActorDefinition : ActorDefinition
{
    //{AutoGen}
    public override Collider Collider { get { return new BoxCollider(Vector3.Zero, new Vector3(0.3f, 0.75f, 0.3f), new Vector3(0f, 0.75f, 0f)); } }
    //{AutoGen}
}