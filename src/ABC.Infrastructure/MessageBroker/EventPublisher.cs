using ABC.Application.Abstractions;
using ABC.Application.Feedbacks.Commands.SubmitFeedback;
using MassTransit;

namespace ABC.Infrastructure.MessageBroker;

public class EventPublisher(IPublishEndpoint publishEndpoint) : IEventPublisher
{
    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) 
        where T : class
    {
        return publishEndpoint.Publish(message, cancellationToken);
    }
}