using System.Collections;
using SlimNet;

[ClientContextPlugin]
public class ClientContextPlugin : DefaultContextPlugin
{
    public override ISpatialPartitioner CreateSpatialPartitioner()
    {
        return new SlimNet.Collections.QuadTree();
    }

    public override void ActorSpawned(Actor actor)
    {
        if (actor.IsMine)
        {
            GameSettings.PlayerActor = actor;
        }
    }
}