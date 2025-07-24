using Common.Configuration;
using Confluent.Kafka;
using Producer.Models.Options;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Producer.Application;

public class Producer(TopicsConfiguration topicConfiguration, KafkaConfiguration kafkaConfiguration) : IProducer
{
    private const string Topic = "test";
    private static readonly ILogger Logger = Log.ForContext<TopicInitializer>();

    private readonly ProducerConfig _producerConfig = new()
    {
        BootstrapServers = kafkaConfiguration.BootstrapServers
    };

    public async Task ProduceAsync(string message)
    {
        using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();
        Logger.Information("-> Producing message: {message}", message);
        await producer.ProduceAsync(Topic, new Message<string, string> { Key = message, Value = message });
        Logger.Information("<- Producing message");
        producer.Flush(TimeSpan.FromSeconds(10));
    }
}