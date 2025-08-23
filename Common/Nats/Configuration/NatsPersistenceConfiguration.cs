namespace Common.Nats.Configuration;

/// <summary>
///     Конфигурация стримов в NATS JetStream.
/// </summary>
public class NatsPersistenceConfiguration
{
    public static string SectionName = "NatsStreams";
    public List<NatsStreamConfiguration> Streams { get; set; } = [];
}