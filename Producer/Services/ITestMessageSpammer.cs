namespace Producer.Services;

public interface ITestMessageSpammer
{
    void StartTestMessageSpamming(string stream, string subject);
}