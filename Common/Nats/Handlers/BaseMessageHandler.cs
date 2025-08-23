using Common.Nats.Configuration;
using MediatR;
using Serilog;

namespace Common.Nats.Handlers;

public class BaseMessageHandler : IBaseMessageHandler
{
    private static readonly ILogger Logger = Log.ForContext<BaseMessageHandler>();
    private readonly INatsClient _client;
    private readonly IMediator _mediator;

    public BaseMessageHandler(INatsClient client, IMediator mediator)
    {
        _client = client;
        _mediator = mediator;
    }

    public void StartMessageProcessing(string serviceName = "")
    {
        Logger.Information("Start nats message processing");
        _client.SubscribeOnSubject($"{serviceName}.{NatsSubjectName.HelloSubject}")
            .Subscribe(OnMessage);
    }

    private async void OnMessage(MessageBase messageBase)
    {
        Logger.Verbose("Get new message. Send it to concrete processor");
        await _mediator.Send(messageBase);
    }
}