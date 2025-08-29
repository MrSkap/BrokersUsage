namespace Common.Nats;

/// <summary>
///     Интерфейс инициализации стримов.
/// </summary>
public interface IStreamInitializer
{
    /// <summary>
    ///     Инициализировать стримы.
    /// </summary>
    void InitStreams();
}