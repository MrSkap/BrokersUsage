using ProtoBuf;

namespace Common.Nats.Contracts;

[ProtoContract]
public class HelloMessageResponse : MessageBase
{
    [ProtoMember(1)] public required string SenderName { get; set; }

    [ProtoMember(2)] public Guid SenderId { get; set; }

    [ProtoMember(3)] public required string Message { get; set; }

    [ProtoMember(4)] public bool ShouldSendResponse { get; set; }

    [ProtoMember(5)] public Guid ReceiverId { get; set; }
}