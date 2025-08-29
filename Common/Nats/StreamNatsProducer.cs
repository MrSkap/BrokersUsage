using NATS.Client;
using NATS.Client.JetStream;
using Serilog;
using Serializer = ProtoBuf.Serializer;

namespace Common.Nats;

/// <summary>
///     Продьюсер Nats. Подключается к Nats и позволяет публиковать сообщения.
/// </summary>
public class StreamNatsProducer : NatsClient, IStreamNatsProducer
{
    private const int Timeout = 30000;
    private static readonly ILogger Logger = Log.ForContext<StreamNatsConsumer>();
    private readonly IJetStream _jetStream;

    public StreamNatsProducer(NatsConnectionOptions connectionOptions) : base(connectionOptions)
    {
        _jetStream = CurrentConnection.CreateJetStreamContext();
    }

    public async Task PublishAsync(string stream, string subject, MessageBase message)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            Serializer.Serialize(memoryStream, message);
            var protobufData = memoryStream.ToArray();
            var options = BuildPublishOptions(stream);
            await _jetStream.PublishAsync(subject, protobufData, options);
        }
        catch (OperationCanceledException ex)
        {
            Logger.Error("Publication was canceled {Ex}", ex);
            throw;
        }
        catch (NATSException ex)
        {
            Logger.Error("Error while publishing the message {Message}. Error: {Ex}", message, ex);
            throw;
        }
    }

    private PublishOptions BuildPublishOptions(string stream)
    {
        var builder = new PublishOptions.PublishOptionsBuilder();

        builder.WithTimeout(Timeout);
        if (!string.IsNullOrEmpty(stream)) builder.WithExpectedStream(stream);

        return builder.Build();
    }
}