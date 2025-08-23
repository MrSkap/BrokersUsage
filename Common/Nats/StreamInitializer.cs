using Common.Nats.Configuration;
using NATS.Client;
using NATS.Client.JetStream;
using Serilog;

namespace Common.Nats;

public class StreamInitializer : NatsClient, IStreamInitializer
{
    private static readonly ILogger Logger = Log.ForContext<StreamInitializer>();
    private readonly NatsPersistenceConfiguration _configuration;

    public StreamInitializer(NatsConnectionOptions connectionOptions, NatsPersistenceConfiguration configuration) :
        base(connectionOptions)
    {
        _configuration = configuration;
    }

    public void InitStreams()
    {
        if (CurrentConnection.State is not ConnState.CONNECTED)
        {
            Logger.Error("Can`t init Nats streams. No connection to Nats");
            throw new Exception("No connection to Nats");
        }

        var jsm = CurrentConnection.CreateJetStreamManagementContext();
        var names = GetStreamNames(jsm);

        var existingStreams = _configuration.Streams.Where(x => names.Contains(x.StreamName));
        Logger.Information("This streams are already exist: {Streams}", existingStreams);

        var notExistingStreams = _configuration.Streams.Where(x => !names.Contains(x.StreamName)).ToList();
        Logger.Information("-> Create not existing streams {Streams}", notExistingStreams);
        foreach (var stream in notExistingStreams) InitStream(stream, jsm);
        Logger.Information("<- Create not existing streams {Streams}", notExistingStreams);
    }

    private void InitStream(NatsStreamConfiguration configuration, IJetStreamManagement jsm)
    {
        var streamConfig = StreamConfiguration.Builder()
            .WithName(configuration.StreamName)
            .WithSubjects(configuration.SubjectWildcard)
            .WithStorageType(StorageType.File)
            // Политика хранения сообщений
            .WithRetentionPolicy(configuration.Policy)
            // Максимальное время хранения сообщений (в секундах)
            .WithMaxAge(configuration.MaxAge)
            // Максимальное количество сообщений в стриме
            .WithMaxMessages(configuration.MaxMessages)
            // Максимальный размер стрима в байтах
            .WithMaxBytes(configuration.MaxBytes)
            .Build();

        try
        {
            jsm.AddStream(streamConfig);
        }
        catch (Exception e)
        {
            Logger.Error("Failed to create new stream {Stream} \n {Ex}", configuration.StreamName, e);
        }
    }

    private IList<string> GetStreamNames(IJetStreamManagement jsm)
    {
        try
        {
            return jsm.GetStreamNames();
        }
        catch (Exception e)
        {
            Logger.Information("There are no Nats streams");
            return [];
        }
    }
}