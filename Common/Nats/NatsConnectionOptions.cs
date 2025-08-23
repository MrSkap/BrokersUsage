namespace Common.Nats;

public sealed class NatsConnectionOptions
{
    public static string SectionName = "Nats";
    public required string ConnectionString { get; set; }
}