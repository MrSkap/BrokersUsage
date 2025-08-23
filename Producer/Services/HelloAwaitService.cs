using Common.Nats;
using Common.Nats.Configuration;
using Common.Nats.Contracts;
using Polly;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Producer.Services;

public class HelloAwaitService : IHelloAwaitService
{
    private static readonly ILogger Logger = Log.ForContext<HelloAwaitService>();
    private readonly INatsClient _natsClient;

    public HelloAwaitService(INatsClient natsClient)
    {
        _natsClient = natsClient;
    }

    public async Task SendHelloAndWaitReplyAsync()
    {
        var message = GetHelloMessage();
        var subject = GetHelloSubject();
        await Policy
            .HandleInner<Exception>(e =>
            {
                Logger.Warning(e, $"Get error while sending hello to {NatsServiceName.ConsumerService}");
                return true;
            })
            .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(10), (_, _) => Logger.Warning("Try again"))
            .ExecuteAsync(async () => await Task.Run(() => _natsClient.PublishMessage(message, subject)));
    }

    private HelloMessage GetHelloMessage()
    {
        return new HelloMessage
        {
            Message = "It's producer. Hello!",
            SenderId = Guid.NewGuid(),
            SenderName = NatsServiceName.ProducerService,
            ShouldSendResponse = true
        };
    }

    private string GetHelloSubject()
    {
        return $"{NatsServiceName.ConsumerService}.{NatsSubjectName.HelloSubject}";
    }
}