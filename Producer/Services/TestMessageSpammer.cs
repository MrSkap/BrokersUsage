using Common.Nats;
using Common.Nats.Contracts;
using Polly;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Producer.Services;

public class TestMessageSpammer : ITestMessageSpammer, IDisposable
{
    private static readonly ILogger Logger = Log.ForContext<TestMessageSpammer>();
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly IStreamNatsProducer _producer;
    private readonly Dictionary<string, Task> _subjectSpamTasks = new();
    private readonly CancellationToken _token;

    public TestMessageSpammer(IStreamNatsProducer producer)
    {
        _producer = producer;
        _cancellationTokenSource = new CancellationTokenSource();
        _token = _cancellationTokenSource.Token;
    }

    public async void Dispose()
    {
        await _cancellationTokenSource.CancelAsync();
        while (_subjectSpamTasks.Any(x => !x.Value.IsCompleted)) await Task.Delay(100);
        _cancellationTokenSource.Dispose();
    }

    public void StartTestMessageSpamming(string stream, string subject)
    {
        _subjectSpamTasks.Add(subject, Spam(stream, subject));
    }

    private async Task Spam(string stream, string subject)
    {
        var spamMessage = new HelloMessage
        {
            Message = "SpamMessage",
            SenderName = "Producer",
            SenderId = Guid.Empty
        };
        await Policy
            .HandleInner<Exception>(e =>
            {
                Logger.Warning(e, $"Get error while sending hello to {subject}");
                return true;
            })
            .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(10), (_, _) => Logger.Warning("Try again"))
            .ExecuteAsync(
                async () => await Task.Run(() => _producer.PublishAsync(stream, subject, spamMessage), _token));
    }
}