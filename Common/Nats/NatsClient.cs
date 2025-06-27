using System.Reactive.Linq;
using NATS.Client;
using NATS.Client.Rx;
using Serilog;
using Serializer = ProtoBuf.Serializer;

namespace Common.Nats;

public class NatsClient : INatsClient, IDisposable
{
    private static readonly ILogger Logger = Log.ForContext<NatsClient>();
    private readonly NatsConnectionOptions _connectionOptions;
    private readonly ConnectionFactory _connectionFactory;
    private IConnection _currentConnection;
    private readonly Dictionary<string, SubscriptionObject> _activeSubscriptionObjects = new();

    public NatsClient(NatsConnectionOptions connectionOptions)
    {
        _connectionOptions = connectionOptions;
        _connectionFactory = new ConnectionFactory();
        Connect();
    }

    public void PublishMessage(MessageBase message, string subject)
    {
        try
        {
            if (_currentConnection.State is not ConnState.CONNECTED) throw new Exception("No connection to nats");

            using var memoryStream = new MemoryStream();
            Serializer.Serialize(memoryStream, message);
            _currentConnection.Publish(subject, memoryStream.GetBuffer());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public IObservable<MessageBase> SubscribeOnSubject(string subject)
    {
        if (_activeSubscriptionObjects.TryGetValue(subject, out var activeSubscription))
            return activeSubscription.Observable;
        var subscription = _currentConnection.SubscribeAsync(subject);
        var natsObservable = subscription.ToObservable();
        var observable = Observable.Create<MessageBase>(observer =>
        {
            var messageBaseObservable = natsObservable.Subscribe(msg =>
            {
                var deserialized = TryDeserialize(msg);
                if (deserialized is not null) observer.OnNext(deserialized);
            });

            return messageBaseObservable;
        });


        var subscriptionObject = new SubscriptionObject
        {
            Observable = observable,
            Subscription = subscription
        };
        _activeSubscriptionObjects.Add(subject, subscriptionObject);

        return observable;
    }

    public void Dispose()
    {
        _currentConnection.Dispose();
        foreach (var active in _activeSubscriptionObjects) active.Value.Dispose();
    }

    private MessageBase? TryDeserialize(Msg message)
    {
        try
        {
            if (message.Data.Length == 0) throw new Exception("Empty message");

            if (message.Status.IsNoResponders()) throw new Exception("No responders");

            return Serializer.Deserialize<MessageBase>(message.Data.AsSpan());
        }
        catch (Exception e)
        {
            Logger.Error("Error while deserializing message, {Error}", e.Message);
        }

        return null;
    }

    private void Connect()
    {
        _currentConnection = _connectionFactory.CreateConnection(
            _connectionOptions.ConnectionString,
            true);
    }

    private class SubscriptionObject : IDisposable
    {
        public required IAsyncSubscription Subscription { get; set; }

        public required IObservable<MessageBase> Observable { get; set; }

        public void Dispose()
        {
            if (Subscription.IsValid && Subscription.Connection.IsClosed() is false) Subscription.Connection.Drain();

            Subscription.Dispose();
        }
    }
}