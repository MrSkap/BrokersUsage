namespace Producer.Models.Options;

/// <summary>
/// Настройки топика Кафки, используется при первоначальной инициализации.
/// </summary>
public class TopicsConfiguration
{
    public TopicConfiguration Kafka { get; set; } = null!;
    public TopicConfiguration Test { get; set; } = null!;
}