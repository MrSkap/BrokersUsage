using Microsoft.AspNetCore.Mvc;
using Producer.Application;

namespace Producer.API.Controller;

[Route("api/[controller]")]
public class ProducerController(IProducer producer) : ControllerBase
{
    private readonly IProducer _producer = producer;

    [HttpGet("produce")]
    public async Task ProduceMessageAsync(string message)
    {
        await producer.ProduceAsync(message);
    }
}