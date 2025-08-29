using Common.Nats.Configuration;
using Common.Nats.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Nats.Initializers;

/// <summary>
///     Класс для иницилизации объектов, необходимых для работы с Nats.
/// </summary>
public class NatsInitializer
{
    /// <summary>
    ///     Начать обработку сообщений.
    /// </summary>
    /// <param name="provider">Провайдер. <see cref="IServiceProvider" />.</param>
    /// <param name="serviceName">Название сервиса. Используется в качестве идентификатора подписчика на стримы Nats.</param>
    public static void StartNatsProcessing(IServiceProvider provider, string serviceName)
    {
        var handler = provider.GetService<IBaseMessageHandler>();
        handler?.StartMessageProcessing($"{serviceName}.{NatsSubjectName.HelloSubject}");
    }

    /// <summary>
    ///     Инициализировать стримы Nats.
    /// </summary>
    /// <param name="provider">Провайдер. <see cref="IServiceProvider" />.</param>
    public static void InitNatsStreams(IServiceProvider provider)
    {
        var initializer = provider.GetRequiredService<IStreamInitializer>();
        initializer.InitStreams();
    }

    public static void StartStreamsMessageProcessing(IServiceProvider provider)
    {
        var configuration = provider.GetRequiredService<NatsStreamSubscriptionConfiguration>();
        var handler = provider.GetRequiredService<IBaseStreamMessageHandler>();
        foreach (var subscription in configuration.Subscriptions)
            handler.StartMessageProcessing(configuration.ConsumerId, subscription.Stream, subscription.Subject);
    }
}