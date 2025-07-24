namespace Common.Configuration;

/// <summary>
///     Настройки подключения Kafka.
///     <remarks>Используются продюсером, консюмером и админклиентом.</remarks>
/// </summary>
public class KafkaConfiguration
{
    /// <summary>
    ///     Имя секции настроек.
    /// </summary>
    public const string SectionName = "Kafka";

    /// <summary>
    ///     Список брокеров kafka.
    /// </summary>
    /// <example>broker1:port1,broker2:port2, broker3:port3 .</example>
    public string BootstrapServers { get; set; } = null!;
}