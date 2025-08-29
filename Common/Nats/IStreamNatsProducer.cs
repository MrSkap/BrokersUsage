namespace Common.Nats;

/// <summary>
/// Продьюсер Nats JetStream.
/// </summary>
public interface IStreamNatsProducer
{
    /// <summary>
    /// Опубликовать сообщение.
    /// </summary>
    /// <param name="stream">Стрим.</param>
    /// <param name="subject">Субъект.</param>
    /// <param name="message">Сообщение <see cref="MessageBase"/>.</param>
    /// <returns>Задача.</returns>
    Task PublishAsync(string stream, string subject, MessageBase message);
}