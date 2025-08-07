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
}