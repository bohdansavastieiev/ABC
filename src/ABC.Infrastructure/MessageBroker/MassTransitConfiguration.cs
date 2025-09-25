using ABC.Application.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ABC.Infrastructure.MessageBroker;

public static class MassTransitConfiguration
{
    public static IServiceCollection AddMessageBroker(
        this IServiceCollection services, 
        IConfiguration configuration,
        bool isConsumer = false,
        params Type[] consumerTypes)
    {
        services.Configure<RabbitMqOptions>(
            configuration.GetSection(RabbitMqOptions.SectionName));
        
        services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter();
            
            if (isConsumer && consumerTypes.Length != 0)
            {
                foreach (var consumerType in consumerTypes)
                {
                    busConfig.AddConsumer(consumerType);
                }
            }
            
            busConfig.UsingRabbitMq((context, rabbitConfig) =>
            {
                var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                
                rabbitConfig.Host(rabbitMqOptions.Host, rabbitMqOptions.Port, rabbitMqOptions.VirtualHost, hostConfig =>
                {
                    hostConfig.Username(rabbitMqOptions.Username);
                    hostConfig.Password(rabbitMqOptions.Password);
                });
                
                rabbitConfig.UseMessageRetry(retryConfig =>
                {
                    retryConfig.Interval(
                        rabbitMqOptions.RetryCount, 
                        TimeSpan.FromSeconds(rabbitMqOptions.RetryIntervalSeconds));
                });
                
                rabbitConfig.UseMessageRetry(retryConfig =>
                {
                    retryConfig.Handle<DbUpdateConcurrencyException>();
                    retryConfig.Exponential(
                        retryLimit: rabbitMqOptions.ConcurrencyRetryLimit,
                        minInterval: TimeSpan.FromMilliseconds(rabbitMqOptions.ConcurrencyRetryMinIntervalMs),
                        maxInterval: TimeSpan.FromMilliseconds(rabbitMqOptions.ConcurrencyRetryMaxIntervalMs),
                        intervalDelta: TimeSpan.FromMilliseconds(rabbitMqOptions.ConcurrencyRetryIntervalDeltaMs));
                });
                
                if (isConsumer)
                {
                    rabbitConfig.ConfigureEndpoints(context, 
                        new KebabCaseEndpointNameFormatter(prefix: "abc", includeNamespace: false));
                }
            });
        });
        
        return services;
    }
}