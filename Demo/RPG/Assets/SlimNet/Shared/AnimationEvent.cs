using SlimNet;

public class AnimationEvent : Event<Actor>
{
    public const byte Forward = 1;
    public const byte Backward = 2;
    public const byte Jump = 3;
    public const byte Fall = 4;
    public const byte Land = 5;
    public const byte Idle = 6;
    public const byte Left = 7;
    public const byte Right = 8;
    public const byte ForwardLeft = 9;
    public const byte ForwardRight = 10;
    public const byte BackwardLeft = 11;
    public const byte BackwardRight = 12;

    public override int DataSize
    {
        get { return 1; }
    }

    public override bool Reliable
    {
        get { return false; }
    }

    public override byte EventId
    {
        get { return HeaderBytes.UserStart + 1; }
    }

    public override LogLevel LogLevel
    {
        get { return SlimNet.LogLevel.Trace; }
    }

    public byte State
    {
        get;
        set;
    }

    public AnimationEvent()
        : base(EventTargets.Owner | EventTargets.Remotes, EventSources.Owner)
    {

    }

    public override void Pack(SlimNet.Network.ByteOutStream stream)
    {
        stream.WriteByte(State);
    }

    public override void Unpack(SlimNet.Network.ByteInStream stream)
    {
        State = stream.ReadByte();
    }
}
