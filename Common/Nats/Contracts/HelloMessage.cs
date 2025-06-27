using ProtoBuf;

namespace Common.Nats.Contracts;

[ProtoContract]
[ProtoInclude(1, typeof(HelloMessageResponse))]
public class HelloMessage : MessageBase
{
    [ProtoMember(1)]
    public required string SenderName { get; set; }
    
    [ProtoMember(2)]
    public Guid SenderId { get; set; }
    
    [ProtoMember(3)]
    public required string Message { get; set; }
    
    [ProtoMember(4)]
    public bool ShouldSendResponse { get; set; }
}