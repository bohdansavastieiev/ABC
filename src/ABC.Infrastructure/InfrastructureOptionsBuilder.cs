namespace ABC.Infrastructure;

public class InfrastructureOptionsBuilder
{
    internal bool IsApiKeyValidationEnabled { get; private set; }
    internal bool IsCachingEnabled { get; private set; }
    internal bool IsPublisherEnabled { get; private set; }
    internal IReadOnlyList<Type> ConsumerTypes { get; private set; } = [];
    
    
    public InfrastructureOptionsBuilder WithApiKeyValidation()
    {
        IsApiKeyValidationEnabled = true;
        return this;
    }
    
    public InfrastructureOptionsBuilder WithCaching()
    {
        IsCachingEnabled = true;
        return this;
    }
    
    public InfrastructureOptionsBuilder WithPublisher()
    {
        IsPublisherEnabled = true;
        return this;
    }
    
    public InfrastructureOptionsBuilder WithMessageBrokerConsumer(params Type[] consumerTypes)
    {
        ConsumerTypes = consumerTypes;
        return this;
    }
}