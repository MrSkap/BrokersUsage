using MediatR;

namespace Common.Nats.Handlers;

public class NatsMessageProcessingRequest<T> : IRequest
    where T : MessageBase
{
    public T? Message { get; set; }
}