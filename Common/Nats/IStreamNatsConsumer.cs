using Common.Nats.Configuration;

namespace Common.Nats;

public interface IStreamNatsConsumer
{
    IObservable<MessageBase> SubscribeOnStreamAsync(string consumerName, NatsStreamConfiguration streamConfiguration,
        string subject);
}