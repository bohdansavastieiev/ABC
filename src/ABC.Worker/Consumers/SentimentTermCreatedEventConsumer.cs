using ABC.Application.ProductRatings.Commands.MarkRatingsOutdated;
using ABC.Application.SentimentTerms.Commands.CreateSentimentTerm;
using ABC.Application.SentimentTerms.Common;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ABC.Worker.Consumers;

public class SentimentTermCreatedEventConsumer(
    ISender sender,
    ISentimentTermCache sentimentTermCache,
    ILogger<SentimentTermCreatedEventConsumer> logger) 
    : IConsumer<SentimentTermCreatedEvent>
{
    public async Task Consume(ConsumeContext<SentimentTermCreatedEvent> context)
    {
        sentimentTermCache.Clear();
        
        var command = new MarkRatingsOutdatedCommand(context.Message.SentimentTermId);
        
        var result = await sender.Send(command);
        if (result.IsFailed)
        {
            logger.LogError(
                "Failed to process new feedback invalidation of ratings with new sentiment term with ID {SentimentTermId}",
                context.Message.SentimentTermId
            );

            throw new InvalidOperationException("An issue occurred during marking ratings outdated after a new sentiment term.");
        }

        logger.LogInformation(
            "Successfully processed rating processing with new sentiment term {SentimentTermId}. Affected product ratings: {AffectedCount}",
            context.Message.SentimentTermId,
            result.Value
        );
    }
}