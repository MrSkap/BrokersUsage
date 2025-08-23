namespace Common.Nats.Configuration;

public class NatsSubscription
{
    public required string Stream { get; set; }
    public required string Subject { get; set; }
}