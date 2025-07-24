namespace Producer.Models.Options;

public class TopicConfiguration
{
    /// <summary>
    ///     Количество партиций - степень параллелизма.
    /// </summary>
    public int NumPartitions { get; set; }

    /// <summary>
    ///     Параметры из librdkafka. Ключ-значение.
    ///     <remarks>https://github.com/edenhill/librdkafka/blob/master/CONFIGURATION.md</remarks>
    /// </summary>
    public Dictionary<string, string> Parameters { get; set; } = new();

    /// <summary>
    ///     Определяет количество реплик.
    ///     <remarks>1 - соответствует отсутствию реплик.</remarks>
    /// </summary>
    public short ReplicationFactor { get; set; }
}