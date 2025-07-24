using Common.Nats.Contracts;
using ProtoBuf;

namespace Common.Nats;

[ProtoContract]
[ProtoInclude(1, typeof(HelloMessage))]
[ProtoInclude(2, typeof(HelloMessageResponse))]
public class MessageBase
{
}