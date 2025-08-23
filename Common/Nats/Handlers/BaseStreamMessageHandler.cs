using MediatR;
using Serilog;

namespace Common.Nats.Handlers;

public class BaseStreamMessageHandler : IBaseStreamMessageHandler
{
    private static readonly ILogger Logger = Log.ForContext<BaseStreamMessageHandler>();
    private readonly IStreamNatsConsumer _consumer;
    private readonly IMediator _mediator;

    public BaseStreamMessageHandler(IStreamNatsConsumer consumer, IMediator mediator)
    {
        _consumer = consumer;
        _mediator = mediator;
    }

    public void StartMessageProcessing(
        string consumerName,
        string stream,
        string subject)
    {
        Logger.Information("Start nats message processing");
        _consumer.SubscribeOnStreamAsync(consumerName, stream, subject)
            .Subscribe(OnMessage);
    }

    private async void OnMessage(MessageBase messageBase)
    {
        Logger.Verbose("Get new message. Send it to concrete processor");
        await _mediator.Send(messageBase);
    }
}