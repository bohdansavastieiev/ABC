using ABC.Application.Abstractions;
using FluentResults;
using MediatR;

namespace ABC.Application.ProductRatings.Commands.RequestRecalculation;

public class RequestRecalculationCommandHandler(
    IEventPublisher publisher) 
    : IRequestHandler<RequestRecalculationCommand, Result>
{
    public async Task<Result> Handle(RequestRecalculationCommand request, CancellationToken cancellationToken)
    {
        var recalculationRequestedEvent = new RecalculationRequestedEvent(request.BatchSize);
        await publisher.PublishAsync(recalculationRequestedEvent, cancellationToken);
        return Result.Ok();
    }
}