namespace Common.Nats;

/// <summary>
///     Клиент Nats.
/// </summary>
public interface INatsClient
{
    /// <summary>
    ///     Опубликовать сообщение.
    /// </summary>
    /// <param name="message">Сообщение <see cref="MessageBase" />.</param>
    /// <param name="subject">Субъект для публикации.</param>
    void PublishMessage(MessageBase message, string subject);

    /// <summary>
    ///     Подписаться на субъект.
    /// </summary>
    /// <param name="subject">Субъкт для подписки.</param>
    /// <returns>Обозреватель новых сообщений.</returns>
    IObservable<MessageBase> SubscribeOnSubject(string subject);
}