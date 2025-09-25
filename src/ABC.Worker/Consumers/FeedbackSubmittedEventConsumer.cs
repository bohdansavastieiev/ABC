using ABC.Application.Feedbacks.Commands.SubmitFeedback;
using ABC.Application.ProductRatings.Commands.ProcessNewFeedback;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ABC.Worker.Consumers;

public class FeedbackSubmittedEventConsumer(
    ISender sender,
    ILogger<FeedbackSubmittedEventConsumer> logger) 
    : IConsumer<FeedbackSubmittedEvent>
{
    public async Task Consume(ConsumeContext<FeedbackSubmittedEvent> context)
    {
        var command = new ProcessNewFeedbackCommand(context.Message.FeedbackId);

        var result = await sender.Send(command);
        if (result.IsFailed)
        {
            logger.LogError(
                "Failed to process new feedback {FeedbackId} for product {ProductId}",
                context.Message.FeedbackId,
                context.Message.ProductId
            );

            throw new InvalidOperationException("An issue occurred during rating processing with new feedback.");
        }

        logger.LogInformation(
            "Successfully processed rating processing with new feedback {FeedbackId} for product {ProductId}",
            context.Message.FeedbackId,
            context.Message.ProductId
        );
    }
}