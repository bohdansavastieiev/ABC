using ABC.Application.ProductRatings.Commands.ProcessOutdatedRatings;
using ABC.Application.ProductRatings.Commands.RequestRecalculation;
using ABC.Application.ProductRatings.Common;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ABC.Worker.Consumers;

public class RecalculationRequestedEventConsumer(
    ISender sender, 
    ILogger<RecalculationRequestedEventConsumer> logger,
    IOptions<RatingRecalculationOptions> options)
    : IConsumer<RecalculationRequestedEvent>
{
    public async Task Consume(ConsumeContext<RecalculationRequestedEvent> context)
    {
        var batchSize = context.Message.BatchSize ?? options.Value.BatchSize;
        var command = new ProcessOutdatedRatingsCommand(batchSize);
        var result = await sender.Send(command);
        
        if (result.IsFailed)
        {
            logger.LogError("Failed to recalculate outdated product ratings");
            throw new InvalidOperationException("An issue occurred during outdated rating recalculation.");
        }

        logger.LogInformation(
            "Successfully processed recalculated outdated product ratings. Affected product ratings: {AffectedCount}",
            result.Value
        );
    }
}