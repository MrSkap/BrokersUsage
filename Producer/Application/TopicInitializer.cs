using Common.Configuration;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Polly;
using Producer.Models.Options;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Producer.Application;

public class TopicInitializer(TopicsConfiguration topicConfiguration, KafkaConfiguration kafkaConfiguration)
{
    private static readonly ILogger Logger = Log.ForContext<TopicInitializer>();
    
    private static readonly IDictionary<string, Func<TopicsConfiguration, TopicConfiguration>> TopicConfigs = new Dictionary<string, Func<TopicsConfiguration, TopicConfiguration>>
    {
        { KafkaConstants.KafkaDefaultName, configuration => configuration.Kafka },
        { KafkaConstants.TestName, configuration => configuration.Test },
    };
    
    private readonly ProducerConfig _producerConfig = new ProducerConfig()
    {
        BootstrapServers = kafkaConfiguration.BootstrapServers,
    };
    
    /// <summary>
    /// Инициализировать топики.
    /// </summary>
    /// <returns>Таска.</returns>
    public async Task InitializeAsync()
    {
        Logger.Information("-> Initializing Kafka topics");

        using (var adminClient = new AdminClientBuilder(new ProducerConfig(_producerConfig))
                   .Build())
        {
            foreach (var topicName in TopicConfigs.Keys)
            {
                if (!IsTopicExists(adminClient, topicName))
                {
                    await CreateTopicAsync(adminClient, topicName);
                }
            }
        }

        Logger.Information("<- Initializing Kafka topics");
    }

    private bool IsTopicExists(IAdminClient adminClient, string topicName)
    {
        var topicMetadata = GetTopicMetadata(adminClient, topicName);
        Logger.Debug("Got {TopicName} metadata: {@Metadata}", topicName, topicMetadata);

        return topicMetadata != null;
    }

    private async Task CreateTopicAsync(IAdminClient adminClient, string topicName)
    {
        Logger.Information("-> Creating topic {TopicName}", topicName);

        var topicConfig = GetTopicConfig(topicName);

        if (topicConfig is null)
        {
            Logger.Warning("Unknown topic name: {TopicName}. Skipping...", topicName);
            return;
        }

        var topicSpec = new TopicSpecification
        {
            Configs = topicConfig!.Parameters,
            Name = topicName,
            NumPartitions = topicConfig.NumPartitions,
            ReplicationFactor = topicConfig.ReplicationFactor,
        };

        try
        {
            await adminClient.CreateTopicsAsync(new[] { topicSpec });
        }
        catch (CreateTopicsException ex)
        {
            var error = ex.Results.First().Error.Code;
            Logger.Warning("Failed to create topic {Topic} {KafkaError}", topicName, error.ToString());
            if (error != ErrorCode.TopicAlreadyExists)
            {
                throw;
            }
        }

        Logger.Information("<- Creating topic {TopicName}", topicName);
    }

    private TopicMetadata? GetTopicMetadata(IAdminClient adminClient, string topicName)
    {
        Logger.Information("-> Retrieving metadata for topic {TopicName}", topicName);
        var metadata = Policy
            .Handle<KafkaException>()
            .OrResult<Metadata>(x =>
            {
                var error = x.Topics.FirstOrDefault(t => t.Topic == topicName)!.Error.Code;
                return error != ErrorCode.NoError && error != ErrorCode.UnknownTopicOrPart;
            })
            .WaitAndRetryForever(retry => TimeSpan.FromSeconds(10),(result, span) => Logger.Information("Retry to get metadata from {Topic} topic", topicName))
            .Execute(() => adminClient.GetMetadata(topicName, TimeSpan.FromSeconds(10)));
        return metadata.Topics.FirstOrDefault(x => x.Topic == topicName && x.Error.Code == ErrorCode.NoError);
    }

    private TopicConfiguration? GetTopicConfig(string topicName)
    {
        TopicConfigs.TryGetValue(topicName, out var valueFunc);
        var value = valueFunc?.Invoke(topicConfiguration);
        if (value is null)
            return null;
        return new TopicConfiguration()
        {
            NumPartitions = value.NumPartitions,
            Parameters = value.Parameters,
            ReplicationFactor = value.ReplicationFactor
        };
    }
}