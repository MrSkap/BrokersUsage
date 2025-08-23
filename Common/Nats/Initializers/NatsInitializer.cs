using Common.Nats.Configuration;
using Common.Nats.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Nats.Initializers;

public class NatsInitializer
{
    public static void StartNatsProcessing(IServiceProvider provider, string serviceName)
    {
        var handler = provider.GetService<IBaseMessageHandler>();
        handler?.StartMessageProcessing(serviceName);
    }

    public static void InitNatsStreams(IServiceProvider provider)
    {
        var initializer = provider.GetRequiredService<IStreamInitializer>();
        initializer.InitStreams();
    }

    public static void StartStreamsMessageProcessing(IServiceProvider provider)
    {
        var configuration = provider.GetRequiredService<NatsStreamSubscriptionConfiguration>();
        var consumer = provider.GetRequiredService<IBaseStreamMessageHandler>();
        foreach (var subscription in configuration.Subscriptions)
            consumer.StartMessageProcessing(configuration.ConsumerId, subscription.Stream, subscription.Subject);
    }
}