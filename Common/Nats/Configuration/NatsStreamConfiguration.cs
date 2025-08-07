using NATS.Client.JetStream;

namespace Common.Nats.Configuration;

/// <summary>
///     Конфигурация стрима в NATS.
/// </summary>
/// <param name="StreamName">Название стрима.</param>
/// <param name="SubjectWildcard">Субъекты стрима (можно задать один или через wildcard).</param>
/// <param name="Policy">Политика хранения. <see cref="RetentionPolicy" />. Дефолтное - по лимитам.</param>
/// <param name="MaxAge">Максимальное время хранения сообщения. Дефолтное - 1 день.</param>
/// <param name="MaxBytes">Максимальный размер хранимых сообщений в стриме.</param>
/// <param name="MaxMessages">Максимальное количество хранимых сообщений.</param>
public record NatsStreamConfiguration(
    string StreamName,
    string? SubjectWildcard,
    RetentionPolicy Policy = RetentionPolicy.Limits,
    int MaxAge = 3600 * 24,
    int MaxBytes = 1024 * 1024 * 1024,
    int MaxMessages = 100000);