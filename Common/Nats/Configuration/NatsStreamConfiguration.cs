using NATS.Client.JetStream;

namespace Common.Nats.Configuration;

/// <summary>
///     Конфигурация стрима в NATS.
/// </summary>
public class NatsStreamConfiguration
{
    public required string StreamName { get; set; }
    public required string SubjectWildcard { get; set; }
    public RetentionPolicy Policy { get; set; } = RetentionPolicy.Limits;
    public int MaxAge { get; set; } = 3600 * 24;
    public int MaxBytes { get; set; } = 1024 * 1024 * 1024;
    public int MaxMessages { get; set; } = 100000;
}