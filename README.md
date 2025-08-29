# BrokersUsage
Примеры работы с брокерами


## Nats
В `Common.Nats` расположены базовые классы для работы с Nats Core и Nats JetStream.


### NatsCore
`NatsClient` - базовый клиент Nats использующийся в качестве родительского для клиентов JetStream. 
При иницилизации автоматически подключается к Nats, умеет публиковать сообщения в субъекты и подписываться на субъекты в Nats Core.
Для работы рекомендуется использовать интерфейс INatsClient предварительно создав экземпляр для подключения.


### Nats JetStream
Для работы с JetStream необходимо использовать StreamNatsConsumer и StreamNatsProducer для прослушивания и публикации сообщений. Также клиенты лучше использовать через соответствующие интерфейсы
IStreamNatsConsumer и IStreamNatsProducer предварительно их проинициализировав.

Для создания новых stream можно использовать StreamInitializer.

### Работа с сообщениями
Для работы с сообщениями используется protobuf. Для отправки и получения сообщений через указанные клиенты необходимо создать наследников от MessageBase и добавить их в атрибуты.
Для подписки на субъекты рекомендуется использовать `BaseMessageHandler` и `BaseStreamMessageHandler` для Core и JetStream соответственно.

```
// для Core
handler?.StartMessageProcessing(subject);

// для JetStream
handler.StartMessageProcessing(consumerId, stream, subject);
```

Для обработки сообщений необходимо создать соответствующий каждому сообщению обработчик.
`MessageHandler` получает сообщение и публоикует его в MediatR.

Пример обработчика сообщений.
```
public class HelloMessageHandler : IRequestHandler<NatsMessageProcessingRequest<HelloMessage>>
{
    private static readonly ILogger Logger = Log.ForContext<HelloMessageHandler>();

    public Task Handle(NatsMessageProcessingRequest<HelloMessage> request, CancellationToken cancellationToken)
    {
        Logger.Information("Get hello message from {Name} with message \"{Message}\"", request.Message?.SenderName,
            request.Message?.Message);
        return Task.CompletedTask;
    }
}
```