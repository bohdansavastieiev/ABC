namespace ABC.Infrastructure.MessageBroker;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";
    
    public string Host { get; set; } = "localhost";
    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public ushort Port { get; set; } = 5672;

    public int RetryCount { get; set; } = 3;
    public int RetryIntervalSeconds { get; set; } = 5;
    
    // ADD THESE:
    public int ConcurrencyRetryLimit { get; set; } = 3;
    public int ConcurrencyRetryMinIntervalMs { get; set; } = 100;
    public int ConcurrencyRetryMaxIntervalMs { get; set; } = 1000;
    public int ConcurrencyRetryIntervalDeltaMs { get; set; } = 100;
}