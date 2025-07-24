using Common.Nats.Contracts;
using MediatR;
using Serilog;

namespace Common.Nats.Handlers.Hello;

public class HelloRequestMessageHandler : IRequestHandler<NatsMessageProcessingRequest<HelloMessageResponse>>
{
    private static readonly ILogger Logger = Log.ForContext<HelloRequestMessageHandler>();

    public Task Handle(NatsMessageProcessingRequest<HelloMessageResponse> request, CancellationToken cancellationToken)
    {
        Logger.Information("Get hello message response from {Name} with message \"{Message}\"",
            request.Message?.SenderName,
            request.Message?.Message);
        return Task.CompletedTask;
    }
}