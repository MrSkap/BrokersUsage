namespace Producer.Services;

public interface IHelloAwaitService
{
    Task SendHelloAndWaitReplyAsync();
}