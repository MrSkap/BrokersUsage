namespace Common.Nats;

/// <summary>
///     Настройки подключения к Nats.
/// </summary>
public sealed class NatsConnectionOptions
{
    public static string SectionName = "Nats";

    /// <summary>
    ///     Строка подключения.
    /// </summary>
    public required string ConnectionString { get; set; }
}