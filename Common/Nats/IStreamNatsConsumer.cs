namespace Common.Nats;

/// <summary>
/// Консъмер Nats JetStream.
/// </summary>
public interface IStreamNatsConsumer
{
    /// <summary>
    /// Подписаться на стрим.
    /// </summary>
    /// <param name="consumerName">Имя консьюмера.</param>
    /// <param name="stream">Название стрима.</param>
    /// <param name="subject">Название субъекта.</param>
    /// <returns>Обозреватель новых сообщений.</returns>
    IObservable<MessageBase> SubscribeOnStreamAsync(string consumerName,
        string stream,
        string subject);
}