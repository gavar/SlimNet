using System.Collections;
using SlimNet;

public class ChatMessage : Event<Actor>
{
    public override int DataSize
    {
        get { return Message.GetNetworkByteCount(); }
    }

    public override byte EventId
    {
        get { return HeaderBytes.UserStart + 0; }
    }

    public override bool SynchronizeEvent
    {
        get { return false; }
    }

    public string Message
    {
        get;
        set;
    }

    public ChatMessage()
        : base(EventTargets.Owner | EventTargets.Remotes | EventTargets.Server, EventSources.Owner)
    {

    }

    public override void Pack(SlimNet.Network.ByteOutStream stream)
    {
        stream.WriteString(Message);
    }

    public override void Unpack(SlimNet.Network.ByteInStream stream)
    {
        Message = stream.ReadString();
    }
}