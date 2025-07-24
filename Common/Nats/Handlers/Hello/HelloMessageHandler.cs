using Common.Nats.Contracts;
using MediatR;
using Serilog;

namespace Common.Nats.Handlers.Hello;

public class HelloMessageHandler : IRequestHandler<NatsMessageProcessingRequest<HelloMessage>>
{
    private static readonly ILogger Logger = Log.ForContext<HelloMessageHandler>();

    public Task Handle(NatsMessageProcessingRequest<HelloMessage> request, CancellationToken cancellationToken)
    {
        Logger.Information("Get hello message from {Name} with message \"{Message}\"", request.Message?.SenderName,
            request.Message?.Message);
        return Task.CompletedTask;
    }
}