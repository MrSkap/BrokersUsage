namespace Common.Nats.Handlers;

public interface IBaseStreamMessageHandler
{
    public void StartMessageProcessing(
        string consumerName,
        string stream,
        string subject);
}