namespace Common.Nats;

public interface IStreamNatsProducer
{
    Task PublishAsync(string stream, string subject, MessageBase message);
}