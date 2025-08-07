using Common.Nats.Contracts;
using Common.Nats.Handlers;
using Common.Nats.Handlers.Hello;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Nats.Extensions;

public static class NatsExtensions
{
    public static IServiceCollection AddNats(this IServiceCollection collection)
    {
        collection
            .AddSingleton<INatsClient, NatsClient>()
            .AddSingleton<IBaseMessageHandler, BaseMessageHandler>()
            .AddHandlers();
        return collection;
    }

    private static IServiceCollection AddHandlers(this IServiceCollection collection)
    {
        collection.AddMediatR(x => { });
        collection.AddTransient<IRequestHandler<NatsMessageProcessingRequest<HelloMessage>>, HelloMessageHandler>();
        return collection;
    }

    public static IServiceCollection AddStreamProducer(this IServiceCollection collection)
    {
        collection.AddSingleton<IStreamInitializer, StreamInitializer>();
        return collection;
    }
}