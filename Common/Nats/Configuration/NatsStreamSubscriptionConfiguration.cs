namespace Common.Nats.Configuration;

public class NatsStreamSubscriptionConfiguration
{
    public static string SectionName = "NatsStreams";
    public string ConsumerId { get; set; } = "DefaultConsumer";
    public List<NatsSubscription> Subscriptions { get; set; } = [];
}