using ProtoBuf;

namespace Common.Nats.Contracts;

[ProtoContract]
public class HelloMessageResponse : HelloMessage
{
    [ProtoMember(5)]
    public Guid ReceiverId { get; set; }
}