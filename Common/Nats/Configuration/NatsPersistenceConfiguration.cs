namespace Common.Nats.Configuration;

/// <summary>
///     Конфигурация стримов в NATS JetStream.
/// </summary>
/// <param name="Streams">Список стримов <see cref="NatsStreamConfiguration" />.</param>
public record NatsPersistenceConfiguration(List<NatsStreamConfiguration> Streams);