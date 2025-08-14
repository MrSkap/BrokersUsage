using System.Reactive.Linq;
using System.Text;
using Common.Nats.Configuration;
using NATS.Client;
using NATS.Client.JetStream;
using Serilog;
using Serializer = ProtoBuf.Serializer;

namespace Common.Nats;

public class StreamNatsConsumer : NatsClient, IStreamNatsConsumer, IDisposable
{
    private static readonly ILogger Logger = Log.ForContext<StreamNatsConsumer>();
    private readonly Dictionary<string, StreamSubscriptionObject> _activeSubscriptionObjects = new();

    public StreamNatsConsumer(NatsConnectionOptions connectionOptions) : base(connectionOptions)
    {
    }

    public new void Dispose()
    {
        foreach (var subscription in _activeSubscriptionObjects) subscription.Value.Dispose();
    }

    public IObservable<MessageBase> SubscribeOnStreamAsync(string consumerName,
        NatsStreamConfiguration streamConfiguration, string subject)
    {
        InitConsumer(consumerName);

        var jetStream = CurrentConnection.CreateJetStreamContext();

        var subIndex = BuildIndex(consumerName, subject);
        if (_activeSubscriptionObjects.TryGetValue(subIndex, out var subscriptionObject))
            return subscriptionObject.Observable;

        IJetStreamPushAsyncSubscription? subscription = null;
        var observable = Observable.Create<MessageBase>(observer =>
        {
            var opts = PushSubscribeOptions.Builder()
                .WithStream(streamConfiguration.StreamName);

            subscription = jetStream.PushSubscribeAsync(subject, (_, args) =>
            {
                var msg = args.Message;
                try
                {
                    Logger.Debug($"Get message: {Encoding.UTF8.GetString(msg.Data)}");

                    var obj = TryDeserialize(msg);
                    if (obj is null) throw new NullReferenceException("After deserializing nats message is null");

                    observer.OnNext(obj);

                    // Notify NATS that message received and processed successfully
                    msg.Ack();
                }
                catch (Exception ex)
                {
                    Logger.Error("Got error while processing Nats message. {Error}", ex);

                    // Notify NATS that message's receiving or processing is failed
                    args.Message.Nak();
                }
            }, false, opts.Build());
            return subscription;
        });
        if (subscription is null) throw new ArgumentException($"Can't subscribe on subject: {subject}");

        _activeSubscriptionObjects.Add(subIndex, new StreamSubscriptionObject(observable, subscription));
        return observable;
    }

    private string BuildIndex(string consumerName, string subject)
    {
        return $"{consumerName}-{subject}";
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

    private void InitConsumer(string consumerName)
    {
        var consumerConfig = ConsumerConfiguration.Builder()
            .WithDurable(consumerName)
            .WithAckPolicy(AckPolicy.Explicit)
            .WithAckWait(60000) // 1 минута
            .WithMaxDeliver(3)
            .WithDeliverPolicy(DeliverPolicy.New)
            .WithReplayPolicy(ReplayPolicy.Original)
            .Build();

        var jsm = CurrentConnection.CreateJetStreamManagementContext();
        jsm.AddOrUpdateConsumer(consumerName, consumerConfig);
    }

    private class StreamSubscriptionObject : IDisposable
    {
        private readonly IDisposable _subscription;
        public readonly IObservable<MessageBase> Observable;

        public StreamSubscriptionObject(IObservable<MessageBase> observable, IDisposable subscription)
        {
            Observable = observable;
            _subscription = subscription;
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}