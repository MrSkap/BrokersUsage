using NATS.Client.Rx;

namespace Common.Nats;

public interface INatsClient
{
    void PublishMessage(MessageBase message, string subject);
    IObservable<MessageBase> SubscribeOnSubject(string subject);
}