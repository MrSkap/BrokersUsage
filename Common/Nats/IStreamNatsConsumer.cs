namespace Common.Nats;

public interface IStreamNatsConsumer
{
    IObservable<MessageBase> SubscribeOnStreamAsync(string consumerName,
        string stream,
        string subject);
}