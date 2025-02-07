namespace Producer.Application;

public interface IProducer
{
    Task ProduceAsync(string message);
}